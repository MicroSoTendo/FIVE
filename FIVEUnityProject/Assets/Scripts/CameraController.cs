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
                transform.Translate(new Vector3(0, 0, 2));
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(new Vector3(0, 0, -2));
            }
            if (Input.GetKey(KeyCode.A))
            {
                transform.Rotate(0f, 2f, 0f);
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Rotate(0f, -2f, 0f);
            }
        }
    }
}