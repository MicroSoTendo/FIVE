using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FIVE.FIVE.Network;
using FIVE.Network.Serializers;
using UnityEngine;

namespace FIVE.Network
{
    public abstract class MainThreadRequest
    {
        public abstract void Resolve();
    }

    public enum ActionScope
    {
        Global,
        Local,
    }

    public class CreateObject : MainThreadRequest
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

        public override void Resolve()
        {
            GameObject gameObject = PrefabID == -1 ?
                PrefabPool.Instance.Instantiate(PrefabID) :
                PrefabPool.Instance.Instantiate(PrefabID, SyncCenter.Instance.NetworkedGameObjects[PrefabID].transform);
        }
    }

    public class CreateObjects : MainThreadRequest
    {
        public List<CreateObject> BatchCreateObjects { get; }
        public CreateObjects(List<CreateObject> batchCreateObjects)
        {
            BatchCreateObjects = batchCreateObjects;
        }

        public override void Resolve()
        {
            foreach (CreateObject createObject in BatchCreateObjects)
            {
                createObject.Resolve();
            }
        }
    }

    public class RemoveObjects : MainThreadRequest
    {
        public RemoveObjects(List<RemoveObjects> batchRemoveObjects)
        {
            BatchRemoveObjects = batchRemoveObjects;
        }

        public List<RemoveObjects> BatchRemoveObjects { get; }
        public override void Resolve()
        {
            foreach (RemoveObjects removeObject in BatchRemoveObjects)
            {
                removeObject.Resolve();
            }
        }
    }

    public class RemoveObject : MainThreadRequest
    {
        public RemoveObject(int networkID)
        {
            NetworkID = networkID;
        }

        public int NetworkID { get; }
        public override void Resolve()
        {
            SyncCenter.Instance.Destroy(NetworkID);
        }
    }

    public class SyncComponent : MainThreadRequest
    {
        public byte[] RawBytes { get; }
        public SyncComponent(byte[] buffer)
        {
            RawBytes = buffer;
        }


        public override unsafe void Resolve()
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

    public class RemoteCall : MainThreadRequest
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
        public override void Resolve()
        {
            object obj = SyncCenter.Instance.NetworkIDMap[NetworkID];
            MethodInfo method = RpcInfos[RpcID];
            method.Invoke(obj, null);
        }
    }

}