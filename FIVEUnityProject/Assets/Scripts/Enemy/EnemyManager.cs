using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager instance;
    private float elapsedTime;

    private HashSet<GameObject> enemies;
    public GameObject Prefab;

    private Queue<Vector3> spawnLocations;

    private void Awake()
    {
        instance = this;

        spawnLocations = new Queue<Vector3>();
        spawnLocations.Enqueue(new Vector3(0, 0, -250));
        spawnLocations.Enqueue(new Vector3(100, 0, 90));
        spawnLocations.Enqueue(new Vector3(-100, 0, 90));
        spawnLocations.Enqueue(new Vector3(0, 0, 200));

        enemies = new HashSet<GameObject>();
        elapsedTime = 0;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= 5.0f && enemies.Count < 10)
        {
            Vector3 pos = spawnLocations.Dequeue();
            AddEnemy(pos);
            spawnLocations.Enqueue(pos);
            elapsedTime = 0;
        }
    }

    public static EnemyManager Instance()
    {
        return instance;
    }

    private void AddEnemy(Vector3 positition)
    {
        GameObject enemy = Instantiate(Prefab, positition, Quaternion.identity);
        enemies.Add(enemy);
    }
}