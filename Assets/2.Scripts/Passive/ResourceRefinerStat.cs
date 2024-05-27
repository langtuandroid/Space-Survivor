using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceRefinerStat Stat", menuName = "Scriptable Object/Equipment/New ResourceRefinerStat Stat", order = int.MaxValue)]
public class ResourceRefinerStat : ScriptableObject, IPassiveEquipment
{
    public float GetPassiveResourceRefinePercent = 0.1f;

    private PlayerStat playerStat;

    public EquipmentSlot CoolTimeSlot;

    public void UpgradePassive(UpgradeModule upgradeModule)
    {
        switch (upgradeModule.upgradeModuleType)
        {
            case upgradeModuleType.IncreaseMineralBonusPercent:
                IncreaseMinealBonus(upgradeModule.value2);
                break;
        }
    }

    public void GetPassiveEffect(PlayerStat playerStat)
    {
        SetCoolTimeSlot();

        this.playerStat = playerStat;

        IncreaseMinealBonus(GetPassiveResourceRefinePercent);
    }

    public IEnumerator StartWhilePassiveEffect()
    {
        yield return null;
    }

    public void SetCoroutine(Coroutine coroutine)
    {

    }

    public void OnEndGame()
    {
        playerStat.getMineralBouse.ClearPercentModifier();
    }

    public void ClearPassive()
    {

    }

    public void SetCoolTimeSlot()
    {
        CoolTimeSlot.weaponCoolTimeImage.disableCoolTime();
    }

    public void StartPassiveSlotCoolTimeImage(float time)
    {
        CoolTimeSlot.weaponCoolTimeImage.StartCoolTime(time);
    }

    public void GetEquipmentSlot(EquipmentSlot slot)
    {
        CoolTimeSlot = slot;
    }


    private void IncreaseMinealBonus(float bonus)
    {
        playerStat.getMineralBouse.AddPercentModifier(bonus);
    }
}
