using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputControlTest : MonoBehaviour
{
    public Material ClickedMaterial;

    private Renderer ObjectRenderer;
    private Rigidbody rb;

    private float speed = 10f;

    void Start()
    {
        ObjectRenderer = GetComponent<Renderer>();
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        
    }

    public void OnSelect()
    {
        ObjectRenderer.material = ClickedMaterial;
        Debug.Log("Clicked");
    }

    public void OnMove(Vector3 position)
    {
        //rb.MovePosition(position * speed);
        rb.MovePosition(position);
    }
}
