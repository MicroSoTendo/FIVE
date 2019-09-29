using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cpu : MonoBehaviour
{
    public int Speed; // instruction executed per frame
    public float PowerConsumption; // 0.0f ~ 1.0f;

    void Start()
    {
        Speed = 1;
        PowerConsumption = 1.0f;
    }
}
