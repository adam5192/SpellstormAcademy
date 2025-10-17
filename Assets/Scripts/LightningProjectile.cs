using UnityEngine;

public class LightningProjectile : ProjectileBase
{
    [Header("lightning boost")]
    public int bonusDamage = 5; // extra dmg for lightning

    protected override void OnHitEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        // add bonus damage on top of base damage
        enemy.TakeDamage(bonusDamage);
    }
}
