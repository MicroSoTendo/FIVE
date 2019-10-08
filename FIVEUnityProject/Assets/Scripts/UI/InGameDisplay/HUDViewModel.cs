using FIVE.EventSystem;
using FIVE.Robot;
using FIVE.RobotComponents;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.InGameDisplay
{
    public class HUDViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/HUD";
        public Button PlayerButton { get; }
        public Scrollbar EnergyScrollbar { get; }
        public Button MenuButton { get; }
        public Button InventoryButton { get; }
        public Button ScanButton { get; }
        public HUDViewModel() : base()
        {
            PlayerButton = Get<Button>(nameof(PlayerButton));
            EnergyScrollbar = Get<Scrollbar>(nameof(EnergyScrollbar));
            MenuButton = Get<Button>(nameof(MenuButton));
            InventoryButton = Get<Button>(nameof(InventoryButton));
            ScanButton = Get<Button>(nameof(ScanButton));
            Bind(InventoryButton).To(OnInventoryClicked);
            Bind(MenuButton).To(OnOptionClicked);
            Bind(ScanButton).To(OnScanClicked);

            EventManager.Subscribe<OnRobotEnergyChanged, RobotEnergyChangedEventArgs>(UpdateEnergy);
        }

        private void UpdateEnergy(object sender, RobotEnergyChangedEventArgs e)
        {
            EnergyScrollbar.size = e.NewEnergyLevel / 100f;
        }
        private void OnScanClicked()
        {
            this.RaiseEvent<OnGlobalScanRequested>();
        }
        private void OnInventoryClicked()
        {
            UIManager.GetViewModel<InventoryViewModel>().ToggleEnabled();
        }
        private void OnOptionClicked()
        {
            UIManager.GetViewModel<InGameMenuViewModel>().SetEnabled(true);
        }

    }
}