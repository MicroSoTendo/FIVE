using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI
{
    public abstract partial class ViewModel
    {
        private static readonly Canvas[] Canvas;
        private static readonly GameObject ViewModelsRoot;
        private static readonly ConcurrentDictionary<string, GameObject> CachedPrefabs;
        private static readonly ConcurrentDictionary<Type, ViewModel> CachedViewModels;
        public static bool Initialized { get; private set; }

        static ViewModel()
        {
            Initialized = false;
            ViewModelsRoot = new GameObject(nameof(ViewModelsRoot));
            CanvasGroup canvasGroup = ViewModelsRoot.AddComponent<CanvasGroup>();
            Canvas = new Canvas[3];
            InitCanvas();
            CachedPrefabs = new ConcurrentDictionary<string, GameObject>();
            CachedViewModels = new ConcurrentDictionary<Type, ViewModel>();
        }

        private static void InitCanvas()
        {
            foreach (object value in Enum.GetValues(typeof(RenderMode)))
            {
                var go = new GameObject {name = value.ToString()};
                go.transform.SetParent(ViewModelsRoot.transform);
                Canvas c = go.AddComponent<Canvas>();
                go.AddComponent<CanvasScaler>();
                GraphicRaycaster graphicRaycaster = go.AddComponent<GraphicRaycaster>();
                
                Canvas[(int)value] = c;
                c.renderMode = (RenderMode)value;
            }
        }
        
        public static IEnumerator InitializeRoutine(Action<float> progressCallBack)
        {
            if (progressCallBack == null) progressCallBack = f => { };
            string pathPrefix = Application.dataPath + "/Resources";
            string searchingDir = "/EntityPrefabs/UI";
            var dirInfo = new DirectoryInfo(pathPrefix + searchingDir);
            FileInfo[] fileInfos = dirInfo.GetFiles("*.prefab", SearchOption.AllDirectories);
            Type[] types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where !type.IsAbstract && typeof(ViewModel).IsAssignableFrom(type)
                select type).ToArray();
            int total = types.Length + fileInfos.Length * 2;
            int finished = 0;
            progressCallBack((float)finished / total);
            foreach (FileInfo fileInfo in fileInfos)
            {
                string trimedPath = fileInfo.FullName.Replace("\\", "/").Replace(pathPrefix, "").Replace(".prefab", "")
                    .Substring(1);
                ResourceRequest prefabRequests = Resources.LoadAsync<GameObject>(trimedPath);
                prefabRequests.completed += OnPrefabLoadCompleted(trimedPath, prefabRequests);
                finished++;
                yield return null;
            }
            while (CachedPrefabs.Count < fileInfos.Length)
            {
                progressCallBack((float)finished / total);
                yield return null;
            }
            finished *= 2;
            foreach (Type type in types)
            {
                var cached = (ViewModel)type.GetConstructor(Type.EmptyTypes)?.Invoke(null);
                CachedViewModels.TryAdd(type, cached);
                finished++;
                progressCallBack((float)finished / total);
                yield return null;
            }
            Initialized = true;
        }

        private static Action<AsyncOperation> OnPrefabLoadCompleted(string trimedPath,
            ResourceRequest resourceRequest)
        {
            return operation =>
            {
                CachedPrefabs.TryAdd(trimedPath, (GameObject)resourceRequest.asset);
            };
        }

        private static GameObject LoadPrefab(string path)
        {
            return CachedPrefabs.TryGetValue(path, out GameObject prefab) ? prefab : Resources.Load<GameObject>(path);
        }

        public static ViewModel Create<T>() where T : ViewModel
        {
            CachedViewModels.TryGetValue(typeof(T), out ViewModel vm);
            return vm;
        }
    }
}