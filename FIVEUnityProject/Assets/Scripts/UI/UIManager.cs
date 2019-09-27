using FIVE.EventSystem;
using FIVE.GameStates;
using FIVE.UI.SplashScreens;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace FIVE.UI
{
    public class UIManager : MonoBehaviour
    {
        private static readonly Dictionary<string, ViewModel> NameToVMs = new Dictionary<string, ViewModel>();
        private static readonly Dictionary<Type, SortedSet<ViewModel>> TypeToVMs = new Dictionary<Type, SortedSet<ViewModel>>();
        private static readonly SortedSet<ViewModel> LayerSortedVMs = new SortedSet<ViewModel>(new ViewModelComparer());

        public static bool TryGetViewModel(string name, out ViewModel viewModel)
        {
            return NameToVMs.TryGetValue(name, out viewModel);
        }

        public static T GetViewModel<T>() where T : ViewModel
        {
            return (T)TypeToVMs[typeof(T)].FirstOrDefault();
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

        }


        private void Start()
        {
        }

        public static T AddViewModel<T>(string name = null) where T : ViewModel, new()
        {
            T newViewModel = new T();
            NameToVMs.Add(name ?? typeof(T).Name, newViewModel);
            if (!TypeToVMs.ContainsKey(typeof(T)))
            {
                TypeToVMs.Add(typeof(T), new SortedSet<ViewModel>(new ViewModelComparer()));
            }
            TypeToVMs[typeof(T)].Add(newViewModel);
            LayerSortedVMs.Add(newViewModel);
            return newViewModel;
        }

        private class ViewModelComparer : IComparer<ViewModel>
        {
            public int Compare(ViewModel x, ViewModel y)
            {
                return x.SortingOrder - y.SortingOrder;
            }
        }

        private void Update()
        {
        }
    }
}
