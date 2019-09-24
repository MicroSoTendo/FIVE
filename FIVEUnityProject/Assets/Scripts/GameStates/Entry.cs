using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FIVE.EventSystem;
using FIVE.UI;
using FIVE.UI.Background;
using FIVE.UI.MainGameDisplay;
using FIVE.UI.OptionsMenu;
using FIVE.UI.SplashScreens;
using FIVE.UI.StartupMenu;
using UnityEngine;

namespace FIVE.GameStates
{
    public abstract class OnLoadingFinished : IEventType { }
    public class Entry : GameState
    {
        private readonly List<Action> loadingActions = new List<Action>();

        private void LoadingBasicUI()
        {
            var startupMenuViewModel = UIManager.AddViewModel<StartupMenuViewModel>();
            var backgroundViewModel = UIManager.AddViewModel<BackgroundViewModel>();
            EventManager.Subscribe<OnLoadingFinished>((o, e) =>
            {
                startupMenuViewModel.SetActive(true);
                backgroundViewModel.SetActive(true);
            });
            UIManager.AddViewModel<OptionsMenuViewModel>().SetActive(false);
            UIManager.AddViewModel<GameDisplayViewModel>().SetActive(false);
        }

        void Awake()
        {
            GameObject terrainPrefab = null;
            loadingActions.Add(() => { terrainPrefab = Resources.Load<GameObject>("EntityPrefabs/CyberPunkPrefab"); });
            loadingActions.Add(() => { Instantiate(terrainPrefab, Vector3.zero, Quaternion.identity).SetActive(true); });
            //Dummy Tasks
            for (int i = 0; i < 200; i++)
            {
                loadingActions.Add(async () => { await Task.Delay(1); });
            }
            loadingActions.Add(LoadingBasicUI);
        }

        IEnumerator Start()
        {
            for (int i = 0; i < loadingActions.Count; i++)
            {
                loadingActions[i]();
                this.RaiseEvent<OnProgressUpdated>(new OnProgressUpdatedEventArgs((float)(i + 1f) / loadingActions.Count));
                yield return null;
            }
            loadingActions.Clear();
            this.RaiseEvent<OnLoadingFinished>(EventArgs.Empty);
        }
    }
}
