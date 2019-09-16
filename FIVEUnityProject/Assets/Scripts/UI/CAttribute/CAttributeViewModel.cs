﻿using System;
using System.Linq.Expressions;
using System.Reflection;
using UnityEngine;

namespace FIVE.UI.CAttribute
{
    public class CAttributeViewModel : ViewModel<CAttributeView, CAttributeViewModel>
    {

        public string TestInputFieldText { get; set; }
        public CAttributeViewModel() : base()
        {

            binder.Bind(view => view.PlayerButton.onClick).
            To(viewModel => viewModel.OnPlayerButtonClicked);

            binder.Bind(view => view.Energy.onClick).
            To(viewModel => viewModel.OnEnergyClicked);

            binder.Bind(view => view.Inventory.onClick).
            To(viewModel => viewModel.OnInventoryClicked);

            binder.Bind(view => view.Option.onClick).
            To(viewModel => viewModel.OnOptionClicked);

            //binder.Bind(view => view.TestInputField.text).
            //To(viewMode => viewMode.TestInputFieldText, BindingMode.TwoWay);
        }

        private void OnPlayerButtonClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnPlayerButtonClicked));
        }
        private void OnEnergyClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnEnergyClicked));
        }
        private void OnInventoryClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnInventoryClicked));
        }
        private void OnOptionClicked(object sender, EventArgs eventArgs)
        {
            Debug.Log(nameof(OnOptionClicked));
        }

    }
}