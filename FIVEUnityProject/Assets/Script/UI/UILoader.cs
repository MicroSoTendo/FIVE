using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Script.EventSystem;

public class UILoader : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        EventSystem.RunOnce();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
