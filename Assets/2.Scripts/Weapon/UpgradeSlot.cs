using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using UnityEngine.UI;
using DG.Tweening;

using UnityEngine.ResourceManagement.AsyncOperations;

public class UpgradeSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moduleName;
    [SerializeField] private TextMeshProUGUI moduleDetail;
    [SerializeField] private TextMeshProUGUI moduleLevel;
    [SerializeField] private Image weaponImage;
    [SerializeField] private Transform upgradeNodeSlotparent;
    [SerializeField] private GameObject upgradeNodePrefab;

    [Space]

    public IEquipment weaponObject;

    public IEquipment equipment;

    public void InitSlot(IEquipment weaponObject)
    {
        this.weaponObject = weaponObject;

        moduleName.text = weaponObject.GetEquipmentType().ToString();


        for (int i = 0; i < LevelUpManager.instance.RequestWeaponLevel(weaponObject.GetEquipmentType()) - 1; i++)
        {
            Instantiate(upgradeNodePrefab, upgradeNodeSlotparent);
        }

        if (LevelUpManager.instance.RequestWeaponLevel(weaponObject.GetEquipmentType()) > 0)
        {
            var expectNode = Instantiate(upgradeNodePrefab, upgradeNodeSlotparent);

            expectNode.GetComponent<Image>().DOFade(0, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
        }


        //sequence = DOTween.Sequence();
        //sequence.Append(expectNode.GetComponent<Image>().DOFade(1.0f, 0.2f)).SetUpdate(true);
        //sequence.Append(expectNode.GetComponent<Image>().DOFade(0.0f, 1.5f)).SetUpdate(true);
        //sequence.Play();

        //���ο� ������ "���ο� ����!" �� ǥ�õǰ� �������� ����� "���緹�� / �ִ뷡��" �� ��µǵ���
        /*
        if(LevelUpManager.instance.RequestPlayerWeapon(weaponObject.type) == null)
        {
            moduleLevel.text = "���ο� ����!";
        }
        else
        {
            moduleLevel.text = LevelUpManager.instance.RequestWeaponLevel(weaponObject.type).ToString() + " / " + LevelUpManager.instance.RequestMaxWeaponLevel(weaponObject.type).ToString();
        }*/

        /*
        var keyName1 = weaponObject.type.ToString();

        var localizedString1 = new LocalizedString("Weapons", keyName1);

        //var stringOperation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Weapons", keyName);
        var stringOperation1 = localizedString1.GetLocalizedStringAsync();
        if (stringOperation1.IsDone && stringOperation1.Status == AsyncOperationStatus.Succeeded)
        {
            string str = stringOperation1.Result;
            moduleName.text = str;
        }*/

        //StartCoroutine(GetLocalizedWeaponNameAsynce());



        //���� �̹����� ����
        weaponImage.sprite = weaponObject.GetEquipmentIamge();


    }

    public IEnumerator GetLocalizedWeaponTextAsynce()
    {
        var keyName1 = weaponObject.GetEquipmentType().ToString();

        var localizedString1 = new LocalizedString("Weapons", keyName1);

        var stringOperation1 = localizedString1.GetLocalizedStringAsync();
        while (true)
        {
            if (stringOperation1.IsDone && stringOperation1.Status == AsyncOperationStatus.Succeeded)
            {
                string str = stringOperation1.Result;
                moduleName.text = str;

                break;
            }
            yield return null;
        }

        int weaponLevel = LevelUpManager.instance.RequestWeaponLevel(weaponObject.GetEquipmentType());

        //���� �������� ���� ������ 0�Ͻ� �������� ������ ��µǵ���
        if (weaponLevel == 0)
        {
            var keyName = weaponObject.GetEquipmentType().ToString() + "_D";

            var localizedString = new LocalizedString("Weapons", keyName);

            //var stringOperation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Weapons", keyName);
            var stringOperation = localizedString.GetLocalizedStringAsync();

            while (true)
            {
                if (stringOperation.IsDone && stringOperation.Status == AsyncOperationStatus.Succeeded)
                {
                    string str = stringOperation.Result;
                    moduleDetail.text = str;

                    break;
                }
                yield return null;
            }
        }
        else  //�������� ������ 1 �̻��̸� ���� ���׷��̵� ������ ��µǵ���
        {
            for (int i = 0; i < weaponObject.GetUpgradeModuleLists()[weaponLevel - 1].upgradeModules.Count; i++)
            {
                var keyName = weaponObject.GetUpgradeModuleLists()[weaponLevel - 1].upgradeModules[i].upgradeModuleType.ToString();

                List<object> arguments = new List<object>();

                arguments.Add(weaponObject.GetUpgradeModuleLists()[weaponLevel - 1].upgradeModules[i].value1);

                var localizedString = new LocalizedString("Upgrades", keyName);
                var dict = new Dictionary<string, string> { { "VALUE1", weaponObject.GetUpgradeModuleLists()[weaponLevel - 1].GetValueHumanReadableValue1(i) }, { "VALUE2", weaponObject.GetUpgradeModuleLists()[weaponLevel - 1].GetValueHumanReadableValue2(i) }, { "VALUE3", weaponObject.GetUpgradeModuleLists()[weaponLevel - 1].GetValueHumanReadableValue3(i) } };
                localizedString.Arguments = new object[] { dict };

                //var stringOperation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Weapons", keyName);
                var stringOperation = localizedString.GetLocalizedStringAsync();

                while (true)
                {
                    if (stringOperation.IsDone && stringOperation.Status == AsyncOperationStatus.Succeeded)
                    {
                        string str = stringOperation.Result;

                        if (i < weaponObject.GetUpgradeModuleLists()[weaponLevel - 1].upgradeModules.Count - 1)
                            str = str + ",";

                        moduleDetail.text = moduleDetail.text + str + "\n";

                        break;
                    }
                    yield return null;
                }

                //LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Upgrades").Completed += result => moduleDetail.text = result.DebugName;
                //moduleDetail.text + 

            }
        }

    }

    public void SelectUpgrade()
    {
        LevelUpManager.instance.SelectUpgrade(weaponObject);

        AudioManager.instance.PlaySFX("upgrade1");

        switch (weaponObject.GetAnyEqupment())
        {
            case AnyEqupment.Weapon:
                EventManager.instance.CustomEvent(AnalyticsType.GAME, "SelectWeaponUpgrade - " + weaponObject.GetWeaponObject().GetEquipmentType().ToString());
                break;

            case AnyEqupment.Passive:
                EventManager.instance.CustomEvent(AnalyticsType.GAME, "SelectPassiveUpgrade - " + weaponObject.GetPassive().GetEquipmentType().ToString());
                break;
        }


    }
}
