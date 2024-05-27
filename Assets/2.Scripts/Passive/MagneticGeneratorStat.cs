using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MagneticGenerator Stat", menuName = "Scriptable Object/Equipment/New MagneticGenerator Stat", order = int.MaxValue)]
public class MagneticGeneratorStat : ScriptableObject, IPassiveEquipment
{
    public float GetPassiveResourcePullRadius = 10f;

    private PlayerStat playerStat;

    public EquipmentSlot CoolTimeSlot;

    public void UpgradePassive(UpgradeModule upgradeModule)
    {
            switch (upgradeModule.upgradeModuleType)
            {
            case upgradeModuleType.IncreaseResourcePullRadius:

                playerStat.GetComponent<CircleCollider2D>().radius = playerStat.GetComponent<CircleCollider2D>().radius + GetPassiveResourcePullRadius;
                break;
            }
    }

    public void GetPassiveEffect(PlayerStat playerStat)
    {
        SetCoolTimeSlot();

        this.playerStat = playerStat;

        playerStat.GetComponent<CircleCollider2D>().radius = playerStat.GetComponent<CircleCollider2D>().radius + GetPassiveResourcePullRadius;
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
