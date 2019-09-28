using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.InGameDisplay
{
    internal class InventoryView : View<InventoryView, InventoryViewModel>
    {
        [UIElement]
        public GameObject InventoryScrollView { get; set; }
        [UIElement(nameof(InventoryScrollView), TargetType.Property)]
        public Button ExitButton { get; set; }

    }
}
