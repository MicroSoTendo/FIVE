using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.StartupMenu
{
    public class StartupMenuView : View
    {
        public Image Background { get; }
        public Button SinglePlayer { get; }
        public Button Multiplayer { get; }
        public Button Options { get; }
        public Button ExitGame { get; }

    }
}