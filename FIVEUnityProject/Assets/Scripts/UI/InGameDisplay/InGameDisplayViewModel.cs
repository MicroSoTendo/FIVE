using FIVE.UI.OptionsMenu;
using System;
using FIVE.Robot;
using UnityEngine;

namespace FIVE.UI.InGameDisplay
{
    public class InGameDisplayViewModel : ViewModel<InGameDisplayView, InGameDisplayViewModel>
    {
        public InGameDisplayViewModel() : base()
        {

            binder.Bind(view => view.PlayerButton.onClick).
            To(viewModel => viewModel.OnPlayerButtonClicked);

            binder.Bind(view => view.Energy.onClick).
            To(viewModel => viewModel.OnEnergyClicked);

            binder.Bind(view => view.Inventory.onClick).
            To(viewModel => viewModel.OnInventoryClicked);

            binder.Bind(view => view.Option.onClick).
            To(viewModel => viewModel.OnOptionClicked);

            binder.Bind(view=>view.Scan.onClick).
                To(vm=>vm.OnScanClicked);
        }

        private void OnScanClicked(object sender, EventArgs e)
        {
            this.RaiseEvent<OnGlobalScanRequested>();
        }

        private void OnPlayerButtonClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnPlayerButtonClicked));
        }
        private void OnEnergyClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnEnergyClicked));
        }
        private void OnInventoryClicked(object sender, EventArgs eventArgs)
        {
            UIManager.GetViewModel<InventoryViewModel>().ToggleEnabled();
        }
        private void OnOptionClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnOptionClicked));
            UIManager.GetViewModel<OptionsMenuViewModel>().SetEnabled(true);
        }

    }
}