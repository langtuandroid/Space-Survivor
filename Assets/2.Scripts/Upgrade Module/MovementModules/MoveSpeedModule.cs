using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

[CreateAssetMenu(fileName = "new Module", menuName = "Scriptable Object/Upgrade Module/MoveSpeed", order = int.MaxValue)]
public class MoveSpeedModule : UpgradeModuleScripableObject
{
    public static readonly float[] moveSpeedValue = { 0.15f, 0.3f, 0.45f, 0.6f, 0.75f };

    public override void ApplyUpgradeModule(PlayerUpgradeModule player, int tier)
    {
        player.module_MoveSpeed.AddFloatModifier(moveSpeedValue[(int)tier]);

        MonoBehaviour.print("이동속도증가 모듈 스탯 적용");
    }

    public override void UnapplyUpgradeModule(PlayerUpgradeModule player, int tier)
    {
        MonoBehaviour.print("이동속도증가 스탯 적용해제");
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

                    str = str.Replace("[ST_VALUE1]", moveSpeedValue[tier].ToString());

                    text.text = str;

                    break;
                }
                yield return null;
            }
        }
    }

    public override string[] GetModuleStats(int tier)
    {
        string[] value = { moveSpeedValue[tier].ToString() };
        return value;
    }
}
