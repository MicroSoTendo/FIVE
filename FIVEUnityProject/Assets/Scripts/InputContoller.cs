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
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = Input.mousePosition;
            mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

            RaycastHit hitInfo;
            Ray mouseToWorld = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(mouseToWorld, out hitInfo))
            {
                Transform objectHit = hitInfo.transform;

                Debug.Log(objectHit.gameObject.name);

                if (objectHit.gameObject.name == "InputControllerTester")
                {
                    selectedTroops.Add(objectHit.gameObject);
                    InputControlTest inputControlTest = objectHit.gameObject.GetComponent<InputControlTest>();
                    inputControlTest.OnSelect();
                }
                else if (objectHit.gameObject.name == "Plane")
                {
                    foreach (GameObject selectedTroop in selectedTroops)
                    {
                        InputControlTest inputControlTest = selectedTroop.gameObject.GetComponent<InputControlTest>();

                        Vector3 objective = new Vector3(hitInfo.point.x, hitInfo.point.y + 0.5f, hitInfo.point.z);
                        inputControlTest.OnMove(objective);
                    }
                }
            }
        }
    }
}
