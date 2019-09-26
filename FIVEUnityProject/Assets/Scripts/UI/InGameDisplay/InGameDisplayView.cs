using UnityEngine.UI;

namespace FIVE.UI.InGameDisplay
{
    public class InGameDisplayView : View<InGameDisplayView, InGameDisplayViewModel>
    {
        public InGameDisplayView()
        {
            PlayerButton = AddUIElement<Button>(nameof(PlayerButton));
            Energy = AddUIElement<Button>(nameof(Energy));
            Inventory = AddUIElement<Button>(nameof(Inventory));
            Option = AddUIElement<Button>(nameof(Option));
        }
        public Button PlayerButton { get; }
        public Button Energy { get; }
        public Button Inventory { get; }
        public Button Option { get; }
    }
}