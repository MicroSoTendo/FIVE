using FIVE.Network.Serializers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

namespace FIVE.Network
{
    internal class HostSyncHandler : SyncHandler
    {
        private static readonly ConcurrentDictionary<int, TcpClient> ConnectedTcpClients =
            new ConcurrentDictionary<int, TcpClient>();
        private static int GetNextPlayerID()
        {
            int i = 1;
            while (ConnectedTcpClients.ContainsKey(i))
            {
                i++;
            }

            return i;
        }

        public HostSyncHandler(TcpClient tcpClient) : base(tcpClient)
        {
            int newID = GetNextPlayerID();
            ConnectedTcpClients.TryAdd(newID, tcpClient);
        }

        public override void Run()
        {
        }

        public override void Stop()
        {
            throw new NotImplementedException();
        }

        private void PreSync()
        {
            //Send all existed gameobjects
            Dictionary<int, GameObject>.ValueCollection gameObjects = SyncCenter.Instance.NetworkedGameObjects.Values;
            Stream.Write(gameObjects.Count.ToBytes());
            foreach (GameObject go in gameObjects)
            {
                NetworkView nv = go.GetComponent<NetworkView>();
                byte[] buffer = new byte[sizeof(int) * 2 + nv.serializedSize];
                buffer.CopyFrom(nv.prefabID, nv.networkID, 0);
                Serializer.Serialize(nv.syncedComponents, buffer, sizeof(int) * 2);
            }
        }
    }
}
