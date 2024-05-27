using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_MeteoriteFlak : ProjectileLogic
{
    [Header("=============MeteoriteFlak===============")]

    public int spreadValue = 15;

    public override void Fire(Transform target, int fireForce)
    {
        base.Fire(target, fireForce);

        Vector2 fireDir = Vector2.zero;

        fireDir = Utility.GetDirection(transform.position, transform.position + weaponObject.GetRandomDir());

        var angle = Mathf.Atan2((fireDir.y + transform.position.y) - transform.position.y,
        (fireDir.x + transform.position.x) - transform.position.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle - 90 + Random.Range(-spreadValue, spreadValue), Vector3.forward);

        rigid.AddForce(transform.up * fireForce);
    }
}
