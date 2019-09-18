using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Pun.Demo.PunBasics;
using UnityEngine;

namespace FIVE.Network
{
    public class PlayerManager : MonoBehaviourPunCallbacks, IPunObservable
    {
        private GameObject player;
        private PhotonView playerPhotonView;
        void Start()
        {

            player = PhotonNetwork.Instantiate("EntityPrefabs/robotSphere", new Vector3(3, 0, 3), Quaternion.identity);
            playerPhotonView = player.GetComponent<PhotonView>();

            CameraWork cameraWork = GetComponent<CameraWork>();
            if (cameraWork != null)
            {
                if (playerPhotonView.IsMine)
                {
                    cameraWork.OnStartFollowing();
                }
            }
            else
            {
                Debug.LogError("<Color=Red><b>Missing</b></Color> CameraWork Component on player Prefab.", this);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {

        }
    }
}
