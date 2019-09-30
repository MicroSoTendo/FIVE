using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPos : MonoBehaviour
{
    
    public GameObject energy;
    void Start()
    {
        var positionArray = new Vector3[7];
        var RotationArray = new Vector3[7];

        positionArray[0] = new  Vector3(41.2f, 0f, -26.1f);
        RotationArray[0] = new Vector3(-90f, 0f, -90f);

        positionArray[1] = new Vector3(-41.34f, 0f, -82.9f);
        RotationArray[1] = new Vector3(-90f, 0f, 90f);

        positionArray[2] = new Vector3(-37.4f, 2.2f, 218.1f);
        RotationArray[2] = new Vector3(-90f, 0f, 90f);

        positionArray[3] = new Vector3(41.8f, 0.7f, -120.6f);
        RotationArray[3] = new Vector3(-90f, 0f, -90f);

        positionArray[4] = new Vector3(39.9f, 0.13f, -233.7f);
        RotationArray[4] = new Vector3(-90f, 0, -90f);
        
        positionArray[5] = new Vector3(-136.7f, 0.12f, -145.8f);
        RotationArray[5] = new Vector3(-90f, 0f, 90f);
        
        positionArray[6] = new Vector3(4.8f, 0.1f, -285.8f);
        RotationArray[6] =new Vector3(-90f, 0f, 0f);
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
