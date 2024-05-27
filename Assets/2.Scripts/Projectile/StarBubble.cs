using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarBubble : MonoBehaviour
{
    public ProjectileType type;
    public int damage = 50;

    public float deleteTime = 3f;
    public VFXType deleteParticleType;

    [SerializeField] private Rigidbody2D rigid;

    private Coroutine deleteCoroutine;

    // private void OnDisable()
    // {
    //     if (deleteCoroutine != null)
    //         StopCoroutine(deleteCoroutine);
    // }

    public void Fire(Transform target, float fireForce)
    {
        rigid.velocity = Vector3.zero;
        Vector2 fireDir;

        fireDir = Utility.GetDirection(transform.position, target.position);

        var angle = Mathf.Atan2((fireDir.y + transform.position.y) - transform.position.y,
        (fireDir.x + transform.position.x) - transform.position.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        rigid.AddForce(transform.up * fireForce);

        deleteCoroutine = StartCoroutine(delete());
    }

    public void FireToHead(Transform target, float fireForce, Vector3 extraAngle)
    {
        rigid.velocity = Vector3.zero;
        Vector2 fireDir;

        fireDir = Utility.GetDirection(transform.position, target.position);

        var angle = Mathf.Atan2((fireDir.y + transform.position.y) - transform.position.y,
        (fireDir.x + transform.position.x) - transform.position.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        transform.Rotate(extraAngle);

        rigid.AddForce(transform.up * fireForce);

        deleteCoroutine = StartCoroutine(delete());
    }

    IEnumerator delete()
    {
        yield return new WaitForSeconds(deleteTime);

        if (gameObject.activeSelf)
        {
            ProjectileGenerator.instance.EnQueueProjectile(type, gameObject);
            VFXGenerator.instance.GenerateVFX(deleteParticleType, transform.position);
            if(GetComponentInChildren<TrailRenderer>() != null)
                GetComponentInChildren<TrailRenderer>().enabled = false;
        }
    }

    // public void OnCollisionEnter2D(Collision2D collision)
    // {
    //     if (collision.gameObject.CompareTag("Player"))
    //     {
    //         ProjectileGenerator.instance.EnQueueProjectile(type, gameObject);

    //         collision.gameObject.GetComponentInParent<PlayerStat>().TakeDamage(damage);
    //     }
    // }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player") && other.TryGetComponent<PlayerBodyBeacon>(out PlayerBodyBeacon beacon))
        {
            ProjectileGenerator.instance.EnQueueProjectile(type, gameObject);
            VFXGenerator.instance.GenerateVFX(deleteParticleType, transform.position);
            other.gameObject.GetComponentInParent<PlayerStat>().TakeDamage(damage, type.ToString());
        }
    }
}
