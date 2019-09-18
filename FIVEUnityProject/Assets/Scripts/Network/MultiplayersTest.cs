using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace FIVE.Network
{
    public class MultiplayersTest : MonoBehaviour
    {
        void Start()
        {
            PhotonNetwork.Instantiate("EntityPrefabs/robotSphere", new Vector3(3, 0, 3), Quaternion.identity);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
