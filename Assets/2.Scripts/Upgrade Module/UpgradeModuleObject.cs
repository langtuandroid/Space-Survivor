using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Diagnostics;
using System;
using System.Linq;

[System.Serializable]
public class UpgradeModuleObject : IUpgradeModule
{
    [SerializeField] public UpgradeModuleType type;
    UpgradeModuleType IUpgradeModule.type => type;

    [SerializeField] public UpgradeModules module;
    UpgradeModules IUpgradeModule.module => module;

    [SerializeField] public UpgradeModuleTier tier;
    UpgradeModuleTier IUpgradeModule.tier => tier;

    [SerializeField] public string key;
    string IUpgradeModule.key => key;

    [SerializeField] public bool isNew = true;


    public static readonly int[] sellCosts = { 0, 8, 50, 200, 400, 1000 };


    public UpgradeModuleObject()
    {

    }

    public UpgradeModuleObject(UpgradeModuleType type, UpgradeModules module, UpgradeModuleTier tier)
    {
        this.type = type;
        this.module = module;
        this.tier = tier;
        key = Guid.NewGuid().ToString();
    }

    /// <summary>
    /// 플레이어에 현재 장착중인 모듈 효과 적용
    /// </summary>
    public virtual void ApplyUpgradeModule(PlayerWeapon playerWeapon)
    {
        UnityEngine.Debug.LogError("기본 모듈 효과 적용 함수 발동");
    }


    public virtual void UnapplyUpgradeModule()
    {
        UnityEngine.Debug.LogError("기본 모듈 장착해제 함수 발동");
    }

    /// <summary>
    /// 모듈 티어업
    /// </summary>
    public virtual void TierUpThisModule()
    {
        //UnityEngine.Debug.LogError("기본 모듈 티어업 함수 발동");

        tier = (UpgradeModuleTier)((int)tier + 1);
    }

    public virtual float GetModuleStat(int tier)
    {
        return 0;
    }

    public void SetModuleInfo(UpgradeModuleType type, UpgradeModules modules, UpgradeModuleTier tier, string key)
    {
        this.type = type;
        this.module = modules;
        this.tier = tier;
        this.key = key;
    }

    public UpgradeModuleObject GetUpgradeModuleObject()
    {
        return this;
    }

    public int GetSellCost()
    {
        return sellCosts[(int)tier];
    }

    public UpgradeModuleScripableObject GetScriptableObejct()
    {
        var scriptableObjects = Resources.LoadAll<UpgradeModuleScripableObject>("UpgradeModules").ToList();
        var find = scriptableObjects.Find(f => f.ID == (int)module);

        return find;
    }
}

