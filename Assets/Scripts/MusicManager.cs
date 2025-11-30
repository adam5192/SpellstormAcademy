using UnityEngine;

// music slowly ramps up in intensity over time
public class MusicManager : MonoBehaviour
{
    [Header("refs")]
    public AudioSource musicSource;

    [Header("time settings")]
    public float timeToMax = 300f; // how long until max pitch (seconds)

    [Header("pitch range")]
    public float minPitch = 1f;     // start calm
    public float maxPitch = 1.25f;  // end chaotic
    public float smooth = 2f;       // how fast we lerp to new pitch

    private float timer = 0f;       // how long game has been running

    void Start()
    {
        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (musicSource == null)
            return;

        // count up (real time, even if framerate dips)
        timer += Time.deltaTime;

        // 0 → 1 as we go from start to "timeToMax"
        float t = Mathf.Clamp01(timer / timeToMax);

        // pick pitch between min and max
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, t);

        // smooth out sudden jumps
        musicSource.pitch = Mathf.Lerp(
            musicSource.pitch,
            targetPitch,
            Time.deltaTime * smooth
        );
    }
}
