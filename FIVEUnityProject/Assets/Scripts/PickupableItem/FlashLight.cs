using FIVE.Robot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.Interactive.Items
{
    public class FlashLight : MonoBehaviour
    {
        private Item item;
        private void Awake()
        {
            item = GetComponent<Item>();
            item.ItemUseAction = UseAction;
        }
        private void UseAction()
        {
            Transform textMsg = GameObject.Find("ViewModelsRoot").transform.Find("ScreenSpaceOverlay").transform.Find("HUD(Clone)").transform.Find("BuffText");
            textMsg.GetComponent<Text>().text = "Flash Light: On \n";
            foreach (GameObject gameObj in GameObject.FindObjectsOfType<GameObject>())
            {
                if (gameObj.name.Contains("Sphere"))
                {
                    gameObj.GetComponent<RobotSphere>().switchOnLight();
                    
                }
            }
        }
    }
}
