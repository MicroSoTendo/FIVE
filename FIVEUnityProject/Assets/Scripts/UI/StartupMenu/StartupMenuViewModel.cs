using System;

namespace FIVE.UI.StartupMenu
{
    public class StartupMenuViewModel : ViewModel<StartupMenuView, StartupMenuViewModel>
    {
        public StartupMenuViewModel() : base()
        {
            binder.Bind(View => View.SinglePlayerButton.onClick).
            To(ViewModel => ViewModel.OnSinglePlayerButtonClicked);
            
            binder.Bind(View => View.MultiplayerButton.onClick).
            To(ViewModel => ViewModel.OnMultiPlayerButtonClicked);
            
            binder.Bind(View => View.OptionsButton.onClick).
            To(ViewModel => ViewModel.OnMultiPlayerButtonClicked);
            
            binder.Bind(View => View.ExitGameButton.onClick).
            To(ViewModel => ViewModel.OnMultiPlayerButtonClicked);
        }

        private void OnSinglePlayerButtonClicked()
        {
        }
        private void OnMultiPlayerButtonClicked()
        {
        }
        private void OnOptionsButtonClicked()
        {
        }
        private void ExitGameButton()
        {
        }

    }
}