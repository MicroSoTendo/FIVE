using UnityEngine;
using FIVE.EventSystem;
using System;

namespace FIVE
{
    public class GUICamera : MonoBehaviour
    {
        void Awake()
        {
            EventManager.Subscribe<OnLoadingGameMode>(OnLoadingGameMode);
        }
        private void OnLoadingGameMode(object sender, EventArgs args)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
            EventManager.Unsubscribe<OnLoadingGameMode>(OnLoadingGameMode);
        }
    }

}
