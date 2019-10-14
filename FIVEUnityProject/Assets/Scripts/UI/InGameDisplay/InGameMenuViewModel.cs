using System;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.InGameDisplay
{
    public class InGameMenuViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/InGameMenu";
        public Button ResumeButton { get; }
        public Button LoadSaveButton { get; }
        public Button SettingsButton { get; }
        public Button MainMenuButton { get; }
        public Button ExitGameButton { get; }

        public InGameMenuViewModel() : base()
        {
            ResumeButton = Get<Button>(nameof(ResumeButton));
            LoadSaveButton = Get<Button>(nameof(LoadSaveButton));
            SettingsButton = Get<Button>(nameof(SettingsButton));
            MainMenuButton = Get<Button>(nameof(MainMenuButton));
            ExitGameButton = Get<Button>(nameof(ExitGameButton));

            Bind(ResumeButton).To(OnResumeClicked);
            Bind(LoadSaveButton).To(OnLoadSaveClicked);
            Bind(SettingsButton).To(OnSettingsClicked);
            Bind(MainMenuButton).To(OnMainMenuClicked);
            Bind(ExitGameButton).To(OnExitGameClicked);
        }

        private void OnLoadSaveClicked()
        {
            IsActive = false;
            Debug.Log(nameof(OnLoadSaveClicked));
        }

        private void OnSaveButtonClicked()
        {
            IsActive = false;
            // UIManager.Get(nameof(GameOptionView)).SetActive(true);
            Debug.Log(nameof(OnSaveButtonClicked));
        }

        private void OnSettingsClicked()
        {
            Debug.Log(nameof(OnSettingsClicked));
            IsActive = false;
            //UIManager.Get(nameof(GameOptionView)).SetActive(true);
        }

        private void OnExitGameClicked()
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Debug.Log(nameof(OnExitGameClicked));
        }

        private void OnResumeClicked()
        {
            Debug.Log(nameof(OnResumeClicked));
            UIManager.Get<InGameMenuViewModel>().IsActive = false;
        }

        private void OnMainMenuClicked()
        {
            IsActive = false;
            Debug.Log(nameof(OnMainMenuClicked));
        }
    }
}