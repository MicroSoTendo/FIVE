using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.EventSystem;
using Assets.Scripts.UI;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Launcher : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject groundPrefab;
        public GameObject gameCharacterPrefab;

        private Canvas canvas;
        private CameraController cameraController;
        private GameObject mainCamera;
        private bool instantiated;

        private GameObject gameCharacter;

        private Queue<Action> loadingTasks;
        private LoadingSplashScreen loadingSplashScreenScreen;
        public Texture texture;

        void Awake()
        {
            canvas = GetComponentInChildren<Canvas>();
            mainCamera = GameObject.Find("Main Camera");
            cameraController = mainCamera.GetComponent<CameraController>();
            // cameraController.enabled = false;
            instantiated = false;
            EventSystem.EventSystem.Subscribe(EventTypes.OnButtonClicked, OnButtonClicked);


            loadingTasks = new Queue<Action>();
            var loadingGui = gameObject.AddComponent<LoadingSplashScreen.LoadingGUI>();
            loadingGui.OnGuiAction = () =>
            {
                GUI.DrawTexture(new Rect(10, 10, 60, 60), texture, ScaleMode.ScaleToFit, true, 10.0F);
            };
            loadingSplashScreenScreen = new LoadingSplashScreen(loadingTasks, loadingGui);
        }
        IEnumerator Start()
        {
            return loadingSplashScreenScreen.OnTransitioning();
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
                        gameCharacter = Instantiate(gameCharacterPrefab, new Vector3(64, 1, 64), Quaternion.identity);
                        var eye = GameObject.Find("eyeDome");
                        instantiated = true;
                        go.GetComponentInChildren<Text>().text = "Resume";
                    }

                    canvas.gameObject.SetActive(false);
                    //cameraController.enabled = true;
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
