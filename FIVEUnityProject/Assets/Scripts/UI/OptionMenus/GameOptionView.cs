using UnityEngine.UI;

namespace FIVE.UI.OptionMenus
{
    public class GameOptionView : View<GameOptionView, GameOptionViewModel>
    {
        [UIElement]
        public Button GameResolutionButton { get; set; }
        [UIElement]
        public Button FullScreenButton { get; set; }
        [UIElement]
        public Button SoundOptionButton { get; set; }
        [UIElement]
        public Button ResumeButton { get; set; }
    }
}