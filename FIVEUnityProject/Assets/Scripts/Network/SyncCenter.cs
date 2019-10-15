using System;
using System.Collections.Generic;
using FIVE.CameraSystem;
using Mirror;
using UnityEngine;

namespace FIVE.Network
{
    public class SyncCenter : NetworkBehaviour
    {
        private Dictionary<string, Camera> cameras;
        private void Awake()
        {
            cameras = CameraManager.Cameras;
        }

        public override bool OnSerialize(NetworkWriter writer, bool initialState)
        {
            return base.OnSerialize(writer, initialState);
        }

        public override void OnDeserialize(NetworkReader reader, bool initialState)
        {
            base.OnDeserialize(reader, initialState);
        }
    }
}
