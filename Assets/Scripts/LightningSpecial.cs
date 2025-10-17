using UnityEngine;
using System.Collections;

public class LightningSpecial : MonoBehaviour
{
    [Header("lightning settings")]
    public float radius = 5f;          // damage radius
    public int damage = 25;            // damage dealt to each enemy
    public float flashDuration = 0.25f;
    public Color flashColor = new Color(1f, 1f, 0f, 0.35f); // semi-transparent yellow

    [Header("visuals")]
    public Sprite circleSprite;    
    public float visualFadeSpeed = 4f; // how fast the circle fades away

    private SpriteRenderer sr;

    void Start()
    {
        // damage all enemies in radius
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy e = hit.GetComponent<Enemy>();
                if (e != null)
                    e.TakeDamage(damage, "Lightning");
            }
        }

        // create and animate the visual flash
        CreateFlashVisual();

        // fade out and destroy after the effect
        StartCoroutine(FadeAndDestroy());
    }

    void CreateFlashVisual()
    {
        GameObject circle = new GameObject("LightningFlash");
        circle.transform.SetParent(transform);
        circle.transform.localPosition = Vector3.zero;
        circle.transform.localScale = Vector3.one * radius * 2f; // diameter in world units

        sr = circle.AddComponent<SpriteRenderer>();
        sr.sprite = circleSprite;
        sr.color = flashColor;
        sr.sortingOrder = 10;
    }

    IEnumerator FadeAndDestroy()
    {
        float elapsed = 0f;
        Color startColor = sr.color;

        while (elapsed < flashDuration)
        {
            elapsed += Time.deltaTime * visualFadeSpeed;
            float alpha = Mathf.Lerp(startColor.a, 0, elapsed / flashDuration);
            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
