using Google.Apis.Auth.OAuth2;
using Google.Apis.Download;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;
using Application = UnityEngine.Application;

namespace FIVE
{
    using GoogleFile = Google.Apis.Drive.v3.Data.File;

    [InitializeOnLoad]
    public class LinkButtonEditor
    {
        private const string ButtonText = "Fetch Assets";
        private static GUIContent content;
        private static GUIStyle style;
        private static Texture2D image_h;
        private static Texture2D image_n;
        private static Texture2D image_p;
        private static bool initialized = false;

        private static readonly string DataPathPrefix = Application.dataPath;
        private static Dictionary<string, string> hashingDictionary;
        private static readonly string[] Scopes = { DriveService.Scope.DriveReadonly };
        private static readonly string ApplicationName = "FIVE Unity Synchronizer";
        private static readonly string FolderId = "1iB1gfjvMDEWYtGelduUyOaGfQamEIoUc"; //"Texture" Folder
        private static readonly string folderMimeType = "application/vnd.google-apps.folder";
        private static readonly string TexturesPath = DataPathPrefix + "/Resources/Textures";
        private static readonly DirectoryInfo Textures = new DirectoryInfo(TexturesPath);
        private static FileInfo[] TexturesFileInfos { get; set; }

        private static DriveService driveService;
        private static readonly ConcurrentDictionary<string, bool> WriteTasks =
            new ConcurrentDictionary<string, bool>();

        private static readonly List<string> CheckedFolders = new List<string>();
        private static readonly List<string> CheckedFiles = new List<string>();
        private static readonly LinkButtonEditor Instance = new LinkButtonEditor();

        static LinkButtonEditor()
        {
            ToolbarExtender.DoToolbarGUI.Add(OnToolbarGUI);
            image_h = Resources.Load<Texture2D>("Textures/UI/Btn_OK_h");
            image_n = Resources.Load<Texture2D>("Textures/UI/Btn_OK_n");
            image_p = Resources.Load<Texture2D>("Textures/UI/Btn_OK_p");
        }

        private LinkButtonEditor() { }

        private static void Initialize()
        {
            content = new GUIContent(ButtonText, "Toggle Unity's auto reload.");
            style = new GUIStyle()
            {
                alignment = TextAnchor.MiddleCenter,
                imagePosition = ImagePosition.ImageAbove,
                font = Font.CreateDynamicFontFromOSFont("Segoe UI Emoji", 12),
                fixedWidth = 100,
                fixedHeight = 24,
                normal = { background = image_n },
                hover = { background = image_h },
                active = { background = image_p },
                onNormal = { background = image_n },
                focused = { background = image_p },
                onHover = { background = image_h },
                onActive = { background = image_n },
                onFocused = { background = image_n },
                border = new RectOffset(0, 0, 0, 0),
            };
            style.normal.textColor = Color.white;
            style.hover.textColor = Color.white;
            style.active.textColor = Color.magenta;
            initialized = true;
        }
        
        private static void SaveDatabase()
        {
            string pathToDatabase = DataPathPrefix + "/Scripts/Editor/.fileHashingMap.json";
            string json = JsonConvert.SerializeObject(hashingDictionary, Formatting.None);
            File.WriteAllText(pathToDatabase, json);
        }


        private static string RemovePrefix(string path)
        {
            path = Trim(path);
            string prefix = Trim(DataPathPrefix);
            return path.Replace(prefix, "");
        }

        private static string CheckSum(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                return CheckSum(fs);
            }
        }

        private static string CheckSum(Stream stream)
        {
            using (var md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private static readonly ConcurrentDictionary<string,string> PathMd5Map = new ConcurrentDictionary<string, string>();
        private static void InitDatabase()
        {
            TexturesFileInfos = Textures.GetFiles("*.*", SearchOption.AllDirectories);
            Parallel.ForEach(TexturesFileInfos, info =>
            {
                string md5Str = CheckSum(info.FullName);
                PathMd5Map.TryAdd(RemovePrefix(info.FullName), md5Str);
            });
            hashingDictionary = PathMd5Map.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            SaveDatabase();
            Debug.Log($"Hashing Finished. Count = {hashingDictionary.Count}");
        }

        private static void InitGoogleService()
        {
            UserCredential credential;
            string credentialPath = EditorUtility.OpenFilePanel("Select your API credential",
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "json");
            if (credentialPath == "")
            {
                Application.OpenURL("https://developers.google.com/drive/api/v3/quickstart/dotnet");
            }

            string credPath = Path.GetDirectoryName(credentialPath) + "\\token.json";
            using (var stream = new FileStream(credentialPath, FileMode.Open, FileAccess.Read))
            {
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    stream,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;

                Debug.Log("Credential file saved to: " + credPath);
            }

            // Create Drive API service.
            driveService = new DriveService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });
        }

