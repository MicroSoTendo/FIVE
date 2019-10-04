using System.Collections.Concurrent;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Network
{
    public enum SyncModule
    {
        Transform,
        Animator,
        Rigidbody,
    }

    public class NetworkProxy
    {
        private static readonly ConcurrentDictionary<GameObject, ConcurrentQueue<NetworkProxy>> NetworkedObjects = new ConcurrentDictionary<GameObject, ConcurrentQueue<NetworkProxy>>();
        private readonly GameObject gameObject;
        private readonly List<Component> observedComponents = null;


        public static NetworkProxy Instantiate(GameObject gameObject, params SyncModule[] syncModules)
        {
            return null;

        }

        private NetworkProxy(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

    }
}
