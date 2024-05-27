using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new ThrusterStat Stat", menuName = "Scriptable Object/Equipment/New ThrusterStat", order = int.MaxValue)]
public class ThrusterStat : ScriptableObject, IPassiveEquipment
{
    public Coroutine selfRepairCoroutine = null;

    private PlayerStat playerStat;
    private EquipmentSlot CoolTimeSlot;

    public float startUpgradeMoveSpeed = 0.5f;
    public float startUpgraderotation = 0.25f;


    public void UpgradePassive(UpgradeModule upgradeModule)
    {
        switch (upgradeModule.upgradeModuleType)
        {
            case upgradeModuleType.IncreaseMoveSpeed:
                playerStat.moveSpeed.AddFloatModifier(upgradeModule.value1);
                break;

            case upgradeModuleType.IncreaseRotationSpeed:
                playerStat.rotationSpeed.AddFloatModifier(upgradeModule.value1);
                break;
        }
    }

    public void GetPassiveEffect(PlayerStat playerStat)
    {
        this.playerStat = playerStat;
        SetCoolTimeSlot();

        playerStat.moveSpeed.AddFloatModifier(startUpgradeMoveSpeed);
        playerStat.rotationSpeed.AddFloatModifier(startUpgraderotation);
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
        selfRepairCoroutine = coroutine;
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
