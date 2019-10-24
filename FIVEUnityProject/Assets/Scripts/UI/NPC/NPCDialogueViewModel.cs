using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.NPC
{
    public class NPCDialogueViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/Conversation/NPCDialog";
        protected override RenderMode ViewModelRenderMode { get; } = RenderMode.ScreenSpaceOverlay;
        public TMP_Text DialogueText { get; }
        public Button Button { get; }
        public override bool IsActive
        {
            get => base.IsActive;
            set
            {
                base.IsActive = value;
                if (value)
                {
                    DialogueText.text = NPCDescription.Description[Random.Range(0, NPCDescription.Description.Length)];
                    DialogueText.ForceMeshUpdate();
                }
            }
        }

        public NPCDialogueViewModel() : base()
        {
            DialogueText = Get<TMP_Text>(nameof(DialogueText));
            Button = Get<Button>(nameof(Button));
            Bind(Button).To(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            IsActive = false;
        }
    }
}