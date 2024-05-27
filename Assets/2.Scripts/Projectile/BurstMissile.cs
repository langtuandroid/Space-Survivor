using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BurstMissile : MonoBehaviour
{

    [SerializeField] private ProjectileLogic projectileLogic;
    [SerializeField] private PlayerStat playerStat;


    public void ClearMissile()
    {
        projectileLogic.ClearDamageModifire();
    }

    public void InitMissile(PlayerStat stat)
    {
        playerStat = stat;
        projectileLogic.AddDamage(playerStat.GetCurrentPlayerLevel() * 5);
        projectileLogic.playerWeapon = playerStat.GetComponent<PlayerWeapon>();
    }

}
