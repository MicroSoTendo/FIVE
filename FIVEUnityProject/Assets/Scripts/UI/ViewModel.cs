using System;
using FIVE.EventSystem;
using System.Collections;
using System.IO;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FIVE.UI
{
    public abstract class OnViewModelActiveChanged : IEventType<ViewModelActiveChangedEventArgs> { }
    public class ViewModelActiveChangedEventArgs : EventArgs { 
        public bool IsActive { get; }
        public ViewModelActiveChangedEventArgs(bool isActive) => IsActive = isActive;
    }
    public abstract class ViewModel
    {
        protected View View { get; set; }
        public int SortingOrder { get => View.ViewCanvas.sortingOrder; set => View.ViewCanvas.sortingOrder = value; }
        public bool IsEnabled => View.ViewCanvas.gameObject.activeSelf;
        public virtual void SetEnabled(bool value)
        {
            View.ViewCanvas.gameObject.SetActive(value);
            this.RaiseEvent<OnViewModelActiveChanged, ViewModelActiveChangedEventArgs>(new ViewModelActiveChangedEventArgs(value));
        }

        public virtual void ToggleEnabled()
        {
            SetEnabled(!IsEnabled);
        }

        public void Destroy()
        {
            View.ViewCanvas.gameObject.SetActive(false);
            Object.Destroy(View.ViewCanvas.gameObject);
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
#if DEBUG
            MainThreadDispatcher.ScheduleCoroutine(MonitorXML());
#endif
        }
#if DEBUG
        private IEnumerator MonitorXML()
        {
            FileSystemWatcher fileSystemWatcher =
                new FileSystemWatcher(Application.dataPath + "/Resources/UI/")
                {
                    Filter = "*.xml",
                    EnableRaisingEvents = true
                };
            fileSystemWatcher.Changed += OnFileUpdated;
            while (true)
            {
                yield return null;
            }
        }

        private void OnFileUpdated(object sender, FileSystemEventArgs e)
        {
            if (e.Name.EndsWith(View.GetType().Name + ".xml"))
            {
                MainThreadDispatcher.ScheduleCoroutine(Func());
                IEnumerator Func()
                {
                    TView newView = new TView();
                    yield return null;
                    View.Unload();
                    yield return null;
                    View = newView;
                    yield return null;
                    binder.ReBind(View, this);
                    yield return null;
                }
            }
        }
#endif

    }
}