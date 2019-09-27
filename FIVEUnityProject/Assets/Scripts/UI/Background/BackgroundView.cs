using UnityEngine.UI;

namespace FIVE.UI.Background
{
    public class BackgroundView : View<BackgroundView, BackgroundViewModel>
    {
        public BackgroundView()
        {
            Background = AddUIElement<Image>(nameof(Background));
        }
        public Image Background { get; }
    }
}