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
using FIVE.UI.Background;
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
            UIManager.Get<BackgroundViewModel>().IsActive = false;
            StartCoroutine(PrefabPool.Instance.LoadPrefabs(prefabList));
            TerrainManager.CreateTerrain(Vector3.zero);
            CameraManager.Remove("GUI Camera");
            UIManager.Create<HUDViewModel>().IsActive = true;
            UIManager.Create<NPCDialogueViewModel>().IsActive = false;
            UIManager.Create<BSCompositeViewModel>().IsActive = false;
            InGameMenuViewModel inGameMenuViewModel = UIManager.Create<InGameMenuViewModel>();
            inGameMenuViewModel.IsActive = false;
            inGameMenuViewModel.ExitGameButton.onClick.AddListener(OnExit);
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

        private void OnExit()
        {
            if (NetworkManager.Instance.State == NetworkManager.NetworkState.Host)
            {
                NetworkManager.Instance.Disconnect();
            }
        }

        //TODO: Refactor spawn location later
        private readonly List<(Vector3, Quaternion)>[] spawnPoints =
        {
            new List<(Vector3, Quaternion)>
            {
                
                (new Vector3(-10, 20, 0), Quaternion.identity),
                (new Vector3(0, 20, 0), Quaternion.identity),
                (new Vector3(10, 20, 0), Quaternion.identity),
                (new Vector3(-10, 20, -10), Quaternion.identity),
                (new Vector3(0, 20, -10), Quaternion.identity),
                (new Vector3(10, 20, -10), Quaternion.identity),
                (new Vector3(-10, 20, -20), Quaternion.identity),
                (new Vector3(0, 20, -20), Quaternion.identity),
                (new Vector3(10, 20, -20), Quaternion.identity),
            },
            new List<(Vector3, Quaternion)>
            {
                (new Vector3(-50, 20, 110), Quaternion.Euler(0,90,0)),
                (new Vector3(-50, 20, 100), Quaternion.Euler(0,90,0)),
                (new Vector3(-50, 20, 90), Quaternion.Euler(0,90,0)),
                (new Vector3(-60, 20, 110), Quaternion.Euler(0,90,0)),
                (new Vector3(-60, 20, 100), Quaternion.Euler(0,90,0)),
                (new Vector3(-60, 20, 90), Quaternion.Euler(0,90,0)),
                (new Vector3(-70, 20, 110), Quaternion.Euler(0,90,0)),
                (new Vector3(-70, 20, 100), Quaternion.Euler(0,90,0)),
                (new Vector3(-70, 20, 90), Quaternion.Euler(0,90,0)),
            },
            new List<(Vector3, Quaternion)>
            {
                (new Vector3(50, 20, 110), Quaternion.Euler(0,-90,0)),
                (new Vector3(50, 20, 100), Quaternion.Euler(0,-90,0)),
                (new Vector3(50, 20, 90), Quaternion.Euler(0,-90,0)),
                (new Vector3(60, 20, 110), Quaternion.Euler(0,-90,0)),
                (new Vector3(60, 20, 100), Quaternion.Euler(0,-90,0)),
                (new Vector3(60, 20, 90), Quaternion.Euler(0,-90,0)),
                (new Vector3(70, 20, 110), Quaternion.Euler(0,-90,0)),
                (new Vector3(70, 20, 100), Quaternion.Euler(0,-90,0)),
                (new Vector3(70, 20, 90), Quaternion.Euler(0,-90,0)),
            },
            new List<(Vector3, Quaternion)>
            {
                (new Vector3(10, 20, 140), Quaternion.Euler(0,-180,0)),
                (new Vector3(0, 20, 140), Quaternion.Euler(0,-180,0)),
                (new Vector3(-10, 20, 140), Quaternion.Euler(0,-180,0)),
                (new Vector3(10, 20, 150), Quaternion.Euler(0,-180,0)),
                (new Vector3(0, 20, 150), Quaternion.Euler(0,-180,0)),
                (new Vector3(-10, 20, 150), Quaternion.Euler(0,-180,0)),
                (new Vector3(10, 20, 160), Quaternion.Euler(0,-180,0)),
                (new Vector3(0, 20, 160), Quaternion.Euler(0,-180,0)),
                (new Vector3(-10, 20, 160), Quaternion.Euler(0,-180,0)),
            },
            new List<(Vector3, Quaternion)>
            {
                (new Vector3(-10, 20, -50), Quaternion.identity),
                (new Vector3(0, 20, -50), Quaternion.identity),
                (new Vector3(10, 20, -50), Quaternion.identity),
                (new Vector3(-10, 20, -60), Quaternion.identity),
                (new Vector3(0, 20, -60), Quaternion.identity),
                (new Vector3(10, 20, -60), Quaternion.identity),
                (new Vector3(-10, 20, -70), Quaternion.identity),
                (new Vector3(0, 20, -70), Quaternion.identity),
                (new Vector3(10, 20, -70), Quaternion.identity),
            },
        };

        private IEnumerator ClientInitRoutine()
        {
            Inventory inventory = null;
            List<(Vector3, Quaternion)> points = spawnPoints[NetworkManager.Instance.PlayerIndex];
            for (int i = 0; i < points.Count; i++)
            {
                (Vector3 position, Quaternion rotation) = points[i];
                GameObject robot = RobotManager.CreateRobot("robotSphere", position, rotation);
                if (i == 0)
                {
                    RobotManager.ActiveRobot = robot;
                    inventory = InventoryManager.AddInventory(robot);
                }

                SyncCenter.Instance.Register(robot);
                SyncCenter.Instance.Register(robot.GetComponent<Transform>());
                SyncCenter.Instance.Register(robot.GetComponent<Animator>());
                yield return null;
            }
            InventoryViewModel inventoryViewModel = UIManager.Create<InventoryViewModel>();
            ItemDialogViewModel itemDialogViewModel = UIManager.Create<ItemDialogViewModel>();
            inventoryViewModel.Inventory = inventory;
            inventoryViewModel.IsActive = false;
            itemDialogViewModel.IsActive = false;
        }

        private IEnumerator HostInitRoutine()
        {
            yield return null;

            CameraManager.AddCamera("Default Camera 1", new Vector3(-40, 115, -138), Quaternion.Euler(30, 10, 0));
            CameraManager.AddCamera("Default Camera 2", new Vector3(33, 70.5f, -49), Quaternion.Euler(50, -32, 0));
            CameraManager.AddCamera("Default Camera 3", new Vector3(36.5f, 180f, 138), Quaternion.Euler(63, 230, -5f));

            List<(Vector3, Quaternion)> points = spawnPoints[0];
            Inventory inventory = null;
            for (int i = 0; i < points.Count; i++)
            {
                (Vector3 position, Quaternion rotation) = points[i];
                GameObject robot = RobotManager.CreateRobot("robotSphere", position, rotation);
                if (i == 0)
                {
                    RobotManager.ActiveRobot = robot;
                    inventory = InventoryManager.AddInventory(robot);
                }
                SyncCenter.Instance.Register(robot);
                SyncCenter.Instance.Register(robot.GetComponent<Transform>());
                SyncCenter.Instance.Register(robot.GetComponent<Animator>());
                yield return null;
            }
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