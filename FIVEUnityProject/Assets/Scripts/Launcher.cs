using Assets.Scripts.EventSystem;
using Assets.Scripts.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class Launcher : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject gameCharacterPrefab;

        private Canvas canvas;
        private bool instantiated;

        private GameObject gameCharacter;

        private Queue<Action> loadingTasks;
        private StartUpScreen loadingSplashScreenScreen;

        private void Awake()
        {
            instantiated = false;
            loadingTasks = new Queue<Action>();
            loadingTasks.Enqueue(() =>
            {
                var MainMenu = new GameObject("Main Menu");
                MainMenu.AddComponent<UILoader>();
            });
            loadingSplashScreenScreen = new StartUpScreen(loadingTasks, numberOfDummyTasks: 10, dummyTaskDuration: 2);
        }

        private IEnumerator Start()
        {
            EventSystem.EventSystem.Subscribe(EventTypes.OnButtonClicked, OnButtonClicked);
            canvas = GetComponentInChildren<Canvas>();
            return loadingSplashScreenScreen.OnTransitioning();
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            var go = sender as Button;
            if (go == null)
            {
                return;
            }

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
                        gameCharacter = Instantiate(gameCharacterPrefab, new Vector3(64, 1, 64), Quaternion.Euler(0, 0, 0));
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
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                canvas.gameObject.SetActive(!canvas.gameObject.activeSelf);
            }
        }
    }
}