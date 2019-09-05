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
            loadingSplashScreenScreen = new StartUpScreen(loadingTasks, numberOfDummyTasks: 200, dummyTaskDuration: 2);
        }

        private IEnumerator Start()
        {
            EventSystem.EventSystem.Subscribe(EventTypes.OnButtonClicked, OnButtonClicked);
            canvas = GetComponentInChildren<Canvas>();
            return loadingSplashScreenScreen.OnTransitioning();
        }

        private Vector3 exitBottonPosition;

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
                        var exitButton = GameObject.Find("ExitButton");
                        exitBottonPosition = exitButton.transform.localPosition;
                        var returnButton = GameObject.Find("Main Menu").GetComponent<UILoader>()
                            .InstantiateNewButton("Return To Home", exitBottonPosition).GetComponent<Button>();
                        returnButton.onClick.AddListener(delegate { EventSystem.EventSystem.RaiseEvent(EventTypes.OnButtonClicked, returnButton, EventArgs.Empty); }); ;
                        exitButton.transform.localPosition = new Vector3(exitBottonPosition.x, exitBottonPosition.y * 1.5f, 0);
                        canvas.gameObject.GetComponent<Image>().CrossFadeAlpha(0, 1, false);
                    }

                    canvas.gameObject.SetActive(false);
                    break;

                case "Return To HomeButton":
                    GameObject.Find("Main Camera").transform.parent = null;
                    GameObject.Find("StartButton").GetComponentInChildren<Text>().text = "Start";
                    var po = GameObject.Find("ExitButton").transform.localPosition;
                    GameObject.Find("ExitButton").transform.localPosition = exitBottonPosition;
                    Destroy(GameObject.Find("Return To HomeButton"));
                    Destroy(gameCharacter);
                    canvas.gameObject.GetComponent<Image>().CrossFadeAlpha(1, 2, false);
                    instantiated = false;
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