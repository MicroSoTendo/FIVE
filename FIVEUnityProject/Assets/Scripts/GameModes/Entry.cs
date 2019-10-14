using FIVE.EventSystem;
using FIVE.UI;
using FIVE.UI.Background;
using FIVE.UI.InGameDisplay;
using FIVE.UI.StartupMenu;
using System.Collections;
using FIVE.CameraSystem;
using UnityEngine;

namespace FIVE.GameModes
{
    public class Entry : MonoBehaviour
    {
        private Camera guiCamera;
        [SerializeField] private GameObject Singelplayer;
        [SerializeField] private GameObject Multiplayers;

        private IEnumerator Start()
        {
            guiCamera = CameraManager.AddCamera("GUI Camera", enableAudioListener: true);
            while (!ViewModel.Initialized)
            {
                yield return null;
            }
            StartupMenuViewModel startupMenuViewModel = UIManager.Create<StartupMenuViewModel>();
            BackgroundViewModel backgroundViewModel = UIManager.Create<BackgroundViewModel>();
            startupMenuViewModel.IsActive = true;
            backgroundViewModel.IsActive = true;
            startupMenuViewModel.ZIndex = 1;
            backgroundViewModel.ZIndex = 0;
            EventManager.Subscribe<OnSinglePlayerButtonClicked>((o, e) =>
            {
                Instantiate(Singelplayer);
            });
            EventManager.Subscribe<OnMultiPlayersButtonClicked>((o, e) =>
            {
                Instantiate(Multiplayers);
            });
        }
    }
}