using FIVE.EventSystem;
using System;
using UnityEngine;

namespace FIVE.CameraSystem
{
    public class GUICamera : MonoBehaviour
    {
        private void Awake()
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
