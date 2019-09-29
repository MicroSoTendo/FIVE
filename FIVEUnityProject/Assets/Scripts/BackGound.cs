using UnityEngine;

public class Backgound : MonoBehaviour
{
    public AudioClip audios;
    private AudioSource auz;

    private void Start()
    {
        auz = GetComponent<AudioSource>();
        gameObject.GetComponent<AudioSource>().clip = audios;
        gameObject.GetComponent<AudioSource>().Play();
    }

    private void Update()
    {
    }

    private void ExecuteScript()
    {
    }
}