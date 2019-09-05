using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Camera[] cameraSet;
    void Start()
    {
        cameraSet[0].enabled = true;
        cameraSet[1].enabled = false;
        cameraSet[2].enabled = false;
        cameraSet[3].enabled = false;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var enab = 0; 
            for(var i = 0; i < cameraSet.Length; i++)
            {
                if (cameraSet[i].isActiveAndEnabled)
                {
                    enab = i;
                }
            }
            var ray = cameraSet[enab].ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit))
            {
                for(var i = 0; i < cameraSet.Length; i++)
                {
                    if(cameraSet[i].transform.position == hit.transform.Find("Camera").position)
                    {
                        cameraSet[enab].enabled = false;
                        cameraSet[i].enabled = true;
                    }
                }
                
            }
        }
    }
}
