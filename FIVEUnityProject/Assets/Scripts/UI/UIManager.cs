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
        private GameObject canvas;
        private Image backgroundImage;
        public GameObject menuButtonPrefab;

        private ViewModel viewModel;

        private void Awake()
        {
            viewModel = new StartupMenuViewModel();
        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}
