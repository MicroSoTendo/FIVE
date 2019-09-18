using Photon.Pun;
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
            player.name = PhotonNetwork.NickName;
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
