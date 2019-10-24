using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine.UI;

namespace FIVE.UI.Multiplayers
{
    class PasswordPopUpViewModel : ViewModel
    {
        protected override string PrefabPath { get; } = "EntityPrefabs/UI/Multiplayers/PasswordPopUp";
        public Button ConfirmButton { get; }
        public TMP_InputField PasswordInputField { get; }
        
        public PasswordPopUpViewModel()
        {
            ZIndex = 2;
            ConfirmButton = Get<Button>(nameof(ConfirmButton));
            PasswordInputField = Get<TMP_InputField>(nameof(PasswordInputField));
        }
    }
}
