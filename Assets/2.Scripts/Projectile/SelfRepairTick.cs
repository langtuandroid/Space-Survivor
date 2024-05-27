using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfRepairTick : MonoBehaviour
{
    [SerializeField] private ProjectileLogic projectileLogic;
    
    void OnEnable()
    {
        if(projectileLogic.GetPlayerWeapon() != null)
        {
            projectileLogic.GetPlayerWeapon().GetComponent<PlayerStat>().Heal((int)projectileLogic.GetDamage().GetFinalStatValue());
            VFXGenerator.instance.GenerateVFX(VFXType.Heal1, projectileLogic.GetPlayerWeapon().transform.position);
        }
        
    }
}
