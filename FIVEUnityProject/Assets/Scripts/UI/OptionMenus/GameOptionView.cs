using UnityEngine.UI;

namespace FIVE.UI.OptionMenus
{ 
    public class GameOptionView : View<GameOptionView, GameOptionViewModel>
    {
        public GameOptionView()
        {
            GameResolutionButton = AddUIElement<Button>(nameof(GameResolutionButton));
            FullScreenButton = AddUIElement<Button>(nameof(FullScreenButton));
            SoundOptionButton = AddUIElement<Button>(nameof(SoundOptionButton));
            ResumeButton = AddUIElement<Button>(nameof(ResumeButton));
        }
        public Button GameResolutionButton { get; }
        public Button FullScreenButton { get; }
        public Button SoundOptionButton { get; }
        public Button ResumeButton { get; }
    }
}