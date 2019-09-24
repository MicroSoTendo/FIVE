using System;
using System.Collections.Generic;
using FIVE.Network;
using FIVE.Network.Views;
using Photon.Pun;
using UnityEngine;
using NetworkView = FIVE.Network.Views.NetworkView;

namespace Assets.Scripts.PrefabPool
{
    public class PrefabPools : IPunPrefabPool
    {

        public static PrefabPools Instance { get; } = new PrefabPools();
        private readonly DefaultPool defaultPool = new DefaultPool();
        private GameObject hackedGameObject;
        private bool isHacking;

        private PrefabPools()
        {
        }

        private readonly Dictionary<SyncModule, Action<GameObject>> addingViewsTable =
            new Dictionary<SyncModule, Action<GameObject>>
            {
                {SyncModule.Animator, AddView<AnimatorView> },
                {SyncModule.Transform, AddView<TransformView> },
                {SyncModule.Rigidbody, AddView<RigidbodyView> },
            };

        private static void AddView<T>(GameObject gameObject) where T: NetworkView
        {
            var view = gameObject.AddComponent<T>();
            gameObject.GetComponent<PhotonView>().ObservedComponents.Add(view);
        }

        public void HackInstantiate(GameObject gameObject, params SyncModule[] syncModules)
        {
            isHacking = true;
            hackedGameObject = gameObject;
            PhotonView photonView = gameObject.GetComponent<PhotonView>();
            if (photonView == null)
            {
                photonView = gameObject.AddComponent<PhotonView>();
            }
            if (photonView.ObservedComponents == null)
            {
                photonView.ObservedComponents = new List<Component>();
            }

            foreach (SyncModule syncModule in syncModules)
            {
                addingViewsTable[syncModule](gameObject);
            }
            PhotonNetwork.Instantiate("", Vector3.zero, Quaternion.identity);
        }

        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            if (isHacking)
            {
                isHacking = false;
                return hackedGameObject;
            }
            return defaultPool.Instantiate(prefabId, position, rotation);
        }

        public void Destroy(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
