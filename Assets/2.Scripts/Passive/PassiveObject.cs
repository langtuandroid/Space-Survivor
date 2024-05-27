using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPassiveEquipment
{
    ///<summary>
    /// 패시브를 처음 획득했을때 발동하는 효과
    ///</summary>
    public void GetPassiveEffect(PlayerStat playerStat);
    public IEnumerator StartWhilePassiveEffect();
    public void UpgradePassive(UpgradeModule upgradeModule);
    public void SetCoroutine(Coroutine coroutine);
    public void ClearPassive();
    public void GetEquipmentSlot(EquipmentSlot slot);
    public void StartPassiveSlotCoolTimeImage(float time);
    public void SetCoolTimeSlot();

    public void OnEndGame();
}

[CreateAssetMenu(fileName = "new Passive", menuName = "Scriptable Object/Equipment/New Passive", order = int.MaxValue)]
public class PassiveObject : ScriptableObject, IEquipment
{
    public int id;
    public IPassiveEquipment passiveStat;

    public Sprite equipmentImage;

    public AnyEqupment anyEqupment;
    public EquipmentType type;

    public int maxWeaponLevel = 4;
    private int currentWeaponLevel = 1;

    public GameObject whileParticlePrefab;
    public GameObject currenWhileParticle;

    public Coroutine passiveWhileCoroutine = null;

    [Space]

    public List<UpgradeModule> currentUpgradeModules = new List<UpgradeModule>();
    public List<UpgradeModuleList> UpgradeModulesForLevel = new List<UpgradeModuleList>();

    public void UpgradeEquipment(UpgradeModuleList modules)
    {
        for (int i = 0; i < modules.upgradeModules.Count; i++)
        {
            passiveStat.UpgradePassive(modules.upgradeModules[i]);
        }

        currentWeaponLevel++;
    }


    // public void GetPassiveEffect(PlayerStat playerStat)
    // {

    // }

    // public void WhilePassiveEffect()
    // {

    // }

    public WeaponObject GetWeaponObject()
    {
        return null;
    }

    public PassiveObject GetPassive()
    {
        return this;
    }

    public EquipmentType GetEquipmentType()
    {
        return type;
    }

    public AnyEqupment GetAnyEqupment()
    {
        return anyEqupment;
    }

    public List<UpgradeModuleList> GetUpgradeModuleLists()
    {
        return UpgradeModulesForLevel;
    }

    public UpgradeModuleList GetUpgradeModuleList()
    {
        return UpgradeModulesForLevel[currentWeaponLevel - 1];
    }

    public Sprite GetEquipmentIamge()
    {
        return equipmentImage;
    }

    public void GenerateWhileParticle(Transform trans)
    {
        if (whileParticlePrefab == null)
            return;

        currenWhileParticle = Instantiate(whileParticlePrefab, trans);
    }

    public int GetCurrentWeaponLevel()
    {
        return currentWeaponLevel;
    }
}
