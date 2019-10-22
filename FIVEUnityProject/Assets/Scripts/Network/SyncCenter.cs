using System;
using System.Collections.Generic;
using FIVE.CameraSystem;
using UnityEngine;

namespace FIVE.Network
{
    public class SyncCenter : MonoBehaviour
    {
        private static SyncCenter instance;
        private Dictionary<string, Camera> cameras;
        private HashSet<Transform> ownedTransforms = new HashSet<Transform>();
        private HashSet<Transform> remoteTransforms = new HashSet<Transform>();
        private Action OnUpdate;
        private bool IsHost;
        private void Awake()
        {
            instance = this;
            cameras = CameraManager.Cameras;
            
        }

        public static void Register(Transform transform)
        {
            instance.ownedTransforms.Add(transform);
        }

        public void Serialize()
        {
            foreach (Transform ownedTransform in ownedTransforms)
            {
            }
        }

        public void Deserialize()
        {
            
        }

        public void Update()
        {
        }

    }
}
