using FIVE.Network.Serializers;
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

        private readonly int ID;

        private Action OnUpdate;
        public HostSyncHandler(TcpClient tcpClient) : base(tcpClient)
        {
            ID = GetNextPlayerID();
            ConnectedTcpClients.TryAdd(ID, tcpClient);
            OnUpdate = PreSync;
        }


        private void PreSync()
        {
            //Send all existed gameobjects
            Dictionary<int, GameObject>.ValueCollection gameObjects = SyncCenter.Instance.NetworkedGameObjects.Values;
            Send(gameObjects.Count.ToBytes());
            foreach (GameObject go in gameObjects)
            {
                NetworkView nv = go.GetComponent<NetworkView>();
                Send(nv.Serialize());
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
