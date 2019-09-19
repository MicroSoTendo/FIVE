using System;
using FIVE.EventSystem;
using FIVE.UI.Multiplayers;
using UnityEngine;
using FIVE.UI.MainGameDisplay;
using FIVE.UI.Background;
using FIVE.UI.OptionMenu;

namespace FIVE.UI.StartupMenu
{
    public class StartupMenuViewModel : ViewModel<StartupMenuView, StartupMenuViewModel>
    {

        public string TestInputFieldText { get; set; }
        public StartupMenuViewModel() : base()
        {

            binder.Bind(view => view.SinglePlayerButton.onClick).
            To(viewModel => viewModel.OnSinglePlayerButtonClicked);

            binder.Bind(v => v.SinglePlayerButton.onClick).
            ToBroadcast<OnSinglePlayerButtonClicked>();

            binder.Bind(view => view.MultiplayerButton.onClick).
            To(viewModel => viewModel.OnMultiPlayerButtonClicked);

            binder.Bind(v=>v.MultiplayerButton.onClick).
            ToBroadcast<OnMultiPlayersButtonClicked>();
            
            binder.Bind(view => view.OptionsButton.onClick).
            To(viewModel => viewModel.OnOptionsButtonClicked);

            binder.Bind(view => view.ExitGameButton.onClick).
            To(viewModel => viewModel.OnExitButtonClicked);

            //binder.Bind(view => view.TestInputField.text).
            //To(viewMode => viewMode.TestInputFieldText, BindingMode.TwoWay);
        }

        private void OnSinglePlayerButtonClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnSinglePlayerButtonClicked));
            View.ViewCanvas.gameObject.SetActive(false);
            UIManager.Get(nameof(GameDisplayView)).SetActive(true);
            this.RaiseEvent<OnLoadingGameMode>(EventArgs.Empty);
        }
        private void OnMultiPlayerButtonClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnMultiPlayerButtonClicked));
            View.ViewCanvas.gameObject.SetActive(false);
            // UIManager.AddViewModel<MultiplayersEntryViewModel>();
        }
        private void OnOptionsButtonClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnOptionsButtonClicked));
            //View.ViewCanvas.gameObject.SetActive(false);
            UIManager.Get(nameof(OptionsMenuView)).SetActive(true);
            //UIManager.Get(nameof(OptionBGView)).SetActive(true);
        }
        private void OnExitButtonClicked(object sender, EventArgs eventArgs)
        {
            Application.Quit();
            Debug.Log(nameof(OnExitButtonClicked));
        }

    }
}