using FIVE.CameraSystem;
using FIVE.EventSystem;
using FIVE.Robot;
using FIVE.RobotComponents;
using System;
using UnityEngine.UI;

namespace FIVE.UI.InGameDisplay
{
    public class HUDViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/HUD";
        public Button PlayerButton { get; }
        public Text BuffText { get; }
        public Scrollbar EnergyScrollbar { get; }
        public Button MenuButton { get; }
        public Button InventoryButton { get; }
        public Button ScanButton { get; }
        public Button MultiCameraModeButton { get; }
        public Button RecipeButton { get; }

        public HUDViewModel() : base()
        {
            PlayerButton = Get<Button>(nameof(PlayerButton));
            EnergyScrollbar = Get<Scrollbar>(nameof(EnergyScrollbar));
            MenuButton = Get<Button>(nameof(MenuButton));
            InventoryButton = Get<Button>(nameof(InventoryButton));
            ScanButton = Get<Button>(nameof(ScanButton));
            RecipeButton = Get<Button>(nameof(RecipeButton));
            MultiCameraModeButton = Get<Button>(nameof(MultiCameraModeButton));
            Bind(MultiCameraModeButton).To(OnMultiCameraModeButtonPressed);
            Bind(InventoryButton).To(OnInventoryClicked);
            Bind(MenuButton).To(OnOptionClicked);
            Bind(ScanButton).To(OnScanClicked);
            Bind(RecipeButton).To(OnRecipeClicked);
            EventManager.Subscribe<OnRobotEnergyChanged, RobotEnergyChangedEventArgs>(UpdateEnergy);
            MultiCameraModeButton.gameObject.SetActive(true);
        }

        private void OnRecipeClicked()
        {
            UIManager.Get<RecipeViewModel>().ToggleEnabled();
        }

        private void OnMultiCameraModeButtonPressed()
        {
            EventManager.RaiseImmediate<OnMultiCameraModeRequested>(this, EventArgs.Empty);
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
            UIManager.Get<InventoryViewModel>().ToggleEnabled();
        }

        private void OnOptionClicked()
        {
            UIManager.Get<InGameMenuViewModel>().ToggleEnabled();
        }
    }
}