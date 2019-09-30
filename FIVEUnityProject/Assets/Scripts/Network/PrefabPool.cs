using FIVE.Network;
using FIVE.Network.Views;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.PrefabPool
{
    public class PrefabPools : IPunPrefabPool
    {

        public static PrefabPools Instance { get; } = new PrefabPools();

        private readonly Dictionary<SyncModule, Action<GameObject>> addingViewsTable =
            new Dictionary<SyncModule, Action<GameObject>>
            {
                {SyncModule.Animator, AddView<AnimatorView> },
                {SyncModule.Transform, AddView<TransformView> },
                {SyncModule.Rigidbody, AddView<RigidbodyView> },
            };

        private readonly Dictionary<string, SyncModule[]> hackDictionary = new Dictionary<string, SyncModule[]>();

        private PrefabPools() { }


        private static void AddView<T>(GameObject gameObject) where T : Component
        {
            T view = gameObject.AddComponent<T>();
            gameObject.GetComponent<PhotonView>().ObservedComponents.Add(view);
        }

        private void SetPhotonView(GameObject gameObject)
        {
            PhotonView view = gameObject.GetComponent<PhotonView>();
            if (view == null)
            {
                view = gameObject.AddComponent<PhotonView>();
            }
            if (view.ObservedComponents == null)
            {
                view.ObservedComponents = new List<Component>();
            }
        }

        public void HackInstantiate(GameObject givenPrefab, params SyncModule[] syncModules)
        {
            //if (!(AssetDatabase.TryGetGUIDAndLocalFileIdentifier(givenPrefab, out string guid, out long _)))
            //{
            //    Debug.LogWarning("Networked objects need to be in asset database.");
            //    return;
            //}

            //if (!hackDictionary.ContainsKey(guid))
            //{
            //    hackDictionary.Add(guid, syncModules);
            //}

            //PhotonNetwork.Instantiate(guid, Vector3.zero, Quaternion.identity);
        }

        //Only be invoked when receiving networking requests
        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            return null;
            //SyncModule[] syncModules = hackDictionary[prefabId];
            //string path = AssetDatabase.GUIDToAssetPath(prefabId);
            //GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            //GameObject instantiatedGameObject = GameObject.Instantiate(prefab, position, rotation);
            //instantiatedGameObject.SetActive(false);
            ////first-time network local object
            //SetPhotonView(instantiatedGameObject);
            //foreach (SyncModule syncModule in syncModules)
            //{
            //    addingViewsTable[syncModule](instantiatedGameObject);
            //}
            //return instantiatedGameObject;
        }

        public void Destroy(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
