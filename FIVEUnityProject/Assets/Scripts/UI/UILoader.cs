using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.EventSystem;
using UnityEngine.UI;
using Assets.Scripts.EventSystem.EventTypes;

public class UILoader : MonoBehaviour
{
    private Canvas canvas;
    public GameObject menuButtonPrefab;
    IEnumerator Start()
    {
        yield return new WaitForSeconds(2f);
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        var image = GameObject.Find("Canvas").GetComponent<Image>();
        image.sprite = Resources.Load<Sprite>("UI/background");
        if (menuButtonPrefab == null)
        {
            menuButtonPrefab = Resources.Load<GameObject>("UI/Prefab/MenuButton");
        }

        var y = menuButtonPrefab.GetComponent<RectTransform>().sizeDelta.y;
        List<GameObject> list = new List<GameObject>()
        {
            InstantiateNewButton("Start", new Vector3(0, y*1.25f, 0)),
            InstantiateNewButton("Continue", new Vector3(0, 0, 0)),
            InstantiateNewButton("Setting", new Vector3(0, -y*1.25f, 0)),
            InstantiateNewButton("Exit", new Vector3(0,-y*2.5f, 0))
        };


        yield return null;
    }

    public GameObject InstantiateNewButton(string displayName, Vector3 position)
    {
        var buttonGameObject = Instantiate(menuButtonPrefab, canvas.transform);
        buttonGameObject.transform.localPosition = position;
        buttonGameObject.name = displayName + "Button";
        var button = buttonGameObject.GetComponent<Button>();
        button.GetComponentInChildren<Text>().text = displayName;
        button.onClick.AddListener(async () => { await EventSystem.RaiseEventAsync<OnButtonClicked>(button, EventArgs.Empty); });
        //Or
        //button.onClick.AddListener(() => { EventSystem.RaiseEvent<OnButtonClicked>(button, EventArgs.Empty); });
        return buttonGameObject;
    }


    // Update is called once per frame
    void Update()
    {

    }
}
