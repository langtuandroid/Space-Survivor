using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[CreateAssetMenu(fileName = "UpgradeModule", menuName = "Scriptable Object/UpgradeModule", order = int.MaxValue)]
public abstract class UpgradeModuleScripableObject : ScriptableObject
{
    [field: SerializeField] public int ID { get; private set; }

    [field: SerializeField] public UpgradeModuleType type { get; private set; }
    [field: SerializeField] public UpgradeModules module { get; private set; }

    [field: SerializeField] public Sprite moduleIcon { get; private set; }

    ///<summary>
    /// 스테이지 시작시 적용되는 모듈의 스탯
    ///</summary>
    public abstract void ApplyUpgradeModule(PlayerUpgradeModule playerWeapon, int tier);
    ///<summary>
    /// 스테이지 종료시 모듈에 적용된 스탯이 지워지도록
    ///</summary>
    public abstract void UnapplyUpgradeModule(PlayerUpgradeModule playerWeapon, int tier);

    public abstract void GetLocalizedModuleStatText(TextMeshProUGUI text, int tier);

    public abstract string[] GetModuleStats(int tier);
}
