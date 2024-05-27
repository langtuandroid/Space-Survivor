using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ReloadingDeviceStat", menuName = "Scriptable Object/Equipment/New ReloadingDevice Stat", order = int.MaxValue)]
public class ReloadingDeviceStat : ScriptableObject, IPassiveEquipment
{
    private PlayerStat playerStat;
    private PlayerWeapon playerWeapon;

    private EquipmentSlot CoolTimeSlot;

    public float decreaseCoolTimeValue = 0.06f;

    public void UpgradePassive(UpgradeModule upgradeModule)
    {
        switch (upgradeModule.upgradeModuleType)
        {
            case upgradeModuleType.IncreaseDamagePercent:
                playerWeapon.additionalCoolTimeStat.AddPercentModifier(upgradeModule.value1);
                break;
        }
    }

    public void GetPassiveEffect(PlayerStat playerStat)
    {
        this.playerStat = playerStat;

        playerWeapon = playerStat.GetComponent<PlayerWeapon>();

        playerWeapon.additionalCoolTimeStat.AddPercentModifier(decreaseCoolTimeValue);

        SetCoolTimeSlot();
    }

    public IEnumerator StartWhilePassiveEffect()
    {
        yield return null;
    }

    public void OnEndGame()
    {

    }

    public void SetCoroutine(Coroutine coroutine)
    {

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
}
