using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeModuleSwapBtn : MonoBehaviour
{
    [SerializeField] private UpgradeModuleEquipment equipment;
    [SerializeField] private int num;

    public void SwapToThisSlot()
    {
        UpgradeModuleManager.instance.SwapModule(equipment.data.slotType, num, equipment.data.equipItems[num].module.GetUpgradeModuleObject());
    }
}
