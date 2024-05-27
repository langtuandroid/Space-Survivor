using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using Sirenix.OdinInspector;

public class LevelUpManager : MonoBehaviour
{
    [FoldoutGroup("참조")][SerializeField] private GameObject UpgradeUI;
    [FoldoutGroup("참조")][SerializeField] private Transform UpgradeSlotParent;
    [FoldoutGroup("참조")][SerializeField] private GameObject upgradeSlotCover;

    [Space]

    [FoldoutGroup("참조")][SerializeField] private Transform weaponSlotParent;
    [FoldoutGroup("참조")][SerializeField] private Transform passiveSlotParent;

    [FoldoutGroup("참조")][SerializeField] private GameObject equipmentSlot;

    [Space]

    [FoldoutGroup("참조")][SerializeField] private PlayerWeapon playerWeapon;
    [FoldoutGroup("참조")] public PlayerStat playerStat;

    [FoldoutGroup("참조")][SerializeField] private GameObject weaponUpgradePanelPrefab;
    [FoldoutGroup("참조")][SerializeField] private GameObject passiveUpgradePanelPrefab;
    [FoldoutGroup("참조")][SerializeField] private GameObject crystalPanelPrefab;


    [Space]

    [SerializeField] public static readonly int maxWeaponCount = 6;
    [SerializeField] public static readonly int maxPassiveCount = 4;

    [Space]

    [SerializeField] private List<WeaponObject> obtainableWeapons = new List<WeaponObject>();
    [SerializeField] private List<PassiveObject> obtainablePassives = new List<PassiveObject>();


    // private List<Equipment> obtainableEquipment = new List<Equipment>();

    private List<IEquipment> currentObtainableList = new List<IEquipment>();
    private List<GameObject> currentUpgradePanel = new List<GameObject>();
    private List<GameObject> crystalPanel = new List<GameObject>();

    private List<IEquipment> currentUpgradeModule = new List<IEquipment>();
    public static int getAllUpgradeCount { get; set; } = 0;
    [SerializeField] private int max_getAllUpgradeCount = 3;
    [SerializeField] private GameObject getAllBtn;


    public static LevelUpManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void StartWeaponUpgrade()
    {
        MakeWeaponsList();

        UpgradeUI.SetActive(true);

        UpgradeSlotParent.DOScale(new Vector3(1, 1, 1), 0.2f).SetEase(Ease.Linear).OnComplete(() => upgradeSlotCover.SetActive(false)).SetUpdate(true);

        Time.timeScale = 0;
    }

    public void EndUpgrade()
    {
        UpgradeUI.SetActive(false);

        for (int i = 0; i < currentUpgradePanel.Count; i++)
        {
            Destroy(currentUpgradePanel[i]);
        }

        for (int i = 0; i < crystalPanel.Count; i++)
        {
            Destroy(crystalPanel[i]);
        }

        crystalPanel.Clear();

        currentUpgradePanel.Clear();
        currentObtainableList.Clear();

        Time.timeScale = 1;

        UpgradeSlotParent.localScale = new Vector3(0, 0, 0);
        upgradeSlotCover.SetActive(true);

        playerStat.whileLevelUp = false;
    }

    private void MakeWeaponsList()
    {
        // 모든 업그레이드 획득 RV 버튼 활성화 여부
        getAllUpgradeCount--;
        if (getAllUpgradeCount <= 0)
            getAllBtn.SetActive(true);
        else
            getAllBtn.SetActive(false);

        //ȹ�� ������ ����� �߰�
        currentObtainableList.AddRange(obtainableWeapons);
        //ȹ�� ������ �нú�� �߰�
        currentObtainableList.AddRange(obtainablePassives);

        var maxWeapons = playerWeapon.GetMaxLevelEquipments();

        //�̹� ������ ���� ����Ʈ���� ����
        for (int i = 0; i < maxWeapons.Count; i++)
        {
            for (int j = 0; j < currentObtainableList.Count; j++)
            {
                if (currentObtainableList[j].GetEquipmentType() == maxWeapons[i].GetEquipmentType())
                    currentObtainableList.Remove(currentObtainableList[j]);
            }
        }

        //���� ���� ������ ������ ���ο� ���Ⱑ �������� �ʰ� ����Ʈ���� ����
        if (playerWeapon.weaponPool.Count >= maxWeaponCount)
        {
            for (int i = 0; i < currentObtainableList.Count; i++)
            {
                if (!playerWeapon.GetIsWeaponHave(currentObtainableList[i].GetEquipmentType()) && currentObtainableList[i].GetAnyEqupment() == AnyEqupment.Weapon)
                {
                    //Debug.LogError(currentObtainableList[i].type);
                    currentObtainableList.Remove(currentObtainableList[i]);
                    i--;
                }
            }
        }

        //���� �нú� ������ ������ ���ο� �нú갡 �������� �ʰ� ����Ʈ���� ����
        if (playerWeapon.passivePool.Count >= maxPassiveCount)
        {
            for (int i = 0; i < currentObtainableList.Count; i++)
            {
                if (!playerWeapon.GetIsPassiveHave(currentObtainableList[i].GetEquipmentType()) && currentObtainableList[i].GetAnyEqupment() == AnyEqupment.Passive)
                {
                    print(currentObtainableList[i]);
                    //Debug.LogError(currentObtainableList[i].type);
                    currentObtainableList.Remove(currentObtainableList[i]);
                    i--;
                }

            }
        }

        currentUpgradeModule.Clear();

        //���ο� ���׷��̵� ���� ����
        for (int i = 0; i < 3; i++)
        {
            if (currentObtainableList.Count <= 0)
            {
                var crystal = Instantiate(crystalPanelPrefab, UpgradeSlotParent);
                crystalPanel.Add(crystal);
                continue;
            }

            GameObject panel = null;
            var randomWeaponObject = currentObtainableList[Random.Range(0, currentObtainableList.Count)];

            switch (randomWeaponObject.GetAnyEqupment())
            {
                case AnyEqupment.Weapon:
                    panel = Instantiate(weaponUpgradePanelPrefab, UpgradeSlotParent);
                    break;

                case AnyEqupment.Passive:
                    panel = Instantiate(passiveUpgradePanelPrefab, UpgradeSlotParent);
                    break;
            }

            panel.GetComponent<UpgradeSlot>().InitSlot(randomWeaponObject);
            StartCoroutine(panel.GetComponent<UpgradeSlot>().GetLocalizedWeaponTextAsynce());
            currentObtainableList.Remove(randomWeaponObject);
            currentUpgradePanel.Add(panel);

            currentUpgradeModule.Add(randomWeaponObject);
        }

    }

