using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using FIVE.Network.Serializers;
using UnityEngine;
using static FIVE.Network.NetworkUtil;
namespace FIVE.Network
{
    internal abstract class InGameHandler : NetworkHandler
    {
        public static InGameHandler CreateClient(TcpClient client)
        {
            return new GameClient(client);
        }

        public static InGameHandler CreateHost(TcpClient client)
        {
            return new GameHost(client);
        }


        private class GameHost : InGameHandler
        {
            private readonly TcpClient client;

            public GameHost(TcpClient client)
            {
                this.client = client;
            }
            protected override Task Handler()
            {
                NetworkStream stream = client.GetStream();
                //Phase 1: send all existed objects
                ICollection<GameObject> networkedGameObjects = SyncCenter.Instance.GameObjectToSyncedComponents.Keys;
                stream.Write(networkedGameObjects.Count);
                foreach (GameObject networkedGameObject in networkedGameObjects)
                {
                    List<Component> components = SyncCenter.Instance.GameObjectToSyncedComponents[networkedGameObject];
                    int prefabID = PrefabPool.Instance[networkedGameObject];
                    int count = components.Count;
                    //TODO: Move buffer calculation into serializer
                    int componentBufferSize = components.Sum(component => Serializer.GetSize(component.GetType()));
                    stream.Write(GetBytes(prefabID, count, componentBufferSize));
                    byte[] buffer = new byte[componentBufferSize + sizeof(int) * count];
                    Serializer.Serialize(components, buffer);
                    stream.Write(buffer);
                }

                //Phase 2: do sync
                while (true)
                {
                }
            }
        }

        private class GameClient : InGameHandler
        {
            private readonly TcpClient client;
            public GameClient(TcpClient client)
            {
                this.client = client;
            }
            protected override Task Handler()
            {
                NetworkStream stream = client.GetStream();
                //Phase 1: fetch all existed objects
                int count = stream.ReadI32();
                for (int i = 0; i < count; i++)
                {
                    byte[] buffer = stream.Read(sizeof(int) * 3);
                    int prefabID = buffer.ToI32();
                    int componentCount = buffer.ToI32(sizeof(int));
                    int componentBufferSize = buffer.ToI32(sizeof(int) * 2);
                    // ComponentType|ComponentData - ComponentType|ComponentData - ...
                    byte[] componentBuffer = stream.Read(componentBufferSize);
                    NetworkRequest networkRequest = new CreateObject(prefabID, -1, componentBuffer, ActionScope.Local);
                    NetworkManager.Instance.Submit(networkRequest);
                }

                //Phase 2: do sync
                while (true)
                {
                }

            }
        }
    }
}