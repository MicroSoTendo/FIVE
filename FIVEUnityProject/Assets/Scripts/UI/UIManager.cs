using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FIVE.EventSystem.EventTypes;
using FIVE.EventSystem;
using FIVE.UI.StartupMenu;

namespace FIVE.UI
{
    public class UIManager : MonoBehaviour
    {
        public ViewModel viewModel;
        private Dictionary<string, ViewModel> viewModels = new Dictionary<string, ViewModel>();
        private void Awake()
        {
            viewModels.Add(nameof(StartupMenuView), new StartupMenuViewModel());
        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}
