using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using Newtonsoft.Json;

public static class Utility
{
    public static T DeepCopy<T>(this T self)
    {
        // JSON 직렬화를 이용한 깊은 복사
        string json = JsonConvert.SerializeObject(self);
        return JsonConvert.DeserializeObject<T>(json);
    }

    public static Vector2 GetDirection(Vector2 origin, Vector2 target)
    {
        return (target - origin).normalized;
    }

    public static Transform FindNearEnemy(Transform origin, float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(origin.position, radius);

        float closestDist = float.PositiveInfinity;

        Transform closestTarget = null;

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<EnemyStat>(out EnemyStat enemy))
            {
                var dist = Vector3.Distance(origin.position, enemy.transform.position);

                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestTarget = enemy.transform;
                }
            }
        }

        return closestTarget;
    }

    public static Transform GetRandomTargetInRadius(Transform origin, float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(origin.position, radius, 1 << LayerMask.NameToLayer("Enemy"));

        List<EnemyStat> enemyList = new List<EnemyStat>();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<EnemyStat>(out EnemyStat enemy))
            {
                enemyList.Add(enemy);
            }
        }

        if (enemyList.Count > 0)
            return enemyList[UnityEngine.Random.Range(0, enemyList.Count)].transform;
        else
            return null;
    }

    public static Transform GetClosestTargetInRadius(Transform origin, float radius)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(origin.position, radius, 1 << LayerMask.NameToLayer("Enemy"));

        List<EnemyStat> enemyList = new List<EnemyStat>();

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<EnemyStat>(out EnemyStat enemy))
            {
                enemyList.Add(enemy);
            }
        }

        var enemyQuery = from enemy in enemyList
                         orderby (Vector3.Distance(origin.position, enemy.transform.position))
                         select enemy;

        var reslut = enemyList.ToArray();

        if (enemyList.Count > 0)
            return reslut[0].transform;
        else
            return null;
    }

    public static void Explode(Vector2 center, float damage, float radius, int knockbackForce, VFXType vfxType, EZCameraShake.CameraShakeInstance cameraShakeInstance = null)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(center, radius);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].TryGetComponent<EnemyStat>(out EnemyStat enemy))
            {
                enemy.TakeDamage(Utility.RountToInt(damage));
                enemy.Knockback(center, -knockbackForce);
            }
        }

        VFXGenerator.instance.GenerateVFX(vfxType, center);

        if (cameraShakeInstance != null)
            EZCameraShake.CameraShaker.Instance.Shake(cameraShakeInstance);
    }

    public static int RountToInt(float value)
    {
        return Mathf.RoundToInt(value);
    }

    public static bool PercentageCalculator(int percent, int maxValue)
    {
        var random = UnityEngine.Random.Range(0, maxValue);

        if (percent > random)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public static TimeSpan GetTimeDiff(DateTime time)
    {
        //print(DateTime.Now + " - " +  time);
        TimeSpan timeDiff = DateTime.Now - time;

        return timeDiff;
    }

    public static string GetFormatedStringFromSecond(int second)
    {
        int min = second / 60;
        int hour = min / 60;
        int sec = second % 60;

        return hour + " : " + min + " : " + sec;
    }

    public static void Copy<T>(T parent, T child)
    {
        var parentProperties = parent.GetType().GetProperties();
        var childProperties = child.GetType().GetProperties();

        foreach (var parentProperty in parentProperties)
        {
            foreach (var childProperty in childProperties)
            {
                if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType)
                {
                    childProperty.SetValue(child, parentProperty.GetValue(parent));
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 랜덤값 가져오기 float 배열에 각각 다른 값들을 넣어서 확률 계산을 하여 값을 리턴한다
    /// </summary>
    public static int GetRandomProb(float[] probs)
    {

        float total = 0;

        foreach (float elem in probs)
        {
            total += elem;
        }

        float randomPoint = UnityEngine.Random.value * total;

        for (int i = 0; i < probs.Length; i++)
        {
            if (randomPoint < probs[i])
            {
                return i;
            }
            else
            {
                randomPoint -= probs[i];
            }
        }
        return probs.Length - 1;
    }

    public static void SetLocalizeTextAsync(TextMeshProUGUI text, string table, string key, string[] smartValues = null)
    {
        CoroutineHelper.StartCoroutine(SetText(text));

        IEnumerator SetText(TextMeshProUGUI text)
        {
            var localizedString = new LocalizedString(table, key);

            var stringOperation = localizedString.GetLocalizedStringAsync();

            while (true)
            {
                if (stringOperation.IsDone && stringOperation.Status == AsyncOperationStatus.Succeeded)
                {
                    string str = stringOperation.Result;

                    if (smartValues != null)
                    {
                        for (int i = 0; i < smartValues.Length; i++)
                        {
                            if (smartValues[i] != "")
                                str = str.Replace("[ST_VALUE" + (i + 1) + "]", smartValues[i]);
                        }
                    }

                    text.text = str;

                    break;
                }
                yield return null;
            }

        }
    }

    public static T GetResource<T>(string path) where T : UnityEngine.Object
    {
        var res = Resources.Load<T>(path);

        return res;
    }

    public static void TextAnimation(TextMeshProUGUI text, int targetValue)
    {
        CoroutineHelper.StartCoroutine(CountText(targetValue));

        IEnumerator CountText(int newValue)
        {
            float CountFPS = 30;
            float Duration = 1f;
            string NumberFormat = "N0";
            int _value = 0;

            WaitForSeconds Wait = new WaitForSeconds(1f / CountFPS);
            int previousValue = _value;
            int stepAmount;

            if (newValue - previousValue < 0)
            {
                stepAmount = Mathf.FloorToInt((newValue - previousValue) / (CountFPS * Duration)); // newValue = -20, previousValue = 0. CountFPS = 30, and Duration = 1; (-20- 0) / (30*1) // -0.66667 (ceiltoint)-> 0
            }
            else
            {
                stepAmount = Mathf.CeilToInt((newValue - previousValue) / (CountFPS * Duration)); // newValue = 20, previousValue = 0. CountFPS = 30, and Duration = 1; (20- 0) / (30*1) // 0.66667 (floortoint)-> 0
            }

            if (previousValue < newValue)
            {
                while (previousValue < newValue)
                {
                    previousValue += stepAmount;
                    if (previousValue > newValue)
                    {
                        previousValue = newValue;
                    }

                    text.SetText(previousValue.ToString(NumberFormat));

                    yield return Wait;
                }
            }
            else
            {
                while (previousValue > newValue)
                {
                    previousValue += stepAmount; // (-20 - 0) / (30 * 1) = -0.66667 -> -1              0 + -1 = -1
                    if (previousValue < newValue)
                    {
                        previousValue = newValue;
                    }

                    text.SetText(previousValue.ToString(NumberFormat));

                    yield return Wait;
                }
            }
        }
    }


}
