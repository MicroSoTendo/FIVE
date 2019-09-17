using System;
using UnityEngine;
namespace FIVE.UI
{
    public abstract class ViewModel
    {
        protected View View { get; set; }
        public int SortingOrder { get => View.ViewCanvas.sortingOrder; set => View.ViewCanvas.sortingOrder = value; }
        public void SetActive(bool value)
        {
            View.ViewCanvas.gameObject.SetActive(value);
        }
    }
    public abstract class ViewModel<TView, TViewModel> : ViewModel
        where TView : View<TView, TViewModel>, new()
        where TViewModel : ViewModel<TView, TViewModel>
    {
        protected new TView View { get => base.View as TView; set => base.View = value; }
        protected Binder<TView, TViewModel> binder;
        protected ViewModel()
        {
            View = View<TView, TViewModel>.Create<TView>();
            binder = new Binder<TView, TViewModel>(View, this);
        }

    }
}