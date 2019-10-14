using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyBar : MonoBehaviour
{
    public static bool BetteryOn = false;
    public float CurrentEnergy;
    public bool decreasingEnergy = false;
    public bool increasingEnergy = false;
    public float TotalEnergy;

    void Start()
    {
        CurrentEnergy = TotalEnergy;
        StartCoroutine(HealthChange());
    }

    void Update()
    {
        if (decreasingEnergy == true)
        {
            if (CurrentEnergy > 5)
            {
                CurrentEnergy -= 5;
                transform.localScale = new Vector3(CurrentEnergy / TotalEnergy, 1, 1);
                decreasingEnergy = false;
            }
            else
            {
                transform.localScale = new Vector3(0, 1, 1);
            }
        }

        if (increasingEnergy == true)
        {
            if (CurrentEnergy < 95)
            {
                CurrentEnergy += 5;
                transform.localScale = new Vector3(CurrentEnergy / TotalEnergy, 1, 1);
                increasingEnergy = false;
            }
            else
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
        }
    }

    IEnumerator HealthChange()
    {
        while (CurrentEnergy != 0 && !BetteryOn)
        {
            yield return new WaitForSeconds(2);
            decreasingEnergy = true;
        }
    }
}