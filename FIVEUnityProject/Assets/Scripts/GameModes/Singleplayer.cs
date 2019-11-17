using FIVE.CameraSystem;
using FIVE.Robot;
using FIVE.TerrainSystem;
using FIVE.UI;
using FIVE.UI.Background;
using FIVE.UI.BSComposite;
using FIVE.UI.CodeEditor;
using FIVE.UI.InGameDisplay;
using FIVE.UI.NPC;
using FIVE.UI.RobotSelection;
using System.Collections;
using UnityEngine;

namespace FIVE.GameModes
{
    public class Singleplayer : MonoBehaviour
    {
        private void Awake()
        {
            UIManager.Get<BackgroundViewModel>().IsActive = false;
        }

        private IEnumerator Start()
        {
            TerrainManager.CreateTerrain(Vector3.zero);

            yield return null;

            CameraManager.AddCamera("Default Camera 1", new Vector3(-40, 115, -138), Quaternion.Euler(30, 10, 0));
            CameraManager.AddCamera("Default Camera 2", new Vector3(33, 70.5f, -49), Quaternion.Euler(50, -32, 0));
            CameraManager.AddCamera("Default Camera 3", new Vector3(36.5f, 180f, 138), Quaternion.Euler(63, 230, -5f));
            CameraManager.Remove("GUI Camera");

            NPCInit.Initialize();

            UIManager.Create<HUDViewModel>().IsActive = true;
            UIManager.Create<NPCDialogueViewModel>().IsActive = false;
            UIManager.Create<BSCompositeViewModel>().IsActive = false;
            UIManager.Create<InGameMenuViewModel>().IsActive = false;
            //TODO: Parametrize prefab name with a UI selector
            CodeEditorViewModel codeEditorViewModel = UIManager.Create<CodeEditorViewModel>();
            StartCoroutine(codeEditorViewModel.ToggleEditorCoroutine());

            GameObject robot = RobotManager.CreateRobot("robotSphere", new Vector3(-10, 20, 0), Quaternion.identity);
            RobotManager.ActiveRobot = robot;
            RobotManager.CreateRobot("robotSphere", new Vector3(0, 20, 0), Quaternion.identity);
            RobotManager.CreateRobot("robotSphere", new Vector3(10, 20, 0), Quaternion.identity);
            RobotManager.CreateRobot("robotSphere", new Vector3(-10, 20, -10), Quaternion.identity);
            RobotManager.CreateRobot("robotSphere", new Vector3(0, 20, -10), Quaternion.identity);
            RobotManager.CreateRobot("robotSphere", new Vector3(10, 20, -10), Quaternion.identity);
            RobotManager.CreateRobot("robotSphere", new Vector3(-10, 20, -20), Quaternion.identity);
            RobotManager.CreateRobot("robotSphere", new Vector3(0, 20, -20), Quaternion.identity);
            RobotManager.CreateRobot("robotSphere", new Vector3(10, 20, -20), Quaternion.identity);

            GameObject enemyManagerPrefab = Resources.Load<GameObject>("InfrastructurePrefabs/EnemyManager");
            Instantiate(enemyManagerPrefab, new Vector3(0, 0, 0), Quaternion.identity);

            InventoryViewModel inventoryViewModel = UIManager.Create<InventoryViewModel>();
            ItemDialogViewModel itemDialogViewModel = UIManager.Create<ItemDialogViewModel>();
            inventoryViewModel.IsActive = false;
            codeEditorViewModel.IsActive = false;
            itemDialogViewModel.IsActive = false;
            CameraManager.SetCameraWall();
        }
    }
}