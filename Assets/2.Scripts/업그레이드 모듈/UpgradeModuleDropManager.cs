using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeModuleDropManager : MonoBehaviour
{
    public List<UpgradeModuleObject> getUpgradeModuleOnThisStage = new List<UpgradeModuleObject>();

    public ObjectPool moduleDropsPool = new ObjectPool();

    public static UpgradeModuleDropManager instance;

    public UpgradeModuleScripableObject[] moduleDatas;

    private List<GameObject> generatedModuleDrops = new List<GameObject>();

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        InGameObjectManager.Instance.AddClearableObejctListener(ref generatedModuleDrops, ref moduleDropsPool);
    }

    /// <summary>
    /// 모듈 드롭스 생성
    /// </summary>
    public void GenerateRandomModuleDrops(Vector3 position)
    {
        var drops = moduleDropsPool.DequeueObject(position);
        drops.GetComponent<ModuleDrops>().SetModuleStat(UpgradeModuleManager.instance.GenerateRandomModule());
        generatedModuleDrops.Add(drops);
    }

    /// <summary>
    /// 이번 스테이지에서 획득한 모듈 습득
    /// </summary>
    public void OptainModules()
    {
        EventManager.instance.CustomEvent(AnalyticsType.GAME, "ObtainModuleOnStage");

        for (int i = 0; i < getUpgradeModuleOnThisStage.Count; i++)
        {
            UserDataManager.instance.currentUserData.moduleInventory.Add(getUpgradeModuleOnThisStage[i]);
        }

        //getUpgradeModuleOnThisStage.Clear();
    }

    public void ClearGeneartedModuleOnThisStage()
    {
    }
}
