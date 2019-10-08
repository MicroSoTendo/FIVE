using FIVE.EventSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace FIVE.UI
{
    public abstract class OnViewModelActiveChanged : IEventType<ViewModelActiveChangedEventArgs>
    {
    }

    public class ViewModelActiveChangedEventArgs : EventArgs
    {
        public bool IsActive { get; }
        public ViewModelActiveChangedEventArgs(bool isActive)
        {
            IsActive = isActive;
        }
    }

    public abstract class ViewModel
    {
        public int SortingOrder { get => ViewCanvas.sortingOrder; set => ViewCanvas.sortingOrder = value; }
        public bool IsEnabled => ViewCanvas.gameObject.activeSelf;
        protected abstract string PrefabPath { get; }
        protected Canvas ViewCanvas { get; }
        protected Dictionary<string, GameObject> UIElements { get; } = new Dictionary<string, GameObject>();
        protected Dictionary<string, GameObject> CanvasResources { get; } = new Dictionary<string, GameObject>();

        protected ViewModel(Canvas parentCanvas = null)
        {
            if (parentCanvas == null)
            {
                var canvasGameObject = new GameObject { name = GetType().Name };
                ViewCanvas = canvasGameObject.AddComponent<Canvas>();
                canvasGameObject.AddComponent<CanvasScaler>();
                canvasGameObject.AddComponent<GraphicRaycaster>();
                ViewCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            }
            else
            {
                ViewCanvas = parentCanvas;
            }
            GameObject prefab = Resources.Load<GameObject>(PrefabPath);
            if (prefab == null)
            {
                Debug.LogWarning($"{PrefabPath} not found. ({GetType().Name})");
            }
            else
            {
                AddAllUIElements(Object.Instantiate(prefab, ViewCanvas.transform));
            }
        }

        private void AddAllUIElements(GameObject instantiatedPrefab)
        {
            for (int i = 0; i < instantiatedPrefab.transform.childCount; i++)
            {
                GameObject child = instantiatedPrefab.transform.GetChild(i).gameObject;
                string name = child.name;
                if (UIElements.ContainsKey(name))
                {
                    Debug.LogWarning($"Prefab:{PrefabPath} {name} already exists.");
                    throw new ArgumentException();
                }
                UIElements.Add(name, child);
                AddAllUIElements(child);
            }
        }

        public virtual void SetEnabled(bool value)
        {
            ViewCanvas.gameObject.SetActive(value);
            this.RaiseEvent<OnViewModelActiveChanged, ViewModelActiveChangedEventArgs>(
                new ViewModelActiveChangedEventArgs(value));
        }

        public virtual void ToggleEnabled()
        {
            SetEnabled(!IsEnabled);
        }

        public virtual void Destroy()
        {
            GameObject gameObject = ViewCanvas.gameObject;
            gameObject.SetActive(false);
            Object.Destroy(gameObject);
        }

        protected T Get<T>(string name)
        {
            if (UIElements.ContainsKey(name))
                return UIElements[name].GetComponent<T>();
            return ViewCanvas.gameObject.FindChildRecursive(name).GetComponent<T>();
        }

        protected GameObject Get(string name)
        {
            if (UIElements.ContainsKey(name))
                return UIElements[name];
            return ViewCanvas.gameObject.FindChildRecursive(name);
        }

        protected ButtonSource Bind(Button button)
        {
            return new ButtonSource(button);
        }

        public class ViewModelComparer : IComparer<ViewModel>
        {
            public int Compare(ViewModel x, ViewModel y)
            {
                if (x == null)
                {
                    return y != null ? -1 : 0;
                }

                return y != null ? x.SortingOrder.CompareTo(y.SortingOrder) : 1;
            }
        }

        protected abstract class BindingSource<T>
        {
            protected T Source { get; }
            protected BindingSource(T source)
            {
                Source = source;
            }
        }

        protected class ButtonSource : BindingSource<Button>
        {
            public ButtonSource(Button button) : base(button) { }

            public void To(Action o)
            {
                Source.onClick.AddListener(new UnityAction(o));
            }
        }

        protected class TextSource : BindingSource<Text>
        {
            public TextSource(Text source) : base(source) { }
            public void To(ref string s) { }
        }
    }
}