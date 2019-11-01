using UnityEngine;

namespace FIVE.Network
{
    public abstract class MainThreadRequest
    {

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
        public CreateObject(int prefabID, int parent,  int componentCount, byte[] componentData, ActionScope scope)
        {
            PrefabID = prefabID;
            Parent = parent;
            ComponentData = componentData;
            Scope = scope;
            ComponentCount = componentCount;
        }
    }

    public class RemoveObject : MainThreadRequest
    {
        public RemoveObject(int networkID)
        {
            NetworkID = networkID;
        }

        public int NetworkID { get; }
    }

    public class SyncComponent : MainThreadRequest
    {
        public byte[] RawBytes { get; }
        public SyncComponent(byte[] buffer)
        {
            RawBytes = buffer;
        }

        
    }

    public class FunctionCall : MainThreadRequest
    {

    }

}