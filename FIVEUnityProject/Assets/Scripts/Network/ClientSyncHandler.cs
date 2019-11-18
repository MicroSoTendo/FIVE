using System;
using System.Net.Sockets;
using UnityEngine;

namespace FIVE.Network
{
    public class ClientSyncHandler : SyncHandler
    {

        private Action OnUpdate;
        public ClientSyncHandler(TcpClient tcpClient) : base(tcpClient)
        {
            OnUpdate = PreSync;
        }
        
        private void PreSync()
        {
            //Receive objects from host
            int objectsCount = Read().As<int>();
            for (int i = 0; i < objectsCount; i++)
            {
                byte[] buffer = Read();
                int prefabID = buffer.As<int>();
                int networkID = buffer.As<int>(4);
                GameObject go = PrefabPool.Instance.Instantiate(prefabID);
                go.AddComponent<NetworkView>().DeserializeFrom(buffer);
                SyncCenter.Instance.RegisterRemote(go, networkID);
            }
            OnUpdate = DoSync;
        }

        private void DoSync()
        {

        }

        public override void Update()
        {
            OnUpdate();
        }
    }
}