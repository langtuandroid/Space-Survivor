using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Firecracker : MonoBehaviour
{
    //[SerializeField] private Stat explodeDamage = new Stat();
    //[SerializeField] private float explodeTime;
    //[SerializeField] private float explodeRadius;
    //[SerializeField] private int knockbackForce;
    [SerializeField] private Stat explodeTime = new Stat();
    [SerializeField] private Stat explodeRadius = new Stat();
    [SerializeField] private Stat knockbackForce = new Stat();
    [SerializeField] private VFXType vfxType;

    [Space]

    [SerializeField] private ProjectileLogic projectileLogic;

    private void OnEnable()
    {
        StartCoroutine(Explode());
    }

    public void AddExplodeRadius(float radius)
    {
        explodeRadius.AddFloatModifier(radius);
    }

    public void ResetStat()
    {
        explodeTime.ClearFloatModifier();
        explodeTime.ClearPercentModifier();

        explodeRadius.ClearFloatModifier();
        explodeRadius.ClearPercentModifier();

        knockbackForce.ClearFloatModifier();
        knockbackForce.ClearPercentModifier();
    }


    IEnumerator Explode()
    {
        yield return new WaitForSeconds(explodeTime.GetFinalStatValue());

        EZCameraShake.CameraShakeInstance cameraShakeInstance = new EZCameraShake.CameraShakeInstance(4f, 4f, .2f, 1f);

        float currentDamage = (projectileLogic.GetDamage().GetFinalStatValue()
         + projectileLogic.playerWeapon.GetPlayerStatDamage());

        Utility.Explode(transform.position, currentDamage, explodeRadius.GetFinalStatValue(), knockbackForce.GetFinalStatValueAsInt(), vfxType, cameraShakeInstance);

        projectileLogic.OffProjectile();

        AudioManager.instance.GenerateAudioAndPlaySFX("explosion2",transform.position);
    }

    public VFXType GetVFXType()
    {
        return vfxType;
    }
}
