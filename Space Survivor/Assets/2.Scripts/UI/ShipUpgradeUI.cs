using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using DG.Tweening;

public class ShipUpgradeUI : MonoBehaviour
{
    public ShipObject currentShipObject;

    [Space]

    public Image shipImage;
    public TextMeshProUGUI shipName;

    [Space]

    public TextMeshProUGUI hpStat;
    public TextMeshProUGUI damageStat;
    public TextMeshProUGUI speedStat;
    public GameObject shipUnlockButton;
    public TextMeshProUGUI shopCostText;

    [Space]

    public ShipUpgradeSlot[] shipUpgradeSlot;

    [Space]

    public GameObject upgradeNode;
    public GameObject shipUpgradeSelectSlot;
    public Transform shipUpgradeSelectSlotParent;
    public ShipList shipList;

    private void OnEnable()
    {
        if (currentShipObject == null)
            currentShipObject = GameManager.instance.currentShip;

        SelectShip(currentShipObject);
    }

    private void Start()
    {
        for (int i = 0; i < shipList.shipList.Count; i++)
        {
            var slot = Instantiate(shipUpgradeSelectSlot, shipUpgradeSelectSlotParent);
            slot.GetComponent<ShipUpgradeSelectSlot>().InitShip(Instantiate(shipList.shipList[i]), this);
        }
    }

    private void Update()
    {
        shipImage.transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * 20);
    }

    public void SelectShip(ShipObject shipObject)
    {
        currentShipObject = shipObject;

        ShipObjectData data = UserDataManager.instance.GetShipData(shipObject.shipCode);

        shipImage.sprite = UserDataManager.instance.GetShipImage(data.shipCode); //data.shipImage;

        //hpStat.text = data.baseMaxHp.GetBaseValue().ToString();
        //damageStat.text = data.baseDamage.GetBaseValue().ToString();
        //speedStat.text = data.baseMoveSpeed.GetBaseValue().ToString();

        SetUpgradeModuleSlot(data, shipObject);

        if (UserDataManager.instance.CheckPlayerHaveShip(data.shipCode))
        {
            shipUnlockButton.SetActive(false);
        }
        else
        {
            shipUnlockButton.SetActive(true);
        }

        shopCostText.text = data.shipCost.ToString();


        if (UserDataManager.instance.currentUserData.crystal >= currentShipObject.shipCost)
            shipUnlockButton.GetComponent<Button>().interactable = true;
        else
            shipUnlockButton.GetComponent<Button>().interactable = false;

        StartCoroutine(ChangeShipNameText());

        IEnumerator ChangeShipNameText()
        {
            var keyName = data.shipCode;

            var localizedString = new LocalizedString("Ship", keyName);

            var stringOperation = localizedString.GetLocalizedStringAsync();

            while (true)
            {
                if (stringOperation.IsDone && stringOperation.Status == AsyncOperationStatus.Succeeded)
                {
                    string str = stringOperation.Result;
                    shipName.text = str;

                    break;
                }
                yield return null;
            }
        }
    }

    public void SetUpgradeModuleSlot(ShipObjectData data, ShipObject shipObject)
    {
        for (int i = 0; i < data.shipUpgradeModuleList.Count; i++)
        {
            switch (data.shipUpgradeModuleList[i].upgradeType)
            {
                case ShipUpgradeType.Health:
                    hpStat.text = data.baseMaxHp.GetBaseValue().ToString() + "(+" + (data.shipUpgradeModuleList[i].currentUpgrade * data.shipUpgradeModuleList[i].statUpgradeValueForLevel).ToString() + ")";
                    break;

                case ShipUpgradeType.Damage:
                    damageStat.text = data.baseDamage.GetBaseValue().ToString() + "(+" + (data.shipUpgradeModuleList[i].currentUpgrade * data.shipUpgradeModuleList[i].statUpgradeValueForLevel).ToString() + ")";
                    break;

                case ShipUpgradeType.Speed:
                    speedStat.text = data.baseMoveSpeed.GetBaseValue().ToString() + "(+" + (data.shipUpgradeModuleList[i].currentUpgrade * data.shipUpgradeModuleList[i].statUpgradeValueForLevel).ToString() + ")";
                    break;
            }

            SetUpgradeSlots(data.shipUpgradeModuleList[i], shipObject);

        }
    }

    private void SetUpgradeSlots(ShipUpgradeModules modules, ShipObject currentShip)
    {

        for (int i = 0; i < shipUpgradeSlot.Length; i++)
        {
            if (shipUpgradeSlot[i].upgradeType == modules.upgradeType)
            {
                shipUpgradeSlot[i].shipObject = currentShip;

                int upgradeCost = (modules.currentUpgrade + 1) * modules.upgradeCostForLevel;
                shipUpgradeSlot[i].upgradeCostText.text = upgradeCost.ToString();

                if (UserDataManager.instance.currentUserData.crystal >= upgradeCost && UserDataManager.instance.CheckPlayerHaveShip(currentShip.shipCode)
                && UserDataManager.instance.currentUserData.crystal >= upgradeCost && modules.currentUpgrade < modules.maxUpgrade)
                {
                    shipUpgradeSlot[i].upgradeButton.SetActive(true);
                }
                else
                {
                    shipUpgradeSlot[i].upgradeButton.SetActive(false);
                }

                if (modules.currentUpgrade < modules.maxUpgrade)
                {
                    shipUpgradeSlot[i].costPanel.SetActive(true);
                    shipUpgradeSlot[i].maxPanel.SetActive(false);
                }
                else
                {
                    shipUpgradeSlot[i].costPanel.SetActive(false);
                    shipUpgradeSlot[i].maxPanel.SetActive(true);
                }

                for (int z = 0; z < shipUpgradeSlot[i].upgradeNodeList.Count; z++)
                {
                    Destroy(shipUpgradeSlot[i].upgradeNodeList[z]);
                }
                shipUpgradeSlot[i].upgradeNodeList.Clear();

                for (int z = 0; z < modules.currentUpgrade; z++)
                {
                    shipUpgradeSlot[i].upgradeNodeList.Add(Instantiate(upgradeNode, shipUpgradeSlot[i].upgradeNodeParent));
                }
            }
        }
    }

    public void BuyShip()
    {
        if (currentShipObject.shipCost > UserDataManager.instance.currentUserData.crystal
        || UserDataManager.instance.CheckPlayerHaveShip(currentShipObject.shipCode))
            return;

        UserDataManager.instance.currentUserData.playerHaveShip.Add(currentShipObject.shipObjectData);

        SelectShip(currentShipObject);

        UserDataManager.instance.AddCrystalValue(-currentShipObject.shipCost);
    }
}