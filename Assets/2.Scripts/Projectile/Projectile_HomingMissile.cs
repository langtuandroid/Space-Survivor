using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_HomingMissile : ProjectileLogic
{
    public override void Fire(Transform target, int fireForce)
    {
        base.Fire(target, fireForce);

        Vector2 fireDir = Vector2.zero;

        Vector2 randomPos = transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);

        fireDir = Utility.GetDirection(transform.position, randomPos);

        var angle = Mathf.Atan2((fireDir.y + transform.position.y) - transform.position.y,
        (fireDir.x + transform.position.x) - transform.position.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }
}
