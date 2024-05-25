// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class MaxBanner : MonoBehaviour
// {
// #if UNITY_IOS
// string bannerAdUnitId = "«iOS-ad-unit-ID»"; // Retrieve the ID from your account
// #else // UNITY_ANDROID
//     string bannerAdUnitId = "613f8b7c19fd48a4"; // Retrieve the ID from your account
// #endif

//     private void Start()
//     {
//         InitializeBannerAds();

//         MAX.MaxBannerInstance = this;
//     }

//     public void ShowBanner()
//     {
//         MaxSdk.ShowBanner(bannerAdUnitId);
//     }

//     public void InitializeBannerAds()
//     {
//         // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
//         // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments
//         MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

//         // Set background or background color for banners to be fully functional
//         MaxSdk.SetBannerBackgroundColor(bannerAdUnitId, Color.black);

//         MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
//         MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
//         MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
//         MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
//         MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
//         MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
//     }

//     private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

//     private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) { }

//     private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

//     private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

//     private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

//     private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
// }
