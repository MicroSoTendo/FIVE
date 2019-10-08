using UnityEngine.UI;

namespace FIVE.UI.NPC
{
    public class NpcConversationViewModel : ViewModel
    {
        protected override string PrefabPath { get; }
        public Text SinglePlayerButton { get; set; }
        public NpcConversationViewModel() : base() { }

    }

}