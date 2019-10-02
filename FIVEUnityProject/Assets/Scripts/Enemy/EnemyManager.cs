using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public GameObject Prefab;

    private static EnemyManager instance;

    private Vector3 spwanLocation;

    private HashSet<GameObject> enemies;
    private float elapsedTime;

    private void Awake()
    {
        instance = this;
        spwanLocation = new Vector3(10, 0, 0);

        enemies = new HashSet<GameObject>();
        elapsedTime = 0;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime >= 5.0f && enemies.Count < 10)
        {
            AddEnemy(spwanLocation);
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
