using FIVE.EventSystem;
using FIVE.UI;
using UnityEngine;

namespace FIVE.Interactive
{
    internal class RobotCursor : MonoBehaviour
    {
        void OnEnable()
        {
            UIManager.SetCursor(UIManager.CursorType.Aim);
        }

        void OnDisable()
        {
            UIManager.SetCursor(UIManager.CursorType.Regular);
        }

        void Update()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.SphereCast(ray, 1, maxDistance: 100, hitInfo: out RaycastHit hit))
            {
                Transform objectHit = hit.transform;
                Item p = objectHit.gameObject.GetComponent<Item>();
                if (Input.GetMouseButtonDown(1))
                {
                    this.RaiseEvent<OnDropItemToInventory, DropedItemToInventoryEventArgs>(new DropedItemToInventoryEventArgs(gameObject, null, objectHit.gameObject));
                }
            }
        }
    }
}
