using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 Target
    {
        get;
        set;
    }

    private float elapsedTime;

    void Start()
    {
        elapsedTime = 0;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 3.0f)
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (Target != null)
            {
                transform.forward = Vector3.Normalize(Target - transform.position);
                transform.Translate(transform.forward * 1.0f);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }
}
