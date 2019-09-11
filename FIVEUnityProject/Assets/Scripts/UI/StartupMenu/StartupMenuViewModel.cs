using System;
using System.Linq.Expressions;
using System.Reflection;

namespace FIVE.UI.StartupMenu
{
    public class StartupMenuViewModel : ViewModel<StartupMenuView, StartupMenuViewModel>
    {

        public StartupMenuViewModel()
        {
            binder.Bind(view => view.SinglePlayerButton.onClick).
            To(viewModel => viewModel.OnSinglePlayerButtonClicked());
            
            binder.Bind(view => view.MultiplayerButton.onClick).
            To(viewModel => viewModel.OnMultiPlayerButtonClicked());
            
            binder.Bind(view => view.OptionsButton.onClick).
            To(viewModel => viewModel.OnMultiPlayerButtonClicked());
            
            binder.Bind(view => view.ExitGameButton.onClick).
            To(viewModel => viewModel.OnMultiPlayerButtonClicked());
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