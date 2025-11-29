using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [Header("projectile settings")]
    public float speed = 10f;
    public int damage = 1;
    public float lifetime = 4f;

    protected Vector2 moveDir;

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // called by player when the projectile is created
    public void SetDirection(Vector2 dir)
    {
        moveDir = dir.normalized;

        // rotate so the sprite's right side points in move direction
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Update()
    {
        // move forward
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
    }

    protected virtual void OnHitEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        enemy.TakeDamage(damage, "Default");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Enemy e = other.GetComponent<Enemy>();
            OnHitEnemy(e);
            Destroy(gameObject);
        }
    }
}
