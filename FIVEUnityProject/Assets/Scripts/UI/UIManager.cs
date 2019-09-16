using System.Collections.Generic;
using UnityEngine;
using FIVE.UI.StartupMenu;

namespace FIVE.UI
{
    public class UIManager : MonoBehaviour
    {
        private static Dictionary<string, ViewModel> viewModels = new Dictionary<string, ViewModel>();

        private void Awake()
        {
            viewModels.Add(nameof(StartupMenuView), new StartupMenuViewModel());
        }

        public static T AddViewModel<T>() where T : ViewModel, new()
        {
            var newViewModel = new T();
            viewModels.Add(typeof(T).Name, newViewModel);
            return newViewModel;
        }
    }
}
