using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.InGameDisplay
{
    internal class InventoryView : View<InventoryView, InventoryViewModel>
    {
        public GameObject InventoryScrollView { get; set; }
        public Button ExitButton { get; set; }
        public InventoryView()
        {
            InventoryScrollView = AddUIElement<GameObject>(nameof(InventoryScrollView));
            ExitButton = InventoryScrollView.gameObject.GetChildGameObject(nameof(ExitButton)).GetComponent<Button>();
        }

    }
}
