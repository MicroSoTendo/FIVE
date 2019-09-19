using System.Collections.Generic;
using FIVE.CameraSystem;
using Photon.Pun;
using UnityEngine;

namespace FIVE.Assets.Scripts.CameraSystem
{
    [RequireComponent(typeof(PhotonView))]
    [RequireComponent(typeof(CameraManager))]
    public class CameraManagerView : MonoBehaviour, IPunObservable
    {
        private CameraManager cameraManager;
        private Dictionary<string, Camera> camerasReceived;
        void Awake()
        {
            cameraManager = GetComponent<CameraManager>();
        }
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            return;
            if (stream.IsWriting)
            {
                stream.SendNext(cameraManager.Cameras);
            }
            else
            {
                camerasReceived = (Dictionary<string, Camera>) stream.ReceiveNext();
            }
        }

        public void Update()
        {
            return;
            foreach (KeyValuePair<string, Camera> keyValuePair in camerasReceived)
            {
                if (cameraManager.Cameras.ContainsKey(keyValuePair.Key))
                {

                }
                else
                {
                    
                }
            }
        }
    }
}
