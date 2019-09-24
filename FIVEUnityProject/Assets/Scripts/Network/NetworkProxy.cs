using System.Collections.Concurrent;
using System.Collections.Generic;
using Assets.Scripts.PrefabPool;
using Photon.Pun;
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


        public static NetworkProxy ProxyIt(GameObject gameObject, params SyncModule[] syncModules)
        {
            if (!PhotonNetwork.IsConnected)
            {
                return null;
            }

            if (!(PhotonNetwork.PrefabPool is PrefabPools))
            {
                PhotonNetwork.PrefabPool = PrefabPools.Instance;
            }
            PrefabPools.Instance.HackInstantiate(gameObject, syncModules);
            var proxy = new NetworkProxy(gameObject);
            return proxy;

        }

        private NetworkProxy(GameObject gameObject)
        {
            this.gameObject = gameObject;
        }

        public bool IsConnected()
        {
            return PhotonNetwork.IsConnected;
        }

        public T AddObservable<T>() 
            where T : Component, IPunObservable, new()
        {
            T observable = new T();
            observedComponents.Add(observable);
            return observable;
        }
    }
}
