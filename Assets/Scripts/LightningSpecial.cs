using UnityEngine;

public class LightningSpecial : MonoBehaviour
{
    public int chainCount = 5;
    public int damage = 15;
    public float lifetime = 0.4f;

    void Start()
    {
        ChainZap();
        Destroy(gameObject, lifetime);
    }

    void ChainZap()
    {
        var enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return;

        for (int i = 0; i < Mathf.Min(chainCount, enemies.Length); i++)
        {
            var target = enemies[Random.Range(0, enemies.Length)];
            target.GetComponent<Enemy>()?.TakeDamage(damage);
        }
    }
}
