using UnityEngine;

public class FireSpecial : MonoBehaviour
{
    public float speed = 10f;
    public float explosionRadius = 3f;
    public int damage = 20;
    public float lifetime = 2f;

    void Start() => Destroy(gameObject, lifetime);

    void Update() => transform.Translate(Vector2.up * speed * Time.deltaTime);

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Enemy"))
        {
            Explode();
        }
    }

    void Explode()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
                hit.GetComponent<Enemy>()?.TakeDamage(damage);
        }
        Destroy(gameObject);
    }
}
