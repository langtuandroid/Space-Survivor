using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Diagnostics;
using System;
using System.Linq;
using UnityEngine.UI;
using Sirenix.OdinInspector;


public class UpgradeModuleManager : MonoBehaviour
{

    [FoldoutGroup("참조")][SerializeField] private GameObject upgradeModulePanel;

    [Space]

    [SerializeField] private UpgradeModuleEquipment[] playerModuleEquips;
    //[SerializeField] private UpgradeModuleEquipment playerModuleEquip_Attack;
    //[SerializeField] private UpgradeModuleEquipment playerModuleEquip_Defence;
    //[SerializeField] private UpgradeModuleEquipment playerModuleEquip_Movement;
    //[SerializeField] private UpgradeModuleEquipment playerModuleEquip_Special;

    [Space]

    [FoldoutGroup("참조")][SerializeField] private GameObject detailPanel;

    [Space]

    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI detail_Name;
    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI detail_Tier;
    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI detail_Stat_Normal;
    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI detail_Stat_Magic;
    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI detail_Stat_Rare;
    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI detail_Stat_Unique;
    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI detail_Stat_Legendary;
    [FoldoutGroup("참조")][SerializeField] private Image detail_SlotCover;
    [FoldoutGroup("참조")][SerializeField] private Image detail_ModuleImage;
    [FoldoutGroup("detail")][SerializeField] private Color detail_NormalColor;
    [FoldoutGroup("detail")][SerializeField] private Color detail_MagicColor;
    [FoldoutGroup("detail")][SerializeField] private Color detail_RareColor;
    [FoldoutGroup("detail")][SerializeField] private Color detail_UniqueColor;
    [FoldoutGroup("detail")][SerializeField] private Color detail_LegendaryColor;

    [Space]

    [FoldoutGroup("참조")][SerializeField] private GameObject sellBtn;
    [FoldoutGroup("참조")][SerializeField] private GameObject upgradeBtn;
    [FoldoutGroup("참조")][SerializeField] private GameObject equipBtn;
    [FoldoutGroup("참조")][SerializeField] private GameObject unequipBtn;

    [Space]

    [FoldoutGroup("참조")][SerializeField] private Transform playerModuleInventoryParent;
    [FoldoutGroup("참조")][SerializeField] private Transform playerModuleEquipParent;


    [FoldoutGroup("참조")][SerializeField] private GameObject modulePrefab;

    [Space]

    [FoldoutGroup("참조")][SerializeField] GameObject moduleUpgradePanel;
    [FoldoutGroup("참조")][SerializeField] Transform moduleUpgradeInventoryParent;
    [FoldoutGroup("참조")][SerializeField] Button moduleUpgradeBtn;
    [Space]
    [Header("모듈 합성메뉴에 올려져있는 모듈 정보")]
    [BoxGroup("모듈 업그레이드 관련")][SerializeField] UpgradeModuleObject moduleUpgrade1;
    [FoldoutGroup("참조")][SerializeField] Image moduleUpgrade1Image;
    [FoldoutGroup("참조")][SerializeField] Image moduleUpgrade1Cover;
    [BoxGroup("모듈 업그레이드 관련")][SerializeField] UpgradeModuleObject moduleUpgrade2;
    [FoldoutGroup("참조")][SerializeField] Image moduleUpgrade2Image;
    [FoldoutGroup("참조")][SerializeField] Image moduleUpgrade2Cover;
    [BoxGroup("모듈 업그레이드 관련")][SerializeField] UpgradeModuleObject moduleUpgrade3;
    [FoldoutGroup("참조")][SerializeField] Image moduleUpgrade3Image;
    [FoldoutGroup("참조")][SerializeField] Image moduleUpgrade3Cover;
    //[TitleGroup("Upgrade")][SerializeField] GameObject moduleUpgradePanel;

    [Space]

    [FoldoutGroup("참조")][SerializeField] private GameObject sellPanel;
    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI costText;
    [FoldoutGroup("참조")][SerializeField] private Button sellYesBtn;
    [ShowInInspector] public static readonly float[] UpgradeModuleTierDropPercent = { 10000, 800, 20, 5, 1 };

    //public List<UpgradeModuleObject> TestequipedModules = new List<UpgradeModuleObject>();
    //public List<UpgradeModuleObject> TestmoduleInventory = new List<UpgradeModuleObject>();

    [Space]

    [FoldoutGroup("참조")][SerializeField] private UpgradeModuleObject swapingModule;
    [FoldoutGroup("참조")][SerializeField] private UpgradeModuleObject selectedModule;
    [FoldoutGroup("참조")][SerializeField] private ModuleSortButton[] sortBtns;


    [field: Space]

    [field: SerializeField] public Color[] tierColor { get; private set; }

