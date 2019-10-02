using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    private GameObject prefab;
    private HashSet<GameObject> enemies;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void AddEnemy(Vector3 positition)
    {
        GameObject enemy = Instantiate(prefab, positition, Quaternion.identity);
    }
}
