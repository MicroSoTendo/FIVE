using System;
using System.Collections.Generic;
using System.Net.Sockets;
using FIVE.CameraSystem;
using UnityEngine;
using UnityEngine.Assertions;

namespace FIVE.Network
{
    public class SyncCenter : MonoBehaviour
    {
        private static SyncCenter instance;


        private Dictionary<string, Camera> cameras;
        private HashSet<Transform> ownedTransforms = new HashSet<Transform>();
        private HashSet<Transform> remoteTransforms = new HashSet<Transform>();
        private Action OnUpdate;
        

        private bool IsHost;
        private void Awake()
        {
            Assert.IsNull(instance);
            instance = this;
        }
        private Dictionary<GameObject, int> networkedObjectToResourceID = new Dictionary<GameObject, int>();
        private Dictionary<GameObject, List<object>> networkedObjectToSynchronizedComponents = new Dictionary<GameObject, List<object>>();
        private Dictionary<int, GameObject> prefabDictionary = new Dictionary<int, GameObject>();


        public static IEnumerable<GameObject> NetworkedGameObjects => instance.networkedObjectToResourceID.Keys;
        public static int GetResourceID(GameObject gameObject) => instance.networkedObjectToResourceID[gameObject];

        public static List<object> GetSynchronizedComponent(GameObject gameObject) =>
            instance.networkedObjectToSynchronizedComponents[gameObject];

        public static GameObject GetPrefab(int resourceID) => instance.prefabDictionary[resourceID];

        public static void RegisterLocal(Transform transform)
        {
            instance.ownedTransforms.Add(transform);
        }        
        
        public static void RegisterRemote(Transform transform)
        {
            instance.ownedTransforms.Add(transform);
        }

        public void SendLocalObjects(NetworkStream stream)
        {

        }

        public void ReceiveRemote(NetworkStream stream)
        {
        }


        public void Update()
        {
        }

    }
}
