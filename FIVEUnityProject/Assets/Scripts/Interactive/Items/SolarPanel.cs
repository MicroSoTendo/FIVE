using FIVE.RobotComponents;
using UnityEngine;

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
        }
    }
}