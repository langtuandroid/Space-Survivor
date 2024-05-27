using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new SelfRepair Stat", menuName = "Scriptable Object/Equipment/New SelfRepair Stat", order = int.MaxValue)]

public class SelfRepairStat : ScriptableObject, IPassiveEquipment
{
    public ParticleSystem currentSelfRepairParticle { get; set; }

    public Stat healValue = new Stat();
    public Stat coolTime = new Stat();

    public Coroutine selfRepairCoroutine = null;

    private PlayerStat playerStat;
    private EquipmentSlot CoolTimeSlot;

    public void UpgradePassive(UpgradeModule upgradeModule)
    {
        switch (upgradeModule.upgradeModuleType)
        {
            case upgradeModuleType.IncreaseRepairValue:
                healValue.AddFloatModifier(upgradeModule.value1);
                break;

            case upgradeModuleType.IncreaseRepairRate:
                coolTime.AddFloatModifier(upgradeModule.value1);
                break;
        }
    }

    public void GetPassiveEffect(PlayerStat playerStat)
    {
        this.playerStat = playerStat;
        SetCoolTimeSlot();
    }

    public IEnumerator StartWhilePassiveEffect()
    {
        while (true)
        {
            if (!playerStat.playerDie)
                SelfRepair();

            yield return new WaitForSeconds(coolTime.GetFinalStatValue());
        }
    }

    public void OnEndGame()
    {
        healValue.ClearFloatModifier();
        coolTime.ClearFloatModifier();

        CoroutineHelper.StopCoroutine(selfRepairCoroutine);
    }

    public void SelfRepair()
    {
        //if (currentSelfRepairParticle == null)
        //    currentSelfRepairParticle = Instantiate(selfRepairParticlePrefab).GetComponent<ParticleSystem>();

        //currentSelfRepairParticle.transform.position = playerStat.transform.position;
        //currentSelfRepairParticle.Play();

        VFXGenerator.instance.GenerateVFX(VFXType.Heal1, playerStat.transform.position);

        playerStat.Heal((int)healValue.GetFinalStatValue());
        StartPassiveSlotCoolTimeImage(coolTime.GetFinalStatValue());
    }

    public void SetCoroutine(Coroutine coroutine)
    {
        selfRepairCoroutine = coroutine;
    }

    public void ClearPassive()
    {
        if (selfRepairCoroutine != null)
            CoroutineHelper.StopCoroutine(selfRepairCoroutine);
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
