using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace FIVE.Network
{
    internal class HostSyncHandler : SyncHandler
    {
        #region Static Members
        private static readonly ConcurrentDictionary<int, TcpClient> ConnectedTcpClients =
            new ConcurrentDictionary<int, TcpClient>();
        private static readonly ConcurrentDictionary<int, (Task readTask, Task sendTask)> SyncTasks = new ConcurrentDictionary<int, (Task readTask, Task sendTask)>();
        private static int GetNextPlayerID()
        {
            int i = 1;
            while (ConnectedTcpClients.ContainsKey(i))
            {
                i++;
            }
            return i;
        }
        #endregion

        private readonly int ID;

        private Action onUpdate;
        public HostSyncHandler(TcpClient tcpClient) : base(tcpClient)
        {
            ID = GetNextPlayerID();
            ConnectedTcpClients.TryAdd(ID, tcpClient);
            onUpdate = PreSync;
            handlers = new Action<byte[], int>[]
            {
                CreateObject,
                RemoveObject,
                ComponentSync,
                RPC,
            };
        }
        private readonly Action<byte[], int>[] handlers;
        private void PreSync()
        {
            //Send all existed gameobjects
            Dictionary<int, GameObject>.ValueCollection gameObjects = SyncCenter.Instance.NetworkedGameObjects.Values;
            Send(gameObjects.Count.ToBytes());
            foreach (GameObject go in gameObjects)
            {
                NetworkView nv = go.GetComponent<NetworkView>();
                Send(nv.SerializeAll());
            }
            onUpdate = DoSync;
        }

        private void DoSync()
        {
            OnReceive();
            OnSend();
        }

        private void OnReceive()
        {
        }

        private void OnSend()
        {

        }

        private void ResolvePakcet(byte[] packet)
        {
            GameSyncHeader header = packet.As<GameSyncHeader>();
            handlers[(ushort)header](packet, 2);
        }

        private void CreateObject(byte[] buffer, int offset = 0)
        {
            int prefabID = buffer.As<int>(offset);
            int networkID = buffer.As<int>(offset + 4);
            GameObject go = PrefabPool.Instance.Instantiate(prefabID);
            NetworkView nv = go.AddComponent<NetworkView>();
            nv.networkID = networkID;
            nv.prefabID = prefabID;
            nv.DeserializeFrom(buffer);
            SyncCenter.Instance.RegisterFromRemote(go, networkID);
            //TODO: Send this to other clients
        }

        private void RemoveObject(byte[] buffer, int offset = 0)
        {

        }

        private void ComponentSync(byte[] buffer, int offset = 0)
        {

        }

        private void RPC(byte[] buffer, int offset = 0)
        {
            int rpcCode = buffer.As<int>(offset);
            int id = buffer.As<int>(offset + 4);
            object go = SyncCenter.Instance.NetworkIDMap[id];
            NetworkManager.Instance.RpcInfos[rpcCode].Invoke(go, null);
        }

        protected override void DoUpdate()
        {
            onUpdate();
        }
    }
}
