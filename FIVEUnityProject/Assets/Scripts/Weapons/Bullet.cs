using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 Target
    {
        get { return _target; }
        set
        {
            _target = value;
            transform.up = Vector3.Normalize(_target - transform.position);
        }
    }

    private Vector3 _target;

    private float elapsedTime;

    private void Start()
    {
        elapsedTime = 0;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 3.0f)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }
        else
        {
            if (Target != null)
            {
                transform.Translate(Vector3.up);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
        Destroy(gameObject);
    }
}