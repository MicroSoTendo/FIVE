using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputContoller : MonoBehaviour
{
    private List<GameObject> selectedTroops;

    void Start()
    {
        selectedTroops = new List<GameObject>();
    }

    void Update()
    {
        if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            RaycastHit hitInfo;
            Ray mouseToWorld = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(mouseToWorld, out hitInfo))
            {
                Transform objectHit = hitInfo.transform;
                if (Input.GetMouseButton(0))
                {
                    if (Input.GetKey("left ctrl"))
                    {
                        Debug.Log("Ctrl");
                    }
                    else
                    {
                        foreach (GameObject selectedTroop in selectedTroops)
                        {
                            selectedTroop.GetComponent<InputControlTest>().DeSelect();
                        }
                        selectedTroops.Clear();
                    }

                    if (objectHit.gameObject.name.StartsWith("robotSphere"))
                    {
                        selectedTroops.Add(objectHit.gameObject);
                        InputControlTest inputControlTest = objectHit.gameObject.GetComponent<InputControlTest>();
                        inputControlTest.OnSelect();
                    }
                }
                else if (Input.GetMouseButton(1))
                {
                    foreach (GameObject selectedTroop in selectedTroops)
                    {
                        InputControlTest inputControlTest = selectedTroop.gameObject.GetComponent<InputControlTest>();

                        Vector3 objective;
                        if (hitInfo.transform.gameObject.name.StartsWith("robotSphere"))
                        {
                            objective = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
                        }
                        else
                        {
                            objective = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
                        }
                        inputControlTest.Move(objective);
                    }
                }
            }
        }
    }
}
