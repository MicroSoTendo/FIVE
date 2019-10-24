using System;
using FIVE.EventSystem;
using FIVE.Network;
using FIVE.UI;
using FIVE.UI.Multiplayers;
using System.Collections;
using FIVE.CameraSystem;
using FIVE.Interactive;
using FIVE.Robot;
using FIVE.TerrainSystem;
using FIVE.UI.Background;
using FIVE.UI.CodeEditor;
using FIVE.UI.InGameDisplay;
using FIVE.UI.NPC;
using UnityEngine;


namespace FIVE.GameModes
{
    [RequireComponent(typeof(NetworkManager))]
    public class Multiplayers : MonoBehaviour
    {
        private LobbyWindowViewModel lobbyWindow;
        private void Awake()
        {
            lobbyWindow = UIManager.Create<LobbyWindowViewModel>();
            lobbyWindow.IsActive = true;
            EventManager.Subscribe<OnCreateRoomRequested, CreateRoomRequestedEventArgs>(CreateRoomHandler);
            EventManager.Subscribe<OnJoinRoomRequested>(JoinRoomHandler);
        }

        private IEnumerator CommonInitRoutine()
        {
            TerrainManager.CreateTerrain(Vector3.zero);
            CameraManager.Remove("GUI Camera"); 
            UIManager.Create<HUDViewModel>().IsActive = true;
            UIManager.Create<NPCDialogueViewModel>().IsActive = false;
            UIManager.Create<InGameMenuViewModel>().IsActive = false;
            yield return null;
        }
        private IEnumerator ClientInitRoutine()
        {
            
            yield return null;
        }
        private IEnumerator HostInitRoutine()
        {
            CameraManager.AddCamera("DefaultCamera-1", new Vector3(-40, 115, -138), Quaternion.Euler(30, 10, 0));
            CameraManager.AddCamera("DefaultCamera-2", new Vector3(33, 70.5f, -49), Quaternion.Euler(50, -32, 0));
            CameraManager.AddCamera("DefaultCamera-3", new Vector3(36.5f, 180f, 138), Quaternion.Euler(63, 230, -5f));
            NPCInit.Initialize();
           
            //TODO: Parametrize prefab name with a UI selector
            GameObject robot = RobotManager.CreateRobot("robotSphere", new Vector3(0, 20, 0), Quaternion.identity);
            RobotManager.ActiveRobot = robot;
            GameObject robot1 = RobotManager.CreateRobot("robotSphere", new Vector3(0, 20, 20), Quaternion.identity);
            GameObject enemyManagerPrefab = Resources.Load<GameObject>("InfrastructurePrefabs/EnemyManager");
            Instantiate(enemyManagerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
            Inventory inventory = InventoryManager.AddInventory(robot);
            InventoryViewModel inventoryViewModel = UIManager.Create<InventoryViewModel>();
            CodeEditorViewModel codeEditorViewModel = UIManager.Create<CodeEditorViewModel>();
            ItemDialogViewModel itemDialogViewModel = UIManager.Create<ItemDialogViewModel>();
            inventoryViewModel.Inventory = inventory;
            inventoryViewModel.IsActive = false;
            codeEditorViewModel.IsActive = false;
            itemDialogViewModel.IsActive = false;
            CameraManager.SetCameraWall();
            yield return null;
        }

        private void CreateRoomHandler(object sender, CreateRoomRequestedEventArgs e)
        {
            NetworkManager.CreateRoom(e.Name, e.MaxPlayer, e.HasPassword, e.Password);
            StartCoroutine(HostInitRoutine());
            lobbyWindow.IsActive = false;
        }
        
        private void JoinRoomHandler(object sender, EventArgs e)
        {
            
        }


    }
}