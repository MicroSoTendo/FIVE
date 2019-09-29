using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ClickSound : MonoBehaviour
{
    public AudioClip sound;

    private Button button => GetComponent<Button>();

    private AudioSource source => GetComponent<AudioSource>();

    private void Start()
    {
        gameObject.AddComponent<AudioSource>();

        source.clip = sound;

        source.playOnAwake = false;

        button.onClick.AddListener(() => PlaySoud());
    }

    private void PlaySoud()
    {
        source.PlayOneShot(sound);
    }
}