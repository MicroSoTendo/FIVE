using UnityEngine;

namespace FIVE.Network
{
    public sealed class PrefabPool
    {
        public static PrefabPool Instance { get; } = new PrefabPool();
        public GameObject this[int i] => prefabDatabase[i];
        public int this[GameObject prefab] => prefabDatabase[prefab];
        private BijectMap<int, GameObject> prefabDatabase;
        private PrefabPool()
        {
            int id = 0;
            GameObject[] prefabs = Resources.LoadAll<GameObject>("EntityPrefabs/");
            foreach (GameObject prefab in prefabs)
            {
                prefabDatabase.Add(id++, prefab);
            }
        }
    }
}