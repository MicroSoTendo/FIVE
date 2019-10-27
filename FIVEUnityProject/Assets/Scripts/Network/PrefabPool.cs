using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace FIVE.Network
{
    public sealed class PrefabPool
    {
        public static PrefabPool Instance { get; } = new PrefabPool();
        public GameObject this[int i] => id2Prefab[i];
        public int this[GameObject prefab] => prefab2ID[prefab];
        private readonly ReadOnlyDictionary<int, GameObject> id2Prefab;
        private readonly ReadOnlyDictionary<GameObject, int> prefab2ID;
        private PrefabPool()
        {
            int id = 0;
            GameObject[] prefabs = Resources.LoadAll<GameObject>("EntityPrefabs/");
            id2Prefab = new ReadOnlyDictionary<int, GameObject>(prefabs.ToDictionary(_ => id++));
            id = 0;
            prefab2ID = new ReadOnlyDictionary<GameObject, int>(prefabs.ToDictionary(go => go, _ => id++));
        }
    }
}