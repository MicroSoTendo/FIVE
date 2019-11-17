using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FIVE.FIVE.Network;
using FIVE.Network.Serializers;
using UnityEngine;

namespace FIVE.Network
{
    public abstract class MainThreadTask
    {
        public abstract void Run();
    }

    public enum ActionScope
    {
        Global,
        Local,
    }

    public class CreateObject : MainThreadTask
    {
        public int PrefabID { get; }
        public int Parent { get; }
        public int ComponentCount { get; }
        public byte[] ComponentData { get; }
        public ActionScope Scope { get; }
        public CreateObject(int prefabID, int parent, int componentCount, byte[] componentData, ActionScope scope)
        {
            PrefabID = prefabID;
            Parent = parent;
            ComponentData = componentData;
            Scope = scope;
            ComponentCount = componentCount;
        }

        public override void Run()
        {
            GameObject gameObject = PrefabID == -1 ?
                PrefabPool.Instance.Instantiate(PrefabID) :
                PrefabPool.Instance.Instantiate(PrefabID, SyncCenter.Instance.NetworkedGameObjects[PrefabID].transform);
        }
    }

    public class CreateObjects : MainThreadTask
    {
        public List<CreateObject> BatchCreateObjects { get; }
        public CreateObjects(List<CreateObject> batchCreateObjects)
        {
            BatchCreateObjects = batchCreateObjects;
        }

        public override void Run()
        {
            foreach (CreateObject createObject in BatchCreateObjects)
            {
                createObject.Run();
            }
        }
    }

    public class RemoveObjects : MainThreadTask
    {
        public RemoveObjects(List<RemoveObjects> batchRemoveObjects)
        {
            BatchRemoveObjects = batchRemoveObjects;
        }

        public List<RemoveObjects> BatchRemoveObjects { get; }
        public override void Run()
        {
            foreach (RemoveObjects removeObject in BatchRemoveObjects)
            {
                removeObject.Run();
            }
        }
    }

    public class RemoveObject : MainThreadTask
    {
        public RemoveObject(int networkID)
        {
            NetworkID = networkID;
        }

        public int NetworkID { get; }
        public override void Run()
        {
            SyncCenter.Instance.Destroy(NetworkID);
        }
    }

    public class SyncComponent : MainThreadTask
    {
        public byte[] RawBytes { get; }
        public SyncComponent(byte[] buffer)
        {
            RawBytes = buffer;
        }


        public override unsafe void Run()
        {
            fixed (byte* pBytes = RawBytes)
            {
                int gameObjectID = *(int*)pBytes;
                GameObject go = SyncCenter.Instance.NetworkedGameObjects[gameObjectID];
                int count = *((int*)pBytes + 1);
                int offset = 8;
                for (int i = 0; i < count; i++)
                {
                    var type = (ComponentType)(*(pBytes + offset));
                    offset += 1;
                    switch (type)
                    {
                        case ComponentType.Transform:
                            Serializer<Transform>.Instance.Deserialize(pBytes + offset, go.transform);
                            offset += 24;
                            break;
                        case ComponentType.Animator:
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }
    }

    public class RemoteCall : MainThreadTask
    {
        private static readonly BijectMap<int, MethodInfo> RpcInfos;
        static RemoteCall()
        {
            var methods = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                           from type in assembly.GetTypes()
                           from method in type.GetMethods()
                           where method.GetCustomAttributes(typeof(RPCAttribute), false).Length > 0
                           select method).ToList();
            if (methods.Count > 0)
            {
                int counter = 0;
                RpcInfos = new BijectMap<int, MethodInfo>();
                methods.Sort();
                foreach (MethodInfo methodInfo in methods)
                {
                    RpcInfos.Add(counter++, methodInfo);
                }
            }
        }

        public RemoteCall(int networkID, int rpcID)
        {
            NetworkID = networkID;
            RpcID = rpcID;
        }
        public int NetworkID { get; }
        public int RpcID { get; }
        public override void Run()
        {
            object obj = SyncCenter.Instance.NetworkIDMap[NetworkID];
            MethodInfo method = RpcInfos[RpcID];
            method.Invoke(obj, null);
        }
    }

}