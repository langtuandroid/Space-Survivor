using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneBullet : Projectile
{
    [field: SerializeField] public DroneAi droneAi { get; set; }

    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {

            //적에게 들어가는 실제 데미지
            int currentDamage = Mathf.RoundToInt((droneAi.GetDamage().GetFinalStatValueAsInt() + (droneAi.playerWeapon.GetPlayerStatDamage() / 2)));

            collision.GetComponent<EnemyStat>().TakeDamage(currentDamage);
            collision.GetComponent<EnemyStat>().Knockback(collision.bounds.center, droneAi.GetKnockbackForce());

            EnQueueThisItem();
        }
    }
}
