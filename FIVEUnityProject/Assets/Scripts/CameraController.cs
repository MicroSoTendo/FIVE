using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    internal class CameraController : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                var d = transform.forward.normalized;
                d.y = 0f;
                d *= 2f;
                d = transform.InverseTransformDirection(d);
                transform.Translate(d);
            }
            if (Input.GetKey(KeyCode.S))
            {
                var d = transform.forward.normalized;
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
