// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System;
// using Unity.VisualScripting;
// using Cysharp.Threading.Tasks.Linq;

// public static class MAX
// {
//     public static MaxReward MaxRewardInstance;
//     public static MaxBanner MaxBannerInstance;
// }

// public class MaxReward : MonoBehaviour
// {
// #if UNITY_IOS
// string adUnitId = "«iOS-ad-unit-ID»";
// #else // UNITY_ANDROID
//     string adUnitId = "489b061c2f3f9f64";
// #endif

//     int retryAttempt;

//     private void Start()
//     {
//         MAX.MaxRewardInstance = this;
//         InitializeRewardedAds();
//     }

//     public void ShowReward(System.Action<string, MaxSdkBase.Reward, MaxSdkBase.AdInfo> reward)
//     {
//         if (MaxSdk.IsRewardedAdReady(adUnitId))
//         {
//             MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += reward;

//             MaxSdk.ShowRewardedAd(adUnitId);

//             MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent -= reward;
//         }
//     }

//     public void InitializeRewardedAds()
//     {
//         // Attach callback
//         MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
//         MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
//         MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
//         MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
//         MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
//         MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
//         MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
//         MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;

//         // Load the first rewarded ad
//         LoadRewardedAd();
//     }

//     private void LoadRewardedAd()
//     {
//         MaxSdk.LoadRewardedAd(adUnitId);
//     }

//     private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
//     {
//         // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

//         // Reset retry attempt
//         retryAttempt = 0;
//     }

//     private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
//     {
//         // Rewarded ad failed to load
//         // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

//         retryAttempt++;
//         double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

//         Invoke("LoadRewardedAd", (float)retryDelay);
//     }

//     private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

//     private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
//     {
//         // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
//         LoadRewardedAd();
//     }

//     private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

//     private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
//     {
//         // Rewarded ad is hidden. Pre-load the next ad
//         LoadRewardedAd();
//     }

//     private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
//     {
//         // The rewarded ad displayed and the user should receive the reward.
//     }

//     private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
//     {
//         // Ad revenue paid. Use this callback to track user revenue.
//     }
// }
