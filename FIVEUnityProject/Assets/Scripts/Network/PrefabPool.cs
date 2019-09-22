using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

namespace Assets.Scripts.PrefabPool
{
    public class PrefabPools : IPunPrefabPool
    {
        public PrefabPools()
        {
        }

        public readonly Dictionary<string, GameObject> ResourceCache = new Dictionary<string, GameObject>();
        public GameObject Instantiate(string prefabId, Vector3 position, Quaternion rotation)
        {
            GameObject res = null;
            bool cached = this.ResourceCache.TryGetValue(prefabId, out res);
            if (!cached)
            {
                res = (GameObject)Resources.Load(prefabId, typeof(GameObject));
                if (res == null)
                {
                    Debug.LogError("failed to load \"" + prefabId + "\" . Make sure it's in a \"Resources\" folder.");
                }
                else
                {
                    this.ResourceCache.Add(prefabId, res);
                }
            }

            bool wasActive = res.activeSelf;
            if (wasActive) res.SetActive(false);

            GameObject instance = GameObject.Instantiate(res, position, rotation) as GameObject;

            if (wasActive) res.SetActive(true);
            return instance;
        }

        public void Destroy(GameObject gameObject)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
