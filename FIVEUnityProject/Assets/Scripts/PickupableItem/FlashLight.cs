using FIVE.Robot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashLight: MonoBehaviour
{
    void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            if (transform.parent.name.Contains("Content"))
            {
                foreach (GameObject gameObj in GameObject.FindObjectsOfType<GameObject>())
                {
                    if (gameObj.name.Contains("Sphere"))
                    {
                        gameObj.GetComponent<RobotSphere>().switchOnLight();
                        Transform textMsg = GameObject.Find("ViewModelsRoot").transform.Find("ScreenSpaceOverlay").transform.Find("HUD(Clone)").transform.Find("BuffText");
                        textMsg.GetComponent<Text>().text = "Flash Light: On \n";
                    }
                }
            }
        }
            
    }
}
