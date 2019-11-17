using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace FIVE.Network.InGameHandlers
{
    internal abstract partial class InGameGameHandler
    {
        public static InGameGameHandler CreateClientHandler(TcpClient client)
        {
            return new GameGameClient(client);
        }

        private class GameGameClient : InGameGameHandler
        {
            private int timeOut = 0;
            public GameGameClient(TcpClient client) : base(client){}
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


            protected  void RemoteCallHandler()
            {
            }

            protected  void CreateObjectHandler()
            {
                int count = Stream.ReadI32();
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
                    NetworkManager.Instance.Schedule(createObjects[0]);
                }
                else
                {
                    NetworkManager.Instance.Schedule(new CreateObjects(createObjects));
                }
            }

            protected void RemoveObjectHandler()
            {
                int count = Stream.ReadI32();
                for (int i = 0; i < count; i++)
                {
                    int networkID = Stream.ReadI32();
                }
            }

            protected void ComponentSyncHandler()
            {
                int count = Stream.ReadI32();
                for (int i = 0; i < count; i++)
                {
                    
                }
            }


            protected override void PreSync()
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
                    MainThreadTask mainThreadTask = new CreateObject(prefabID, -1, componentCount, componentData,
                        ActionScope.Local);
                    NetworkManager.Instance.Schedule(mainThreadTask);
                }
            }

            protected override void OnSend()
            {
                throw new NotImplementedException();
            }

            protected override void OnReceive()
            {
                throw new NotImplementedException();
            }


            protected override async Task Handler()
            {
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
                        MainThreadTask mainThreadTask = new SyncComponent(buffer);
                        NetworkManager.Instance.Schedule(mainThreadTask);
                    }
                }
            }
        }
    }
}