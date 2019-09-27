using UnityEngine.UI;

namespace FIVE.UI.CAttribute
{
    public class CAttributeView : View<CAttributeView, CAttributeViewModel>
    {
        public CAttributeView()
        {
            Background = AddUIElement<Image>(nameof(Background));
            PlayerButton = AddUIElement<Button>(nameof(PlayerButton));
            Energy = AddUIElement<Button>(nameof(Energy));
            Inventory = AddUIElement<Button>(nameof(Inventory));
            Option = AddUIElement<Button>(nameof(Option));
            //TestInputField = AddUIElement<InputField>(nameof(TestInputField));
        }
        public Image Background { get; }
        public Button PlayerButton { get; }
        public Button Energy { get; }
        public Button Inventory { get; }
        public Button Option { get; }
        //public InputField TestInputField { get; }
    }
}