using System;
using System.Collections.Generic;
using FIVE.Network.Serializers;
using UnityEngine;

namespace FIVE.Network
{
    public class NetworkView : MonoBehaviour
    {
        public int prefabID;
        public int networkID;
        public List<(int componentID, Component component)> syncedComponents;
        public int serializedSize;

        public void Awake()
        {
            syncedComponents = new List<(int componentID, Component component)>();
        }

        public byte[] Serialize()
        {
            return default;
        }

        public void DeserializeFrom(byte[] buffer)
        {

        }

        public void LateUpdate()
        {
            // serializedComponents.Add(networkID.ToBytes());
            // serializedComponents.Add(0.ToBytes());
            // int count = 0;
            // foreach (Component syncedComponent in syncedComponents)
            // {
            //     switch (syncedComponent)
            //     {
            //         case Transform t:
            //             if (t.hasChanged)
            //             {
            //                 Serializer<Transform>.Instance.Serialize(t, out byte[] bytes);
            //                 serializedComponents.Add(bytes);
            //             }
            //             break;
            //         default:
            //             break;
            //     }
            //     count++;
            // }

            // if (count > 0)
            // {
            //     serializedComponents[1] = count.ToBytes();
            //     SyncCenter.Instance.AddComponentsForSync(NetworkUtil.CombineUnsafe(serializedComponents));
            // }

            // serializedComponents.Clear();
        }
    }
}