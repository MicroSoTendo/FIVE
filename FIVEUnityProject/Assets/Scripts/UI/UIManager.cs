using FIVE.UI.Background;
using FIVE.UI.MainGameDisplay;
using FIVE.UI.OptionMenu;
using FIVE.UI.OptionMenus;
using FIVE.UI.StartupMenu;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.UI
{
    public class UIManager : MonoBehaviour
    {
        private static Dictionary<string, ViewModel> nameToVMs = new Dictionary<string, ViewModel>();
        private static SortedSet<ViewModel> sortedVMs = new SortedSet<ViewModel>(new VMComparer());


        public static ViewModel Get(string name) => nameToVMs[name];
        private void Awake()
        {
            ViewModel startupMenuViewModel = new StartupMenuViewModel();
            startupMenuViewModel.SortingOrder = -6;
            nameToVMs.Add(nameof(StartupMenuViewModel),startupMenuViewModel);

            ViewModel optionsMenuViewModel = new OptionsMenuViewModel();
            optionsMenuViewModel.SortingOrder = -1;
            optionsMenuViewModel.SetActive(false);
            nameToVMs.Add(nameof(OptionsMenuView), optionsMenuViewModel);

            ViewModel optionBGViewModel = new OptionBGViewModel();
            optionBGViewModel.SortingOrder = -2;
            optionBGViewModel.SetActive(false);
            nameToVMs.Add(nameof(OptionBGView), optionBGViewModel);

            ViewModel gameDisplayViewModel = new GameDisplayViewModel();
            gameDisplayViewModel.SetActive(false);
            gameDisplayViewModel.SortingOrder = -12;
            nameToVMs.Add(nameof(GameDisplayView), gameDisplayViewModel);

            ViewModel gameOptionViewModel = new GameOptionViewModel();
            gameOptionViewModel.SetActive(false);
            nameToVMs.Add(nameof(GameOptionView), gameOptionViewModel);

            ViewModel backgroundViewModel = new BackgroundViewModel();
            backgroundViewModel.SortingOrder = -10;
            nameToVMs.Add(nameof(BackgroundView), backgroundViewModel);
        }

        public static T AddViewModel<T>() where T : ViewModel, new()
        {
            var newViewModel = new T();
            nameToVMs.Add(typeof(T).Name, newViewModel);
            sortedVMs.Add(newViewModel);
            return newViewModel;
        }

        private class VMComparer : IComparer<ViewModel>
        {
            public int Compare(ViewModel x, ViewModel y)
            {
                return x.SortingOrder - y.SortingOrder;
            }
        }
    }
}
