using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.InGameDisplay
{
    public class ItemDialogView : View<ItemDialogView, ItemDialogViewModel>
    {
        [UIElement]
        public GameObject DialogPanel { get; set; }
        [UIElement]
        public Text ItemName { get; set; }
        [UIElement]
        public Text Description { get; set; }
    }
}
