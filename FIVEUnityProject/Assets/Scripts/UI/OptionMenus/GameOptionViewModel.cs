using FIVE.EventSystem;
using FIVE.UI.Background;
using FIVE.UI.InGameDisplay;
using FIVE.UI.OptionsMenu;
using System;
using UnityEngine;

namespace FIVE.UI.OptionMenus
{
    public class GameOptionViewModel : ViewModel<GameOptionView, GameOptionViewModel>
    {
        public GameOptionViewModel() : base()
        {

            binder.Bind(view => view.GameResolutionButton.onClick).
            To(viewModel => viewModel.OnGameResolutionButtonClicked);

            binder.Bind(view => view.FullScreenButton.onClick).
            To(viewModel => viewModel.OnFullScreenButtonClicked);

            binder.Bind(view => view.SoundOptionButton.onClick).
            To(viewModel => viewModel.OnSoundOptionButtonClicked);

            binder.Bind(view => view.ResumeButton.onClick).
            To(viewModel => viewModel.OnResumeButtonClicked);
        }

        private void OnGameResolutionButtonClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnGameResolutionButtonClicked));
            View.ViewCanvas.gameObject.SetActive(false);
            UIManager.GetViewModel<InGameDisplayViewModel>().SetEnabled(true);
            UIManager.GetViewModel<BackgroundViewModel>().SetEnabled(false);
            this.RaiseEvent<OnLoadingGameMode>(EventArgs.Empty);
        }
        private void OnFullScreenButtonClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnFullScreenButtonClicked));
            View.ViewCanvas.gameObject.SetActive(false);
            UIManager.GetViewModel<InGameDisplayViewModel>().SetEnabled(true);
            UIManager.GetViewModel<BackgroundViewModel>().SetEnabled(false);
            this.RaiseEvent<OnLoadingGameMode>(EventArgs.Empty);
        }
        private void OnSoundOptionButtonClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnSoundOptionButtonClicked));
            UIManager.GetViewModel<OptionsMenuViewModel>().SetEnabled(true);
        }
        private void OnResumeButtonClicked(object sender, EventArgs eventArgs)
        {
            Application.Quit();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
            Debug.Log(nameof(OnResumeButtonClicked));
        }

    }
}