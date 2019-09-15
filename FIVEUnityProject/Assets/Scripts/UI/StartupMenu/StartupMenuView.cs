using UnityEngine.UI;

namespace FIVE.UI.StartupMenu
{
    public class StartupMenuView : View<StartupMenuView, StartupMenuViewModel>
    {
        public StartupMenuView()
        {
            Background = AddUIElement<Image>(nameof(Background));
            SinglePlayerButton = AddUIElement<Button>(nameof(SinglePlayerButton));
            MultiplayerButton = AddUIElement<Button>(nameof(MultiplayerButton));
            OptionsButton = AddUIElement<Button>(nameof(OptionsButton));
            ExitGameButton = AddUIElement<Button>(nameof(ExitGameButton));
            TestInputField = AddUIElement<InputField>(nameof(TestInputField));
        }
        public Image Background { get; }
        public Button SinglePlayerButton { get; }
        public Button MultiplayerButton { get; }
        public Button OptionsButton { get; }
        public Button ExitGameButton { get; }
        public InputField TestInputField { get; }
    }
}