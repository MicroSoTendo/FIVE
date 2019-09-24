using System;
using Photon.Pun;
using UnityEngine;

namespace FIVE.Network.Views
{
    public abstract class NetworkView : MonoBehaviour, IPunObservable
    {
        protected PhotonView PhotonView { get; private set; }
        protected event Action<PhotonStream, PhotonMessageInfo> OnSending;
        protected event Action<PhotonStream, PhotonMessageInfo> OnReading;
        protected void SetStreamingDelegate(
            Action<PhotonStream, PhotonMessageInfo> sendingHandler,
            Action<PhotonStream, PhotonMessageInfo> readingHandler, 
            bool value)
        {
            if (value)
            {
                OnSending += sendingHandler;
                OnReading += readingHandler;
            }
            else
            {
                OnSending -= sendingHandler;
                OnReading -= readingHandler;
            }
        }

        protected virtual void Awake()
        {
            PhotonView = gameObject.GetComponent<PhotonView>();
        }

        public virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                OnSending?.Invoke(stream, info);
            }
            else
            {
                OnReading?.Invoke(stream, info);
            }
        }
    }
}
