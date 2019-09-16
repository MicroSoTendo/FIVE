using System;
using UnityEngine;

namespace FIVE.UI.Background
{
    public class BackgroundViewModel : ViewModel<BackgroundView, BackgroundViewModel>
    {

        public string TestInputFieldText { get; set; }
        public BackgroundViewModel() : base()
        {

        }

    }
}