using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

[CreateAssetMenu(fileName = "new Module", menuName = "Scriptable Object/Upgrade Module/RotationSpeed", order = int.MaxValue)]
public class RotationSpeedModule : UpgradeModuleScripableObject
{

    public static readonly float[] rotationValue = { 0.15f, 0.3f, 0.45f, 0.6f, 0.75f };

    public override void ApplyUpgradeModule(PlayerUpgradeModule player, int tier)
    {
        player.module_RotationSpeed.AddFloatModifier(rotationValue[(int)tier]);

        MonoBehaviour.print("회전속도증가 모듈 스탯 적용");
    }

    public override void UnapplyUpgradeModule(PlayerUpgradeModule player, int tier)
    {
        MonoBehaviour.print("회전속도증가 스탯 적용해제");
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

                    str = str.Replace("[ST_VALUE1]", rotationValue[tier].ToString());

                    text.text = str;

                    break;
                }
                yield return null;
            }
        }
    }

    public override string[] GetModuleStats(int tier)
    {
        string[] value = { rotationValue[tier].ToString() };
        return value;
    }
}
