using UnityEngine;
using UnityEngine.UI;

namespace FIVE.Sound
{
    [RequireComponent(typeof(Button))]
    public class ButtonSound : MonoBehaviour
    {
        public AudioClip sound;

        private Button Button => GetComponent<Button>();

        private AudioSource Source => GetComponent<AudioSource>();

        private void Start()
        {
            gameObject.AddComponent<AudioSource>();

            Source.clip = sound;

            Source.playOnAwake = false;

            Button.onClick.AddListener(PlaySoud);
        }

        private void PlaySoud()
        {
            Source.PlayOneShot(sound);
        }
    }
}