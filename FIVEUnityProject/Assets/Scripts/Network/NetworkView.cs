using System;
using System.Collections.Generic;
using FIVE.Network.Serializers;
using UnityEngine;

namespace FIVE.Network
{
    public class NetworkView : MonoBehaviour
    {
        internal int networkID;
        internal List<Component> syncedComponents;
        internal List<byte[]> serializedComponents;
        public void Awake()
        {
            syncedComponents = new List<Component>();
            serializedComponents = new List<byte[]>();
        }

        public void LateUpdate()
        {
            serializedComponents.Add(networkID.ToBytes());
            serializedComponents.Add(0.ToBytes());
            int count = 0;
            foreach (Component syncedComponent in syncedComponents)
            {
                switch (syncedComponent)
                {
                    case Transform t:
                        if (t.hasChanged)
                        {
                            Serializer<Transform>.Instance.Serialize(t, out byte[] bytes);
                            serializedComponents.Add(bytes);
                        }
                        break;
                    default:
                        break;
                }
                count++;
            }

            if (count > 0)
            {
                serializedComponents[1] = count.ToBytes();
                SyncCenter.Instance.AddComponentsForSync(NetworkUtil.CombineUnsafe(serializedComponents));
            }

            serializedComponents.Clear();
        }
    }
}