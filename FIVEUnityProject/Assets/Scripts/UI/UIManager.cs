using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FIVE.EventSystem.EventTypes;
using FIVE.EventSystem;
using FIVE.UI.StartupMenu;
using FIVE.UI.OptionMenu;
using FIVE.UI.MainGameDisplay;
using FIVE.UI.Background;

namespace FIVE.UI
{
    public class UIManager : MonoBehaviour
    {
        private ViewModel viewModel;
        private Dictionary<string, ViewModel> viewModels = new Dictionary<string, ViewModel>();
        private void Awake()
        {
            viewModels.Add(nameof(StartupMenuView), new StartupMenuViewModel());

            viewModel = new OptionMenuViewModel();
            viewModel.SetActive(false);
            viewModels.Add(nameof(OptionMenuView), viewModel);

            viewModel = new GameDisplayViewModel();
            viewModel.SetActive(false);
            viewModels.Add(nameof(GameDisplayView), viewModel);

            viewModels.Add(nameof(BackgroundView), new BackgroundViewModel());
        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}
