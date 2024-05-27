using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeModuleDatas : ScriptableObject
{
    public UpgradeModuleObject[] upgradeModuleObjects;

    /// <summary>
    /// 모듈 데이터 가져오기
    /// </summary> 
    public UpgradeModuleObject GetCurrentModuleData(UpgradeModules module)
    {
        for (int i = 0; i < upgradeModuleObjects.Length; i++)
        {
            if (upgradeModuleObjects[i].module == module)
                return upgradeModuleObjects[i];
        }

        return null;
    }
}
