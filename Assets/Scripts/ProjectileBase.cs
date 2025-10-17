using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [Header("base stats")]
    public float speed = 2f;     // movement speed
    public int damage = 3;        // base damage
    public float lifetime = 2f;   // destroy after x seconds

    void Start()
    {
        // cleanup timer
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        // move straight up
        transform.Translate(Vector2.up * speed * Time.deltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // only hit enemies
        if (!other.CompareTag("Enemy")) return;

        var enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        // deal base damage
        enemy.TakeDamage(damage);

        // call element-specific effect
        OnHitEnemy(enemy);

        // remove self
        Destroy(gameObject);
    }

    // this will be overridden by subclasses for extra effects
    protected virtual void OnHitEnemy(Enemy enemy) { }
}
