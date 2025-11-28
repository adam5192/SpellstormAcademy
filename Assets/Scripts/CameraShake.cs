using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    [Header("defaults")]
    public float defaultDuration = 0.1f;
    public float defaultMagnitude = 0.2f;

    private Transform camTransform;
    private Vector3 originalPos;
    private Coroutine shakeRoutine;

    void Awake()
    {
        instance = this;
        camTransform = transform;
        originalPos = camTransform.localPosition;
    }

    public void Shake(float duration, float magnitude)
    {
        if (shakeRoutine != null)
            StopCoroutine(shakeRoutine);

        shakeRoutine = StartCoroutine(DoShake(duration, magnitude));
    }

    IEnumerator DoShake(float duration, float magnitude)
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            Vector2 offset = Random.insideUnitCircle * magnitude;
            camTransform.localPosition = originalPos + new Vector3(offset.x, offset.y, 0f);

            yield return null;
        }

        camTransform.localPosition = originalPos;
        shakeRoutine = null;
    }
}
