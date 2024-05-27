using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerUpgradeModule : MonoBehaviour
{
    //[field: SerializeField] public List<UpgradeModuleObject> equipedModules = new List<UpgradeModuleObject>();

    //[field: SerializeField] public List<UpgradeModuleObject> moduleInventory = new List<UpgradeModuleObject>();

    [FoldoutGroup("참조")][SerializeField] private PlayerStat playerStat;
    [FoldoutGroup("참조")][SerializeField] private PlayerWeapon playerWeapon;

    [Space]

    [FoldoutGroup("참조")][SerializeField] private GameObject moduleItemPrefab;
    [FoldoutGroup("참조")][SerializeField] private Transform moduleInventoryParent;

    [field: Space]

    [field: SerializeField] public Stat module_Damage { get; private set; } = new Stat();
    [field: SerializeField] public Stat module_Firerate { get; private set; } = new Stat();
    [field: SerializeField] public Stat module_MoveSpeed { get; private set; } = new Stat();
    [field: SerializeField] public Stat module_RotationSpeed { get; private set; } = new Stat();
    [field: SerializeField] public Stat module_MaxHp { get; private set; } = new Stat();
    [field: SerializeField] public Stat module_Armor { get; private set; } = new Stat();
    [field: SerializeField] public Stat module_HpRepair { get; private set; } = new Stat();
    [field: SerializeField] public Stat module_MagnetRange { get; private set; } = new Stat();
    [field: SerializeField] public Stat module_StartMineral { get; private set; } = new Stat();


    /// <sumamry>
    /// 현재 장착중인 모듈을 게임 시작할때 스탯에 적용함
    /// </summary>
    public void AddUpgradeModulesToPlayerStat()
    {
        try
        {
            foreach (UserData.EquipModuleSaveData data in UserDataManager.instance.currentUserData.equipModuleSaveDatas)
            {
                foreach (UpgradeModuleObject module in data.equipedModules)
                {
                    foreach (UpgradeModuleScripableObject obj in UpgradeModuleDropManager.instance.moduleDatas)
                    {
                        if (module.module == obj.module)
                        {
                            obj.ApplyUpgradeModule(this, (int)module.tier - 1);
                            break;
                        }
                    }
                }
            }
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError(e);
        }
    }

    /// <summary>
    /// 모듈 스텟 초기화
    /// </summary>
    public void UnapplyModuleStat()
    {
        module_Damage.ClearAllModifier();
        module_Firerate.ClearAllModifier();
        module_MoveSpeed.ClearAllModifier();
        module_RotationSpeed.ClearAllModifier();
        module_Armor.ClearAllModifier();
        module_HpRepair.ClearAllModifier();
        module_MagnetRange.ClearAllModifier();
        module_StartMineral.ClearAllModifier();
    }

    /// <summary>
    /// 인벤토리 새로고침
    /// </summary>
    public void RefreshInventory()
    {

    }

    /// <summary>
    /// 모듈 판매
    /// </summary>
    public void SellModule()
    {
        RefreshInventory();
    }

    public void StartMineral()
    {
        var tempTable = new DropTable();

        tempTable.crystalType = resourceType.Mineral1;

        tempTable.dropExpMinAmount = 1;
        tempTable.dropCrystalmaxAmount = 1;

        for (int i = 0; i < module_StartMineral.GetFinalStatValue(); i++)
        {
            ResourceGenerator.instance.DeQueueResource(resourceType.Mineral1, new Vector2(transform.position.x, transform.position.y) + Random.insideUnitCircle * 5, tempTable);
        }
    }
}

