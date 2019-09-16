using UnityEngine.UI;

namespace FIVE.UI.OptionsMenu
{
    public class OptionsMenuView : View<OptionsMenuView, OptionsMenuViewModel>
    {
        public OptionsMenuView()
        {
            LoadButton = AddUIElement<Button>(nameof(LoadButton));
            SaveButton = AddUIElement<Button>(nameof(SaveButton));
            GameOptionButton = AddUIElement<Button>(nameof(GameOptionButton));
            SoundOptionButton = AddUIElement<Button>(nameof(SoundOptionButton));
            ResumeButton = AddUIElement<Button>(nameof(ResumeButton));
            ExitGameButton = AddUIElement<Button>(nameof(ExitGameButton));
        }
        public Button LoadButton { get; }
        public Button SaveButton { get; }
        public Button GameOptionButton { get; }
        public Button SoundOptionButton { get; }
        public Button ExitGameButton { get; }
        public Button ResumeButton { get; }
    }
}