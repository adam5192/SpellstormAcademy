using UnityEngine;

public class IceProjectile : ProjectileBase
{
    [Header("ice effect")]
    public float freezeTime = 2f; // how long enemy stays frozen

    protected override void OnHitEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        enemy.Freeze(freezeTime);
    }
}
