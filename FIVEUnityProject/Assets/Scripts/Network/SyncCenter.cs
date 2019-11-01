using FIVE.Network.Serializers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;
using Random = System.Random;

namespace FIVE.Network
{
    public sealed class SyncCenter
    {
        public static SyncCenter Instance { get; } = new SyncCenter();
        private readonly Random random;
        private SyncCenter()
        {
            GameObjectToSyncedComponents = new ConcurrentDictionary<GameObject, List<Component>>();
            SyncedObjectBufferSize = new ConcurrentDictionary<GameObject, int>();
            ClientID2GameObjects = new ConcurrentDictionary<int, List<GameObject>>();
            GameObject2ClientsID = new ConcurrentDictionary<GameObject, int>();
            componentsForSync = new ConcurrentStack<byte[]>();
            NetworkedGameObjects = new BijectMap<int, GameObject>();
            random = new Random(GetHashCode());
        }

        /// <summary>
        /// Used by <b>Host</b> only.
        /// </summary>
        public ConcurrentDictionary<int, List<GameObject>> ClientID2GameObjects { get; }
        /// <summary>
        /// Used by <b>Host</b> only.
        /// </summary>
        public ConcurrentDictionary<GameObject, int> GameObject2ClientsID { get; }


        public ConcurrentDictionary<GameObject, List<Component>> GameObjectToSyncedComponents { get; }
        public ConcurrentDictionary<GameObject, int> SyncedObjectBufferSize { get; }

        public BijectMap<int, GameObject> NetworkedGameObjects { get; } = new BijectMap<int, GameObject>();
        public BijectMap<int, Component> IDSyncedComponent { get; } = new BijectMap<int, Component>();

        private readonly ConcurrentStack<byte[]> componentsForSync;

        public byte[][] GetComponentsForSync()
        {
            return componentsForSync.ToArray();
        }

        public void AddComponentsForSync(byte[] bytes)
        {
            componentsForSync.Push(bytes);
        }

        //Instantiate object in both local and remote games
        public void InstantiateRemote(GameObject prefab, Vector3 position, Quaternion rotation)
        {
        }


        public void Register(Component component)
        {
            if (IDSyncedComponent.Contains(component))
            {
                throw new ArgumentException(component + " already existed.");
            }

            if (!NetworkedGameObjects.Contains(component.gameObject))
            {
                NetworkView networkView = component.gameObject.AddComponent<NetworkView>();
                int newID = random.Next(0, int.MaxValue);
                while (NetworkedGameObjects.Contains(newID))
                {
                    newID =  random.Next(0, int.MaxValue);
                }

                networkView.networkID = newID;
            }

            IDSyncedComponent.Add(IDSyncedComponent.Count, component);
            if (GameObjectToSyncedComponents.TryGetValue(component.gameObject, out List<Component> list))
            {
                list.Add(component);
                SyncedObjectBufferSize[component.gameObject] += Serializer.GetSize(component);
            }
            else
            {
                GameObjectToSyncedComponents.TryAdd(component.gameObject, new List<Component> { component });
                int size = Serializer.GetSize(component);
                SyncedObjectBufferSize.TryAdd(component.gameObject, size);
            }
        }

    }
}
