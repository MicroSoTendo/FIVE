using System;
using FIVE.UI.Background;
using FIVE.UI.MainGameDisplay;
using FIVE.UI.OptionsMenu;
using FIVE.UI.StartupMenu;
using System.Collections.Generic;
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
            var canvas = canvasGameObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGameObject.AddComponent<CanvasScaler>();
            canvasGameObject.AddComponent<GraphicRaycaster>();

            var loadingActions = new Queue<Action>();
            GameObject go = null;
            loadingActions.Enqueue(() =>
            {
                go = Resources.Load<GameObject>("EntityPrefabs/CyberPunkPrefab");
                // go.SetActive(true);
            });
            loadingActions.Enqueue(() =>
            {
                Instantiate(go, Vector3.zero, Quaternion.identity).SetActive(true);
            });
            loadingActions.Enqueue(() =>
            {
                nameToVMs.Add(nameof(StartupMenuViewModel), new StartupMenuViewModel());

                ViewModel optionsMenuViewModel = new OptionsMenuViewModel();
                optionsMenuViewModel.SetActive(false);
                nameToVMs.Add(nameof(OptionsMenuView), optionsMenuViewModel);

                ViewModel gameDisplayViewModel = new GameDisplayViewModel();
                gameDisplayViewModel.SetActive(false);
                nameToVMs.Add(nameof(GameDisplayView), gameDisplayViewModel);

                ViewModel backgroundViewModel = new BackgroundViewModel();
                backgroundViewModel.SortingOrder = -10;
                nameToVMs.Add(nameof(BackgroundView), backgroundViewModel);
            });
            var startUpScreen = new StartUpScreen(canvasGameObject, loadingActions, numberOfDummyTask);
            StartCoroutine(startUpScreen.OnTransitioning());

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
