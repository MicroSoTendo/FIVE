using UnityEngine;

namespace FIVE
{
    public class AvoidDestroy : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
