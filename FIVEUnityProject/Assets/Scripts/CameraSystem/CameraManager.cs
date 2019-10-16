using FIVE.EventSystem;
using FIVE.UI;
using FIVE.UI.CodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace FIVE.CameraSystem
{
    public class CameraManager : MonoBehaviour
    {
        private static CameraManager instance;
        private GameObject cameraPrefab;
        private readonly Dictionary<string, Camera> name2cam = new Dictionary<string, Camera>();
        private readonly Dictionary<Camera, string> cam2name = new Dictionary<Camera, string>();

        public enum StateEnum { Single, Multiple };

        public StateEnum State { get; private set; } = StateEnum.Multiple;

        public static Camera CurrentActiveCamera { get; private set; }

        private int index = 0;

        public static IEnumerable<Camera> GetFpsCameras =>
            from c in instance.name2cam where c.Key.ToLower().Contains("fps") select c.Value;

        public static Dictionary<string, Camera> Cameras => instance.name2cam;

        private void Awake()
        {
            Assert.IsNull(instance);
            instance = this;
            cameraPrefab = Resources.Load<GameObject>("InfrastructurePrefabs/Camera/Camera");

            CurrentActiveCamera = Camera.current ?? Camera.main;

            EventManager.Subscribe<OnSwitchCameraModeRequested>((_, __) => SetCameraWall());
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
            instance.State = StateEnum.Single;
            foreach (Camera c in instance.cam2name.Keys)
            {
                c.enabled = false;
            }
            cam.enabled = true;
            CurrentActiveCamera = cam;
        }

        public static void SetCamera(string name)
        {
            instance.State = StateEnum.Single;
            foreach (Camera c in instance.cam2name.Keys)
            {
                c.enabled = false;
            }
            Camera cam = instance.name2cam[name];
            cam.enabled = true;
            CurrentActiveCamera = cam;
        }

        public static void SetCameraWall()
        {
            instance.State = StateEnum.Multiple;
            foreach (Camera c in instance.cam2name.Keys)
            {
                c.enabled = false;
            }
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
                if (Time.frameCount % 120 != 0)
                    return;
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
                }
            }
            else if (Input.GetKeyUp(KeyCode.C) && name2cam.Count > 0)
            {
                index %= name2cam.Count;

                Camera cam = name2cam.ElementAt(index).Value;
                SetAudioListener(cam);
                SetCamera(cam);
                this.RaiseEvent<OnCameraSwitched, CameraSwitchedEventArgs>(new CameraSwitchedEventArgs(activeCamera: cam));
                index++;
            }
        }
    }
}