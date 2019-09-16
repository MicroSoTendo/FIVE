using System;
using UnityEngine;
namespace FIVE.UI
{
    public abstract class ViewModel
    {
        protected View View;
        public void SetActive(bool value)
        {
            Debug.Log(this.GetType().Name);
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
            base.View = View<TView, TViewModel>.Create<TView>();
            binder = new Binder<TView, TViewModel>(View, this);
        }

    }
}