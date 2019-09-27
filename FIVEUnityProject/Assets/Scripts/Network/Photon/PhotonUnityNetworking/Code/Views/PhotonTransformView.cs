// ----------------------------------------------------------------------------
// <copyright file="PhotonTransformView.cs" company="Exit Games GmbH">
//   PhotonNetwork Framework for Unity - Copyright (C) 2018 Exit Games GmbH
// </copyright>
// <summary>
//   Component to synchronize Transforms via PUN PhotonView.
// </summary>
// <author>developer@exitgames.com</author>
// ----------------------------------------------------------------------------


namespace Photon.Pun
{
    using UnityEngine;


    [AddComponentMenu("Photon Networking/Photon Transform View")]
    [HelpURL("https://doc.photonengine.com/en-us/pun/v2/gameplay/synchronization-and-state")]
    [RequireComponent(typeof(PhotonView))]
    public class PhotonTransformView : MonoBehaviour, IPunObservable
    {
        private float m_Distance;
        private float m_Angle;

        private PhotonView m_PhotonView;

        private Vector3 m_Direction;
        private Vector3 m_NetworkPosition;
        private Vector3 m_StoredPosition;

        private Quaternion m_NetworkRotation;

        public bool m_SynchronizePosition = true;
        public bool m_SynchronizeRotation = true;
        public bool m_SynchronizeScale = false;
        private bool m_firstTake = false;

        public void Awake()
        {
            m_PhotonView = GetComponent<PhotonView>();

            m_StoredPosition = transform.position;
            m_NetworkPosition = Vector3.zero;

            m_NetworkRotation = Quaternion.identity;
        }

        private void OnEnable()
        {
            m_firstTake = true;
        }

        public void Update()
        {
            if (!m_PhotonView.IsMine)
            {
                transform.position = Vector3.MoveTowards(transform.position, m_NetworkPosition, m_Distance * (1.0f / PhotonNetwork.SerializationRate));
                transform.rotation = Quaternion.RotateTowards(transform.rotation, m_NetworkRotation, m_Angle * (1.0f / PhotonNetwork.SerializationRate));
            }
        }

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                if (m_SynchronizePosition)
                {
                    m_Direction = transform.position - m_StoredPosition;
                    m_StoredPosition = transform.position;

                    stream.SendNext(transform.position);
                    stream.SendNext(m_Direction);
                }

                if (m_SynchronizeRotation)
                {
                    stream.SendNext(transform.rotation);
                }

                if (m_SynchronizeScale)
                {
                    stream.SendNext(transform.localScale);
                }
            }
            else
            {


                if (m_SynchronizePosition)
                {
                    m_NetworkPosition = (Vector3)stream.ReceiveNext();
                    m_Direction = (Vector3)stream.ReceiveNext();

                    if (m_firstTake)
                    {
                        transform.position = m_NetworkPosition;
                        m_Distance = 0f;
                    }
                    else
                    {
                        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                        m_NetworkPosition += m_Direction * lag;
                        m_Distance = Vector3.Distance(transform.position, m_NetworkPosition);
                    }


                }

                if (m_SynchronizeRotation)
                {
                    m_NetworkRotation = (Quaternion)stream.ReceiveNext();

                    if (m_firstTake)
                    {
                        m_Angle = 0f;
                        transform.rotation = m_NetworkRotation;
                    }
                    else
                    {
                        m_Angle = Quaternion.Angle(transform.rotation, m_NetworkRotation);
                    }
                }

                if (m_SynchronizeScale)
                {
                    transform.localScale = (Vector3)stream.ReceiveNext();
                }

                if (m_firstTake)
                {
                    m_firstTake = false;
                }
            }
        }
    }
}