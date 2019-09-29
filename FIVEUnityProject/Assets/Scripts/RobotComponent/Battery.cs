using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery : MonoBehaviour
{
    public float Capacity;
    private float currEnergy;
    private bool isCharging;
    public int DechargeSpeed;
    private int chargeSpeed;

    public void Start()
    {
        Capacity = 100.0f;
        currEnergy = Capacity;
        DechargeSpeed = 1;
    }

    public void Update()
    {
        if (isCharging)
        {
            currEnergy += Mathf.Clamp(chargeSpeed * Time.deltaTime, 0, Capacity);
        }
        else
        {
            currEnergy -= Mathf.Clamp(DechargeSpeed * Time.deltaTime, 0, Capacity);
        }
    }

    public void Charge(int chargeSpeed)
    {
        isCharging = true;
        this.chargeSpeed = chargeSpeed;
    }

    public void UnCharge()
    {
        isCharging = false;
    }
}
