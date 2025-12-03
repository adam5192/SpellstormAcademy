using System.Collections;
using UnityEngine;

public class HitPause : MonoBehaviour
{
    public static HitPause instance;

    private bool isPausing = false;
    private Coroutine pauseRoutine;

    void Awake()
    {
        instance = this;
    }

    public void DoHitPause(float duration)
    {
        if (isPausing) return;
        if (duration <= 0f) return;

        // dont start new hit pauses after game over
        if (GameManager.instance != null && GameManager.instance.IsGameOver)
            return;

        pauseRoutine = StartCoroutine(HitPauseRoutine(duration));
    }

    IEnumerator HitPauseRoutine(float duration)
    {
        isPausing = true;

        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        // if the game ended while paused, DONT restore timeScale
        if (GameManager.instance == null || !GameManager.instance.IsGameOver)
        {
            Time.timeScale = originalTimeScale;
        }

        isPausing = false;
        pauseRoutine = null;
    }

    // kill any active pause
    public void CancelOnGameOver()
    {
        if (pauseRoutine != null)
            StopCoroutine(pauseRoutine);

        pauseRoutine = null;
        isPausing = false;
    }
}
