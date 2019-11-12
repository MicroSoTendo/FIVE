using FIVE.Interactive;
using FIVE.Interactive.Blacksmith;
using FIVE.Robot;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.RobotSelection
{
    /*
    public class BSCompositeViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/RobotSelection";
        protected override RenderMode ViewModelRenderMode { get; } = RenderMode.ScreenSpaceCamera;
        public GameObject Inventory { get; }
        public GameObject Composite { get; }
        public Button Result { get; }
        public Button BackButton { get; }
        private int[] emptyComposites;
        private int[] emptyInventory;
        private List<Button> inventoryButtons;
        private List<Button> compositeButtons;

        public BSCompositeViewModel() : base()
        {
            inventoryButtons = new List<Button>();
            //Add inventory item buttons
            Inventory = Get(nameof(Inventory));
            compositeButtons = new List<Button>();
            Composite = Get(nameof(Composite));
            emptyComposites = new int[3] { 0, 0, 0 };
            emptyInventory = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            BackButton = Get<Button>(nameof(BackButton));
            //Bind(BackButton).To(OnBackButtonClick);
            Result = Get<Button>(nameof(Result));
            //Bind(Result).To(() => OnResultButtonClick(Result));
        }

    }*/ 
}