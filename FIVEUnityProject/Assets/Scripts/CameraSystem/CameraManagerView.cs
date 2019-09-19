using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

namespace FIVE.Assets.Scripts.CameraSystem
{
    [RequireComponent(typeof(PhotonView))]
    public class CameraManagerView : MonoBehaviour, IPunObservable
    {
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
        }
    }
}
