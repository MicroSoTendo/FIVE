using FIVE.UI;
using FIVE.UI.CodeEditor;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using static FIVE.Util;

namespace FIVE.CameraSystem
{
    public class CameraManager : MonoBehaviour
    {
        private static CameraManager instance;
        private GameObject cameraPrefab;
        private readonly Dictionary<string, Camera> name2cam = new Dictionary<string, Camera>();
        private readonly Dictionary<Camera, string> cam2name = new Dictionary<Camera, string>();

        public enum StateEnum { Single, Multiple };

        public static StateEnum State { get; private set; } = StateEnum.Multiple;

        public static Camera CurrentActiveCamera { get; private set; }

        private int index = 0;
        private bool forceSwitch = false;
        private List<Camera> wall = new List<Camera>();

        public static IEnumerable<Camera> GetFpsCameras =>
            from c in instance.name2cam where c.Key.ToLower().Contains("fps") select c.Value;

        public static Dictionary<string, Camera> Cameras => instance.name2cam;

        private void Awake()
        {
            Assert.IsNull(instance);
            instance = this;
            cameraPrefab = Resources.Load<GameObject>("InfrastructurePrefabs/Camera/Camera");

            CurrentActiveCamera = Camera.current != null ? Camera.current : Camera.main;
            Subscribe<OnMultiCameraModeRequested>(SetCameraWall);
        }

        public static Camera AddCamera(string cameraName = null, Vector3 position = default, Quaternion rotation = default, Transform parent = null, bool enableAudioListener = false)
        {
            GameObject gameObject = Instantiate(instance.cameraPrefab, parent);
            gameObject.transform.localPosition = position;
            gameObject.transform.localRotation = rotation;

            gameObject.name = cameraName ?? nameof(Camera) + gameObject.GetInstanceID();
            Camera cam = gameObject.GetComponent<Camera>();
            instance.name2cam.Add(gameObject.name, cam);
            instance.cam2name.Add(cam, gameObject.name);
            instance.RaiseEvent<OnCameraCreated>(new CameraCreatedEventArgs(gameObject.name, cam));

            if (enableAudioListener) //Make sure only one audio listener active simutaneously
            {
                SetAudioListener(cam);
            }

            return gameObject.GetComponent<Camera>();
        }

        public static void SetAudioListener(Camera cam)
        {
            foreach (Camera c in instance.cam2name.Keys)
            {
                AudioListener audioListener = c.GetComponent<AudioListener>();
                if (audioListener != null)
                {
                    audioListener.enabled = false;
                }
            }
            cam.gameObject.GetComponent<AudioListener>().enabled = true;
        }

        public static void SetCamera(Camera cam)
        {
            State = StateEnum.Single;
            foreach (Camera c in instance.cam2name.Keys)
            {
                c.enabled = false;
            }
            cam.enabled = true;
            cam.rect = new Rect(0, 0, 1, 1);
            CurrentActiveCamera = cam;
        }

        public static void SetCamera(string name)
        {
            SetCamera(instance.name2cam[name]);
        }

        public static void SetCameraWall()
        {
            State = StateEnum.Multiple;
            instance.index = 0;
            instance.forceSwitch = true;
        }

        public static void Remove(Camera camera)
        {
            string name = instance.cam2name[camera];
            instance.cam2name.Remove(camera);
            instance.name2cam.Remove(name);
            Destroy(camera.gameObject);
        }

        public static void Remove(string name)
        {
            Camera c = instance.name2cam[name];
            instance.name2cam.Remove(name);
            instance.cam2name.Remove(c);
            Destroy(c.gameObject);
        }

        private void Update()
        {
            if (UIManager.Get<CodeEditorViewModel>()?.IsActive ?? false)
            {
                return;
            }

            if (State == StateEnum.Multiple)
            {
                if (Input.GetMouseButtonDown(0) && wall.Count > 0)
                {
                    int x = Mathf.FloorToInt(Input.mousePosition.x / Screen.width * 2f);
                    int y = Mathf.FloorToInt(Input.mousePosition.y / Screen.height * 2f);
                    Camera ca = wall[x * 2 + y];
                    SetCamera(ca);
                    SetAudioListener(ca);
                    this.RaiseEvent<OnCameraSwitched, CameraSwitchedEventArgs>(new CameraSwitchedEventArgs(activeCamera: ca));
                    wall.Clear();
                    return;
                }

                if (!forceSwitch && (int)(Time.time * 1000f) % 1000 != 0)
                {
                    return;
                }
                forceSwitch = false;

                wall.Clear();
                foreach (Camera c in cam2name.Keys)
                {
                    c.enabled = false;
                }
                for (int count = 0; count < 4; count++, index++)
                {
                    index %= cam2name.Keys.Count;
                    Camera c = instance.cam2name.Keys.ElementAt(index);
                    c.enabled = true;
                    float x = count / 2 / 2f, y = count % 2 / 2f;
                    c.rect = new Rect(x, y, 1f / 2f, 1f / 2f);
                    wall.Add(c);
                }
            }
        }
    }
}