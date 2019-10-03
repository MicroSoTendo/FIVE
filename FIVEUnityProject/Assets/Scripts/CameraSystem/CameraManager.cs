using FIVE.EventSystem;
using System.Collections.Generic;
using System.Linq;
using FIVE.UI;
using FIVE.UI.AWSLEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace FIVE.CameraSystem
{
    public class CameraManager : MonoBehaviour
    {
        public readonly Dictionary<string, Camera> cameras = new Dictionary<string, Camera>();
        private GameObject cameraPrefab;
        private static CameraManager instance;
        private int index = 0;
        public static Camera CurrentActiveCamera { get; private set; }

        public static IEnumerable<Camera> GetFpsCameras =>
            from c in instance.cameras where c.Key.ToLower().Contains("fps") select c.Value;

        private void Awake()
        {
            Assert.IsNull(instance);
            instance = this;
            cameraPrefab = Resources.Load<GameObject>("InfrastructurePrefabs/Camera/Camera");
            Camera[] existedCameras = FindObjectsOfType<Camera>();
            foreach (Camera cam in existedCameras)
            {
                if (cam.name.Contains("DefaultCamera"))
                {
                    cameras.Add(cam.name, cam);
                }
            }
            CurrentActiveCamera = Camera.current ?? Camera.main;
        }

        public static Camera AddCamera(string cameraName = null, Transform parent = null, bool enableAudioListener = false)
        {
            GameObject gameObject = Instantiate(instance.cameraPrefab);
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
            if (parent != null)
            {
                gameObject.transform.SetParent(parent);
            }
            instance.cameras.Add(gameObject.name, camera);
            instance.RaiseEvent<OnCameraCreated>(new CameraCreatedEventArgs(gameObject.name, camera));
            return gameObject.GetComponent<Camera>();
        }

        private void Update()
        {
            if (UIManager.GetViewModel<AWSLEditorViewModel>()?.IsEnabled ?? false)
            {
                return;
            }
            if (Input.GetKeyUp(KeyCode.C) && cameras.Count > 0)
            {
                foreach (Camera c in cameras.Values)
                {
                    c.enabled = false;
                    (c.gameObject.GetComponent<AudioListener>() ?? c.gameObject.GetComponentInChildren<AudioListener>()).enabled = false;
                }
                index %= cameras.Count;
                Camera ca = cameras.ElementAt(index).Value;
                ca.enabled = true;
                CurrentActiveCamera = ca;
                (ca.gameObject.GetComponent<AudioListener>() ?? ca.gameObject.GetComponentInChildren<AudioListener>()).enabled = true;
                this.RaiseEvent<OnCameraSwitched, CameraSwitchedEventArgs>(new CameraSwitchedEventArgs(activeCamera: ca));
                index++;
            }
        }
    }
}