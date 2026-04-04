using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class NarrationTrigger : MonoBehaviour
{
    [Header("narration")]
    public AudioClip narrationClip;
    public float narrationVolume = 1f;
    public bool playOnce = true;
    public bool duckMusic = true;
    public float duckAmount = 0.25f;

    private bool hasPlayed = false;
    private AudioSource narrationSource;

    void Awake()
    {
        narrationSource = GetComponent<AudioSource>();

        if (narrationSource == null)
            narrationSource = gameObject.AddComponent<AudioSource>();

        narrationSource.playOnAwake = false;
        narrationSource.loop = false;
        narrationSource.spatialBlend = 0f;
        narrationSource.ignoreListenerPause = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (playOnce && hasPlayed)
            return;

        if (narrationClip == null)
            return;

        narrationSource.clip = narrationClip;
        narrationSource.volume = narrationVolume;
        narrationSource.Play();

        if (duckMusic && MusicManager.instance != null)
            MusicManager.instance.DuckForSeconds(duckAmount, narrationClip.length);

        hasPlayed = true;
    }
}
