using UnityEngine.UI;

namespace FIVE.UI.Background
{
    public class BackgroundView : View<BackgroundView, BackgroundViewModel>
    {
        [UIElement]
        public Image Background { get; set; }
    }
}