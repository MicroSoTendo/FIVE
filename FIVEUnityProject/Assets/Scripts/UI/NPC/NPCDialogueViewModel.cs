using TMPro;
using UnityEngine;

namespace FIVE.UI.NPC
{
    public class NPCDialogueViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/NPCDialogue";
        protected override RenderMode ViewModelRenderMode { get; } = RenderMode.ScreenSpaceCamera;
        public TMP_Text DialogueText { get; }

        public NPCDialogueViewModel() : base()
        {
            DialogueText = Get<TMP_Text>(nameof(DialogueText));
        }
    }
}