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
        public Vector3 Position { get; }
        public Vector3 Rotation { get; }
        public int Parent { get; }
        public ActionScope Scope { get; }
        public CreateObject(int prefabID, Vector3 position, Vector3 rotation, int parent, ActionScope scope)
        {
            PrefabID = prefabID;
            Position = position;
            Rotation = rotation;
            Parent = parent;
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



}