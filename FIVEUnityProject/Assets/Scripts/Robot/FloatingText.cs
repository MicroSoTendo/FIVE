﻿using FIVE.Robot;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TextMeshPro>().text = gameObject.GetComponentInParent<RobotSphere>().ID.ToString();
    }

    private void FixedUpdate()
    {
        if (Camera.current)
        {
            transform.LookAt(transform.position + Camera.current.transform.rotation * Vector3.forward, Camera.current.transform.rotation * Vector3.up);
        }
    }
}