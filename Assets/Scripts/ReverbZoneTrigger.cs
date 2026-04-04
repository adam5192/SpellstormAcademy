using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class ReverbZoneTrigger : MonoBehaviour
{
    [Header("reverb")]
    public AudioReverbPreset enterPreset = AudioReverbPreset.Hallway;
    public AudioReverbPreset exitPreset = AudioReverbPreset.Off;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        SetReverb(enterPreset);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        SetReverb(exitPreset);
    }

    void SetReverb(AudioReverbPreset preset)
    {
        AudioListener listener = FindObjectOfType<AudioListener>();
        if (listener == null)
            return;

        AudioReverbFilter filter = listener.GetComponent<AudioReverbFilter>();
        if (filter == null)
            filter = listener.gameObject.AddComponent<AudioReverbFilter>();

        filter.reverbPreset = preset;
    }
}
