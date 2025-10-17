using UnityEngine;

public class IceSpecial : MonoBehaviour
{
    public float radius = 4f;
    public float freezeTime = 3f;
    public float lifetime = 0.5f;

    void Start()
    {
        FreezeNearby();
        Destroy(gameObject, lifetime);
    }

    void FreezeNearby()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy"))
                hit.GetComponent<Enemy>()?.Freeze(freezeTime);
        }
    }
}
