using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager instance;
    private float elapsedTime;

    private HashSet<GameObject> enemies;
    public GameObject Prefab;

    private Vector3 spwanLocation;

    private void Awake()
    {
        instance = this;
        spwanLocation = new Vector3(0, 0, 100);

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