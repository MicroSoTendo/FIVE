using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace FIVE.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        private static EnemyManager instance;
        private float elapsedTime = 0;

        public GameObject Prefab;
        public static HashSet<GameObject> Enemies => instance.enemies;
        private HashSet<GameObject> enemies = new HashSet<GameObject>();
        private Queue<Vector3> spawnLocations = new Queue<Vector3>();

        private void Awake()
        {
            Assert.IsNull(instance);
            instance = this;

            for (int i = 0; i < transform.childCount; i++)
            {
                spawnLocations.Enqueue(transform.GetChild(i).position);
            }
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 5.0f && Enemies.Count < 25)
            {
                Vector3 pos = spawnLocations.Dequeue();
                AddEnemy(pos);
                spawnLocations.Enqueue(pos);
                elapsedTime = 0;
            }
        }

        public void AddEnemy(Vector3 positition)
        {
            GameObject enemy = Instantiate(Prefab, positition, Quaternion.identity);
            Enemies.Add(enemy);
        }

        public static void Remove(GameObject enemy)
        {
            instance.enemies.Remove(enemy);
        }
    }
}