        private static IList<GoogleFile> ListFolders(string folderId)
        {
            FilesResource.ListRequest request = driveService.Files.List();
            request.Q = $"'{folderId}' in parents and mimeType = '{folderMimeType}'";
            request.Fields = "files(name,id)";
            return request.Execute().Files;
        }

        private static IList<GoogleFile> ListFiles(string folderId)
        {
            FilesResource.ListRequest request = driveService.Files.List();
            request.Q = $"'{folderId}' in parents and mimeType != '{folderMimeType}'";
            request.Fields = "files(name,id,md5Checksum)";
            return request.Execute().Files;
        }


        private static IEnumerator DownloadRoutine(IDownloadProgress progress, MemoryStream ms, string fileId, string name, long size, string targetDirectory)
        {
            bool isDownloading = true;
            while (isDownloading)
            {
                switch (progress.Status)
                {
                    case DownloadStatus.NotStarted:
                        Debug.LogWarning($"{name} not started");
                        break;
                    case DownloadStatus.Downloading:
                        Debug.Log($"{name} Downloading Progress: {(float)progress.BytesDownloaded / size:P2}");
                        break;
                    case DownloadStatus.Completed:
                        string filePath = $"{targetDirectory}\\{name}";
                        using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            Debug.Log($"Writing File: {filePath}, {ms.Length} bytes");
                            ms.CopyTo(fs);
                            hashingDictionary[RemovePrefix(filePath)] = CheckSum(ms);
                        }
                        ms.Dispose();
                        WriteTasks[fileId] = true;
                        isDownloading = false;
                        Debug.Log($"{name} downloaded");
                        break;
                    case DownloadStatus.Failed:
                        Debug.LogWarning($"{name} download failed");
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                if(isDownloading)
                    yield return new EditorWaitForSeconds(0.1f);
            }
        }

        private static void DownloadFile(string fileId, string targetDirectory)
        {
            FilesResource.GetRequest request = driveService.Files.Get(fileId);
            request.Fields = "size,name,id,md5Checksum";
            GoogleFile fileinfo = request.Execute();
            var stream = new MemoryStream();
            IDownloadProgress progress = request.DownloadWithStatus(stream);
            EditorCoroutineUtility.StartCoroutine(DownloadRoutine(progress, stream, fileId, fileinfo.Name, fileinfo.Size.Value, targetDirectory), Instance);
            WriteTasks.TryAdd(fileId, false);
        }


        private static void DownloadFolder(string folderId, string parentFolderPath)
        {
            FilesResource.GetRequest request = driveService.Files.Get(folderId);
            request.Fields = "name,mimeType";
            GoogleFile fileInfo = request.Execute();
            //Makesure metadata gives a folder type
            if (fileInfo.MimeType != folderMimeType)
            {
                Debug.LogWarning($"{folderId} is not a folder");
                return;
            }

            IList<GoogleFile> subFolders = ListFolders(folderId);
            IList<GoogleFile> subFiles = ListFiles(folderId);
            string pathToNewFolder = parentFolderPath.Replace("/", "\\") + $"\\{fileInfo.Name}";
            Directory.CreateDirectory(pathToNewFolder);
            //Download files in current folder
            foreach (GoogleFile subFile in subFiles)
            {
                DownloadFile(subFile.Id, pathToNewFolder);
            }

            //Recursively download subfolder stuff
            foreach (GoogleFile subFolder in subFolders)
            {
                DownloadFolder(subFolder.Id, pathToNewFolder);
            }
        }

        private static string Trim(string path)
        {
            string s = path.Replace("/", "\\");
            s = s.EndsWith("\\") ? s.Remove(s.Length - 1) : s;
            return s;
        }

        private static void Checked(string path, string type)
        {
            if (type == "folder")
            {
                CheckedFolders.Add(path);
            }
            else
            {
                CheckedFiles.Add(path);
            }
        }

        private static void PrintFinal()
        {
            Debug.LogFormat("Checked files: {0} Checked folders: {1}", CheckedFiles.Count, CheckedFolders.Count);
        }

