using FIVE.UI.Background;
using FIVE.UI.MainGameDisplay;
using FIVE.UI.OptionMenu;
using FIVE.UI.StartupMenu;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.UI
{
    public class UIManager : MonoBehaviour
    {
        private static Dictionary<string, ViewModel> viewModels = new Dictionary<string, ViewModel>();

        private void Awake()
        {
            viewModels.Add(nameof(StartupMenuView), new StartupMenuViewModel());

            ViewModel viewModel = new OptionMenuViewModel();
            viewModel.SetActive(false);
            viewModels.Add(nameof(OptionMenuView), viewModel);

            viewModel = new GameDisplayViewModel();
            viewModel.SetActive(false);
            viewModels.Add(nameof(GameDisplayView), viewModel);

            viewModels.Add(nameof(BackgroundView), new BackgroundViewModel());
        }

        public static T AddViewModel<T>() where T : ViewModel, new()
        {
            var newViewModel = new T();
            viewModels.Add(typeof(T).Name, newViewModel);
            return newViewModel;
        }
    }
}
