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
        [SerializeField] private GameObject Singelplayer;
        [SerializeField] private GameObject Multiplayers;
        private Camera guiCamera;
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
            UIManager.Create<InGameMenuViewModel>().IsActive = false;
            yield return null;
            UIManager.Create<HUDViewModel>().IsActive = false;
            yield return null;
            EventManager.Subscribe<OnSinglePlayerButtonClicked>((o, e) =>
            {
                Instantiate(Singelplayer);
                CameraManager.Remove(guiCamera);
            });
            EventManager.Subscribe<OnMultiPlayersButtonClicked>((o, e) =>
            {
                Instantiate(Multiplayers);
                CameraManager.Remove(guiCamera);
            });
        }
    }
}
