using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Network
{
    public sealed class SyncCenter
    {
        public static SyncCenter Instance { get; } = new SyncCenter();
        private SyncCenter() { }


        //Used only if host
        public ConcurrentDictionary<int, List<GameObject>> ClientID2GameObjects { get; } = new ConcurrentDictionary<int, List<GameObject>>();
        public ConcurrentDictionary<GameObject, int> GameObject2ClientsID { get; } = new ConcurrentDictionary<GameObject, int>();


        public List<Component> SyncedComponentsOwned { get; } = new List<Component>();
        public List<Component> SyncedComponentsRemote { get; } = new List<Component>();

        public BijectMap<int, Component> IDSyncedComponent { get; } = new BijectMap<int, Component>();

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

            IDSyncedComponent.Add(IDSyncedComponent.Count, component);
        }

    }
}
