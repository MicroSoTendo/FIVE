using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPos : MonoBehaviour
{
    
    public GameObject energy;
    void Start()
    {
        Vector3[] positionArray = new Vector3[7];
        Vector3[] RotationArray = new Vector3[7];

        positionArray[0] = new  Vector3(3.44f, 0.28f, -25.6f);
        RotationArray[5] = new Vector3(-90f, 0f, 90f);

        positionArray[1] = new Vector3(5.36f, 0.14f, -11.69f);
        RotationArray[5] = new Vector3(-90f, 0f, 90f);

        positionArray[2] = new Vector3(-3.58f, 0.18f, -8.46f);
        RotationArray[5] = new Vector3(-90f, 0f, 90f);

        positionArray[3] = new Vector3(-5.62f, 0.13f, 5.99f);
        
        positionArray[4] = new Vector3(-11.43f, 0f, 10.5f);
        RotationArray[4] = new Vector3(-77.15f, 0, 90f);
        
        positionArray[5] = new Vector3(-3.5f, 0.12f, 13.85f);
        RotationArray[5] = new Vector3(-90f, 0f, 90f);
        
        positionArray[6] = new Vector3(4.82f, 0.12f, 13.6f);
        RotationArray[6] =new Vector3(-90f, 0f, 180f);
        //-90, 0 180
        for (int i = 0; i < positionArray.Length; i++)
        {
            double random = Random.Range(0.0f,1.0f);
            if(random >= 0.5)
            {
                Instantiate(energy, positionArray[i], Quaternion.Euler(RotationArray[i]));
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
