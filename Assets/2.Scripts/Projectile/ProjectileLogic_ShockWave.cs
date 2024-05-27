using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileLogic_ShockWave : ProjectileLogic
{
    [Space]

    [SerializeField] private ShockWave shockWave;
    [SerializeField] private Transform particle;
    [SerializeField] private Vector3 originParticleSize = new Vector3(1,1,1);

    public override void ResetProjectileStat()
    {
        base.ResetProjectileStat();

        shockWave.ResetStat();
        particle.localScale = originParticleSize;
    }

    public override void IncreaseExplodeRadius(float value)
    {
        base.IncreaseExplodeRadius(value);

        shockWave.AddRadius(value);
        particle.localScale += new Vector3(0.3f, 0.3f, 0.3f);
    }
}
