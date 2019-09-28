using UnityEngine.UI;

namespace FIVE.UI.InGameDisplay
{
    public class InGameDisplayView : View<InGameDisplayView, InGameDisplayViewModel>
    {
        [UIElement]
        public Button PlayerButton { get; set; }
        [UIElement]
        public Button Energy { get; set; }
        [UIElement]
        public Button Inventory { get; set; }
        [UIElement]
        public Button Option { get; set; }
    }
}