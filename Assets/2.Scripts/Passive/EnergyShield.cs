using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new EnergyShield Stat", menuName = "Scriptable Object/Equipment/New EnergyShield Stat", order = int.MaxValue)]
public class EnergyShield : ScriptableObject, IPassiveEquipment
{
    public Stat invinsibleTime;
    public Stat shieldReloadTime;
    public int shieldStackAmount = 1;

    public GameObject shieldPrefab;
    public GameObject currentShieldObject;

    public EquipmentSlot CoolTimeSlot;

    private PlayerStat playerStat;

    public void UpgradePassive(UpgradeModule upgradeModule)
    {
        switch (upgradeModule.upgradeModuleType)
        {
            case upgradeModuleType.DecreaseCoolTime:
                playerStat.shieldReloadTime.AddPercentModifier(-upgradeModule.value1);
                break;

            case upgradeModuleType.ShieldCountIncrease:
                AddShieldStack();
                break;
        }
    }

    public void GetPassiveEffect(PlayerStat playerStat)
    {
        GiveShield(playerStat);
        SetCoolTimeSlot();

        this.playerStat = playerStat;
    }

    public IEnumerator StartWhilePassiveEffect()
    {
        yield return null;
    }

    public void OnEndGame()
    {

    }

    private void GiveShield(PlayerStat playerStat)
    {
        if (playerStat.maxShieldStack < shieldStackAmount)
        {
            if (currentShieldObject == null)
                currentShieldObject = Instantiate(shieldPrefab, playerStat.transform);

            playerStat.GetShield(currentShieldObject);
        }

        //playerStat
    }

    private void AddShieldStack()
    {
        shieldStackAmount++;

        playerStat.GetShield(currentShieldObject);
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
