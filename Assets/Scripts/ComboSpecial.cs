using UnityEngine;

public class ComboSpecial : MonoBehaviour
{
    public float radius = 5f;
    public int damage = 25;
    public float lifetime = 0.5f;

    void Start()
    {
        ComboBlast();
        Destroy(gameObject, lifetime);
    }

    void ComboBlast()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
            {
                hit.GetComponent<Enemy>()?.TakeDamage(damage);
                hit.GetComponent<Enemy>()?.Freeze(1f); // lil freeze to show element mix
            }
        }
    }
}
