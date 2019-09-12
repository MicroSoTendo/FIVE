namespace FIVE.UI
{
    public abstract class ViewModel
    {

    }
    public abstract class ViewModel<TView, TViewModel> : ViewModel
        where TView : View<TView, TViewModel>, new()
        where TViewModel : ViewModel<TView, TViewModel>
    {
        protected TView view;
        protected Binder<TView, TViewModel> binder;
        protected ViewModel()
        {
            view = View<TView, TViewModel>.Create<TView>();
            binder = new Binder<TView, TViewModel>(view, this);
        }

    }
}