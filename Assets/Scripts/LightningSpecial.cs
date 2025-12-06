using UnityEngine;
using System.Collections;

public class LightningSpecial : MonoBehaviour
{
    [Header("lightning settings")]
    public float radius = 5f;          // damage radius
    public int damage = 25;            // damage dealt to each enemy

    [Header("visuals")]
    public float animDuration = 0.4f;  // how long the lightning anim lasts

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

        // medium shake
        if (CameraShake.instance != null)
            CameraShake.instance.Shake(0.12f, 0.3f);

        // destroy after the animation is done
        StartCoroutine(DestroyAfterAnim());
    }

    IEnumerator DestroyAfterAnim()
    {
        // wait for the animation duration
        yield return new WaitForSecondsRealtime(animDuration);

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 1f, 0f, 0.3f);
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
