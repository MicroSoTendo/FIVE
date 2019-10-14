using FIVE.EventSystem;
using System.Collections.Generic;
using System.Linq;
using FIVE.UI;
using FIVE.UI.CodeEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace FIVE.CameraSystem
{
    public class CameraManager : MonoBehaviour
    {
        private static CameraManager instance;
        private readonly Dictionary<string, Camera> cameras = new Dictionary<string, Camera>();
        private readonly Dictionary<Camera, string> camerasReversed = new Dictionary<Camera, string>();
        private GameObject cameraPrefab;
        private int index = 0;
        public static Camera CurrentActiveCamera { get; private set; }

        public static IEnumerable<Camera> GetFpsCameras =>
            from c in instance.cameras where c.Key.ToLower().Contains("fps") select c.Value;

        public static Dictionary<string, Camera> Cameras => instance.cameras;

        private void Awake()
        {
            Assert.IsNull(instance);
            instance = this;
            cameraPrefab = Resources.Load<GameObject>("InfrastructurePrefabs/Camera/Camera");
            Camera[] existedCameras = FindObjectsOfType<Camera>();
            foreach (Camera cam in existedCameras)
            {
                cameras.Add(cam.name, cam);
                camerasReversed.Add(cam, cam.name);
            }

            CurrentActiveCamera = Camera.current ?? Camera.main;
        }

        public static Camera AddCamera(string cameraName = null, Vector3 position = default,
            Quaternion rotation = default, Transform parent = null, bool enableAudioListener = false)
        {
            GameObject gameObject = Instantiate(instance.cameraPrefab, parent);
            gameObject.transform.localPosition = position;
            gameObject.transform.localRotation = rotation;
            if (enableAudioListener) //Make sure only one audio listener active simutaneously
            {
                foreach (Camera camerasValue in instance.cameras.Values)
                {
                    AudioListener audioListener = camerasValue.GetComponent<AudioListener>();
                    if (audioListener != null)
                    {
                        audioListener.enabled = false;
                    }
                }
            }

            gameObject.GetComponent<AudioListener>().enabled = enableAudioListener;
            gameObject.name = cameraName ?? nameof(Camera) + gameObject.GetInstanceID();
            Camera camera = gameObject.GetComponent<Camera>();
            instance.cameras.Add(gameObject.name, camera);
            instance.camerasReversed.Add(camera, gameObject.name);
            instance.RaiseEvent<OnCameraCreated>(new CameraCreatedEventArgs(gameObject.name, camera));
            return gameObject.GetComponent<Camera>();
        }

        public static void Remove(Camera camera)
        {
            string name = instance.camerasReversed[camera];
            instance.camerasReversed.Remove(camera);
            instance.cameras.Remove(name);
            Destroy(camera.gameObject);
        }

        public static void Remove(string cameraName)
        {
            Camera c = instance.cameras[cameraName];
            instance.cameras.Remove(cameraName);
            instance.camerasReversed.Remove(c);
            Destroy(c.gameObject);
        }

        private void Update()
        {
            if (UIManager.Get<CodeEditorViewModel>()?.IsActive ?? false)
            {
                return;
            }

            if (Input.GetKeyUp(KeyCode.C) && cameras.Count > 0)
            {
                foreach (Camera c in cameras.Values)
                {
                    c.enabled = false;
                    (c.gameObject.GetComponent<AudioListener>() ?? c.gameObject.GetComponentInChildren<AudioListener>())
                        .enabled = false;
                }

                index %= cameras.Count;
                Camera ca = cameras.ElementAt(index).Value;
                ca.enabled = true;
                CurrentActiveCamera = ca;
                (ca.gameObject.GetComponent<AudioListener>() ?? ca.gameObject.GetComponentInChildren<AudioListener>())
                    .enabled = true;
                this.RaiseEvent<OnCameraSwitched, CameraSwitchedEventArgs>(
                    new CameraSwitchedEventArgs(activeCamera: ca));
                index++;
            }
        }
    }
}