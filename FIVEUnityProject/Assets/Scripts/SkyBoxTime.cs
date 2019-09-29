using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class SkyBoxTime : MonoBehaviour
{
    public float time;
    public TimeSpan currentTime;
    public Transform sunTrans;
    public Light sun;

    public float intensity;
    public ConsoleColor forday = ConsoleColor.Gray;
    public ConsoleColor forNight = ConsoleColor.Black;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.RotateAround(Vector3.zero, Vector3.right, 10f * Time.deltaTime);
        transform.LookAt(Vector3.zero);
    }
}
