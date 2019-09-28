using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Battery
{
    private float capacity;
    private float currEnergy;
    private bool isCharging;
    private int dechargeSpeed;
    private int chargeSpeed;

    public Battery(float capacity = 100.0f)
    {
        this.capacity = capacity;
        currEnergy = capacity;
        dechargeSpeed = 1;
    }

    public void Update()
    {
        if (isCharging)
        {
            currEnergy += Mathf.Clamp(chargeSpeed * Time.deltaTime, 0, capacity);
        }
        else
        {
            currEnergy -= Mathf.Clamp(dechargeSpeed * Time.deltaTime, 0, capacity);
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
