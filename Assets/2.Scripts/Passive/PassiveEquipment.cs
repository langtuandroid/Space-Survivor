using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassiveEquipment : MonoBehaviour
{
    public PassiveType passiveType;

    public void GetPassive()
    {

    }

    public void UpgradePassive()
    {
        
    }
}

public enum PassiveType 
{
    Armor, Thruster, EnergyShield, Magnetic, MineralPurifier
}