using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_MachineGun : ProjectileLogic
{
    [Header("============MachineGun===========")]

    public Vector3 projectileSpawnRandomOffset;
    public float spreadAngle = 10f;

    public override void Fire(Transform target, int fireForce)
    {
        base.Fire(target, fireForce);

        Vector2 fireDir = Vector2.zero;

        fireDir = Utility.GetDirection(transform.position, weaponObject.GetFireDir().position);

        var angle = Mathf.Atan2((fireDir.y + transform.position.y) - transform.position.y,
        (fireDir.x + transform.position.x) - transform.position.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle - 90 + Random.Range(-spreadAngle, spreadAngle), Vector3.forward);

        transform.position += new Vector3(Random.Range(-projectileSpawnRandomOffset.x, projectileSpawnRandomOffset.x),
        Random.Range(-projectileSpawnRandomOffset.y,projectileSpawnRandomOffset.y));

        rigid.AddForce(transform.up * fireForce);
    }
}
