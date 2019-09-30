using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.InGameDisplay
{
    public class ItemDialogView : View<ItemDialogView, ItemDialogViewModel>
    {
        [UIElement]
        public GameObject DialogPanel { get; set; }
        [UIElement(nameof(DialogPanel), TargetType.Property)]
        public InputField ItemNameInputField { get; set; }
        [UIElement(nameof(DialogPanel), TargetType.Property)]
        public InputField DescriptionInputField { get; set; }
    }
}
