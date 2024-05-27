using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public ProjectileType type;

    public float deleteTime = 3f;
    public VFXType deleteParticleType;

    [SerializeField] protected TrailRenderer trail;

    [SerializeField] private Rigidbody2D rigid;

    private Coroutine deleteCoroutine;


    private void startFind()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        if (trail != null)
        {
            trail.Clear();
            trail.enabled = true;
            trail.emitting = true;

            StartCoroutine(delay());

            IEnumerator delay()
            {
                yield return null;

                trail.enabled = true;
                trail.emitting = true;
            }
        }
    }

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
            //ProjectileGenerator.instance.EnQueueProjectile(type, gameObject);
            EnQueueThisItem();
            VFXGenerator.instance.GenerateVFX(deleteParticleType, transform.position);

            if (trail != null)
            {
                trail.Clear();
                trail.enabled = false;
                trail.emitting = false;
            }
        }
    }

    public void EnQueueThisItem()
    {
        ProjectileGenerator.instance.EnQueueProjectile(type, gameObject);

        if (deleteCoroutine != null)
            StopCoroutine(deleteCoroutine);
        gameObject.SetActive(false);

        if (trail != null)
        {
            trail.Clear();
            trail.enabled = false;
            trail.emitting = false;
        }
    }
}
