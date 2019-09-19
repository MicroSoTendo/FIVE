using FIVE.EventSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FIVE.CameraSystem
{
    public class CameraManager : MonoBehaviour
    {
        private static readonly Dictionary<string, Camera> Cameras = new Dictionary<string, Camera>();

        private static readonly Dictionary<Camera, Transform> Bindings = new Dictionary<Camera, Transform>();
        private static readonly Dictionary<Transform, (Vector3, Quaternion)> Last = new Dictionary<Transform, (Vector3, Quaternion)>();

        public static void AddBinding(Camera c, Transform t)
        {
            Bindings.Add(c, t);
            Last.Add(t, (t.position, t.localRotation));
        }

        private void Awake()
        {
            EventManager.Subscribe<OnCameraCreated, OnCameraCreatedArgs>((sender, args) => Cameras.Add(args.Id, args.Camera));
        }

        private void Update()
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

        private void LateUpdate()
        {
            return;
            foreach (KeyValuePair<Camera, Transform> binding in Bindings)
            {
                var camera = binding.Key;
                var transform = binding.Value;
                if (!transform.hasChanged) continue;
                var (lastPosition, lastRotation) = Last[transform];
                camera.transform.Translate(transform.position - lastPosition);
            }
            foreach (Transform lastKey in Last.Keys.ToList())
            {
                Last[lastKey] = (lastKey.position, lastKey.localRotation);
            }
        }
    }
}