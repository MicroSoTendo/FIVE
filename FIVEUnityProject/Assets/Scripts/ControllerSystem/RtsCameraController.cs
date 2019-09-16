using UnityEngine;

namespace FIVE.ControllerSystem
{
    internal class RtsCameraController : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                Vector3 d = transform.forward.normalized;
                d.y = 0f;
                d *= 2f;
                d = transform.InverseTransformDirection(d);
                transform.Translate(d);
            }
            if (Input.GetKey(KeyCode.S))
            {
                Vector3 d = transform.forward.normalized;
                d.y = 0f;
                d *= -2f;
                d = transform.InverseTransformDirection(d);
                transform.Translate(d);
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0f, -2f, 0f, Space.World);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0f, 2f, 0f, Space.World);
            }
        }
    }
}