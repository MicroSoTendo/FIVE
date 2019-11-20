using System;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.InGameDisplay
{
    public class RecipeViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/Recipes";
        public Button BackButton { get; }

        public RecipeViewModel() : base()
        {
            BackButton = Get<Button>(nameof(BackButton));
            Bind(BackButton).To(OnExitGameClicked);
        }
        private void OnExitGameClicked()
        {
            IsActive = false;
        }
    }
}