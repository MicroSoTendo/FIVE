using System;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Assertions;

namespace FIVE.Network
{
    public class SyncCenter : MonoBehaviour
    {
        private static SyncCenter instance;
        private bool IsHost;
        private void Awake()
        {
            Assert.IsNull(instance);
            instance = this;
        }
        private Dictionary<GameObject, int> networkedObjectToResourceID = new Dictionary<GameObject, int>();
        private Dictionary<GameObject, List<object>> networkedObjectToSynchronizedComponents = new Dictionary<GameObject, List<object>>();
        private Dictionary<int, GameObject> prefabDictionary = new Dictionary<int, GameObject>();

        private Dictionary<int, List<GameObject>> clientIDToSyncedObjects = new Dictionary<int, List<GameObject>>();

        public static List<GameObject> GetClientObjects(int id) => instance.clientIDToSyncedObjects[id];

        //Instantiate object in both local and remote games
        public static void NetworkInstantiate(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            if (instance.networkedObjectToResourceID.ContainsKey(prefab))
            {
                
            }

        }

        public static IEnumerable<GameObject> NetworkedGameObjects => instance.networkedObjectToResourceID.Keys;
        public static int GetResourceID(GameObject gameObject) => instance.networkedObjectToResourceID[gameObject];

        public static List<object> GetSynchronizedComponent(GameObject gameObject) =>
            instance.networkedObjectToSynchronizedComponents[gameObject];

        public static GameObject GetPrefab(int resourceID) => instance.prefabDictionary[resourceID];

    }
}
