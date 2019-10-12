using FIVE.Interactive;
using FIVE.Robot;
using FIVE.UI;
using FIVE.UI.CodeEditor;
using FIVE.UI.InGameDisplay;
using UnityEngine;

namespace FIVE.GameStates
{
    public class SinglePlayer : GameMode
    {
        private void Start()
        {
            GameObject robot;
            robot = RobotManager.CreateRobot("robotSphere", new Vector3(0, 20, 0), Quaternion.identity);
            Inventory inventory = InventoryManager.AddInventory(robot);
            InventoryViewModel inventoryViewModel = UIManager.Create<InventoryViewModel>();
            CodeEditorViewModel codeEditorViewModel = UIManager.Create<CodeEditorViewModel>();
            ItemDialogViewModel itemDialogViewModel = UIManager.Create<ItemDialogViewModel>();
            inventoryViewModel.Inventory = inventory;
            inventoryViewModel.SetActive(false);
            codeEditorViewModel.SetActive(false);
            itemDialogViewModel.SetActive(false);
        }
    }
}