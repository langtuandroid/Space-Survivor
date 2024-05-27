using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShockWave : MonoBehaviour
{
    //[SerializeField] private Stat damage = new Stat();
    [SerializeField] private Stat damageRadius = new Stat();
    [SerializeField] private int knockbackForce;

    [Space]

    [SerializeField] private ProjectileLogic projectileLogic;

    public void AddRadius(float addRadius)
    {
        damageRadius.AddFloatModifier(addRadius);
    }

    private void OnEnable()
    {
        StartCoroutine(generatorShockWave());

        IEnumerator generatorShockWave()
        {
            yield return null;

            if (projectileLogic.playerWeapon == null)
                yield break;

            EZCameraShake.CameraShakeInstance cameraShakeInstance = new EZCameraShake.CameraShakeInstance(4f, 4f, .2f, 1f);

            float currentDamage = (projectileLogic.GetDamage().GetFinalStatValue()
             + projectileLogic.playerWeapon.GetPlayerStatDamage());

            Utility.Explode(transform.position, currentDamage, damageRadius.GetFinalStatValue(), knockbackForce, VFXType.none, cameraShakeInstance);
            AudioManager.instance.GenerateAudioAndPlaySFX("explosion2", transform.position);
        }


    }

    public void ResetStat()
    {
        damageRadius.ClearFloatModifier();
        damageRadius.ClearPercentModifier();
    }
}
