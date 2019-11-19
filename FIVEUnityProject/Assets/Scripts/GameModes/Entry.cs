using FIVE.CameraSystem;
using FIVE.EventSystem;
using FIVE.UI;
using FIVE.UI.Background;
using FIVE.UI.StartupMenu;
using System.Collections;
using UnityEngine;

namespace FIVE.GameModes
{
    public class Entry : MonoBehaviour
    {
        private Camera guiCamera;
        [SerializeField] private GameObject Singelplayer = null;
        [SerializeField] private GameObject Multiplayers = null;

        public enum Mode { Single, Multi }

        public static Mode CurrentMode { get; private set; }

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
                CurrentMode = Mode.Single;
            });
            EventManager.Subscribe<OnMultiPlayersButtonClicked>((o, e) =>
            {
                Instantiate(Multiplayers);
                CurrentMode = Mode.Multi;
            });
        }
    }
}