    public UpgradeModuleType currentSortType = UpgradeModuleType.None;

    public static UpgradeModuleManager instance;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        GenerateInventoryModulePrefabs();
        GenerateEquipModulePrefabs();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            GetNewModule(GenerateRandomModule());
    }

    public void UpdateUI(UpgradeModuleType sortType = UpgradeModuleType.None)
    {
        ClearModuleDisplay();

        GenerateInventoryModulePrefabs(false, sortType);
        GenerateEquipModulePrefabs(false);
    }

    /// <summary>
    /// 게임 시작시 인벤토리 모듈 프리팹 생성
    /// </summary>
    public void GenerateInventoryModulePrefabs(bool tweening = true, UpgradeModuleType sortType = UpgradeModuleType.None)
    {
        // print(UserDataManager.instance.currentUserData.moduleInventory.Count);

        currentSortType = sortType;

        var sortedModuleList = UserDataManager.instance.currentUserData.moduleInventory.OrderByDescending(n => n.tier).ToList();

        if (sortType == UpgradeModuleType.None)
        {
            for (int i = 0; i < sortedModuleList.Count; i++)
            {
                GenerateModuleSlot(i);
            }
        }
        else
        {
            for (int i = 0; i < sortedModuleList.Count; i++)
            {
                if (sortedModuleList[i].GetUpgradeModuleObject().type == sortType)
                {
                    GenerateModuleSlot(i);
                }
            }

            for (int i = 0; i < sortedModuleList.Count; i++)
            {
                if (sortedModuleList[i].GetUpgradeModuleObject().type != sortType)
                {
                    GenerateModuleSlot(i, _lock: true);
                }
            }
        }

        void GenerateModuleSlot(int i, bool _lock = false)
        {
            var module = Instantiate(modulePrefab, Vector3.zero, Quaternion.identity, playerModuleInventoryParent).GetComponent<ModuleItem>();

            module.InitModule(sortedModuleList[i].GetUpgradeModuleObject(), ModuleItem.SlotType.inventory, tweening: tweening, Lock: _lock);
        }

        foreach (var slot in sortBtns)
        {
            if (currentSortType == slot.type)
                slot.OnSelect();
            else
                slot.OnUnselect();
        }

    }

    /// <summary>
    /// 게임 시작시 장착중인 모듈 프리팹 생성
    /// </summary>
    public void GenerateEquipModulePrefabs(bool tweening = true)
    {
        try
        {
            // print(UserDataManager.instance.currentUserData.equipModuleSaveDatas.Length);
            for (int e = 0; e < UserDataManager.instance.currentUserData.equipModuleSaveDatas.Length; e++)
            {
                for (int i = 0; i < UserDataManager.instance.currentUserData.equipModuleSaveDatas[e].equipedModules.Length; i++)
                {
                    if (UserDataManager.instance.currentUserData.equipModuleSaveDatas[e].equipedModules[i] != null)
                    {
                        if (UserDataManager.instance.currentUserData.equipModuleSaveDatas[e].equipedModules[i].type != UpgradeModuleType.None
                         && UserDataManager.instance.currentUserData.equipModuleSaveDatas[e].equipedModules[i].key != "")
                        {
                            for (int j = 0; j < playerModuleEquips.Length; j++)
                            {
                                if (playerModuleEquips[j].data.slotType == UserDataManager.instance.currentUserData.equipModuleSaveDatas[e].equipedModules[i].type)
                                    playerModuleEquips[j].EquipModuleAsNumber(modulePrefab, UserDataManager.instance.currentUserData.equipModuleSaveDatas[e].equipedModules[i].GetUpgradeModuleObject(), i, tweening: tweening);
                            }
                        }
                    }
                }
            }

        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError(e);
        }
    }

    public void ClearModuleDisplay()
    {
        foreach (UpgradeModuleEquipment equipment in playerModuleEquips)
        {
            equipment.ClearItems();
        }

        var items = playerModuleInventoryParent.GetComponentsInChildren<ModuleItem>();

        for (int i = 0; i < items.Length; i++)
        {
            Destroy(items[i].gameObject);
        }
    }

    /// <summary>
    /// 모듈 메뉴
    /// </summary>
    public void ActiveUpgradeModulePanel()
    {
        if (!upgradeModulePanel.activeSelf)
        {
            ClearModuleDisplay();

            GenerateInventoryModulePrefabs(false, currentSortType);
            GenerateEquipModulePrefabs(false);

            EventManager.instance.CustomEvent(AnalyticsType.UI, "UI_UpgradeModuleUI");
        }
        else
        {
            currentSortType = UpgradeModuleType.None;

            foreach (var module in UserDataManager.instance.currentUserData.moduleInventory)
            {
                module.GetUpgradeModuleObject().isNew = false;
            }

            foreach (var equip in playerModuleEquips)
            {
                for (int i = 0; i < equip.data.equipItems.Count(); i++)
                {
                    if (equip.data.equipItems[i] != null)
                        equip.data.equipItems[i].module.GetUpgradeModuleObject().isNew = false;
                }
            }

            UserDataManager.instance.Save();
            // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
        }

        upgradeModulePanel.SetActive(!upgradeModulePanel.activeSelf);
    }

    /// <summary>
    /// 모듈 설명창 열기
    /// </summary>
    public void OpenModuleUpgradeDetail(ModuleItem item)
    {

        UpdateModuleUpgradeDetail(item.module.GetUpgradeModuleObject());

        //해당 모듈 스탯 현지화


        //버튼 활성화 여부 인벤에 있으면 장착 버튼 활성화 장비중이면 장착해제 버튼 활성화
        switch (item.slotType)
        {
            case ModuleItem.SlotType.inventory:
                equipBtn.SetActive(true);
                equipBtn.GetComponent<Button>().onClick.RemoveAllListeners();
                equipBtn.GetComponent<Button>().onClick.AddListener(() => EquipModule(item));
                unequipBtn.SetActive(false);
                break;

            case ModuleItem.SlotType.equip:
                equipBtn.SetActive(false);
                unequipBtn.GetComponent<Button>().onClick.RemoveAllListeners();
                unequipBtn.GetComponent<Button>().onClick.AddListener(() => UnEquipModule(item));
                unequipBtn.SetActive(true);
                break;

            default:
                equipBtn.SetActive(true);
                equipBtn.GetComponent<Button>().onClick.RemoveAllListeners();
                equipBtn.GetComponent<Button>().onClick.AddListener(() => EquipModule(item));
                unequipBtn.SetActive(false);
                break;
        }

        sellBtn.GetComponent<Button>().onClick.RemoveAllListeners();
        sellBtn.GetComponent<Button>().onClick.AddListener(() => OnClickModuleSellBtn(item));

        detail_SlotCover.sprite = GetUpgradeModuleCover(item.module.tier);
        detail_ModuleImage.sprite = GetUpgradeModuleImage((int)item.module.module);

        ModuleItem.SetModuleImageColor(detail_ModuleImage, item.module.type);

        var scriptableObjects = Resources.LoadAll<UpgradeModuleScripableObject>("UpgradeModules").ToList();
        var find = scriptableObjects.Find(f => f.ID == (int)item.module.module);

        find.GetLocalizedModuleStatText(detail_Stat_Normal, 0);
        find.GetLocalizedModuleStatText(detail_Stat_Magic, 1);
        find.GetLocalizedModuleStatText(detail_Stat_Rare, 2);
        find.GetLocalizedModuleStatText(detail_Stat_Unique, 3);
        find.GetLocalizedModuleStatText(detail_Stat_Legendary, 4);

        if (item.module.tier == UpgradeModuleTier.Normal)
        {
            detail_Stat_Normal.color = Color.white;
            detail_Stat_Normal.GetComponentInChildren<Image>().color = Color.white;
            detail_Tier.color = detail_NormalColor;
        }
        else
        {
            detail_Stat_Normal.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 40f / 255f);
            detail_Stat_Normal.GetComponentInChildren<Image>().color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 40f / 255f);
        }

        if (item.module.tier == UpgradeModuleTier.Magic)
        {
            detail_Stat_Magic.color = Color.white;
            detail_Stat_Magic.GetComponentInChildren<Image>().color = Color.white;
            detail_Tier.color = detail_MagicColor;
        }
        else
        {

            detail_Stat_Magic.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 40f / 255f);
            detail_Stat_Magic.GetComponentInChildren<Image>().color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 40f / 255f);
        }

        if (item.module.tier == UpgradeModuleTier.Rare)
        {
            detail_Stat_Rare.color = Color.white;
            detail_Stat_Rare.GetComponentInChildren<Image>().color = Color.white;
            detail_Tier.color = detail_RareColor;
        }
        else
        {

            detail_Stat_Rare.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 40f / 255f);
            detail_Stat_Rare.GetComponentInChildren<Image>().color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 40f / 255f);
        }

        if (item.module.tier == UpgradeModuleTier.Unique)
        {
            detail_Stat_Unique.color = Color.white;
            detail_Stat_Unique.GetComponentInChildren<Image>().color = Color.white;
            detail_Tier.color = detail_UniqueColor;
        }
        else
        {
            detail_Stat_Unique.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 40f / 255f);
            detail_Stat_Unique.GetComponentInChildren<Image>().color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 40f / 255f);

        }

        if (item.module.tier == UpgradeModuleTier.Legendary)
        {
            detail_Stat_Legendary.color = Color.white;
            detail_Stat_Legendary.GetComponentInChildren<Image>().color = Color.white;
            detail_Tier.color = detail_LegendaryColor;
        }
        else
        {

            detail_Stat_Legendary.color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 40f / 255f);
            detail_Stat_Legendary.GetComponentInChildren<Image>().color = new Color(255f / 255f, 255f / 255f, 255f / 255f, 40f / 255f);
        }

        ActiveModuleDetailPanel();

        selectedModule = item.module.GetUpgradeModuleObject();
    }

    public void UpdateModuleUpgradeDetail(UpgradeModuleObject module)
    {
        //이름 티어 현지화
        Utility.SetLocalizeTextAsync(detail_Name, "Module", (int)module.module + "_Name");
        Utility.SetLocalizeTextAsync(detail_Tier, "Module", module.tier.ToString());
    }

    /// <summary>
    /// 모듈 설명창 열기
    /// </summary>
    public void ActiveModuleDetailPanel()
    {
        detailPanel.SetActive(!detailPanel.activeSelf);
    }

    /// <summary>
    /// 모듈 장착
    /// </summary>
    public void EquipModule(ModuleItem module)
    {
        bool success = false;

        for (int i = 0; i < UserDataManager.instance.currentUserData.equipModuleSaveDatas.Length; i++)
        {
            if (UserDataManager.instance.currentUserData.equipModuleSaveDatas[i].CheckingCurrentEquipModuleData(module.module.type))
            {
                for (int j = 0; j < playerModuleEquips.Length; j++)
                {
                    if (playerModuleEquips[j].data.slotType == module.module.type)
                    {
                        success = playerModuleEquips[j].EquipModule(modulePrefab, module.module.GetUpgradeModuleObject());
                        break;
                    }
                }

                for (int j = 0; j < UserDataManager.instance.currentUserData.equipModuleSaveDatas[i].equipedModules.Length; j++)
                {
                    if (UserDataManager.instance.currentUserData.equipModuleSaveDatas[i].equipedModules[j].type == UpgradeModuleType.None)
                    {
                        UserDataManager.instance.currentUserData.equipModuleSaveDatas[i].equipedModules[j] = module.module.GetUpgradeModuleObject();
                        break;
                    }
                }
            }
        }

        if (success)
        {
            for (int i = 0; i < UserDataManager.instance.currentUserData.moduleInventory.Count; i++)
            {
                if (UserDataManager.instance.currentUserData.moduleInventory[i].key == module.module.key)
                {
                    UserDataManager.instance.currentUserData.moduleInventory.RemoveAt(i);
                    Destroy(module.gameObject);

                    break;
                }
            }
        }
        else
        {
            print("Swap Mode On");
            SwapModeOn(module.module.GetUpgradeModuleObject());
        }

        ActiveModuleDetailPanel();
    }

    /// <summary>
    /// 모듈 장착 해제
    /// </summary>
    public void UnEquipModule(ModuleItem module)
    {
        GetNewModule(module.module.GetUpgradeModuleObject(), false);

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Module_UnEquipModule");
        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Module_UnEquipModule_tier : " + module.module.tier);

        // foreach (UserData.EquipModuleSaveData data in UserDataManager.instance.currentUserData.equipModuleSaveDatas)
        // {
        //     if (data.CheckingCurrentEquipModuleData(module.module.type))
        //     {
        //         for (int j = 0; j < data.equipedModules.Length; j++)
        //         {
        //             if (data.equipedModules[j].key == module.module.key)
        //             {
        //                 data.equipedModules[j] = new UpgradeModuleObject();
        //                 break;
        //             }
        //         }
        //         break;
        //     }
        // }

        DestroyImmediate(module.gameObject);

        // print(module.gameObject);

        DeleteModule(module.module.GetUpgradeModuleObject());


        ActiveModuleDetailPanel();

        UpdateUI(currentSortType);
    }

    /// <summary>
    /// 랜덤 모듈 생성
    /// </summary>
    public UpgradeModuleObject GenerateRandomModule(int tierNum = -1, int typeNum = -1)
    {
        UpgradeModules modules;
        if (typeNum == -1)
            modules = (UpgradeModules)UnityEngine.Random.Range(1, System.Enum.GetValues(typeof(UpgradeModules)).Length);
        else
            modules = (UpgradeModules)typeNum;

        UpgradeModuleTier tier;
        if (tierNum == -1)
            tier = (UpgradeModuleTier)(Utility.GetRandomProb(UpgradeModuleTierDropPercent)) + 1;
        else
            tier = (UpgradeModuleTier)tierNum;

        UpgradeModuleObject moduleObject = null;

        foreach (UpgradeModuleScripableObject upgradeModule in UpgradeModuleDropManager.instance.moduleDatas)
        {
            if (upgradeModule.module == modules)
            {
                moduleObject = new UpgradeModuleObject(upgradeModule.type, upgradeModule.module, tier);
                break;
            }
        }

        if (moduleObject == null)
            UnityEngine.Debug.LogError("해당하는 모듈의 정보를 찾지 못했습니다.");

        return moduleObject;
    }

    /// <summary>
    /// 새로운 모듈 획득
    /// </summary>
    public void GetNewModule(UpgradeModuleObject newModule, bool _update = true)
    {
        UserDataManager.instance.currentUserData.moduleInventory.Add(newModule);

        var newModuleItem = Instantiate(modulePrefab, playerModuleInventoryParent);

        newModuleItem.GetComponent<ModuleItem>().InitModule(newModule, ModuleItem.SlotType.inventory);

        print("새로운 모듈 획득 : " + newModule.module + " | " + newModule.tier + " | " + newModule.type + " | " + newModule.key);

        // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
        UserDataManager.instance.Save();

        if (_update)
            UpdateUI(currentSortType);
    }

    /// <summary>
    /// 플레이어의 모듈 정보에 변화가 생겼을때 저장하는 함수
    /// </summary>
    public void OnChangeUpgardeModuleSystem()
    {
        // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
        UserDataManager.instance.Save();
    }

    public void ActiveModuleSellPanel()
    {
        sellPanel.SetActive(!sellPanel.activeSelf);
    }

    public void OnClickModuleSellBtn(ModuleItem item)
    {
        costText.text = item.module.GetSellCost().ToString();

        sellYesBtn.onClick.RemoveAllListeners();
        sellYesBtn.onClick.AddListener(() => SellModule(item));

        ActiveModuleSellPanel();
    }

    public void SellModule(ModuleItem item)
    {
        UserDataManager.instance.AddCrystalValue(item.module.GetSellCost());

        switch (item.slotType)
        {
            case ModuleItem.SlotType.inventory:
                for (int i = 0; i < UserDataManager.instance.currentUserData.moduleInventory.Count; i++)
                {
                    if (item.module.key == UserDataManager.instance.currentUserData.moduleInventory[i].key)
                    {
                        UserDataManager.instance.currentUserData.moduleInventory.RemoveAt(i);
                        break;
                    }
                }
                break;

            case ModuleItem.SlotType.equip:
                foreach (UserData.EquipModuleSaveData data in UserDataManager.instance.currentUserData.equipModuleSaveDatas)
                {
                    if (data.CheckingCurrentEquipModuleData(item.module.type))
                    {
                        for (int i = 0; i < data.equipedModules.Length; i++)
                        {
                            if (data.equipedModules[i].key == item.module.key)
                            {
                                data.equipedModules[i] = new UpgradeModuleObject();
                                break;
                            }
                        }
                        break;
                    }
                }
                break;
        }

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Module_SellModule");
        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Module_SellModule_tier : " + item.module.tier);
        Destroy(item.gameObject);

        ActiveModuleSellPanel();
        ActiveModuleDetailPanel();
    }

    public void SwapModule(UpgradeModuleType type, int num, UpgradeModuleObject swapModule)
    {
        UpgradeModuleObject tempModule = null;

        foreach (UserData.EquipModuleSaveData data in UserDataManager.instance.currentUserData.equipModuleSaveDatas)
        {
            if (data.CheckingCurrentEquipModuleData(type))
            {
                tempModule = data.equipedModules[num];

                data.equipedModules[num] = swapingModule;

                for (int i = 0; i < UserDataManager.instance.currentUserData.moduleInventory.Count; i++)
                {
                    if (swapingModule.key == UserDataManager.instance.currentUserData.moduleInventory[i].key)
                    {
                        UserDataManager.instance.currentUserData.moduleInventory[i] = tempModule;
                        break;
                    }
                }
                break;
            }
        }

        foreach (UpgradeModuleEquipment equipment in playerModuleEquips)
        {
            equipment.ClearItems();
        }

        var items = playerModuleInventoryParent.GetComponentsInChildren<ModuleItem>();

        for (int i = 0; i < items.Length; i++)
        {
            Destroy(items[i].gameObject);
        }

        GenerateInventoryModulePrefabs(false, currentSortType);
        GenerateEquipModulePrefabs(false);

        SwapModeOff();
    }

    public void SwapModeOn(UpgradeModuleObject moduleObject)
    {
        foreach (UpgradeModuleEquipment equipment in playerModuleEquips)
        {
            if (equipment.data.slotType == moduleObject.type)
            {
                swapingModule = moduleObject;
                equipment.SwapModeOn();
                break;
            }
        }

    }

    public void SwapModeOff()
    {
        swapingModule = null;

        foreach (UpgradeModuleEquipment equipment in playerModuleEquips)
        {
            equipment.SwapModeOff();
        }
    }


    public void OnClickModuleUpgradeBtn()
    {
        ClearModuleUpgradeInventory();

        moduleUpgrade1 = selectedModule;

        var scriptableObjects = Resources.LoadAll<UpgradeModuleScripableObject>("UpgradeModules").ToList();
        var find = scriptableObjects.Find(f => f.ID == (int)selectedModule.module);
        moduleUpgrade1Image.sprite = find?.moduleIcon;

        moduleUpgrade1Cover.sprite = GetUpgradeModuleCover(selectedModule.tier);

        ModuleItem.SetModuleImageColor(moduleUpgrade1Image, selectedModule.type);

        ActiveModuleUpgradePanel();
        GenerateModuleUpgradeInventory();
    }

    public void ActiveModuleUpgradePanel()
    {
        moduleUpgradePanel.gameObject.SetActive(!moduleUpgradePanel.activeSelf);
    }

    public void GenerateModuleUpgradeInventory()
    {
        foreach (UpgradeModuleObject module in UserDataManager.instance.currentUserData.moduleInventory)
        {
            if (moduleUpgrade1.key != module.key && moduleUpgrade1.type == module.type && moduleUpgrade1.tier == module.tier)
            {
                var slot = Instantiate(modulePrefab, moduleUpgradeInventoryParent).GetComponent<ModuleItem>();
                slot.InitModule(module, ModuleItem.SlotType.inventory, tweening: false);
                slot.moduleUpgradeCoverImage.gameObject.SetActive(true);
            }
        }
    }

    public void ClearModuleUpgradeInventory()
    {
        var inventory = moduleUpgradeInventoryParent.GetComponentsInChildren<ModuleItem>();

        for (int i = 0; i < inventory.Length; i++)
        {
            Destroy(inventory[i].gameObject);
        }

        var transparentImage = Resources.Load<Sprite>("UI/transparent");

        moduleUpgrade1 = null;
        moduleUpgrade1Image.sprite = transparentImage;


        moduleUpgrade2 = null;
        moduleUpgrade2Image.sprite = transparentImage;
        moduleUpgrade2Cover.enabled = false;

        moduleUpgrade3 = null;
        moduleUpgrade3Image.sprite = transparentImage;
        moduleUpgrade3Cover.enabled = false;

    }

    public void ModuleUpgrade()
    {
        // if (moduleUpgrade1.module != moduleUpgrade2.module ||
        //  moduleUpgrade2.module != moduleUpgrade3.module ||
        //  moduleUpgrade1.module != moduleUpgrade3.module)
        // {
        //     UnityEngine.Debug.LogError("모듈들의 종류가 다릅니다.");
        //     ActiveModuleUpgradePanel();
        //     return;
        // }

        if (moduleUpgrade1.type != moduleUpgrade2.type ||
        moduleUpgrade2.type != moduleUpgrade3.type ||
        moduleUpgrade1.type != moduleUpgrade3.type)
        {
            UnityEngine.Debug.LogError("모듈들의 타입이 다릅니다.");
            ActiveModuleUpgradePanel();
            return;
        }

        //모듈 합성 UI 생성하고 에니메이션 시작
        var upgradeUIPrefab = Resources.Load<GameObject>("UI/ModuleUpgradeUI");

        var upgradeUI = Instantiate(upgradeUIPrefab, GameManager.instance.MainUIParent);

        List<UpgradeModuleObject> moduleList = new List<UpgradeModuleObject>();
        moduleList.Add(moduleUpgrade1);
        moduleList.Add(moduleUpgrade2);
        moduleList.Add(moduleUpgrade3);

        upgradeUI.GetComponentInChildren<ModuleUpgradeUI>().StartUpgrade(moduleList);

        moduleUpgrade1.TierUpThisModule();
        DeleteModule(moduleUpgrade2);
        DeleteModule(moduleUpgrade3);

        ClearModuleDisplay();

        GenerateInventoryModulePrefabs(sortType: currentSortType);
        GenerateEquipModulePrefabs();

        UpdateModuleUpgradeDetail(selectedModule);

        ActiveModuleUpgradePanel();
        ActiveModuleDetailPanel();

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Module_ModuleUpgrade");
        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Module_ModuleUpgrade_tier - " + moduleUpgrade1.tier.ToString());
    }

    ///<summary>
    /// 인벤토리 또는 장비중인 모듈 삭제
    ///</summary>
    public void DeleteModule(UpgradeModuleObject module)
    {
        //장비창 검색
        foreach (UserData.EquipModuleSaveData data in UserDataManager.instance.currentUserData.equipModuleSaveDatas)
        {
            for (int i = 0; i < data.equipedModules.Length; i++)
            {
                if (data.equipedModules[i].key == module.key)
                {
                    data.equipedModules[i] = new UpgradeModuleObject();

                    ResetModuleStatText(module);
                    return;
                }
            }
        }

        //인벤토리 검색
        for (int i = 0; i < UserDataManager.instance.currentUserData.moduleInventory.Count; i++)
        {
            if (UserDataManager.instance.currentUserData.moduleInventory[i].key == module.key)
            {
                UserDataManager.instance.currentUserData.moduleInventory.RemoveAt(i);
                return;
            }
        }
    }

    public void ResetModuleStatText(UpgradeModuleObject module)
    {
        for (int i = 0; i < playerModuleEquips.Length; i++)
        {
            if (playerModuleEquips[i].data.slotType == module.type)
            {
                for (int j = 0; j < playerModuleEquips[i].data.equipItems.Length; j++)
                {
                    if (playerModuleEquips[i].data.equipItems[j].module.GetUpgradeModuleObject().key == module.key)
                    {
                        playerModuleEquips[i].moduleEquipStats[j].ClearStat();
                        return;
                    }
                }
            }
        }
    }

    public bool SelectModuleUpgradeSlot(ModuleItem item)
    {

        if (moduleUpgrade2 != null)
        {
            if (moduleUpgrade2.key == item.module.key)
            {
                item.UnselectSlot();

                var transparentImage = Resources.Load<Sprite>("UI/transparent");

                moduleUpgrade2 = null;
                moduleUpgrade2Image.sprite = transparentImage;
                moduleUpgrade2Cover.enabled = false;

                fillSelectedSlot();

                return false;
            }
        }

        if (moduleUpgrade3 != null)
        {
            if (moduleUpgrade3.key == item.module.key)
            {
                item.UnselectSlot();

                var transparentImage = Resources.Load<Sprite>("UI/transparent");

                moduleUpgrade3 = null;
                moduleUpgrade3Image.sprite = transparentImage;
                moduleUpgrade3Cover.enabled = false;

                fillSelectedSlot();

                return false;
            }
        }

        if (moduleUpgrade2 == null)
        {
            moduleUpgrade2 = item.module.GetUpgradeModuleObject();

            // var scriptableObjects = Resources.LoadAll<UpgradeModuleScripableObject>("UpgradeModules").ToList();
            // var find = scriptableObjects.Find(f => f.ID == (int)item.module.GetUpgradeModuleObject().module);
            // moduleUpgrade2Image.sprite = find?.moduleIcon;

            moduleUpgrade2Cover.sprite = GetUpgradeModuleCover(item.module.tier);
            moduleUpgrade2Image.sprite = GetUpgradeModuleImage((int)item.module.module);

            moduleUpgrade2Cover.enabled = true;

            ModuleItem.SetModuleImageColor(moduleUpgrade2Image, item.module.type);

            fillSelectedSlot();
            return true;
        }

        if (moduleUpgrade3 == null)
        {
            moduleUpgrade3 = item.module.GetUpgradeModuleObject();

            // var scriptableObjects = Resources.LoadAll<UpgradeModuleScripableObject>("UpgradeModules").ToList();
            // var find = scriptableObjects.Find(f => f.ID == (int)item.module.GetUpgradeModuleObject().module);
            // moduleUpgrade3Image.sprite = find?.moduleIcon;

            moduleUpgrade3Cover.sprite = GetUpgradeModuleCover(item.module.tier);
            moduleUpgrade3Image.sprite = GetUpgradeModuleImage((int)item.module.module);

            moduleUpgrade3Cover.enabled = true;

            ModuleItem.SetModuleImageColor(moduleUpgrade3Image, item.module.type);

            fillSelectedSlot();
            return true;
        }

        void fillSelectedSlot()
        {
            if (moduleUpgrade2 != null && moduleUpgrade3 != null)
            {
                moduleUpgradeBtn.interactable = true;
                moduleUpgradeBtn.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                moduleUpgradeBtn.interactable = false;
                moduleUpgradeBtn.GetComponentInChildren<TextMeshProUGUI>().color = new Color(1f, 1f, 1f, 65f / 255f);
            }
        }
        return false;
    }

    public Sprite GetUpgradeModuleCover(UpgradeModuleTier tier)
    {
        switch (tier)
        {
            case UpgradeModuleTier.Normal:
                return Resources.Load<Sprite>("UI/slot1");

            case UpgradeModuleTier.Magic:

                return Resources.Load<Sprite>("UI/slot2");
            case UpgradeModuleTier.Rare:

                return Resources.Load<Sprite>("UI/slot3");
            case UpgradeModuleTier.Unique:

                return Resources.Load<Sprite>("UI/slot4");
            case UpgradeModuleTier.Legendary:

                return Resources.Load<Sprite>("UI/slot5");

            default:
                UnityEngine.Debug.LogError("해당되는 티어 커버 이미지가 없습니다.");
                return null;
        }
    }

    public Sprite GetUpgradeModuleImage(int ID)
    {
        var scriptableObjects = Resources.LoadAll<UpgradeModuleScripableObject>("UpgradeModules").ToList();
        var find = scriptableObjects.Find(f => f.ID == ID);
        return find?.moduleIcon;
    }

    public void OpenModulePurchaseUI()
    {
        var UI = Resources.Load<GameObject>("UI/ModulePurchaseMenu");
        Instantiate(UI, GameManager.instance.MainUIParent);

        EventManager.instance.CustomEvent(AnalyticsType.UI, "UI_ModulePurchaseUI");
    }

    public void MergeAll()
    {
        EventManager.instance.CustomEvent(AnalyticsType.UI, "UI_OnClickMergeAllBtn");

        List<UpgradeModuleObject> completeMergeModuleList = new List<UpgradeModuleObject>();

        UpgradeModuleObject module1 = null, module2 = null, module3 = null;
        for (int i = 0; i < UserDataManager.instance.currentUserData.moduleInventory.Count; i++)
        {
            var nextModule = UserDataManager.instance.currentUserData.moduleInventory[i];

            if (nextModule.tier == UpgradeModuleTier.Legendary)
                continue;

            var result = UserDataManager.instance.currentUserData.moduleInventory
            .Where(f => f != nextModule && f.type == nextModule.type && f.tier == nextModule.tier).ToList();


            if (result.Count >= 2)
            {
                var merged = GenerateRandomModule((int)nextModule.tier + 1, (int)nextModule.module);

                completeMergeModuleList.Add(merged);

                print(nextModule.type);

                DeleteModule(nextModule);
                DeleteModule(result[0]);
                DeleteModule(result[1]);

                GetNewModule(merged);

                merged.GetUpgradeModuleObject().isNew = true;

                i = 0;

                print(merged.type);

                EventManager.instance.CustomEvent(AnalyticsType.GAME, "Module_MergeAll");
            }

            // var nextModule = UserDataManager.instance.currentUserData.moduleInventory[i];

            // if (!completeMergeModuleList.Contains(nextModule))
            // {
            //     // module1 = nextModule;
            //     // if (MergeNext() != null)
            //     //     i = 0;

            //     if (module1 == null)
            //     {
            //         module1 = nextModule;


            //     }
            //     else if (module2 == null)
            //     {
            //         if (module1.type == nextModule.type && module1.tier == nextModule.tier && module1 != nextModule)
            //         {
            //             module2 = nextModule;
            //         }
            //     }
            //     else if (module3 == null)
            //     {
            //         if (module1.type == nextModule.type && module1.tier == nextModule.tier && module1 != nextModule && module2 != nextModule)
            //         {
            //             module3 = nextModule;

            //             var result = GenerateRandomModule((int)module1.tier, (int)module1.type);

            //             completeMergeModuleList.Add(result);

            //             DeleteModule(module1);
            //             DeleteModule(module2);
            //             DeleteModule(module3);

            //             GetNewModule(result);

            //             module1 = null;
            //             module2 = null;
            //             module3 = null;

            //             i = 0;
            //         }
            //     }
            // }
        }

        if (completeMergeModuleList.Count > 0)
        {

        }

        UpgradeModuleObject MergeNext()
        {
            for (int i = 0; i < UserDataManager.instance.currentUserData.moduleInventory.Count; i++)
            {
                var nextModule = UserDataManager.instance.currentUserData.moduleInventory[i];

                if (!completeMergeModuleList.Contains(nextModule))
                {
                    if (module2 == null)
                    {
                        if (module1.type == nextModule.type && module1.tier == nextModule.tier && module1 != nextModule)
                        {
                            module2 = nextModule;
                        }
                    }
                    else if (module3 == null)
                    {
                        if (module1.type == nextModule.type && module1.tier == nextModule.tier && module1 != nextModule && module2 != nextModule)
                        {
                            module3 = nextModule;

                            var result = GenerateRandomModule((int)module1.tier, (int)module1.type);

                            completeMergeModuleList.Add(result);

                            DeleteModule(module1);
                            DeleteModule(module2);
                            DeleteModule(module3);

                            GetNewModule(result);

                            module1 = null;
                            module2 = null;
                            module3 = null;
                            return result;
                        }
                    }
                }
            }
            return null;
        }

        UpdateUI();
    }
}
