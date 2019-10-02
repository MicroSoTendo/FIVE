using UnityEngine.UI;

namespace FIVE.UI.NPC
{
    public class NpcConversationView : View<NpcConversationView, NpcConversationViewModel>
    {
        [UIElement]
        public Text SinglePlayerButton { get; set; }
    }
}