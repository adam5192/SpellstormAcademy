using System.Collections;
using UnityEngine;

// music manager with simple fades, ducking, and calm/combat switching
public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    [Header("refs")]
    public AudioSource musicSource;
    public EnemySpawner enemySpawner;

    [Header("music clips")]
    public AudioClip calmMusic;
    public AudioClip combatMusic;

    [Header("volume")]
    public float baseVolume = 1f;
    public float fadeDuration = 1f;

    [Header("combat switching")]
    public int combatEnemyThreshold = 12;
    public int calmEnemyThreshold = 6;
    public float stateCheckInterval = 0.5f;
    public float minSwitchGap = 2f;

    [Header("time settings")]
    public float timeToMax = 300f; // how long until max pitch (seconds)

    [Header("pitch range")]
    public float minPitch = 1f;     // start calm
    public float maxPitch = 1.1f;   // end slightly more intense
    public float smooth = 2f;       // how fast to new pitch

    private float timer = 0f;       // how long game has been running
    private float stateTimer = 0f;
    private float switchTimer = 0f;

    private bool inCombatMusic = false;
    private bool isSwitching = false;
    private float duckMultiplier = 1f;
    private Coroutine duckRoutine;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (musicSource == null)
            musicSource = GetComponent<AudioSource>();

        if (enemySpawner == null)
            enemySpawner = FindObjectOfType<EnemySpawner>();

        if (musicSource != null)
        {
            musicSource.playOnAwake = false;
            musicSource.loop = true;
            musicSource.spatialBlend = 0f;
            musicSource.volume = baseVolume;
        }

        if (musicSource != null && calmMusic != null)
        {
            musicSource.clip = calmMusic;
            musicSource.Play();
        }
    }

    void Update()
    {
        if (musicSource == null)
            return;

        timer += Time.deltaTime;
        switchTimer += Time.deltaTime;
        stateTimer += Time.deltaTime;

        float t = Mathf.Clamp01(timer / timeToMax);
        float targetPitch = Mathf.Lerp(minPitch, maxPitch, t);
        musicSource.pitch = Mathf.Lerp(
            musicSource.pitch,
            targetPitch,
            Time.deltaTime * smooth
        );

        musicSource.volume = Mathf.Lerp(
            musicSource.volume,
            baseVolume * duckMultiplier,
            Time.deltaTime * 4f
        );

        if (stateTimer >= stateCheckInterval)
        {
            stateTimer = 0f;
            UpdateMusicState();
        }
    }

    void UpdateMusicState()
    {
        if (enemySpawner == null || isSwitching || switchTimer < minSwitchGap)
            return;

        int activeEnemies = enemySpawner.GetActiveEnemies();

        if (!inCombatMusic && activeEnemies >= combatEnemyThreshold && combatMusic != null)
        {
            StartCoroutine(SwitchMusic(combatMusic, true));
        }
        else if (inCombatMusic && activeEnemies <= calmEnemyThreshold && calmMusic != null)
        {
            StartCoroutine(SwitchMusic(calmMusic, false));
        }
    }

    IEnumerator SwitchMusic(AudioClip newClip, bool combatState)
    {
        if (musicSource == null || newClip == null)
            yield break;

        isSwitching = true;
        switchTimer = 0f;

        float startVolume = musicSource.volume;
        float targetVolume = baseVolume * duckMultiplier;

        float t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / fadeDuration);
            yield return null;
        }

        musicSource.clip = newClip;
        musicSource.Play();
        inCombatMusic = combatState;

        t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            musicSource.volume = Mathf.Lerp(0f, targetVolume, t / fadeDuration);
            yield return null;
        }

        isSwitching = false;
    }

    public void PlayTrack(AudioClip clip)
    {
        if (clip == null || clip == musicSource.clip || isSwitching)
            return;

        bool combatState = clip == combatMusic;
        StartCoroutine(SwitchMusic(clip, combatState));
    }

    public void DuckForSeconds(float duckAmount, float duration)
    {
        if (duckRoutine != null)
            StopCoroutine(duckRoutine);

        duckRoutine = StartCoroutine(DuckRoutine(duckAmount, duration));
    }

    IEnumerator DuckRoutine(float duckAmount, float duration)
    {
        duckMultiplier = Mathf.Clamp01(duckAmount);
        yield return new WaitForSecondsRealtime(duration);
        duckMultiplier = 1f;
        duckRoutine = null;
    }
}
