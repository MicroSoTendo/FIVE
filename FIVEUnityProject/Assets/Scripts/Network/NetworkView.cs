using System;
using FIVE.Network.Serializers;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Network
{
    public class NetworkView : MonoBehaviour
    {
        public int prefabID;
        public int networkID;
        public List<(int componentID, Component component)> syncedComponents;
        public int serializedSize;
        public List<byte[]> serializedComponent;
        public void Awake()
        {
            syncedComponents = new List<(int componentID, Component component)>();
            serializedComponent = new List<byte[]>();
        }

        public byte[] SerializeAll()
        {
            byte[] buffer = new byte[serializedSize];
            prefabID.CopyTo(buffer, 0);
            networkID.CopyTo(buffer, 4);
            Serializer.Serialize(syncedComponents, buffer, 8);
            return buffer;
        }

        public void DeserializeFrom(byte[] buffer)
        {
            Serializer.Deserialize(gameObject, buffer);
        }

        public void LateUpdate()
        {
            serializedComponent.Clear();
            foreach ((int componentID, Component component) in syncedComponents)
            {
                if (Serializer.TrySerialize(componentID, component, out byte[] buffer))
                    serializedComponent.Add(buffer);
            }
        }
    }
}