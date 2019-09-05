using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FpsController
{
    private GameObject gameObject;
    private CharacterController cc;

    public FpsController(CharacterController cc)
    {
        this.cc = cc;
    }

    public void Update()
    {
        if (Input.GetKey(KeyCode.W))
        {
            cc.SimpleMove(Vector3.forward);
        }
        if (Input.GetKey(KeyCode.S))
        {
        }
        if (Input.GetKey(KeyCode.A))
        {
        }
        if (Input.GetKey(KeyCode.D))
        {
        }

    }
}
