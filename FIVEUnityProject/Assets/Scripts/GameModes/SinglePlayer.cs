using FIVE.Interactive;
using FIVE.Robot;
using FIVE.UI;
using FIVE.UI.AWSLEditor;
using FIVE.UI.InGameDisplay;
using UnityEngine;

namespace FIVE.GameStates
{
    public class SinglePlayer : GameMode
    {
        private void Start()
        {
            GameObject robot = RobotManager.CreateRobot();
            Inventory inventory = InventoryManager.AddInventory(robot);
            InventoryViewModel inventoryViewModel = UIManager.AddViewModel<InventoryViewModel>();
            inventoryViewModel.Inventory = inventory;
            inventoryViewModel.SetEnabled(false);
            AWSLEditorViewModel awslEditorViewModel = UIManager.AddViewModel<AWSLEditorViewModel>();
            awslEditorViewModel.SetEnabled(false);
            ItemDialogViewModel itemDialogViewModel = UIManager.AddViewModel<ItemDialogViewModel>();
            itemDialogViewModel.SetEnabled(false);
        }
    }
}
