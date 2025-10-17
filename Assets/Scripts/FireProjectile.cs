using UnityEngine;

public class FireProjectile : ProjectileBase
{
    protected override void OnHitEnemy(Enemy enemy)
    {
        if (enemy == null) return;

        // tell enemy it was fire damage (for color flash, etc.)
        enemy.TakeDamage(damage, "Fire");
    }
}
