namespace FIVE.UI.StartupMenu
{
    public class StartupMenuViewModel : ViewModel
    {
        private Binder<StartupMenuView, StartupMenuViewModel> binder = new Binder<StartupMenuView, StartupMenuViewModel>();
        public StartupMenuViewModel()
        {
            binder.Bind(View => View.SinglePlayer.onClick).To(ViewModel => ViewModel.OnSinglePlayerButtonClicked);
        }

        private void OnSinglePlayerButtonClicked()
        {

        }
    }
}