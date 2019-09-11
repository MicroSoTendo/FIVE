using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FIVE.EventSystem.EventTypes;
using FIVE.EventSystem;

namespace FIVE.UI
{
    public class UIManager : MonoBehaviour
    {
        private GameObject canvas;
        private Image backgroundImage;
        public GameObject menuButtonPrefab;

        private void Awake()
        {
            canvas = new GameObject("Canvas");
            var canvasComponent = canvas.AddComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.AddComponent<CanvasScaler>();
            canvas.AddComponent<GraphicRaycaster>();

            backgroundImage = canvas.gameObject.AddComponent<Image>();
        }

        void Start()
        {
            backgroundImage.sprite = Resources.Load<Sprite>("Graphics/UI/background");
            if (menuButtonPrefab == null)
            {
                menuButtonPrefab = Resources.Load<GameObject>("EntityPrefabs/MenuButton");
            }

            var y = menuButtonPrefab.GetComponent<RectTransform>().sizeDelta.y;
            List<GameObject> list = new List<GameObject>()
            {
                InstantiateNewButton("Start", new Vector3(0, y*1.25f, 0)),
                InstantiateNewButton("Continue", new Vector3(0, 0, 0)),
                InstantiateNewButton("Setting", new Vector3(0, -y*1.25f, 0)),
                InstantiateNewButton("Exit", new Vector3(0,-y*2.5f, 0))
            };

        }

        public GameObject InstantiateNewButton(string displayName, Vector3 position)
        {
            var buttonGameObject = Instantiate(menuButtonPrefab, canvas.GetComponent<Canvas>().transform);
            buttonGameObject.transform.localPosition = position;
            buttonGameObject.name = displayName + "Button";
            var button = buttonGameObject.GetComponent<Button>();
            button.GetComponentInChildren<Text>().text = displayName;
            button.onClick.AddListener(async () => { await button.RaiseEventAsync<OnButtonClicked>(EventArgs.Empty); });
            //Or
            //button.onClick.AddListener(() => { EventSystem.RaiseEvent<OnButtonClicked>(button, EventArgs.Empty); });
            return buttonGameObject;
        }


        // Update is called once per frame
        void Update()
        {

        }
    }
}
