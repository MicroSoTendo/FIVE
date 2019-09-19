using UnityEngine;
using FIVE.EventSystem;

namespace FIVE
{
    public class GUICamera : MonoBehaviour
    {
        void Awake()
        {
            EventManager.Subscribe<OnLoadingGameMode>((o, e) => { gameObject.SetActive(false); });
        }
    }

}
