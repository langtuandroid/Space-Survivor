using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

[CreateAssetMenu(fileName = "new Module", menuName = "Scriptable Object/Upgrade Module/KillChain", order = int.MaxValue)]
public class KillChainModule : UpgradeModuleScripableObject
{
    public override void ApplyUpgradeModule(PlayerUpgradeModule player, int tier)
    {
        MonoBehaviour.print("킬체인 모듈 적용");
    }

    public override void UnapplyUpgradeModule(PlayerUpgradeModule player, int tier)
    {
        MonoBehaviour.print("킬체인 모듈 적용해제");
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

                    //str = str.Replace("[ST_VALUE1]", rotationValue[tier].ToString());

                    text.text = str;

                    break;
                }
                yield return null;
            }
        }
    }

    public override string[] GetModuleStats(int tier)
    {
        return null;
    }
}
