using FIVE.EventSystem;
using FIVE.Interactive;
using FIVE.Interactive.Blacksmith;
using FIVE.Robot;
using FIVE.UI.StartupMenu;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.RobotSelection
{
    
    public class RobotSelectionViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/RobotSelection";
        public Button SelectOne { get; }
        public Button SelectTwo { get; }
        public Button SelectThree { get; }

        public RobotSelectionViewModel() : base()
        {
            SelectOne = Get<Button>(nameof(SelectOne));
            Bind(SelectOne).To(OnSelectOneButtonClick);
            SelectTwo = Get<Button>(nameof(SelectTwo));
            Bind(SelectTwo).To(OnSelectTwoButtonClick);
            SelectThree = Get<Button>(nameof(SelectThree));
            Bind(SelectThree).To(OnSelectThreeButtonClick);
        }

        private void OnSelectThreeButtonClick()
        {
            
            IsActive = false;
            this.RaiseEvent<OnSinglePlayerButtonClicked>();
        }

        private void OnSelectTwoButtonClick()
        {
            IsActive = false;
            this.RaiseEvent<OnSinglePlayerButtonClicked>();
        }

        private void OnSelectOneButtonClick()
        {
            IsActive = false;
            this.RaiseEvent<OnSinglePlayerButtonClicked>();
        }
    }
}