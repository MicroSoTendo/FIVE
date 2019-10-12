using FIVE.EventSystem;
using FIVE.UI;
using FIVE.UI.Background;
using FIVE.UI.InGameDisplay;
using FIVE.UI.SplashScreens;
using FIVE.UI.StartupMenu;
using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace FIVE.GameStates
{
    public class Entry : GameMode
    {
        private IEnumerator Start()
        {
            GameObject terrainPrefab = null;
            terrainPrefab = Resources.Load<GameObject>("EntityPrefabs/CyberPunkPrefab"); Instantiate(terrainPrefab, Vector3.zero, Quaternion.identity).SetActive(true);
            while (!ViewModel.Initialized)
            {
                yield return null;
            }
            StartupMenuViewModel startupMenuViewModel = UIManager.Create<StartupMenuViewModel>();
            BackgroundViewModel backgroundViewModel = UIManager.Create<BackgroundViewModel>();
            startupMenuViewModel.SetActive(true);
            backgroundViewModel.SetActive(true);
            startupMenuViewModel.ZIndex = 1;
            backgroundViewModel.ZIndex = 0;
            UIManager.Create<InGameMenuViewModel>().SetActive(false);
            yield return null;
            UIManager.Create<HUDViewModel>().SetActive(false);
            yield return null;
            EventManager.Subscribe<OnSinglePlayerButtonClicked>((o, e) => { SwitchTo<SinglePlayer>(); });
            EventManager.Subscribe<OnMultiPlayersButtonClicked>((o, e) => { SwitchTo<MultiPlayer>(); });
        }
    }
}
