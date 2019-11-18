using FIVE.Network.Serializers;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using static FIVE.Network.NetworkUtil;

namespace FIVE.Network.InGameHandlers
{
    internal partial class InGameGameHandler
    {

        //public static InGameGameHandler CreateHostHandler(TcpClient client)
        //{
        //    return new Host(client);
        //}

        //private class Host : InGameGameHandler
        //{
        //    public Host(TcpClient client) : base(client) { }


        //    protected async Task Handler2()
        //    {
        //        //TODO: Possible exceptions
        //        //Phase 1: send all existed objects
        //        ICollection<GameObject> networkedGameObjects = SyncCenter.Instance.GameObjectToSyncedComponents.Keys;
        //        Stream.Write(networkedGameObjects.Count);
        //        foreach (GameObject networkedGameObject in networkedGameObjects)
        //        {
        //            List<Component> components = SyncCenter.Instance.GameObjectToSyncedComponents[networkedGameObject];
        //            int prefabID = PrefabPool.Instance[networkedGameObject];
        //            int componentsCount = components.Count;
        //            int componentsBufferSize = SyncCenter.Instance.SyncedObjectBufferSize[networkedGameObject];
        //            //Serializer.Serialize(components, out byte[] buffer);
        //            Stream.Write(ToBytes(prefabID, componentsCount, componentsBufferSize));
        //            //Stream.Write(buffer);
        //        }

        //        //Phase 2: do sync
        //        {
        //            while (TcpClient.Connected)
        //            {
        //                //Receive changed components
        //                int length = Stream.ReadI32();
        //                for (int i = 0; i < length; i++)
        //                {
        //                    byte[] buffer = new byte[Stream.ReadI32()];
        //                    Stream.Read(buffer);
        //                }
        //                //Wait Received All
        //                //Send changed components
        //                int count = Stream.ReadI32();
        //                for (int i = 0; i < count; i++)
        //                {
        //                    byte[] buffer = new byte[Stream.ReadI32()];
        //                    Stream.Read(buffer);
        //                    MainThreadTask mainThreadTask = new SyncComponent(buffer);
        //                    NetworkManager.Instance.Schedule(mainThreadTask);
        //                }
        //            }
        //        }
        //        if (!TcpClient.Connected)
        //        {
        //            //TODO: Handle disconnect
        //        }
        //    }

        //    protected override void PreSync()
        //    {
        //        Dictionary<int, GameObject>.ValueCollection gameObjects = SyncCenter.Instance.NetworkedGameObjects.Values;
        //        Stream.Write(gameObjects.Count.ToBytes());
        //        foreach (GameObject go in gameObjects)
        //        {
        //            NetworkView nv = go.GetComponent<NetworkView>();
        //            byte[] buffer = new byte[sizeof(int) * 2 + nv.serializedSize];
        //            buffer.CopyFrom(nv.prefabID, nv.networkID, 0);
        //            Serializer.Serialize(nv.syncedComponents, buffer, sizeof(int) * 2);
        //        }
        //    }

        //    protected override void OnSend()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    protected override void OnReceive()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public override void Start()
        //    {
        //        throw new NotImplementedException();
        //    }

        //    public override void Stop()
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
    }
}