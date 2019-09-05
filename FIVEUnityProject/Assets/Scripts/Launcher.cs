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
        private StartUpScreen loadingSplashScreenScreen;
        void Awake()
        {
            instantiated = false;
            loadingTasks = new Queue<Action>();
            loadingTasks.Enqueue(() =>
            {
                var MainMenu = new GameObject("Main Menu");
                MainMenu.AddComponent<UILoader>();
            });
            loadingSplashScreenScreen = new StartUpScreen(loadingTasks);
        }
        IEnumerator Start()
        {
            EventSystem.EventSystem.Subscribe(EventTypes.OnButtonClicked, OnButtonClicked);
            canvas = GetComponentInChildren<Canvas>();
            mainCamera = GameObject.Find("Main Camera");
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
                        gameCharacter = Instantiate(gameCharacterPrefab, new Vector3(64, 1, 64), Quaternion.Euler(0, 0, 0));
                        var eye = GameObject.Find("eyeDome");
                        mainCamera.transform.parent = eye.transform;
                        mainCamera.transform.localPosition = new Vector3(0, 0, 0);
                        mainCamera.transform.localRotation = Quaternion.Euler(0, 0, 0);
                        //mainCamera.transform.rotation = Quaternion.AngleAxis(5f, Vector3.right);
                        instantiated = true;
                        go.GetComponentInChildren<Text>().text = "Resume";
                        Destroy(canvas.gameObject.GetComponent<Image>());
                    }

                    canvas.gameObject.SetActive(false);
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
            }
        }
    }
}
