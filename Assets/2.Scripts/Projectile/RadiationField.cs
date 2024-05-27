using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadiationField : MonoBehaviour
{
    //[SerializeField] private Stat damage = new Stat();
    [SerializeField] private Stat damageRadius = new Stat();
    [SerializeField] private int knockbackForce;

    [Space]

    [SerializeField] private float shipDamageFixedPercent = 0.25f;

    [Space]

    [SerializeField] private ProjectileLogic projectileLogic;

    public void AddRadius(float addRadius)
    {
        damageRadius.AddFloatModifier(addRadius);
    }

    public void ResetStat()
    {
        //damage.ClearFloatModifier();
        //damage.ClearPercentModifier();
        damageRadius.ClearFloatModifier();
        damageRadius.ClearPercentModifier();
    }

    private void OnEnable()
    {
        if (projectileLogic.playerWeapon == null)
            return;

        EZCameraShake.CameraShakeInstance cameraShakeInstance = new EZCameraShake.CameraShakeInstance(4f, 4f, .2f, 1f);

        float currentDamage = (projectileLogic.GetDamage().GetFinalStatValue()
         + (projectileLogic.playerWeapon.GetPlayerStatDamage()));

        Utility.Explode(transform.position, currentDamage, damageRadius.GetFinalStatValue() * projectileLogic.weaponObject.currentSize.GetFinalStatValue(), knockbackForce, VFXType.none, cameraShakeInstance);
    }
}
