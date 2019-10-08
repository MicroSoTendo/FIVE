using FIVE.Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

namespace FIVE.UI.Multiplayers
{
    public class MultiplayersEntryViewModel : ViewModel
    {
        public GameObject ScrollWindow { get; }
        public Button JoinButton { get; }
        public Button CreateButton { get; }
        public Transform ContentTransform { get; }
        public MultiplayersEntryViewModel()
        {
        }

        protected override string PrefabPath { get; }
    }
}