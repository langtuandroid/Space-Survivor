using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using UnityEngine.UI;

using UnityEngine.ResourceManagement.AsyncOperations;

public class UpgradeSlot : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI moduleName;
    [SerializeField] private TextMeshProUGUI moduleDetail;
    [SerializeField] private TextMeshProUGUI moduleLevel;
    [SerializeField] private Image weaponImage;

    [Space]

    public WeaponObject weaponObject;

    public void InitSlot(WeaponObject weaponObject)
    {
        this.weaponObject = weaponObject;

        moduleName.text = weaponObject.type.ToString();

        //���ο� ������ "���ο� ����!" �� ǥ�õǰ� �������� ����� "���緹�� / �ִ뷡��" �� ��µǵ���
        if(LevelUpManager.instance.RequestPlayerWeapon(weaponObject.type) == null)
        {
            moduleLevel.text = "���ο� ����!";
        }
        else
        {
            moduleLevel.text = LevelUpManager.instance.RequestWeaponLevel(weaponObject.type).ToString() + "/" + LevelUpManager.instance.RequestMaxWeaponLevel(weaponObject.type).ToString();
        }

        var keyName1 = weaponObject.type.ToString();

        var localizedString1 = new LocalizedString("Weapons", keyName1);

        //var stringOperation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Weapons", keyName);
        var stringOperation1 = localizedString1.GetLocalizedStringAsync();
        if (stringOperation1.IsDone && stringOperation1.Status == AsyncOperationStatus.Succeeded)
        {
            string str = stringOperation1.Result;
            moduleName.text = str;
        }

        int weaponLevel = LevelUpManager.instance.RequestWeaponLevel(weaponObject.type);

        //���� �������� ���� ������ 0�Ͻ� �������� ������ ��µǵ���
        if(weaponLevel == 0)
        {
            var keyName = weaponObject.type.ToString() + "_D";

            var localizedString = new LocalizedString("Weapons", keyName);

            //var stringOperation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Weapons", keyName);
            var stringOperation = localizedString.GetLocalizedStringAsync();
            if (stringOperation.IsDone && stringOperation.Status == AsyncOperationStatus.Succeeded)
            {
                string str = stringOperation.Result;
                moduleDetail.text = str;
            }
        }
        else  //�������� ������ 1 �̻��̸� ���� ���׷��̵� ������ ��µǵ���
        {
            for (int i = 0; i < weaponObject.UpgradeModulesForLevel[weaponLevel-1].upgradeModules.Count; i++)
            {
                var keyName = weaponObject.UpgradeModulesForLevel[weaponLevel-1].upgradeModules[i].upgradeModuleType.ToString();

                List<object> arguments = new List<object>();

                arguments.Add(weaponObject.UpgradeModulesForLevel[weaponLevel-1].upgradeModules[i].value1);

                var localizedString = new LocalizedString("Upgrades", keyName);
                var dict = new Dictionary<string, string> { { "VALUE1", weaponObject.UpgradeModulesForLevel[weaponLevel-1].GetValueHumanReadableValue(i) } };
                localizedString.Arguments = new object[] { dict };

                //var stringOperation = LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Weapons", keyName);
                var stringOperation = localizedString.GetLocalizedStringAsync();
                if (stringOperation.IsDone && stringOperation.Status == AsyncOperationStatus.Succeeded)
                {
                    string str = stringOperation.Result;
                    moduleDetail.text = moduleDetail.text + str + "\n";
                }

                //LocalizationSettings.StringDatabase.GetLocalizedStringAsync("Upgrades").Completed += result => moduleDetail.text = result.DebugName;
                //moduleDetail.text + 

            }
        }

        //���� �̹����� ����
        weaponImage.sprite = weaponObject.weaponImage;

        
    }

    public void SelectUpgrade()
    {
        LevelUpManager.instance.SelectUpgrade(weaponObject);
    }
}
