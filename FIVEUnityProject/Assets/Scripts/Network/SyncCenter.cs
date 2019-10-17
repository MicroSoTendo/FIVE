using System;
using System.Collections.Generic;
using FIVE.CameraSystem;
using UnityEngine;

namespace FIVE.Network
{
    public class SyncCenter : MonoBehaviour
    {
        private Dictionary<string, Camera> cameras;
        private void Awake()
        {
            cameras = CameraManager.Cameras;
            
        }

    }
}