    public void SelectUpgrade(IEquipment equipment)
    {
        playerWeapon.UpgradeWeapon(equipment);

        playerStat.AfterUpgrade();
        EndUpgrade();
    }

    public WeaponObject RequestPlayerWeapon(EquipmentType type)
    {
        return playerWeapon.GetCurrentWeapon(type);
    }

    public int RequestWeaponLevel(EquipmentType type)
    {
        return playerWeapon.GetWeaponLevel(type);
    }

    public int RequestMaxWeaponLevel(EquipmentType type)
    {
        return playerWeapon.GetMaxWeaponLevel(type);
    }

    public void AddNewWeaponImage(IEquipment equipmentObject, List<EquipmentSlot> slotList, IPassiveEquipment passiveEquipment = null)
    {
        Transform slotParent = null;

        switch (equipmentObject.GetAnyEqupment())
        {
            case AnyEqupment.Weapon:
                slotParent = weaponSlotParent;
                break;

            case AnyEqupment.Passive:
                slotParent = passiveSlotParent;
                break;

        }

        var slot = Instantiate(equipmentSlot, slotParent);

        slotList.Add(slot.GetComponent<EquipmentSlot>());

        slot.GetComponent<EquipmentSlot>().SetEquipment(equipmentObject);

        //print(passiveEquipment);

        if (passiveEquipment != null)
            passiveEquipment.GetEquipmentSlot(slot.GetComponent<EquipmentSlot>());
    }

    public void AddUpgradeNode(IEquipment weaponObject)
    {
        var weaponSlots = weaponSlotParent.GetComponentsInChildren<EquipmentSlot>();
        var PassiveSlots = passiveSlotParent.GetComponentsInChildren<EquipmentSlot>();

        switch (weaponObject.GetAnyEqupment())
        {
            case AnyEqupment.Weapon:
                for (int i = 0; i < weaponSlots.Length; i++)
                {
                    if (weaponSlots[i].weaponObject.GetEquipmentType() == weaponObject.GetEquipmentType())
                    {
                        weaponSlots[i].AddUpgradeNode();
                    }
                }
                break;

            case AnyEqupment.Passive:
                for (int i = 0; i < PassiveSlots.Length; i++)
                {
                    if (PassiveSlots[i].weaponObject.GetEquipmentType() == weaponObject.GetEquipmentType())
                    {
                        PassiveSlots[i].AddUpgradeNode();
                    }
                }
                break;
        }
    }

    public void ClearWeaponImages()
    {
        Transform[] childList = gameObject.GetComponentsInChildren<Transform>();

        if (childList != null)
        {
            for (int i = 1; i < childList.Length; i++)
            {
                if (childList[i] != transform)
                    Destroy(childList[i].gameObject);
            }
        }
    }

    /// <summary>
    /// 생성된 모든 업그레이드 모듈 획득하기 RV에 사용
    /// </summary>
    public void GetAllCurrentUpgrade()
    {
        getAllUpgradeCount = max_getAllUpgradeCount;

        for (int i = 0; i < currentUpgradeModule.Count; i++)
        {
            playerWeapon.UpgradeWeapon(currentUpgradeModule[i]);
        }

        for (int i = 0; i < crystalPanel.Count; i++)
        {
            LevelUpManager.instance.playerStat.GetCrystal(crystalPanel[i].GetComponent<CrystalSlot>().crystalValue);
        }

        playerStat.AfterUpgrade();
        EndUpgrade();
    }

}
