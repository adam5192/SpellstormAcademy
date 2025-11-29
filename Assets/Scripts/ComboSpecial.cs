using System.Collections.Generic;
using UnityEngine;

public class ComboSpecial : MonoBehaviour
{
    [Header("combo settings")]
    public float speed = 12f;
    public int damage = 30;
    public float lifetime = 3f;
    public GameObject hitEffect;

    private Vector2 moveDir;
    private float hitRadius;
    private HashSet<Enemy> hitEnemies = new HashSet<Enemy>();
    private bool triggeredBigFeedback = false;

    void Start()
    {
        hitRadius = transform.localScale.x * 0.5f;
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 dir)
    {
        moveDir = dir.normalized;

        // sprite facing right
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Update()
    {
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
        CheckHits();
    }

    void CheckHits()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                Enemy e = hit.GetComponent<Enemy>();
                if (e != null && !hitEnemies.Contains(e))
                {
                    hitEnemies.Add(e);

                    e.TakeDamage(damage, "Fire");

                    if (hitEffect != null)
                        Instantiate(hitEffect, e.transform.position, Quaternion.identity);

                    if (!triggeredBigFeedback)
                    {
                        triggeredBigFeedback = true;

                        if (CameraShake.instance != null)
                            CameraShake.instance.Shake(0.18f, 0.4f);
                    }
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x * 0.5f);
    }
}
