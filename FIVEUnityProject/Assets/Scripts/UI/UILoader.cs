using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Scripts.EventSystem;
using UnityEngine.UI;

public class UILoader : MonoBehaviour
{
    private Canvas canvas;
    public GameObject menuButtonPrefab;
    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        InstantiateNewButton("Start", new Vector3(0,40,0));
        InstantiateNewButton("Continue", new Vector3(0,0,0));
        InstantiateNewButton("Setting", new Vector3(0,-40,0));
        InstantiateNewButton("Exit", new Vector3(0,-80,0));
    }

    private void InstantiateNewButton(string displayName, Vector3 position)
    {
        var buttonGameObject = Instantiate(menuButtonPrefab, canvas.transform);
        buttonGameObject.transform.localPosition = position;
        buttonGameObject.name = displayName + "Button";
        var button = buttonGameObject.GetComponent<Button>();
        button.GetComponentInChildren<Text>().text = displayName;
        button.onClick.AddListener(delegate { EventSystem.RaiseEvent(EventTypes.OnButtonClicked, button, EventArgs.Empty); });
    }


    // Update is called once per frame
    void Update()
    {

    }
}
