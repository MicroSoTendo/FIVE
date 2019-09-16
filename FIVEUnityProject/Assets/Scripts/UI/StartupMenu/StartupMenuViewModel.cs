using System;
using FIVE.UI.Multiplayers;
using UnityEngine;

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

            binder.Bind(view => view.TestInputField.text).
            To(viewMode => viewMode.TestInputFieldText, BindingMode.TwoWay);
        }

        private void OnSinglePlayerButtonClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnSinglePlayerButtonClicked));
            View.ViewCanvas.gameObject.SetActive(false);
        }
        private void OnMultiPlayerButtonClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnMultiPlayerButtonClicked));
            View.ViewCanvas.gameObject.SetActive(false);
            UIManager.AddViewModel<MultiplayersEntryViewModel>();
        }
        private void OnOptionsButtonClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnOptionsButtonClicked));
        }
        private void OnExitButtonClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnExitButtonClicked));
        }

    }
}