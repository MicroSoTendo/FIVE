using System;
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
            onUpdate = DoSync;
        }

        private void DoSync()
        {
            OnWrite();
            OnRead();
        }

        private void OnRead()
        {
            GameSyncHeader header = Read().As<GameSyncHeader>();
            switch (header)
            {
                case GameSyncHeader.JoinRequest:
                    break;
                case GameSyncHeader.AcceptJoin:
                    break;
                case GameSyncHeader.CreateObject:
                    break;
                case GameSyncHeader.RemoveObject:
                    break;
                case GameSyncHeader.ComponentSync:
                    break;
                case GameSyncHeader.RemoteCall:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void OnWrite()
        {

        }

        public override void Update()
        {
            onUpdate();
        }
    }
}