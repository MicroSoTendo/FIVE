using FIVE.Robot;
using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    private void Start()
    {
        GetComponent<TextMeshPro>().text = gameObject.GetComponentInParent<RobotSphere>().ID.ToString();
    }
}