using FIVE.RobotComponents;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.Interactive.Items
{
    public class SolarPanel : MonoBehaviour
    {
        private float Capacity = 100;
        private Item item;
        private float Remaining = 100;

        private void Awake()
        {
            item = GetComponent<Item>();
            item.ItemUseAction = UseAction;
        }

        private void UseAction()
        {
            if (SkyBoxTime.isDayTime())
            {
                SolarSingleton.PowerCharge = 1.0f;
                
            }
            Transform textMsg = GameObject.Find("ViewModelsRoot").transform.Find("ScreenSpaceOverlay").transform.Find("HUD(Clone)").transform.Find("BuffText");
            textMsg.GetComponent<Text>().text += "Solar Panel: On \n";
        }
    }
}