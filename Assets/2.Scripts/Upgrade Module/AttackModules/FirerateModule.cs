using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

[CreateAssetMenu(fileName = "new Module", menuName = "Scriptable Object/Upgrade Module/Firerate", order = int.MaxValue)]
public class FirerateModule : UpgradeModuleScripableObject
{
    public static readonly float[] firerateValue = { 0.05f, 0.08f, 0.11f, 0.14f, 0.17f };

    public override void ApplyUpgradeModule(PlayerUpgradeModule player, int tier)
    {
        player.module_Firerate.AddFloatModifier(-firerateValue[(int)tier]);

        MonoBehaviour.print("발사속도 증가 스탯 적용");
    }

    public override void UnapplyUpgradeModule(PlayerUpgradeModule player, int tier)
    {
        MonoBehaviour.print("발사속도 증가 스탯 적용해제");
    }

    public override void GetLocalizedModuleStatText(TextMeshProUGUI text, int tier)
    {
        CoroutineHelper.StartCoroutine(ChangeShipNameText(text));

        IEnumerator ChangeShipNameText(TextMeshProUGUI text)
        {
            var keyName = (int)module;

            var localizedString = new LocalizedString("Module", keyName.ToString());

            var stringOperation = localizedString.GetLocalizedStringAsync();

            while (true)
            {
                if (stringOperation.IsDone && stringOperation.Status == AsyncOperationStatus.Succeeded)
                {
                    string str = stringOperation.Result;

                    str = str.Replace("[ST_VALUE1]", firerateValue[tier].ToString());

                    text.text = str;

                    break;
                }
                yield return null;
            }
        }
    }

    public override string[] GetModuleStats(int tier)
    {
        string[] value = { firerateValue[tier].ToString() };
        return value;
    }
}
