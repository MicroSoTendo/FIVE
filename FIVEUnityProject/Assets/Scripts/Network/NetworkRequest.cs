using UnityEngine;

namespace FIVE.Network
{
    public abstract class NetworkRequest
    {

    }
    
    public enum ActionScope
    {
        Global,
        Local,
    }

    public class CreateObject : NetworkRequest
    {
        public int PrefabID { get; }
        public int Parent { get; }
        public byte[] ComponentBuffer { get; }
        public ActionScope Scope { get; }
        public CreateObject(int prefabID, int parent, byte[] componentBuffer, ActionScope scope)
        {
            PrefabID = prefabID;
            Parent = parent;
            ComponentBuffer = componentBuffer;
            Scope = scope;
        }
    }

    public class RemoveObject : NetworkRequest
    {
        public RemoveObject(int networkID)
        {
            NetworkID = networkID;
        }

        public int NetworkID { get; }
    }

    public class SyncObject : NetworkRequest
    {

    }

    public class FunctionCall : NetworkRequest
    {

    }

}