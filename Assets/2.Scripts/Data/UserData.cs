using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Drawing.Printing;

[System.Serializable]
public class UserData
{
    public string userId;

    public int crystal = 0;

    public bool RemoveAds = false;
    public bool Survey = false;

    [SerializeField]
    public List<string> playerHaveShip = new List<string>();

    public string usingShipTrialTime = "2000-01-01 01:01:01";
    public string usingFreeCrystalTime = "2000-01-01 01:01:01";
    public string usingFreeModuleTime = "2000-01-01 01:01:01";

    public string version;

    [SerializeField]
    public List<int> clearedStageNumber = new List<int>();
    [SerializeField] public EquipModuleSaveData[] equipModuleSaveDatas = { new EquipModuleSaveData(UpgradeModuleType.AttackType), new EquipModuleSaveData(UpgradeModuleType.DefenceType), new EquipModuleSaveData(UpgradeModuleType.MovementType), new EquipModuleSaveData(UpgradeModuleType.SpecialType) };
    public List<UpgradeModuleObject> moduleInventory = new List<UpgradeModuleObject>();


    // public UserData()
    // {
    //     for (int i = 0; i < equipModuleSaveDatas.Length; i++)
    //     {
    //         equipModuleSaveDatas[i].type = (UpgradeModuleType)(i + 1);
    //     }
    // }

    [System.Serializable]
    public class EquipModuleSaveData
    {
        [SerializeField] public UpgradeModuleType type;
        public UpgradeModuleObject[] equipedModules = new UpgradeModuleObject[ModuleEquipData.UPGRADE_MODULE_EQUIP_MAX_COUNT];

        public bool CheckingCurrentEquipModuleData(UpgradeModuleType type)
        {
            return this.type == type ? true : false;
        }

        public EquipModuleSaveData(UpgradeModuleType type)
        {
            this.type = type;
        }
    }



    //public UpgradeModuleObject[] equipedModules_Attack = new UpgradeModuleObject[ModuleEquipData.UPGRADE_MODULE_EQUIP_MAX_COUNT];
    //public UpgradeModuleObject[] equipedModules_Defence = new UpgradeModuleObject[ModuleEquipData.UPGRADE_MODULE_EQUIP_MAX_COUNT];
    //public UpgradeModuleObject[] equipedModules_Movement = new UpgradeModuleObject[ModuleEquipData.UPGRADE_MODULE_EQUIP_MAX_COUNT];
    //public UpgradeModuleObject[] equipedModules_Special = new UpgradeModuleObject[ModuleEquipData.UPGRADE_MODULE_EQUIP_MAX_COUNT];



    public void Init()
    {
        // equipModuleSaveDatas = new EquipModuleSaveData[4];

        for (int i = 0; i < equipModuleSaveDatas.Length; i++)
        {
            if (equipModuleSaveDatas[i].type == UpgradeModuleType.None)
                equipModuleSaveDatas[i].type = (UpgradeModuleType)(i + 1);
        }
    }

    /// <summary>
    /// 코드로 소지한 함선 가져오기
    /// </summary>
    public ShipObjectData GetShipDataFromPlayerHaveShip(string code)
    {
        foreach (string _code in playerHaveShip)
        {
            if (_code.Equals(code))
                return UserDataManager.instance.shipList.shipList.Find((n) => n.shipObjectData.shipCode.Equals(code)).shipObjectData;
        }

        Debug.Log(code + " 이 코드에 해당하는 소지중인 함선이 없습니다.");

        foreach (ShipObject ship in GameManager.instance.shipList.shipList)
        {
            if (ship.shipObjectData.shipCode.Equals(code))
            {
                // MonoBehaviour.print(ship.shipObjectData.baseMoveSpeed.GetBaseValue() + " " + ship.shipObjectData.baseRotationSpeed.GetBaseValue());
                return ship.shipObjectData;
            }
        }

        return UserDataManager.instance.shipList.shipList[0].shipObjectData;
    }
}
