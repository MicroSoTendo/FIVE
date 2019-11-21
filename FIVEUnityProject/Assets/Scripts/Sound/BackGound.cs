using UnityEngine;

namespace FIVE.Sound
{
    public class Backgound : MonoBehaviour
    {
        public AudioClip audios;

        private void Start()
        {
            gameObject.GetComponent<AudioSource>().clip = audios;
            gameObject.GetComponent<AudioSource>().Play();
        }
    }
}