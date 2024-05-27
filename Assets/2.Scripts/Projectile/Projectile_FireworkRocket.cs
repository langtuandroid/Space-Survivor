using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_FireworkRocket : ProjectileLogic
{
    [Space]

    [SerializeField] private Firecracker firecracker;

    public override void Fire(Transform target, int fireForce)
    {
        /*
        base.Fire(target, fireForce);

        Vector2 fireDir;

        var randomNearEnemy = Utility.GetRandomTargetInRadius(transform, fireToNearEnemyRadius);

        fireDir = Utility.GetDirection(transform.position, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));

        rigid.AddForce(fireDir * fireForce);

        var angle = Mathf.Atan2((fireDir.y + transform.position.y) - transform.position.y,
    (fireDir.x + transform.position.x) - transform.position.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        */

        base.Fire(target, fireForce);

        Vector2 fireDir;

        var randomNearEnemy = Utility.GetRandomTargetInRadius(transform, fireToNearEnemyRadius);

        if (randomNearEnemy == null)
        {
            fireDir = Utility.GetDirection(transform.position, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));

            rigid.AddForce(fireDir * fireForce);

            var angle = Mathf.Atan2((fireDir.y + transform.position.y) - transform.position.y,
        (fireDir.x + transform.position.x) - transform.position.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        }
        else
        {
            AddForce(randomNearEnemy, fireForce);
        }
    }

    public override void ResetProjectileStat()
    {
        base.ResetProjectileStat();

        firecracker.ResetStat();
    }

    public override void IncreaseExplodeRadius(float value)
    {
        base.IncreaseExplodeRadius(value);

        firecracker.AddExplodeRadius(value);
    }
}
