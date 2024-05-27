using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] private ItemType itemType;
    [SerializeField] private LayerMask resourceLayer;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player") && collision.GetComponent<PlayerBodyBeacon>() != null)
        {
            switch (itemType)
            {
                case ItemType.MissileBurst:
                    MissileBurst(collision);
                    break;

                case ItemType.Repair:
                    Repair(collision);
                    break;

                case ItemType.Magnetic:
                    Magnetic(collision);
                    break;

                case ItemType.AtomicExplosion:
                    AtomicExplosion(collision);
                    break;
            }

            EnQueueThisItem();
        }
    }


    private void MissileBurst(Collider2D player)
    {
        player.GetComponentInParent<PlayerWeapon>().MissileBurst(player);
    }


    private void Repair(Collider2D player)
    {
        player.GetComponentInParent<PlayerStat>().Heal(200);
        VFXGenerator.instance.GenerateVFX(VFXType.Heal1, player.transform.position);
    }

    private void Magnetic(Collider2D player)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 20f, resourceLayer);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<Resource>() != null)
                colliders[i].GetComponent<Resource>().StartPull(player.transform);
        }
    }

    public static void CrystalOnlyMagnetic(Transform player)
    {
        int layer = 1 << LayerMask.NameToLayer("Resource");

        Collider2D[] colliders = Physics2D.OverlapCircleAll(player.transform.position, 50f, layer);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<Resource>() != null)
            {
                if (colliders[i].GetComponent<Resource>().type == resourceType.Crystal)
                {
                    colliders[i].GetComponent<Resource>().StartPull(player.transform);
                }
            }
                
        }
    }

    private void AtomicExplosion(Collider2D player)
    {
        int layer = 1 << LayerMask.NameToLayer("Enemy");

        int damage = player.GetComponentInParent<PlayerStat>().GetCurrentPlayerLevel() * 10;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 25f, layer);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<EnemyStat>() != null)
            {
                colliders[i].GetComponent<EnemyStat>().TakeDamage(damage, false);
                colliders[i].GetComponent<EnemyStat>().Knockback(Utility.GetDirection(player.transform.position, colliders[i].transform.position), 5);
            }
        }

        VFXGenerator.instance.GenerateVFX(VFXType.AtomicExplosion, player.transform.position);
        //EZCameraShake.CameraShaker.Instance.ShakeOnce(6f, 6f, .2f, 1.5f);
        CinemachinShake.instance.ShakeCamera(5, 1f);

        AudioManager.instance.GenerateAudioAndPlaySFX("explosion1", transform.position);
    }

    public static void HyperAtomicExplosion(Vector3 position)
    {
        int layer = 1 << LayerMask.NameToLayer("Enemy");

        int damage = 9999;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 25f, layer);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<EnemyStat>() != null)
            {
                colliders[i].GetComponent<EnemyStat>().TakeDamage(damage);
            }
        }

        VFXGenerator.instance.GenerateVFX(VFXType.AtomicExplosion, position);
        //EZCameraShake.CameraShaker.Instance.ShakeOnce(6f, 6f, .2f, 1.5f);
        CinemachinShake.instance.ShakeCamera(5, 1f);
    }

    public void EnQueueThisItem()
    {
        ItemGenerator.instance.EnQueueItem(itemType, gameObject);
        gameObject.SetActive(false);
    }
}

public enum ItemType{none, MissileBurst, Repair, Magnetic, AtomicExplosion }
