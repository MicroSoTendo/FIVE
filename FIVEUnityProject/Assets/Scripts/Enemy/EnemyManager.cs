using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace FIVE.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        private static EnemyManager instance;
        private float elapsedTime;
        
        [SerializeField] private GameObject prefab;
        public static List<GameObject> Enemies => instance.enemies;
        private List<GameObject> enemies;
        private Queue<Vector3> spawnLocations;

        private void Awake()
        {
            Assert.IsNull(instance);
            instance = this;

            spawnLocations = new Queue<Vector3>();
            spawnLocations.Enqueue(new Vector3(-300, 5, 500));
            spawnLocations.Enqueue(new Vector3(250, 5, 500));
            spawnLocations.Enqueue(new Vector3(200, 5, 450));
            spawnLocations.Enqueue(new Vector3(150, 5, 520));

            enemies = new List<GameObject>();
            elapsedTime = 0;
        }

        private void Update()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 5.0f && Enemies.Count < 10)
            {
                Vector3 pos = spawnLocations.Dequeue();
                AddEnemy(pos);
                spawnLocations.Enqueue(pos);
                elapsedTime = 0;
            }
        }

        private void AddEnemy(Vector3 positition)
        {
            GameObject enemy = Instantiate(prefab, positition, Quaternion.identity);
            Enemies.Add(enemy);
        }
    }
}