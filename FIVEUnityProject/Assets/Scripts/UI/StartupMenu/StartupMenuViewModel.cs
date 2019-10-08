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
            ViewCanvas.gameObject.SetActive(false);
            UIManager.GetViewModel<HUDViewModel>().SetEnabled(true);
            UIManager.GetViewModel<BackgroundViewModel>().SetEnabled(false);
            this.RaiseEvent<OnLoadingGameMode>(EventArgs.Empty);
        }
        private void OnMultiplayersButtonClicked()
        {
            ViewCanvas.gameObject.SetActive(false);
            UIManager.GetViewModel<HUDViewModel>().SetEnabled(true);
            UIManager.GetViewModel<BackgroundViewModel>().SetEnabled(false);
            this.RaiseEvent<OnLoadingGameMode>(EventArgs.Empty);
        }
        private void OnOptionsButtonClicked()
        {
            UIManager.GetViewModel<InGameMenuViewModel>().SetEnabled(true);
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