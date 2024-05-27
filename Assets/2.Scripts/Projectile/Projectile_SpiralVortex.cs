using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile_SpiralVortex : ProjectileLogic
{
    [SerializeField] private float spawnRadius = 2f;
    private List<Collider2D> enemyOnCollider = new List<Collider2D>();

    private Coroutine damageTick;

    private static int[] spiralVortexSpawnPosition = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11 };

    //private static List<int> usingPosition = new List<int>();

    private int currentSpawnPosition = 0;

    public override void Fire(Transform target, int fireForce)
    {
        base.Fire(target, fireForce);

        StartCoroutine(GetPosition());

        IEnumerator GetPosition()
        {
            while (true)
            {
                currentSpawnPosition = Random.Range(0, spiralVortexSpawnPosition.Length);

                print(PlayerWeapon.activeSpiralVortexPositions.Count);

                if (!PlayerWeapon.activeSpiralVortexPositions.Contains(spiralVortexSpawnPosition[currentSpawnPosition]))
                {
                    foreach (int num in PlayerWeapon.activeSpiralVortexPositions)
                    {
                        print(num);
                    }

                    PlayerWeapon.activeSpiralVortexPositions.Add(spiralVortexSpawnPosition[currentSpawnPosition]);
                    //usingPosition.Add(currentSpawnPosition);
                    SetPosition(spiralVortexSpawnPosition[currentSpawnPosition]);
                    break;
                }

                yield return null;
            }
        }
    }

    private void OnEnable()
    {
        damageTick = StartCoroutine(DamageTick());

        IEnumerator DamageTick()
        {
            while (true)
            {
                for (int i = 0; i < enemyOnCollider.Count; i++)
                {
                    TakeDamage(enemyOnCollider[i]);
                }

                yield return new WaitForSeconds(.33f);
            }
        }
    }

    protected override void TakeDamage(Collider2D collision)
    {
        if (!collision.gameObject.activeSelf)
            return;

        //적에게 들어가는 실제 데미지
        int currentDamage = Mathf.RoundToInt(((damage.GetFinalStatValueAsInt() + playerWeapon.GetPlayerStatDamage()) / 2));

        collision.GetComponent<EnemyStat>().TakeDamage(currentDamage);
        collision.GetComponent<EnemyStat>().Knockback(collision.bounds.center, knockBackForce);
    }

    private void SetPosition(int positionNum)
    {
        transform.position = playerWeapon.transform.position + new Vector3(0, spawnRadius, 0);

        transform.RotateAround(playerWeapon.transform.position, new Vector3(0, 0, 1), positionNum * (360 / 12));
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy") && !enemyOnCollider.Contains(other))
        {
            enemyOnCollider.Add(other);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (enemyOnCollider.Contains(other))
        {
            enemyOnCollider.Remove(other);
        }
    }

    private void OnDisable()
    {
        enemyOnCollider.Clear();
        StopCoroutine(damageTick);
        PlayerWeapon.activeSpiralVortexPositions.Remove(currentSpawnPosition);
    }
}
