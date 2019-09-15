using UnityEngine.UI;
namespace FIVE.UI.Multiplayers
{
    public class MultiplayersEntryView : View<MultiplayersEntryView, MultiplayersEntryViewModel>
    {
        public ScrollRect LobbyListScrollRect { get; }

        public MultiplayersEntryView()
        {
            LobbyListScrollRect = AddUIElement<ScrollRect>(nameof(LobbyListScrollRect));
        }
    }
}