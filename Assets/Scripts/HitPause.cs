using System.Collections;
using UnityEngine;

public class HitPause : MonoBehaviour
{
    public static HitPause instance;

    private bool isPausing = false;

    void Awake()
    {
        instance = this;
    }

    public void DoHitPause(float duration)
    {
        if (isPausing) return;
        if (duration <= 0f) return;

        StartCoroutine(HitPauseRoutine(duration));
    }

    IEnumerator HitPauseRoutine(float duration)
    {
        isPausing = true;

        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
        isPausing = false;
    }
}
