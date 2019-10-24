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
        Target = new Vector3();
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
            transform.forward = Vector3.Normalize(Target - transform.position);
            transform.Translate(transform.forward * 1.0f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }
}
