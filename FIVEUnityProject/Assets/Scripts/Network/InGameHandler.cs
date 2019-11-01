using System.Collections.Concurrent;
using FIVE.Network.Serializers;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
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
                    int componentsCount = components.Count;
                    int componentsBufferSize = SyncCenter.Instance.SyncedObjectBufferSize[networkedGameObject];
                    Serializer.Serialize(components, out byte[] buffer);
                    stream.Write(ToBytes(prefabID, componentsCount, componentsBufferSize));
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

            private static unsafe byte[] SizeHelper(ConcurrentBag<byte[]> componentsForSync)
            {
                byte[] bytes = new byte[componentsForSync.Count * 4];
                int i = 0;
                fixed (byte* pBytes = bytes)
                {
                    foreach (byte[] b in componentsForSync)
                    {
                        *(int*)(pBytes + i) = b.Length;
                        i += 4;
                    }
                }
                return bytes;
            }

            protected override Task Handler()
            {
                NetworkStream stream = client.GetStream();
                //Phase 1: fetch all existed objects
                {
                    int count = stream.ReadI32();
                    for (int i = 0; i < count; i++)
                    {
                        byte[] buffer = stream.Read(12);
                        int prefabID = buffer.ToI32();
                        int componentCount = buffer.ToI32(4);
                        int componentBufferSize = buffer.ToI32(8);
                        // ComponentType|ComponentData - ComponentType|ComponentData - ...
                        byte[] componentData = stream.Read(componentBufferSize);
                        MainThreadRequest mainThreadRequest = new CreateObject(prefabID, -1, componentCount, componentData,
                            ActionScope.Local);
                        NetworkManager.Instance.Submit(mainThreadRequest);
                    }
                }
                //Phase 2: do sync
                {
                    while (true)
                    {
                        //Send changed components
                        byte[][] components = SyncCenter.Instance.GetComponentsForSync();
                        stream.Write(components.Length);
                        foreach (byte[] buffer in components)
                        {
                            stream.Write(buffer.Length);
                            stream.Write(buffer);
                        }
                        //Receive changed components
                        int count = stream.ReadI32();
                        for (int i = 0; i < count; i++)
                        {
                            byte[] buffer = new byte[stream.ReadI32()];
                            stream.Read(buffer);
                            MainThreadRequest mainThreadRequest = new SyncComponent(buffer);
                            NetworkManager.Instance.Submit(mainThreadRequest);
                        }
                    }
                }
            }
        }
    }
}