using UnityEngine.UI;

namespace FIVE.UI.OptionsMenu
{
    public class OptionsMenuView : View<OptionsMenuView, OptionsMenuViewModel>
    {
        [UIElement]
        public Button LoadButton { get; set; }
        [UIElement]
        public Button SaveButton { get; set; }
        [UIElement]
        public Button GameOptionButton { get; set; }
        [UIElement]
        public Button SoundOptionButton { get; set; }
        [UIElement]
        public Button ExitGameButton { get; set; }
        [UIElement]
        public Button ResumeButton { get; set; }
    }
}