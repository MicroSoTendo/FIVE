using FIVE.CameraSystem;
using FIVE.EventSystem;
using FIVE.Interactive;
using FIVE.Network;
using FIVE.Robot;
using FIVE.TerrainSystem;
using FIVE.UI;
using FIVE.UI.BSComposite;
using FIVE.UI.CodeEditor;
using FIVE.UI.InGameDisplay;
using FIVE.UI.Multiplayers;
using FIVE.UI.NPC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE.GameModes
{
    [RequireComponent(typeof(NetworkManager))]
    public class Multiplayers : MonoBehaviour
    {
        private readonly List<string> prefabList = new List<string>()
        {
            "EntityPrefabs/Network/RobotPrefabs/robotSphere",
        };
        private LobbyWindowViewModel lobbyWindow;

        private void Awake()
        {
            lobbyWindow = UIManager.Create<LobbyWindowViewModel>();
            lobbyWindow.IsActive = true;
            EventManager.Subscribe<OnJoinRoomRequested, JoinRoomArgs>(JoinRoomHandler);
            EventManager.Subscribe<OnCreateRoomRequested, CreateRoomArgs>(CreateRoomHandler);
        }

        private void JoinRoomHandler(object sender, JoinRoomArgs joinRoomArgs)
        {
            NetworkManager.Instance.JoinRoom(joinRoomArgs.Guid, joinRoomArgs.Password);
            StartCoroutine(JoinRoomWait());
        }

        private IEnumerator JoinRoomWait()
        {
            const float waitTime = 0.1f;
            float timer = 0;
            //TODO: Refactor this
            while (NetworkManager.Instance.State != NetworkManager.NetworkState.Client)
            {
                timer += waitTime;
                if (timer > 2f)
                {
                    //TODO: POP up timeout
                    break;
                }
                yield return new WaitForSeconds(waitTime);
            }

            if (NetworkManager.Instance.State == NetworkManager.NetworkState.Client)
            {
                StartCoroutine(CommonInitRoutine());
                StartCoroutine(ClientInitRoutine());
            }
        }

        private void CreateRoomHandler(object sender, CreateRoomArgs createRoomArgs)
        {
            NetworkManager.Instance.CreateRoom(createRoomArgs.Name, createRoomArgs.MaxPlayers, createRoomArgs.HasPassword, createRoomArgs.Password);
            StartCoroutine(CommonInitRoutine());
            StartCoroutine(HostInitRoutine());
        }

        private IEnumerator CommonInitRoutine()
        {
            StartCoroutine(PrefabPool.Instance.LoadPrefabs(prefabList));
            TerrainManager.CreateTerrain(Vector3.zero);
            CameraManager.Remove("GUI Camera");
            UIManager.Create<HUDViewModel>().IsActive = true;
            UIManager.Create<NPCDialogueViewModel>().IsActive = false;
            UIManager.Create<BSCompositeViewModel>().IsActive = false;
            UIManager.Create<InGameMenuViewModel>().IsActive = false;
            NPCInit.Initialize();
            CodeEditorViewModel codeEditorViewModel = UIManager.Create<CodeEditorViewModel>();
            StartCoroutine(codeEditorViewModel.ToggleEditorCoroutine());
            codeEditorViewModel.IsActive = false;
            while (!PrefabPool.Instance.Initialized)
            {
                yield return null;
            }
            yield return null;
        }

        private Vector3 GetSpawnLocation(int playerIndex)
        {
            //TODO: Implement
            return default;
        }

        private IEnumerator ClientInitRoutine()
        {
            Vector3 spawnLocation = GetSpawnLocation(NetworkManager.Instance.PlayerIndex);
            GameObject robot = RobotManager.CreateRobot("robotSphere", spawnLocation, Quaternion.identity);
            SyncCenter.Instance.Register(robot.GetComponent<Transform>());
            SyncCenter.Instance.Register(robot.GetComponent<Animator>());
            yield break;
        }

        private IEnumerator HostInitRoutine()
        {
            yield return null;

            CameraManager.AddCamera("Default Camera 1", new Vector3(-40, 115, -138), Quaternion.Euler(30, 10, 0));
            CameraManager.AddCamera("Default Camera 2", new Vector3(33, 70.5f, -49), Quaternion.Euler(50, -32, 0));
            CameraManager.AddCamera("Default Camera 3", new Vector3(36.5f, 180f, 138), Quaternion.Euler(63, 230, -5f));

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

            Inventory inventory = InventoryManager.AddInventory(robot);
            InventoryViewModel inventoryViewModel = UIManager.Create<InventoryViewModel>();
            ItemDialogViewModel itemDialogViewModel = UIManager.Create<ItemDialogViewModel>();
            inventoryViewModel.Inventory = inventory;
            inventoryViewModel.IsActive = false;
            itemDialogViewModel.IsActive = false;
            CameraManager.SetCameraWall();
            yield return null;
        }
    }
}