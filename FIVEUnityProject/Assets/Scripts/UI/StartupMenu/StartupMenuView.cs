using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.StartupMenu
{
    public class StartupMenuView : View<StartupMenuView, StartupMenuViewModel>
    {
        public StartupMenuView() : base()
        {
            this.Background = AddUIElement<Image>();
            this.SinglePlayerButton = AddUIElement<Button>(nameof(SinglePlayerButton));
            this.MultiplayerButton = AddUIElement<Button>(nameof(MultiplayerButton));
            this.OptionsButton = AddUIElement<Button>(nameof(OptionsButton));
            this.ExitGameButton = AddUIElement<Button>(nameof(ExitGameButton));
        }

        public Image Background { get; }
        public Button SinglePlayerButton { get; }
        public Button MultiplayerButton { get; }
        public Button OptionsButton { get; }
        public Button ExitGameButton { get; }


    }
}