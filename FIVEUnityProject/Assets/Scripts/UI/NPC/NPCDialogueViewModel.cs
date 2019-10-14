using TMPro;

namespace FIVE.UI.NPC
{
    public class NPCDialogueViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/NPCDialogue";
        public TMP_Text DialogueText { get; }

        public NPCDialogueViewModel() : base()
        {
            DialogueText = Get<TMP_Text>(nameof(DialogueText));
        }
    }
}