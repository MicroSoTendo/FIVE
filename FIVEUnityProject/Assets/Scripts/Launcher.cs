using System;
using UnityEngine;
using Assets.Scripts.EventSystem;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Launcher : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject groundPrefab;

        private Canvas canvas;
        private CameraController cameraController;
        private bool instantiated;
        void Start()
        {
            canvas = GetComponentInChildren<Canvas>();
            cameraController = GameObject.Find("Main Camera").GetComponent<CameraController>();
            cameraController.enabled = false;
            instantiated = false;
            EventSystem.EventSystem.Subscribe(EventTypes.OnButtonClicked, OnButtonClicked);
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            var go = sender as Button;
            if (go == null) return;
            switch (go.name)
            {
                case "ExitButton":
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#endif
                    Application.Quit(0);
                    break;
                case "StartButton":
                    if (!instantiated)
                    {
                        Instantiate(groundPrefab);
                        instantiated = true;
                        go.GetComponentInChildren<Text>().text = "Resume";
                    }

                    canvas.gameObject.SetActive(false);
                    cameraController.enabled = true;
                    break;
                default:
                    break;

            }

        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
                cameraController.enabled = !cameraController.enabled;
            }
        }
    }
}
