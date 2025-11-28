using UnityEngine;

public class ProjectileBase : MonoBehaviour
{
    [Header("base stats")]
    public float speed = 10f;      // movement speed
    public int damage = 3;         // base damage
    public float lifetime = 2f;    // destroy after x seconds

    protected Vector2 moveDir = Vector2.up; // direction to travel in

    void Start()
    {
        // cleanup timer
        Destroy(gameObject, lifetime);
    }

    // set direction from outside (player)
    public void SetDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.001f)
            moveDir = dir.normalized;
        else
            moveDir = Vector2.up;

        // rotate sprite to face direction
        float angle = Mathf.Atan2(moveDir.y, moveDir.x) * Mathf.Rad2Deg - 90f;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Update()
    {
        // move in assigned direction
        transform.Translate(moveDir * speed * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // only hit enemies
        if (!other.CompareTag("Enemy")) return;

        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy == null) return;

        // call element-specific effect
        OnHitEnemy(enemy);

        // remove self
        Destroy(gameObject);
    }

    // will be overridden by subclasses for extra effects
    protected virtual void OnHitEnemy(Enemy enemy) { }
}
