using System;
using System.Collections.Generic;
using FIVE.Network;
using FIVE.Network.Views;
using Photon.Pun;
using UnityEditor;
using UnityEngine;
using NetworkView = FIVE.Network.Views.NetworkView;

namespace Assets.Scripts.PrefabPool
{
    public class PrefabPools : IPunPrefabPool
    {

        public static PrefabPools Instance { get; } = new PrefabPools();

        private readonly DefaultPool defaultPool = new DefaultPool();
        private readonly Dictionary<SyncModule, Action<GameObject>> addingViewsTable =
            new Dictionary<SyncModule, Action<GameObject>>
            {
                {SyncModule.Animator, AddView<AnimatorView> },
                {SyncModule.Transform, AddView<PhotonTransformView> },
                {SyncModule.Rigidbody, AddView<RigidbodyView> },
            };

        private readonly Dictionary<string, GameObject> hackDictionary = new Dictionary<string, GameObject>();

        private bool isHacking = false;
        private GameObject gameObjectBeingHacking;

        private PrefabPools() { }


        private static void AddView<T>(GameObject gameObject) where T: Component
        {
            var view = gameObject.AddComponent<T>();
            gameObject.GetComponent<PhotonView>().ObservedComponents.Add(view);
        }

        private void SetPhotonView(GameObject gameObject)
        {
            var view = gameObject.GetComponent<PhotonView>();
            if (view == null)
            {
                view = gameObject.AddComponent<PhotonView>();
            }
            if (view.ObservedComponents == null)
            {
                view.ObservedComponents = new List<Component>();
            }
        }
        
        public void HackInstantiate(GameObject gameObject, params SyncModule[] syncModules)
        {
            if (!(AssetDatabase.TryGetGUIDAndLocalFileIdentifier(gameObject, out string guid, out long _)))
            {
                Debug.LogWarning("Networked objects need to be in asset database.");
                return;
            }
            
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            

            //first-time network local object
            SetPhotonView(gameObject);
            SetPhotonView(prefab);

            foreach (SyncModule syncModule in syncModules)
            {
                addingViewsTable[syncModule](gameObject);
                addingViewsTable[syncModule](prefab);
            }

            isHacking = true;
            gameObjectBeingHacking = gameObject;
            PhotonNetwork.Instantiate(guid, Vector3.zero, Quaternion.identity);
        }

        //Only be invoked when receiving networking requests
        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            GameObject result = isHacking ? gameObjectBeingHacking : hackDictionary[prefabId];
            gameObjectBeingHacking = null;
            isHacking = false;
            return result;
        }

        public void Destroy(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
