using UnityEngine.UI;

namespace FIVE.UI.StartupMenu
{
    public class StartupMenuView : View<StartupMenuView, StartupMenuViewModel>
    {
        [UIElement]
        public Button SinglePlayerButton { get; set; }
        [UIElement]
        public Button MultiplayerButton { get; set; }
        [UIElement]
        public Button OptionsButton { get; set; }
        [UIElement]
        public Button ExitGameButton { get; set; }
        [UIElement]
        public Image Logo { get; set; }
    }
}