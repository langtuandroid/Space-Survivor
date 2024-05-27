using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ModuleDropTable", menuName = "Scriptable Object/Upgrade Module/ModuleDropTable", order = int.MaxValue)]
public class UpgradeModuleDropTable : ScriptableObject
{
    [Range(0, dropMaxPercent)]
    public int dropPercent = 0;

    public const int dropMaxPercent = 10000;
    //public int[] moduleDropWeight = new int[System.Enum.GetValues(typeof(UpgradeModuleTier)).Length];
}
