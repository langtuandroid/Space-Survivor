using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAi : ProjectileLogic
{
    [Space]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float fireLate;
    [SerializeField] private float fireForce;

    [SerializeField] private float findTargetRadius;
    [SerializeField] private float minDistBetweenEnemy;


    [SerializeField] private ProjectileType projectileType;

    [SerializeField] fireInfo[] fireInfo;

    private float currentMoveSpeed;
    private float angle;
    private bool bulletReady = true;
    private bool flyStrate = true;
    [SerializeField] private Transform target = null;
    private Coroutine findEnemyCoriotine = null;

    private void OnEnable()
    {
        bulletReady = true;
        flyStrate = true;

        StartCoroutine(FlyStrate());

        IEnumerator FlyStrate()
        {
            yield return new WaitForSeconds(0.6f);

            flyStrate = false;
        }
    }

    private void Update()
    {
        if (target != null)
        {
            if (!target.gameObject.activeSelf)
                target = null;

            if (bulletReady && !flyStrate)
                DroneAttack();
        }

        if (target == null)
            FindTarget();
    }

    private void FixedUpdate()
    {
        if (!flyStrate)
        {
            if (target != null)
            {
                if (Vector3.Distance(transform.position, target.position) > minDistBetweenEnemy)
                {
                    rigid.velocity += new Vector2(transform.up.x, transform.up.y) * Time.deltaTime * moveSpeed;
                }

                angle = Mathf.Atan2((target.position.y) - transform.position.y,
                    (target.position.x) - transform.position.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), rotationSpeed * Time.deltaTime);
            }
            else
            {
                if (Vector3.Distance(transform.position, GameManager.instance.playerTrans.position) > minDistBetweenEnemy)
                {
                    rigid.velocity += new Vector2(transform.up.x, transform.up.y) * Time.deltaTime * moveSpeed;
                }

                angle = Mathf.Atan2((GameManager.instance.playerTrans.position.y) - transform.position.y,
                    (GameManager.instance.playerTrans.position.x) - transform.position.x) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), rotationSpeed * Time.deltaTime);
            }
        }
        else
        {
            rigid.velocity += new Vector2(transform.up.x, transform.up.y) * Time.deltaTime * moveSpeed;
        }
    }

    public void InitDrone()
    {

    }

    public override void Fire(Transform target, int fireForce)
    {
        base.Fire(target, fireForce);

        Vector2 fireDir;

        fireDir = Utility.GetDirection(transform.position, transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f)));

        rigid.AddForce(fireDir * fireForce);

        var angle = Mathf.Atan2((fireDir.y + transform.position.y) - transform.position.y,
    (fireDir.x + transform.position.x) - transform.position.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
    }


    public void DroneAttack()
    {
        StartCoroutine(fire());

        IEnumerator fire()
        {
            bulletReady = false;

            for (int i = 0; i < fireInfo.Length; i++)
            {
                var bullet = ProjectileGenerator.instance.DeQueueProjectile(ProjectileType.droneShot, fireInfo[i].firepos.position);
                bullet.GetComponent<DroneBullet>().Fire(fireInfo[i].firedir, fireForce);
                bullet.GetComponent<DroneBullet>().droneAi = this;
            }

            yield return new WaitForSeconds(fireLate);
            bulletReady = true;

            AudioManager.instance.PlaySFX("laser1");
        }
    }

    private void FindTarget()
    {
        findEnemyCoriotine = StartCoroutine(startFind());

        IEnumerator startFind()
        {
            while (target == null)
            {
                target = Utility.GetClosestTargetInRadius(GameManager.instance.playerStat.transform, findTargetRadius);

                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private void OnDisable()
    {
        if (findEnemyCoriotine != null)
            StopCoroutine(findEnemyCoriotine);
    }

}
