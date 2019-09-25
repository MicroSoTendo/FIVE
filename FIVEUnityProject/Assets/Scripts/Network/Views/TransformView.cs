using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;

namespace FIVE.Network.Views
{
    public class TransformView : NetworkView
    {
        private float distance;
        private float angle;

        private Vector3 direction;
        private Vector3 networkPosition;
        private Vector3 storedPosition;

        private Quaternion networkRotation;

        [SerializeField] private bool synchronizePosition = true;
        [SerializeField] private bool synchronizeRotation = true;
        [SerializeField] private bool synchronizeScale = true;

        public bool SynchronizePosition
        {
            get => synchronizePosition;
            set
            {
                if (synchronizePosition == value)
                {
                    return;
                }
                SetStreamingDelegate(SendPosition, FirstReadPosition, value);
                synchronizePosition = value;
            }

        }

        public bool SynchronizeRotation
        {
            get => synchronizeRotation;
            set
            {
                if (synchronizeRotation == value)
                {
                    return;
                }
                SetStreamingDelegate(SendRotation, FirstReadRotation, value);
                synchronizeRotation = value;
            }
        }

        public bool SynchronizeScale
        {
            get => synchronizeScale;
            set
            {
                if (synchronizeScale == value)
                {
                    return;
                }
                SetStreamingDelegate(SendScale, FirstReadScale, value);
                synchronizeScale = value;
            }
        }

        private void Start()
        {
            storedPosition = transform.position;
            networkPosition = Vector3.zero;
            networkRotation = Quaternion.identity;
        }

        private void Update()
        {
            //Update position from other clients
            if (PhotonView.IsMine)
            {
                return;
            }
            transform.position = Vector3.MoveTowards(transform.position, networkPosition,
                distance * (1.0f / PhotonNetwork.SerializationRate));
            transform.rotation = Quaternion.RotateTowards(transform.rotation, networkRotation,
                angle * (1.0f / PhotonNetwork.SerializationRate));
        }

        private void SendPosition(PhotonStream s, PhotonMessageInfo info)
        {
            direction = transform.position - storedPosition;
            storedPosition = transform.position;
            s.SendNext(transform.position);
            s.SendNext(direction);
        }

        private void SendRotation(PhotonStream s, PhotonMessageInfo info)
        {
            s.SendNext(transform.rotation);
        }

        private void SendScale(PhotonStream s, PhotonMessageInfo info)
        {
            s.SendNext(transform.localScale);
        }

        private void FirstReadPosition(PhotonStream s, PhotonMessageInfo info)
        {
            networkPosition = (Vector3)s.ReceiveNext();
            direction = (Vector3)s.ReceiveNext();
            transform.position = this.networkPosition;
            distance = 0f;
            OnReading -= FirstReadPosition;
            OnReading += ReadPosition;
        }

        private void ReadPosition(PhotonStream s, PhotonMessageInfo info)
        {
            networkPosition = (Vector3)s.ReceiveNext();
            direction = (Vector3)s.ReceiveNext();
            float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
            networkPosition += direction * lag;
            distance = Vector3.Distance(transform.position, networkPosition);
        }


        private void FirstReadRotation(PhotonStream s, PhotonMessageInfo info)
        {
            networkRotation = (Quaternion)s.ReceiveNext();
            angle = 0f;
            transform.rotation = networkRotation;
            OnReading -= FirstReadRotation;
            OnReading += ReadRotation;
        }

        private void ReadRotation(PhotonStream s, PhotonMessageInfo info)
        {
            networkRotation = (Quaternion)s.ReceiveNext();
            angle = Quaternion.Angle(transform.rotation, networkRotation);

        }

        private void FirstReadScale(PhotonStream s, PhotonMessageInfo info)
        {
            transform.localScale = (Vector3)s.ReceiveNext();
        }

    }
}
