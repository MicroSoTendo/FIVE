using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControlTest : MonoBehaviour
{
    //public Material ClickedMaterial;
    //public Material UnClickedMaterial;

    private Renderer ObjectRenderer;
    private Rigidbody rb;
    private Vector3 currTarget;
    private Vector3 velocity;
    private float speed = 10f;

    void Start()
    {
        //ObjectRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Vector3.Distance(currTarget, transform.position) > Constants.floatPrecision)
        {
            transform.Translate(velocity * speed * Time.deltaTime);
        }
    }

    public void OnSelect()
    {
        //ObjectRenderer.material = ClickedMaterial;
    }

    public void DeSelect()
    {
        //ObjectRenderer.material = UnClickedMaterial;
    }

    public void Move(Vector3 target)
    {
        currTarget = target;
        velocity = Vector3.Normalize(target - transform.position);
    }
}
