using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LevelUpManager : MonoBehaviour
{
    [SerializeField] private GameObject UpgradeUI;
    [SerializeField] private Transform UpgradeSlotParent;

    [SerializeField] private PlayerWeapon playerWeapon;
    [SerializeField] private GameObject upgradePanelPrefab;

    [Space]

    [SerializeField] private List<WeaponObject> obtainableWeapons = new List<WeaponObject>();

    private List<WeaponObject> currentObtainableList = new List<WeaponObject>();
    private List<GameObject> currentUpgradePanel = new List<GameObject>();

    public static LevelUpManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void StartWeaponUpgrade()
    {
        MakeWeaponsList();

        UpgradeUI.SetActive(true);

        Time.timeScale = 0;
    }

    public void EndUpgrade()
    {
        UpgradeUI.SetActive(false);

        for(int i = 0; i < currentUpgradePanel.Count; i++)
        {
            Destroy(currentUpgradePanel[i]);
        }

        currentUpgradePanel.Clear();
        currentObtainableList.Clear();

        Time.timeScale = 1;
    }

    private void MakeWeaponsList()
    {
        //ȹ�� ������ ����� �߰�
        currentObtainableList.AddRange(obtainableWeapons);                  

        var maxWeapons = playerWeapon.GetMaxLevelWeaponList();

        //�̹� ������ ���� ����Ʈ���� ����
        for (int i = 0; i < maxWeapons.Count; i++)                           
        {
            for(int j = 0; j < currentObtainableList.Count; j++)
            {
                if (currentObtainableList[j].type == maxWeapons[i].type)
                    currentObtainableList.Remove(currentObtainableList[j]);
            }
        }

        //���ο� ���׷��̵� ���� ����
        for (int i = 0; i < 3; i++)
        {
            if (currentObtainableList.Count <= 0)
                return;

            var panel = Instantiate(upgradePanelPrefab, UpgradeSlotParent);
            var randomWeaponObject = currentObtainableList[Random.Range(0, currentObtainableList.Count)];
            panel.GetComponent<UpgradeSlot>().InitSlot(randomWeaponObject);
            currentObtainableList.Remove(randomWeaponObject);
            currentUpgradePanel.Add(panel);
        }
        
    }

    public void SelectUpgrade(WeaponObject weaponObject)
    {
        playerWeapon.UpgradeWeapon(weaponObject);

        EndUpgrade();
    }

    public WeaponObject RequestPlayerWeapon(WeaponType type)
    {
        return playerWeapon.GetCurrentWeapon(type);
    }

    public int RequestWeaponLevel(WeaponType type)
    {
        return playerWeapon.GetWeaponLevel(type);
    }

    public int RequestMaxWeaponLevel(WeaponType type)
    {
        return playerWeapon.GetMaxWeaponLevel(type);
    }
}
