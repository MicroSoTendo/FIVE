using System.Collections.Generic;
using UnityEngine;

namespace FIVE.ControllerSystem
{
    public class RtsContoller : MonoBehaviour
    {
        private List<GameObject> selectedTroops;

        private void Start() => selectedTroops = new List<GameObject>();

        private void Update()
        {
            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                Vector3 mousePosition = Input.mousePosition;
                mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);

                Ray mouseToWorld = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(mouseToWorld, out RaycastHit hitInfo))
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
                                selectedTroop.GetComponent<RtsControlUnit>().DeSelect();
                            }
                            selectedTroops.Clear();
                        }

                        if (objectHit.gameObject.name.StartsWith("robotSphere"))
                        {
                            selectedTroops.Add(objectHit.gameObject);
                            RtsControlUnit inputControlTest = objectHit.gameObject.GetComponent<RtsControlUnit>();
                            inputControlTest.OnSelect();
                        }
                    }
                    else if (Input.GetMouseButton(1))
                    {
                        foreach (GameObject selectedTroop in selectedTroops)
                        {
                            RtsControlUnit inputControlTest = selectedTroop.gameObject.GetComponent<RtsControlUnit>();

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
}