﻿using System.Collections;
using FIVE.CameraSystem;
using FIVE.Interactive;
using FIVE.Robot;
using FIVE.TerrainSystem;
using FIVE.UI;
using FIVE.UI.Background;
using FIVE.UI.CodeEditor;
using FIVE.UI.InGameDisplay;
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
            CameraManager.AddCamera("DefaultCamera-1", new Vector3(-40, 115, -138), Quaternion.Euler(30, 10, 0));
            CameraManager.AddCamera("DefaultCamera-2", new Vector3(33, 70.5f, -49), Quaternion.Euler(50, -32, 0));
            CameraManager.AddCamera("DefaultCamera-3", new Vector3(36.5f, 180f, 138), Quaternion.Euler(63, 230, -5f));
            CameraManager.Remove("GUI Camera");
            var NPC = Resources.Load<GameObject>("EntityPrefabs/NPC/NPCsinglePrefabs/NPCBlue");
            Instantiate(NPC, new Vector3(-2.91f, 0f, 3.96f), Quaternion.Euler(new Vector3(0f, 140f, 0)));
            Instantiate(NPC, new Vector3(3.55f, 0f, -4.11f), Quaternion.Euler(new Vector3(0f, 270f, 0)));
            Instantiate(NPC, new Vector3(3.64f, 0.22f, 12.76f), Quaternion.Euler(new Vector3(0f, 180f, 0)));
            Instantiate(NPC, new Vector3(-2.91f, 0f, 3.96f), Quaternion.Euler(new Vector3(0f, 140f, 0)));
            Instantiate(NPC, new Vector3(3f, 0f, 3.96f), Quaternion.Euler(new Vector3(0f, 270f, 0)));
            Instantiate(NPC, new Vector3(-3f, 0f, 13f), Quaternion.Euler(new Vector3(0f, 140f, 0)));
            UIManager.Create<HUDViewModel>().IsActive = true;
            UIManager.Create<InGameMenuViewModel>().IsActive = false;
            //TODO: Parametrize prefab name with a UI selector
            GameObject robot = RobotManager.CreateRobot("robotSphere", new Vector3(0, 20, 0), Quaternion.identity);
            Inventory inventory = InventoryManager.AddInventory(robot);
            InventoryViewModel inventoryViewModel = UIManager.Create<InventoryViewModel>();
            CodeEditorViewModel codeEditorViewModel = UIManager.Create<CodeEditorViewModel>();
            ItemDialogViewModel itemDialogViewModel = UIManager.Create<ItemDialogViewModel>();
            inventoryViewModel.Inventory = inventory;
            inventoryViewModel.IsActive = false;
            codeEditorViewModel.IsActive = false;
            itemDialogViewModel.IsActive = false;
            CameraManager.SetCameraWall();
        }
    }
}