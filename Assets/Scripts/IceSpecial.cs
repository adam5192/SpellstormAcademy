using UnityEngine;

public class IceSpecial : MonoBehaviour
{
    [Header("ice settings")]
    public float speed = 12f;
    public float freezeTime = 3f; // how long enemies stay frozen
    public float lifetime = 3f;
    public GameObject hitEffect;

    private Vector2 moveDir;
    private float hitRadius;

    void Start()
    {
        hitRadius = transform.localScale.x * 0.5f;
        Destroy(gameObject, lifetime);
    }

    public void SetDirection(Vector2 dir)
    {
        moveDir = dir.normalized;

        // rotate the projectile so it points the correct way
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
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
                if (e != null)
                {
                    e.Freeze(freezeTime); // freeze instead of damage

                    if (hitEffect != null)
                        Instantiate(hitEffect, hit.transform.position, Quaternion.identity);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 0.7f, 1f, 0.4f);
        Gizmos.DrawWireSphere(transform.position, transform.localScale.x * 0.5f);
    }
}
