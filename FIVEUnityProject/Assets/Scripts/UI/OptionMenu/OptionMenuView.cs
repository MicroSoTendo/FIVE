using UnityEngine.UI;

namespace FIVE.UI.OptionMenu
{
    public class OptionMenuView : View<OptionMenuView, OptionMenuViewModel>
    {
        public OptionMenuView()
        {
            //Background = AddUIElement<Image>(nameof(Background));
            //LoadButton = AddUIElement<Button>(nameof(LoadButton));
            //SaveButton = AddUIElement<Button>(nameof(SaveButton));
            //GameOptionButton = AddUIElement<Button>(nameof(GameOptionButton));
            //SoundOptionButton = AddUIElement<Button>(nameof(SoundOptionButton));
            //ResumeButton = AddUIElement<Button>(nameof(ResumeButton));
            //ExitGameButton = AddUIElement<Button>(nameof(ExitGameButton));
            //TestInputField = AddUIElement<InputField>(nameof(TestInputField));
        }
        public Image Background { get; }
        public Button LoadButton { get; }
        public Button SaveButton { get; }
        public Button GameOptionButton { get; }
        public Button SoundOptionButton { get; }
        public Button ExitGameButton { get; }
        public Button ResumeButton { get; }
        //public InputField TestInputField { get; }
    }
}