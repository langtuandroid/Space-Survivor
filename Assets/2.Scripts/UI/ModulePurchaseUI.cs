using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class ModulePurchaseUI : MonoBehaviour
{
    public const int oneModuleCost = 50;
    public const int tenModuleCost = 450;

    [SerializeField] TextMeshProUGUI crystalText;

    [Space]
    [SerializeField] TextMeshProUGUI oneModuleCostText;
    [SerializeField] TextMeshProUGUI tenModuleCostText;

    [SerializeField] Button freeModuleButton;
    [SerializeField] Image freeModuleButtonImage;

    [SerializeField] Text freeModuleLeftTime;

    Coroutine timeCheck = null;

    private void Start()
    {
        timeCheck = StartCoroutine(FreeModuleTimeChecking());

        IEnumerator FreeModuleTimeChecking()
        {
            while (true)
            {
                if (RewardedInterstitialAdCaller.instance.IsFreeModuleReady())
                {
                    freeModuleButton.enabled = true;
                    freeModuleButtonImage.gameObject.SetActive(true);
                    freeModuleLeftTime.gameObject.SetActive(false);
                }
                else
                {
                    freeModuleButton.enabled = false;
                    freeModuleButtonImage.gameObject.SetActive(false);
                    freeModuleLeftTime.text = Utility.GetFormatedStringFromSecond((int)RewardedInterstitialAdCaller.instance.GetFreeModuleLeftTime());
                    freeModuleLeftTime.gameObject.SetActive(true);
                }

                yield return new WaitForSeconds(1f);
            }

        }
    }

    private void OnEnable()
    {
        UserDataManager.instance.crystalTextList.Add(crystalText);

        crystalText.text = UserDataManager.instance.currentUserData.crystal.ToString();
        oneModuleCostText.text = oneModuleCost.ToString();
        tenModuleCostText.text = tenModuleCost.ToString();
    }

    private void OnDestroy()
    {
        UserDataManager.instance.crystalTextList.Remove(crystalText);

        if (timeCheck != null)
            StopCoroutine(timeCheck);
    }

    public void CloseUI()
    {
        AudioManager.instance.PlaySFX("click2");
        Destroy(gameObject);
    }

    public void CallFreeModuleRVAds()
    {
        AdManager.instance.ShowReward(() =>
        {
            UserDataManager.instance.currentUserData.usingFreeModuleTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            var newModule = UpgradeModuleManager.instance.GenerateRandomModule();
            UpgradeModuleManager.instance.GetNewModule(newModule);
            List<UpgradeModuleObject> newModules = new List<UpgradeModuleObject>();
            newModules.Add(newModule);
            OpenModuleBoxUnboxingUI(newModules, TextAnchor.MiddleCenter);

            EventManager.instance.CustomEvent(AnalyticsType.ADS, "ADS_RvAdsComplete_FreeModule");
        }, RewardType.FreeModule.ToString());

        AudioManager.instance.PlaySFX("click2");
    }

    public void PurchaseOneModule()
    {
        if (UserDataManager.instance.currentUserData.crystal < oneModuleCost)
            return;

        UserDataManager.instance.AddCrystalValue(-oneModuleCost);

        var newModule = UpgradeModuleManager.instance.GenerateRandomModule();

        List<UpgradeModuleObject> newModules = new List<UpgradeModuleObject>();

        newModules.Add(newModule);
        UserDataManager.instance.currentUserData.moduleInventory.Add(newModule);

        AudioManager.instance.PlaySFX("click2");

        OpenModuleBoxUnboxingUI(newModules, TextAnchor.MiddleCenter);

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Module_BuyOneModule");
    }

    public void PurchaseTenModule()
    {
        if (UserDataManager.instance.currentUserData.crystal < tenModuleCost)
            return;

        UserDataManager.instance.AddCrystalValue(-tenModuleCost);

        List<UpgradeModuleObject> newModules = new List<UpgradeModuleObject>();

        for (int i = 0; i < 10; i++)
        {
            var newModule = UpgradeModuleManager.instance.GenerateRandomModule();
            UserDataManager.instance.currentUserData.moduleInventory.Add(newModule);
            newModules.Add(newModule);
        }

        AudioManager.instance.PlaySFX("click2");

        OpenModuleBoxUnboxingUI(newModules, TextAnchor.UpperLeft);

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Module_BuyTenModule");
    }

    public static void OpenModuleBoxUnboxingUI(List<UpgradeModuleObject> modules, TextAnchor anchor)
    {
        var ui = Resources.Load<GameObject>("UI/GainModues");
        var obj = Instantiate(ui, GameManager.instance.MainUIParent);

        obj.GetComponent<ModuleUnboxingUI>().Init(modules, anchor);
    }

}
