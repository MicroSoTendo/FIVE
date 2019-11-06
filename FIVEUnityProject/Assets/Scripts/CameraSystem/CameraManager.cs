using FIVE.UI;
using FIVE.UI.CodeEditor;
using System.Collections.Generic;
using System.Linq;
using FIVE.EventSystem;
using UnityEngine;
using UnityEngine.Assertions;
using static FIVE.EventSystem.Util;

namespace FIVE.CameraSystem
{
    public class CameraManager : MonoBehaviour
    {
        private static CameraManager instance;
        private GameObject cameraPrefab;
        private readonly BijectMap<string, Camera> namedCameras = new BijectMap<string, Camera>();

        public enum StateEnum { Single, Multiple };

        public static StateEnum State { get; private set; } = StateEnum.Multiple;

        public static Camera CurrentActiveCamera { get; private set; }

        private int index;
        private float switchTimeout; // ms
        private List<Camera> wall = new List<Camera>();

        public static IEnumerable<Camera> GetFpsCameras =>
            from c in instance.namedCameras where c.key.ToLower().Contains("fps") select c.value;

        public static IEnumerable<Camera> Cameras => instance.namedCameras.Values;

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

            Vector3 ps = gameObject.transform.localToWorldMatrix.lossyScale;
            ps.x = 1f / ps.x;
            ps.y = 1f / ps.y;
            ps.z = 1f / ps.z;
            gameObject.transform.localScale = ps;

            gameObject.name = cameraName ?? nameof(Camera) + gameObject.GetInstanceID();
            Camera cam = gameObject.GetComponent<Camera>();
            instance.namedCameras.Add(gameObject.name, cam);
            instance.RaiseEvent<OnCameraCreated>(new CameraCreatedEventArgs(gameObject.name, cam));

            if (enableAudioListener) //Make sure only one audio listener active simutaneously
            {
                SetAudioListener(cam);
            }

            return gameObject.GetComponent<Camera>();
        }

        public static void SetAudioListener(Camera cam)
        {
            foreach (Camera c in instance.namedCameras.Values)
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
            foreach (Camera c in instance.namedCameras.Values)
            {
                c.enabled = false;
            }
            cam.enabled = true;
            cam.rect = new Rect(0, 0, 1, 1);
            CurrentActiveCamera = cam;
        }

        public static void SetCamera(string name)
        {
            SetCamera(instance.namedCameras[name]);
        }

        public static void SetCameraWall()
        {
            State = StateEnum.Multiple;
            instance.index = 0;
            instance.switchTimeout = -1f;
        }

        public static void Remove(Camera camera)
        {
            instance.namedCameras.Remove(camera);
            Destroy(camera.gameObject);
        }

        public static void Remove(string name)
        {
            Camera camera = instance.namedCameras[name];
            instance.namedCameras.Remove(camera);
            Destroy(camera.gameObject);
        }

        private void FixedUpdate()
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

                if (switchTimeout >= 0f)
                {
                    switchTimeout -= Time.fixedDeltaTime * 1000f;
                    return;
                }
                else
                {
                    switchTimeout = 2000f;
                }

                wall.Clear();
                if (namedCameras.Count > 0)
                {
                    foreach (Camera c in namedCameras.Values)
                    {
                        c.enabled = false;
                    }
                    for (int count = 0; count < 4; count++, index++)
                    {
                        index %= namedCameras.Keys.Count;
                        Camera c = instance.namedCameras.Values.ElementAt(index);
                        c.enabled = true;
                        float x = count / 2 / 2f, y = count % 2 / 2f;
                        c.rect = new Rect(x, y, 1f / 2f, 1f / 2f);
                        wall.Add(c);
                    }
                }
            }
        }
    }
}