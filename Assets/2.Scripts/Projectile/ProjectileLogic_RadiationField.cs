using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLogic_RadiationField : ProjectileLogic
{
    [Space]

    [SerializeField] private RadiationField radiationField;

    public override void IncreaseProjectileSize(float value)
    {
        base.IncreaseProjectileSize(value);

        radiationField.AddRadius(value);
    }

    public override void ResetProjectileStat()
    {
        base.ResetProjectileStat();

        radiationField.ResetStat();
    }
}
