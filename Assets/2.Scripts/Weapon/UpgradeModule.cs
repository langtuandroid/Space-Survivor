using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Plugin", menuName = "Scriptable Object/Plugin Data", order = int.MaxValue)]
public class UpgradeModule : ScriptableObject
{
    public upgradeModuleType upgradeModuleType;

    public float value1;
    public float value2;
    public float value3;
}

public enum upgradeModuleType { CoolTimeDecrease, DamageIntIncrease, IncreasedProjectiles, IncreaseSize, IncreaseFireForce, IncreseRotateSpeed, IncreseExplodeRadius, IncreaseResourcePullRadius,
        IncreaseMineralBonusPercent, IncreaseRepairValue, IncreaseRepairRate, IncreaseMoveSpeed, IncreaseRotationSpeed, IncreaseDamagePercent, DecreaseCoolTime,
        ShieldCountIncrease, IncreaseHitCount, IncreaseDroneNumber
}
