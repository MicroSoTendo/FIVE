using UnityEngine;

namespace FIVE.GameStates
{
    public class GameMode : MonoBehaviour
    {
        public void SwitchTo<T>() where T : GameMode
        {
            gameObject.AddComponent<T>().transform.SetParent(null);
            Destroy(this);
        }
    }
}
