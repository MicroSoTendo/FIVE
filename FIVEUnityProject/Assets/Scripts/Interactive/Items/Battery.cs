using UnityEngine;

namespace FIVE.Interactive.Items
{
    public class Battery : MonoBehaviour
    {
        private float Capacity = 100;
        private float Remaining = 100;
        private Item item;
        void Awake()
        {
            item = GetComponent<Item>();
            item.ItemAction = ItemAction;
        }

        private void ItemAction(GameObject owner, GameObject o)
        {
            owner.GetComponent<RobotComponent.Battery>().CurrentEnergy = Remaining;
        }
    }

}
