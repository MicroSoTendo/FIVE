using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FIVE.CameraSystem
{
    public class CameraManager : MonoBehaviour
    {
        public Camera CameraPrefab;

        public readonly Dictionary<string, Camera> Cameras = new Dictionary<string, Camera>();

        public Camera NewCamera(string id)
        {
            Camera cam = Instantiate(CameraPrefab);
            Cameras[id] = cam;
            return cam;
        }

        public Camera GetCamera(string id) => Cameras[id];

        private void Start()
        {
            Camera cam = NewCamera("deep_space");
            cam.transform.position = new Vector3(64f, 30f, 64f);
            cam.transform.rotation = Quaternion.LookRotation(Vector3.down, Vector3.forward);
            cam.enabled = true;
        }

        public void Update()
        {
            if (Input.GetKeyUp(KeyCode.C))
            {
                foreach (KeyValuePair<string, Camera> c in Cameras)
                {
                    c.Value.enabled = false;
                }
                Cameras.ElementAt(Random.Range(0, Cameras.Count)).Value.enabled = true;
            }
        }
    }
}