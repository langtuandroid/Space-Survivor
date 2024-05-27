using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public interface IUpgradeModule
{
    [SerializeField] public UpgradeModuleType type { get; }
    [SerializeField] public UpgradeModules module { get; }
    [SerializeField] public UpgradeModuleTier tier { get; }
    [SerializeField] public string key { get; }

    public static int[] sellCost { get; private set; }

    /// <summary>
    /// 플레이어에 현재 장착중인 모듈 효과 적용
    /// </summary>
    public void ApplyUpgradeModule(PlayerWeapon playerWeapon);

    public void UnapplyUpgradeModule();

    /// <summary>
    /// 모듈 티어업
    /// </summary>
    public void TierUpThisModule();

    /// <summary>
    /// 모듈 스탯 가져오기
    /// </summary>
    public float GetModuleStat(int tier);

    public void SetModuleInfo(UpgradeModuleType type, UpgradeModules modules, UpgradeModuleTier tier, string key);

    public int GetSellCost();

    public UpgradeModuleObject GetUpgradeModuleObject();



}

[SerializeField] public enum UpgradeModuleType { None = 0, AttackType = 1, DefenceType = 2, MovementType = 3, SpecialType = 4 }

[SerializeField] public enum UpgradeModules { None = 0, DamageUp = 1, ArmorUp = 2, MovespeedUp = 3, FirerateUp = 4, MaxHpUp = 5, StartMineral, MagnetRangeUp = 7 }

[SerializeField] public enum UpgradeModuleTier { None = 0, Normal = 1, Magic = 2, Rare = 3, Unique = 4, Legendary = 5 }