using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FIVE
{
    public class AvoidDestroy : MonoBehaviour
    {
        void Awake() {
            DontDestroyOnLoad(gameObject);
        }
    }
}