        private static bool ReplaceAll = false;
        private static IEnumerator FetchCoroutine(string remoteFolderId, string localFolderPath)
        {
            localFolderPath = Trim(localFolderPath);
            var dir = new DirectoryInfo(localFolderPath);
            DirectoryInfo[] directories = dir.GetDirectories();
            var foldersExisted = new HashSet<string>(from d in directories select d.Name);
            IList<GoogleFile> subFolders = ListFolders(remoteFolderId);
            foreach (GoogleFile subFolder in subFolders)
            {
                if (foldersExisted.Contains(subFolder.Name))
                {
                    string folderPath = $"{localFolderPath}\\{subFolder.Name}";
                    Checked(folderPath, "folder");
                    EditorCoroutineUtility.StartCoroutine(FetchCoroutine(subFolder.Id, folderPath), Instance);
                    FetchTasks.TryAdd(subFolder.Id, false);
                }
                else
                {
                    DownloadFolder(subFolder.Id, localFolderPath);
                }

                yield return null;
            }

            IList<GoogleFile> subFiles = ListFiles(remoteFolderId);

            bool CheckWithRemote(string md5Local, string fileKey, GoogleFile subFile)
            {
                string md5Remote = subFile.Md5Checksum;
                if (md5Local != md5Remote)
                {
                    if (ReplaceAll)
                    {
                        DownloadFile(subFile.Id, localFolderPath);
                        return true;
                    }
                    int result = EditorUtility.DisplayDialogComplex("File Conflict",
                        $"File:{fileKey} checksum does not match remote, do you still want to download it from remote to replace it?",
                        "Yes", "No", "Yes for all");
                    
                    if (result == 0 || result == 2)
                    {
                        ReplaceAll = true;
                        DownloadFile(subFile.Id, localFolderPath);
                    }

                    return false;
                }

                return true;
            }

            foreach (GoogleFile subFile in subFiles)
            {
                string filePath = $"{localFolderPath}\\{subFile.Name}";
                if (File.Exists(filePath))
                {
                    string fileKey = RemovePrefix(filePath);
                    if (hashingDictionary.ContainsKey(fileKey))
                    {
                        string md5Local = hashingDictionary[fileKey];
                        bool checkResult = CheckWithRemote(md5Local, fileKey, subFile);
                        if (checkResult)
                        {
                            Checked(filePath, "file");
                        }
                    }
                    else
                    {
                        string md5Local = CheckSum(filePath);
                        hashingDictionary.Add(fileKey, md5Local);
                        bool checkedResult = CheckWithRemote(md5Local, fileKey, subFile);
                        if (checkedResult)
                        {
                            Checked(filePath, "file");
                        }
                    }
                }
                else
                {
                    DownloadFile(subFile.Id, localFolderPath);
                }

                yield return null;
            }

            FetchTasks[remoteFolderId] = true;
        }

        private static ConcurrentDictionary<string, bool> FetchTasks = new ConcurrentDictionary<string, bool>();
        private static void OnToolbarGUI()
        {
            if (!initialized)
            {
                Initialize();
            }

            GUILayout.FlexibleSpace();
            if (GUILayout.Button(content, style))
            {
                Debug.Log("Start hashing files, this may take a while.");
                InitDatabase();
                InitGoogleService();
                if (EditorPrefs.HasKey("kAutoRefresh")) EditorPrefs.SetBool("kAutoRefresh", false);
                EditorCoroutineUtility.StartCoroutine(FetchCoroutine(FolderId, TexturesPath), Instance);
                FetchTasks.TryAdd(FolderId, false);
                EditorCoroutineUtility.StartCoroutine(TaskMonitor(), Instance);
            }
        }

        private static IEnumerator TaskMonitor()
        {
            while (true)
            {
                int fetchFinishedNum, writeFinishedNum;
                while (true)
                {
                    fetchFinishedNum = 0;
                    foreach (bool value in FetchTasks.Values)
                    {
                        if (value) fetchFinishedNum++;
                        else break;
                    }

                    writeFinishedNum = 0;
                    foreach (bool writeTasksValue in WriteTasks.Values)
                    {
                        if (writeTasksValue) writeFinishedNum++;
                        else break;
                    }
                    break;
                }
                yield return new EditorWaitForSeconds(0.5f);
                if (FetchTasks.Count == fetchFinishedNum && WriteTasks.Count == writeFinishedNum)
                    break;
            }
            if (EditorPrefs.HasKey("kAutoRefresh")) EditorPrefs.SetBool("kAutoRefresh", true);
            PrintFinal();
            SaveDatabase();
        }
    }
}