using UnityEngine;

public class LightningProjectile : ProjectileBase
{
    [Header("lightning boost")]
    public int bonusDamage = 10; // extra dmg for lightning

    protected override void OnHitEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        enemy.TakeDamage(bonusDamage, "Lightning");
    }
}
