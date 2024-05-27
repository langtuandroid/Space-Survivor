using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

[CreateAssetMenu(fileName = "new Module", menuName = "Scriptable Object/Upgrade Module/Magnet Range Up Module", order = int.MaxValue)]
public class MagnetRangeUpModule : UpgradeModuleScripableObject
{
    public static readonly float[] magnetRangeUpValue = { 3, 5, 10, 15, 25 };

    public override void ApplyUpgradeModule(PlayerUpgradeModule player, int tier)
    {
        player.module_MagnetRange.AddFloatModifier(magnetRangeUpValue[(int)tier]);

        MonoBehaviour.print("자원 획득 범위 스탯 적용");
    }

    public override void UnapplyUpgradeModule(PlayerUpgradeModule player, int tier)
    {
        MonoBehaviour.print("자원 획득 범위 스탯 적용해제");
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

                    str = str.Replace("[ST_VALUE1]", magnetRangeUpValue[tier].ToString());

                    text.text = str;

                    break;
                }
                yield return null;
            }
        }
    }

    public override string[] GetModuleStats(int tier)
    {
        string[] value = { magnetRangeUpValue[tier].ToString() };
        return value;
    }
}
