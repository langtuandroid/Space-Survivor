using System.Collections;
using UnityEngine;

public static class Vibration
{
    public static bool enableVibration = true;
    public static float vibraitonCoolTime = 0.1f;

#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass AndroidPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject AndroidcurrentActivity = AndroidPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject AndroidVibrator = AndroidcurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif
    public static void Vibrate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate");
#else
        Handheld.Vibrate();
#endif
    }

    public static void Vibrate(long milliseconds)
    {
        if ((ES3.KeyExists("Vibration") ? ES3.Load<int>("Vibration") : 1) == 0 || !enableVibration)
            return;

#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate", milliseconds);
#else
        Handheld.Vibrate();
#endif

        IEnumerator vibrationCoolTime()
        {
            enableVibration = false;
            yield return new WaitForSeconds(vibraitonCoolTime);
            enableVibration = true;
        }

        CoroutineHelper.StartCoroutine(vibrationCoolTime());
    }
    public static void Vibrate(long[] pattern, int repeat)
    {


#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate", pattern, repeat);
#else
        Handheld.Vibrate();
#endif
    }

    public static void Cancel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidVibrator.Call("cancel");
#endif
    }

}