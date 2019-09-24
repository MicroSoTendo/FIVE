using System;
using FIVE.UI.Background;
using FIVE.UI.MainGameDisplay;
using FIVE.UI.OptionsMenu;
using FIVE.UI.StartupMenu;
using System.Collections.Generic;
using FIVE.EventSystem;
using FIVE.GameStates;
using FIVE.UI.SplashScreens;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI
{
    public class UIManager : MonoBehaviour
    {
        private static Dictionary<string, ViewModel> nameToVMs = new Dictionary<string, ViewModel>();
        private static SortedSet<ViewModel> sortedVMs = new SortedSet<ViewModel>(new VMComparer());
        [SerializeField] private int numberOfDummyTask = 2000;

        public static ViewModel Get(string name) => nameToVMs[name];

        private void Awake()
        {
            var canvasGameObject = new GameObject { name = "LoadingSplashCanvas" };
            Canvas canvas = canvasGameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGameObject.AddComponent<CanvasScaler>();
            canvasGameObject.AddComponent<GraphicRaycaster>();
            var startUpScreen = new StartUpScreen(canvasGameObject);
            StartCoroutine(startUpScreen.OnTransitioning());
            EventManager.Subscribe<OnLoadingFinished>((sender, args) => startUpScreen.DoFadingOut());
        }

        private void Start()
        {
        }

        public static T AddViewModel<T>() where T : ViewModel, new()
        {
            var newViewModel = new T();
            nameToVMs.Add(typeof(T).Name, newViewModel);
            sortedVMs.Add(newViewModel);
            return newViewModel;
        }

        private class VMComparer : IComparer<ViewModel>
        {
            public int Compare(ViewModel x, ViewModel y)
            {
                return x.SortingOrder - y.SortingOrder;
            }
        }
    }
}
