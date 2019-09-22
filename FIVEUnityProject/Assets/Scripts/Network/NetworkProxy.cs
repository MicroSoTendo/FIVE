using System.Collections.Concurrent;
using System.Collections.Generic;
using Assets.Scripts.PrefabPool;
using Photon.Pun;
using UnityEngine;

namespace FIVE.Network
{
    public class NetworkProxy
    {
        private static readonly ConcurrentDictionary<GameObject, ConcurrentQueue<NetworkProxy>> NetworkedObjects = new ConcurrentDictionary<GameObject, ConcurrentQueue<NetworkProxy>>();
        private readonly GameObject gameObject;
        private readonly List<Component> observedComponents;

        public static bool TryCreateProxy(GameObject gameObject, out NetworkProxy proxy)
        {
            if (!PhotonNetwork.IsConnected)
            {
                proxy = null;
                return false;
            }

            if (!(PhotonNetwork.PrefabPool is PrefabPools))
            {
                PhotonNetwork.PrefabPool = new PrefabPools();
            }

            proxy = new NetworkProxy(gameObject);
            return true;

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
