using FIVE.UI.StartupMenu;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.UI
{
    public class UIManager : MonoBehaviour
    {
        public ViewModel viewModel;
        private readonly Dictionary<string, ViewModel> viewModels = new Dictionary<string, ViewModel>();
        private void Awake()
        {
            viewModels.Add(nameof(StartupMenuView), new StartupMenuViewModel());
        }


        // Update is called once per frame
        private void Update()
        {

        }
    }
}
