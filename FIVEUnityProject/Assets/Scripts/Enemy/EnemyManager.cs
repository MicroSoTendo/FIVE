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
        private List<Vector3> spawnLocations = new List<Vector3>();

        private void Awake()
        {
            Assert.IsNull(instance);
            instance = this;

            for (int i = -1; i <= 1; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    spawnLocations.Add(transform.localToWorldMatrix.MultiplyPoint(new Vector3(i * 80, 5, j * 80)));
                }
            }
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 2.0f && Random.value < 0.5 && Enemies.Count < 10)
            {
                Vector3 pos = spawnLocations[Random.Range(0, spawnLocations.Count)];
                AddEnemy(pos);
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