using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace FIVE.Network
{
    public class ClientSyncHandler : SyncHandler
    {

        private Action onUpdate;
        public ClientSyncHandler(TcpClient tcpClient) : base(tcpClient)
        {
            onUpdate = PreSync;
        }

        private void PreSync()
        {
            if (!TryRead(out byte[] data))
            {
                return;
            }
            //Receive objects from host
            int objectsCount = data.As<int>();
            for (int i = 0; i < objectsCount; i++)
            {
                if (TryRead(out byte[] buffer))
                {
                    int prefabID = buffer.As<int>();
                    int networkID = buffer.As<int>(4);
                    GameObject go = PrefabPool.Instance.Instantiate(prefabID);
                    go.AddComponent<NetworkView>().DeserializeFrom(buffer);
                    SyncCenter.Instance.RegisterFromRemote(go, networkID);
                }
            }
            onUpdate = DoSync;
        }

        private void DoSync()
        {
            OnSend();
            OnReceive();
        }

        private void OnSend()
        {
            SendCreateObject();
            SendRemoveObject();
            SendComponentSync();
            SendRemoteCall();
        }

        private void SendRemoteCall()
        {
            foreach ((int networkID, int rpcID) in SyncCenter.Instance.GetScheduledCalls())
            {
                //TODO:
            }
        }

        private void SendComponentSync()
        {
            foreach (NetworkView nv in SyncCenter.Instance.NetworkViews.Values)
            {
                foreach (byte[] buffer in nv.serializedComponent)
                {
                    Send(buffer);
                }
            }
        }

        private void SendRemoveObject()
        {
        }

        private void SendCreateObject()
        {
            
        }

        private void OnReceive()
        {
            TryRead(out byte[] buffer);
        }



        protected override void DoUpdate()
        {
            onUpdate();
        }
    }
}