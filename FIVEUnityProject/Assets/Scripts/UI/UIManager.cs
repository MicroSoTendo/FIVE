using FIVE.EventSystem;
using FIVE.GameStates;
using FIVE.UI.SplashScreens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI
{
    public class UIManager : MonoBehaviour
    {
        private static readonly Dictionary<Type, ConstructorInfo> VmConstructorInfos = new Dictionary<Type, ConstructorInfo>();
        private static readonly Dictionary<string, ViewModel> NameToVMs = new Dictionary<string, ViewModel>();
        private static readonly Dictionary<Type, SortedSet<ViewModel>> TypeToVMs = new Dictionary<Type, SortedSet<ViewModel>>();
        private static readonly SortedSet<ViewModel> LayerSortedVMs = new SortedSet<ViewModel>(new ViewModelComparer());

        private static Texture2D cursorTexture;
        public enum CursorType
        {
            Aim,
            Regular,
            Hidden
        }

        public static void SetCursor(CursorType cursorType)
        {
            switch (cursorType)
            {
                case CursorType.Aim:
                    Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
                    Cursor.visible = true;
                    break;
                case CursorType.Regular:
                    Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                    Cursor.visible = true;
                    break;
                case CursorType.Hidden:
                    Cursor.visible = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(cursorType), cursorType, null);
            }
        }

        public static bool TryGetViewModel(string name, out ViewModel viewModel)
        {
            return NameToVMs.TryGetValue(name, out viewModel);
        }

        public static T GetViewModel<T>(string name = null) where T : ViewModel
        {
            if (name != null)
            {
                return (T)NameToVMs[name];
            }
            if (TypeToVMs.TryGetValue(typeof(T), out SortedSet<ViewModel> values))
            {
                return (T)values.First();
            }
            return null;
        }

        public static SortedSet<ViewModel> GetViewModels<T>() where T : ViewModel
        {
            return TypeToVMs[typeof(T)];
        }

        private void Awake()
        {
            GameObject canvasGameObject = new GameObject { name = "LoadingSplashCanvas" };
            Canvas canvas = canvasGameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGameObject.AddComponent<CanvasScaler>();
            canvasGameObject.AddComponent<GraphicRaycaster>();
            StartUpScreen startUpScreen = new StartUpScreen(canvasGameObject);
            StartCoroutine(startUpScreen.OnTransitioning());
            EventManager.Subscribe<OnLoadingFinished>((sender, args) => startUpScreen.DoFadingOut());
            cursorTexture = Resources.Load<Texture2D>("Textures/UI/Cursor");
        }


        private void Start()
        {
            StartCoroutine(Initialize());
            StartCoroutine(View.Initialize());
        }

        private IEnumerator Initialize()
        {
            IEnumerable<Type> types =
                from assembly in AppDomain.CurrentDomain.GetAssemblies()
                from type in assembly.GetTypes()
                where typeof(ViewModel).IsAssignableFrom(type)
                select type;
            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }
                VmConstructorInfos.Add(type, type.GetConstructor(Type.EmptyTypes));
                yield return null;
            }
        }

        public static T AddViewModel<T>(string name = null) where T : ViewModel, new()
        {
            T newViewModel;
            if (VmConstructorInfos.TryGetValue(typeof(T), out ConstructorInfo ctor))
            {
                newViewModel = (T)ctor.Invoke(null);
            }
            else
            {
                newViewModel = new T();
            }
            if (!TypeToVMs.ContainsKey(typeof(T)))
            {
                TypeToVMs.Add(typeof(T), new SortedSet<ViewModel>(new ViewModelComparer()));
            }
            NameToVMs.Add(name ?? typeof(T).Name, newViewModel);
            TypeToVMs[typeof(T)].Add(newViewModel);
            LayerSortedVMs.Add(newViewModel);
            return newViewModel;
        }

        public static void RemoveViewModel(string name)
        {
            if (NameToVMs.ContainsKey(name))
            {
                ViewModel vm = NameToVMs[name];
                NameToVMs.Remove(name);
                LayerSortedVMs.Remove(vm);
                TypeToVMs[vm.GetType()].Remove(vm);
                vm.SetEnabled(false);
                vm.Destroy();
            }
        }

        private class ViewModelComparer : IComparer<ViewModel>
        {
            public int Compare(ViewModel x, ViewModel y)
            {
                return x.SortingOrder - y.SortingOrder;
            }
        }
    }
}
