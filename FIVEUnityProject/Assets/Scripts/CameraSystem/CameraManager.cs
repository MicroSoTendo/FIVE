using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using FIVE.EventSystem;
using FIVE.EventSystem.EventTypes;
using System;

namespace FIVE.CameraSystem
{
    public class CameraManager : MonoBehaviour
    {
        private readonly Dictionary<string, Camera> Cameras = new Dictionary<string, Camera>();

        private void Awake()
        {
            EventManager.Subscribe<OnCameraCreated, EventHandler<OnCameraCreatedArgs>, OnCameraCreatedArgs>
                  ((sender, args) => { });
        }

        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.C))
            {
                foreach (KeyValuePair<string, Camera> c in Cameras)
                {
                    c.Value.enabled = false;
                }
                Cameras.ElementAt(UnityEngine.Random.Range(0, Cameras.Count)).Value.enabled = true;
            }
        }
    }
}