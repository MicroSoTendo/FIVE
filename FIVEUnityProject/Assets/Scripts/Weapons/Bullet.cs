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
            transform.forward = Vector3.Normalize(_target - transform.position);
        }
    }

    private Vector3 _target;

    private float elapsedTime;

    void Start()
    {
        elapsedTime = 0;
    }

    void Update()
    {
        Debug.Log(transform.forward);
        elapsedTime += Time.deltaTime;
        if (elapsedTime > 3.0f)
        {
            gameObject.SetActive(false);
        }
        else
        {
            if (Target != null)
            {
                transform.Translate(transform.forward * 1.0f);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }
}
