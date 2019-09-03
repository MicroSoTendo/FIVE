using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    class CameraController : MonoBehaviour
    {

        void Update()
        {
            float xAxisValue = Input.GetAxis("Horizontal");
            float zAxisValue = Input.GetAxis("Vertical");
            if (Camera.current != null)
            {
                Camera.current.transform.Translate(new Vector3(xAxisValue, 0.0f, zAxisValue));
            }
        }
    }
}
