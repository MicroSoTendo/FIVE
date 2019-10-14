using FIVE.EventSystem;
using FIVE.UI.Background;
using FIVE.UI.InGameDisplay;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.StartupMenu
{
    public class StartupMenuViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/StartupMenu";
        public Button SinglePlayerButton { get; }
        public Button MultiplayersButton { get; }
        public Button OptionsButton { get; }
        public Button ExitButton { get; }

        public StartupMenuViewModel() : base()
        {
            SinglePlayerButton = Get<Button>(nameof(SinglePlayerButton));
            MultiplayersButton = Get<Button>(nameof(MultiplayersButton));
            OptionsButton = Get<Button>(nameof(OptionsButton));
            ExitButton = Get<Button>(nameof(ExitButton));
            Bind(SinglePlayerButton).To(OnSinglePlayerButtonClicked);
            Bind(MultiplayersButton).To(OnMultiplayersButtonClicked);
            Bind(OptionsButton).To(OnOptionsButtonClicked);
            Bind(ExitButton).To(OnExitButtonClicked);
        }

        private void OnSinglePlayerButtonClicked()
        {
            IsActive = false;
            this.RaiseEvent<OnSinglePlayerButtonClicked>();
        }

        private void OnMultiplayersButtonClicked()
        {
            IsActive = false;
            this.RaiseEvent<OnMultiPlayersButtonClicked>();
        }

        private void OnOptionsButtonClicked()
        {
            UIManager.Get<InGameMenuViewModel>().IsActive = true;
        }

        private void OnExitButtonClicked()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Debug.Log(nameof(OnExitButtonClicked));
        }
    }
}