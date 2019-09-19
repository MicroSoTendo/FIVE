using FIVE.EventSystem;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using UnityEngine;

namespace FIVE.CameraSystem
{
    [RequireComponent(typeof(PhotonView))]
    public class CameraManager : MonoBehaviourPun
    {
        public readonly Dictionary<string, Camera> Cameras = new Dictionary<string, Camera>();

        private void Awake()
        {
            EventManager.Subscribe<OnCameraCreated, OnCameraCreatedArgs>(OnCameraCreated);
        }

        private void OnCameraCreated(object sender, OnCameraCreatedArgs args)
        {
            Debug.Log(nameof(OnCameraCreated) + " Called.");
            Cameras.Add(args.Id, args.Camera);
        }

        private void Update()
        {
            if (photonView.IsMine == false && PhotonNetwork.IsConnected) return;
            if (Input.GetKeyUp(KeyCode.C) && Cameras.Count > 0)
            {
                foreach (Camera c in Cameras.Values)
                {
                    c.enabled = false;
                }
                Cameras.ElementAt(Random.Range(0, Cameras.Count)).Value.enabled = true;
            }
        }
    }
}