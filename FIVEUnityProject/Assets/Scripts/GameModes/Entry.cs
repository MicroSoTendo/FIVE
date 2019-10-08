using FIVE.EventSystem;
using FIVE.UI;
using FIVE.UI.Background;
using FIVE.UI.InGameDisplay;
using FIVE.UI.SplashScreens;
using FIVE.UI.StartupMenu;
using FIVE.UI.NPC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace FIVE.GameStates
{
    public class Entry : GameMode
    {
        private readonly List<Action> loadingActions = new List<Action>();

        private IEnumerator LoadingBasicUI()
        {
            StartupMenuViewModel startupMenuViewModel = UIManager.AddViewModel<StartupMenuViewModel>();
            yield return null;
            BackgroundViewModel backgroundViewModel = UIManager.AddViewModel<BackgroundViewModel>();
            backgroundViewModel.SortingOrder = -10;
            yield return null;
            EventManager.Subscribe<OnFadedOut>(async (o, e) =>
            {
                await Task.Delay(1800);
                startupMenuViewModel.SetEnabled(true);
                backgroundViewModel.SetEnabled(true);
            });
            UIManager.AddViewModel<InGameMenuViewModel>().SetEnabled(false);
            yield return null;
            UIManager.AddViewModel<HUDViewModel>().SetEnabled(false);
            yield return null;
        }

        private void Awake()
        {
            GameObject terrainPrefab = null;
            loadingActions.Add(() => { terrainPrefab = Resources.Load<GameObject>("EntityPrefabs/CyberPunkPrefab"); });
            loadingActions.Add(() => { Instantiate(terrainPrefab, Vector3.zero, Quaternion.identity).SetActive(true); });
            //Dummy Tasks
            for (int i = 0; i < 200; i++)
            {
                loadingActions.Add(async () => { await Task.Delay(1); });
            }
            loadingActions.Add(() => { StartCoroutine(LoadingBasicUI()); });
        }

        private IEnumerator Start()
        {
            for (int i = 0; i < loadingActions.Count; i++)
            {
                loadingActions[i]();
                this.RaiseEvent<OnProgressUpdated>(new OnProgressUpdatedEventArgs((i + 1f) / loadingActions.Count));
                yield return null;
            }
            loadingActions.Clear();
            this.RaiseEvent<OnLoadingFinished>(EventArgs.Empty);
            EventManager.Subscribe<OnSinglePlayerButtonClicked>((o, e) => { SwitchTo<SinglePlayer>(); });
            EventManager.Subscribe<OnMultiPlayersButtonClicked>((o, e) => { SwitchTo<MultiPlayer>(); });
        }
    }
    public abstract class OnLoadingFinished : IEventType { }
}
