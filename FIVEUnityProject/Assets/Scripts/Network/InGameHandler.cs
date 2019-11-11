using System;
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
        protected readonly TcpClient Client;
        protected readonly NetworkStream Stream;
        protected readonly Dictionary<GameSyncCode, Action<GameSyncCode>> GameSyncHandlers;
        protected InGameHandler(TcpClient client)
        {
            Client = client;
            Stream = client.GetStream();
            GameSyncHandlers = new Dictionary<GameSyncCode, Action<GameSyncCode>>
            {
                {GameSyncCode.AliveTick, AliveTickHandler},
                {GameSyncCode.CreateObject, CreateObjectHandler },
                {GameSyncCode.RemoveObject, RemoveObjectHandler },
                {GameSyncCode.ComponentSync, ComponentSyncHandler },
                {GameSyncCode.RemoteCall, RemoteCallHandler }
            };

        }

        protected static unsafe T CastEnum<T>(byte[] bytes) where T : unmanaged
        {
            fixed (byte* pBytes = bytes)
                return *(T*)pBytes;
        }

        protected int GetCount(GameSyncCode syncCode)
        {
            int count = 1;
            if ((syncCode & GameSyncCode.MultipleObjects) != 0)
            {
                count = Stream.ReadI32();
            }
            return count;
        }

        protected abstract void RemoteCallHandler(GameSyncCode syncCode);
        protected abstract void CreateObjectHandler(GameSyncCode syncCode);
        protected abstract void RemoveObjectHandler(GameSyncCode syncCode);
        protected abstract void ComponentSyncHandler(GameSyncCode syncCode);
        protected abstract void AliveTickHandler(GameSyncCode syncCode);

        public static InGameHandler CreateClientHandler(TcpClient client)
        {
            return new GameClient(client);
        }

        public static InGameHandler CreateHostHandler(TcpClient client)
        {
            return new GameHost(client);
        }

        private class GameHost : InGameHandler
        {
            public GameHost(TcpClient client) : base(client) {}

            private async Task OnSend()
            {

            }


            protected override async Task Handler()
            {
                //TODO: Possible exceptions
                //Phase 1: send all existed objects
                ICollection<GameObject> networkedGameObjects = SyncCenter.Instance.GameObjectToSyncedComponents.Keys;
                Stream.Write(networkedGameObjects.Count);
                foreach (GameObject networkedGameObject in networkedGameObjects)
                {
                    List<Component> components = SyncCenter.Instance.GameObjectToSyncedComponents[networkedGameObject];
                    int prefabID = PrefabPool.Instance[networkedGameObject];
                    int componentsCount = components.Count;
                    int componentsBufferSize = SyncCenter.Instance.SyncedObjectBufferSize[networkedGameObject];
                    Serializer.Serialize(components, out byte[] buffer);
                    Stream.Write(ToBytes(prefabID, componentsCount, componentsBufferSize));
                    Stream.Write(buffer);
                }
                
                //Phase 2: do sync
                {
                    while (Client.Connected)
                    {
                        //Receive changed components
                        int length = Stream.ReadI32();
                        for (int i = 0; i < length; i++)
                        {
                           byte[] buffer = new byte[Stream.ReadI32()];
                           Stream.Read(buffer);
                        }
                        //Wait Received All
                        //Send changed components
                        int count = Stream.ReadI32();
                        for (int i = 0; i < count; i++)
                        {
                            byte[] buffer = new byte[Stream.ReadI32()];
                            Stream.Read(buffer);
                            MainThreadRequest mainThreadRequest = new SyncComponent(buffer);
                            NetworkManager.Instance.Submit(mainThreadRequest);
                        }
                    }
                }
                if (!Client.Connected)
                {
                    //TODO: Handle disconnect
                }
            }

            protected override void RemoteCallHandler(GameSyncCode syncCode)
            {
                throw new NotImplementedException();
            }

            protected override void CreateObjectHandler(GameSyncCode syncCode)
            {
                throw new NotImplementedException();
            }

            protected override void RemoveObjectHandler(GameSyncCode syncCode)
            {
                throw new NotImplementedException();
            }

            protected override void ComponentSyncHandler(GameSyncCode syncCode)
            {
                throw new NotImplementedException();
            }

            protected override void AliveTickHandler(GameSyncCode syncCode)
            {
                throw new NotImplementedException();
            }
        }

        private class GameClient : InGameHandler
        {
            private int timeOut = 0;
            public GameClient(TcpClient client) : base(client){}
            

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


            private void Initialize()
            {
                int count = Stream.ReadI32();
                for (int i = 0; i < count; i++)
                {
                    byte[] buffer = Stream.Read(12);
                    int prefabID = buffer.ToI32();
                    int componentCount = buffer.ToI32(4);
                    int componentBufferSize = buffer.ToI32(8);
                    // ComponentType|ComponentData - ComponentType|ComponentData - ...
                    byte[] componentData = Stream.Read(componentBufferSize);
                    MainThreadRequest mainThreadRequest = new CreateObject(prefabID, -1, componentCount, componentData,
                        ActionScope.Local);
                    NetworkManager.Instance.Submit(mainThreadRequest);
                }
            }

            protected override void RemoteCallHandler(GameSyncCode syncCode)
            {
                int count = GetCount(syncCode);
            }

            protected override void CreateObjectHandler(GameSyncCode syncCode)
            {
                int count = GetCount(syncCode);
                var createObjects = new List<CreateObject>();
                for (int i = 0; i < count; i++)
                {
                    int prefabID = Stream.ReadI32();
                    int parentNetworkID = Stream.ReadI32();
                    int componentsCount = Stream.ReadI32();
                    int componentsSize = Stream.ReadI32();
                    byte[] componentsBytes = new byte[componentsSize];
                    Stream.Read(componentsBytes);
                    createObjects.Add(new CreateObject(prefabID, parentNetworkID, componentsCount, componentsBytes, ActionScope.Local));
                }
                if (count == 1)
                {
                    NetworkManager.Instance.Submit(createObjects[0]);
                }
                else
                {
                    NetworkManager.Instance.Submit(new CreateObjects(createObjects));
                }
            }

            protected override void RemoveObjectHandler(GameSyncCode syncCode)
            {
                int count = GetCount(syncCode);
                for (int i = 0; i < count; i++)
                {
                    int networkID = Stream.ReadI32();

                }
            }

            protected override void ComponentSyncHandler(GameSyncCode syncCode)
            {
                int count = GetCount(syncCode);
                for (int i = 0; i < count; i++)
                {
                    
                }
            }
            
            protected override void AliveTickHandler(GameSyncCode syncCode)
            {
                timeOut = 0;
            }


            private async Task OnReceive()
            {
                byte[] headerBuffer = new byte[4];
                await Stream.ReadAsync(headerBuffer, 0, 4);
                GameSyncCode code = CastEnum<GameSyncCode>(headerBuffer);
                foreach (KeyValuePair<GameSyncCode, Action<GameSyncCode>> kvp in GameSyncHandlers)
                {
                    if ((code & kvp.Key) != 0)
                    {
                        kvp.Value(code);
                    }
                }

            }

            private async Task OnSend()
            {

            }


            protected override async Task Handler()
            {
                //Phase 1: fetch all existed objects
                Initialize();
                //Phase 2: do sync
                Task receiveTasks = OnReceive();
                Task sendTasks = OnSend();
                await Task.WhenAll(receiveTasks, sendTasks);
                while (true)
                {
                    //Send changed components
                    byte[][] components = SyncCenter.Instance.GetComponentsForSync();
                    Stream.Write(components.Length);
                    foreach (byte[] buffer in components)
                    {
                        Stream.Write(buffer.Length);
                        Stream.Write(buffer);
                    }
                    //Receive changed components
                    int count = Stream.ReadI32();
                    for (int i = 0; i < count; i++)
                    {
                        byte[] buffer = new byte[Stream.ReadI32()];
                        Stream.Read(buffer);
                        MainThreadRequest mainThreadRequest = new SyncComponent(buffer);
                        NetworkManager.Instance.Submit(mainThreadRequest);
                    }
                }
            }
        }
    }
}