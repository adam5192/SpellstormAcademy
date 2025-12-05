using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class UIButtonSfx : MonoBehaviour
{
    public AudioClip clickClip;
    public float volume = 1f;

    private AudioSource src;

    void Awake()
    {
        src = GetComponent<AudioSource>();
        src.playOnAwake = false;
        src.loop = false;
    }

    public void PlayClick()
    {
        if (clickClip != null)
            src.PlayOneShot(clickClip, volume);
    }
}
