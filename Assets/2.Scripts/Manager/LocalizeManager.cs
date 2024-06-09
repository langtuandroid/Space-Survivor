using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;

/// <summary>
/// 이 클래스는 현지화를 도와주는 기능을 포함하고 있습니다.
/// </summary>
public class LocalizeManager
{

    /// <summary>
    /// 테이블 이름과, 키값을 통해 텍스트를 현지화된 텍스트로 변경합니다.
    /// </summary>
    public static void CallLocalizedString(TextMeshProUGUI text ,string tableRef, string key)
    {
        CoroutineHelper.StartCoroutine(LoadString());

        IEnumerator LoadString()
        {

            var localizedString = new LocalizedString(tableRef, key);

            var stringOperation = localizedString.GetLocalizedStringAsync();

            while (true)
            {
                if (stringOperation.IsDone && stringOperation.Status == AsyncOperationStatus.Succeeded)
                {
                    string str = stringOperation.Result;
                    text.text = str;

                    break;
                }
                yield return null;
            }
        }
    }
}
