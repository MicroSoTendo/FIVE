using FIVE.EventSystem;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace FIVE.UI
{
    public abstract class OnUIActiveChanged : IEventType<UIActiveChangedEventArgs> { }

    public class UIActiveChangedEventArgs : EventArgs
    {
        public bool IsActive { get; }

        public UIActiveChangedEventArgs(bool isActive)
        {
            IsActive = isActive;
        }
    }

    public abstract partial class ViewModel
    {
        protected Canvas this[RenderMode mode] => Canvas[(int)mode];
        public int ZIndex
        {
            get => Root.transform.GetSiblingIndex();
            set => Root.transform.SetSiblingIndex(value);
        }
        public virtual bool IsActive
        {
            get => Root.activeSelf;
            set
            {
                Root.SetActive(value);
                this.RaiseEvent<OnUIActiveChanged, UIActiveChangedEventArgs>(
                    new UIActiveChangedEventArgs(value));
            }
        }

        protected abstract string PrefabPath { get; }
        protected virtual RenderMode ViewModelRenderMode { get; } = RenderMode.ScreenSpaceOverlay;
        protected GameObject Root { get; }
        protected Dictionary<string, GameObject> UIElements { get; } = new Dictionary<string, GameObject>();

        protected ViewModel()
        {
            Root = Instantiate();
            AddAllUIElements(Root);
        }

        private GameObject Instantiate()
        {
            if (PrefabPath == null) return null;
            GameObject prefab = LoadPrefab(PrefabPath);
            Transform parent = this[ViewModelRenderMode].transform;
            GameObject go = Object.Instantiate(prefab, parent);
            go.SetActive(false);
            return go;
        }

        private void AddAllUIElements(GameObject instantiatedPrefab)
        {
            if (instantiatedPrefab == null) return;
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

        public virtual void ToggleEnabled()
        {
            IsActive ^= true;
        }

        public virtual void Destroy()
        {
            Root.SetActive(false);
            Object.Destroy(Root);
        }

        protected T Get<T>(string name)
        {
            if (UIElements.ContainsKey(name))
            {
                return UIElements[name].GetComponent<T>();
            }

            return Root.FindChildRecursive(name).GetComponent<T>();
        }

        protected GameObject Get(string name)
        {
            if (UIElements.ContainsKey(name))
            {
                return UIElements[name];
            }

            return Root.FindChildRecursive(name);
        }
    }
}