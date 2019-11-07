using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.Network
{
    public sealed class PrefabPool
    {
        public static PrefabPool Instance { get; } = new PrefabPool();
        public GameObject this[int i] => prefabDatabase[i];
        public int this[GameObject gameObjectInstance] => instancePrefabID[gameObjectInstance];
        private readonly Dictionary<int, GameObject> prefabDatabase;
        private readonly Dictionary<GameObject, int> instancePrefabID;
        private PrefabPool()
        {
            prefabDatabase = new Dictionary<int, GameObject>();
            instancePrefabID = new Dictionary<GameObject, int>();
        }
        public bool Initialized { get; private set; }
        public IEnumerator LoadPrefabs(IEnumerable<string> pathList)
        {
            int id = 0;
            foreach (string s in pathList)
            {
               GameObject prefab= Resources.Load<GameObject>(s);
               prefabDatabase.Add(id++, prefab);
               yield return null;
            }
            Initialized = true;
        }

        public GameObject Instantiate(int id)
        {
            var go = GameObject.Instantiate(prefabDatabase[id]);
            instancePrefabID.Add(go, id);
            return go;
        }
    }
}