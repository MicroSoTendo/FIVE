using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Movable : MonoBehaviour
{
    public Vector3 MoveTarget
    {
        get; set;
    }

    public float MoveSpeed
    {
        get; set;
    }

    public float RotateTarget
    {
        get; set;
    }

    public float RotateSpeed
    {
        get; set;
    }

    private CharacterController cc;

    void Start()
    {
        cc = GetComponent<CharacterController>();
        MoveSpeed = 5.0f;
        RotateSpeed = 30.0f;
    }

    void FixedUpdate()
    {
        if (MoveTarget != null && Vector3.Distance(transform.position, MoveTarget) > 0.5f)
        {
            cc.SimpleMove(transform.forward * MoveSpeed * Time.deltaTime);
        }
    }
}
