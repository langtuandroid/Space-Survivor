using UnityEngine.Events;
using UnityEngine;
using System.Collections;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;
using Firebase.Analytics;

public class DefaultRvCaller : MonoBehaviour
{

//     //Test ID ca-app-pub-3940256099942544/5224354917
//     public string androidAdUnitId = "ca-app-pub-5179254807136480/1031690389";
//     //Test ID ca-app-pub-3940256099942544/1712485313
//     public string iosAdUnitId;

//     [Space]

//     [SerializeField] private string adUnitId;

//     [SerializeField] private GameObject touchProjectPanel;

//     private static RewardedAd rewardedAd;

//     [field: SerializeField] public static bool useCrystalDoubleThisStage { get; set; } = false;

//     private static List<IEnumerator> rewardList = new List<IEnumerator>();

//     private void Awake()
//     {

// #if UNITY_ANDROID
//         adUnitId = androidAdUnitId;
// #elif UNITY_IPHONE
//              adUnitId = iosAdUnitId;
// #else
//              adUnitId = "unexpected_platform";
// #endif

//         CreateAndLoadRewardedAd();
//     }

//     private void Update()
//     {
//         if (rewardList.Count > 0)
//         {
//             StartCoroutine(rewardList[0]);
//             rewardList.Clear();
//         }
//     }

//     public static void CreateAndLoadRewardedAd()
//     {
// #if UNITY_ANDROID
//         string adUnitId = "ca-app-pub-3940256099942544/5224354917";
// #elif UNITY_IPHONE
//             string adUnitId = "ca-app-pub-3940256099942544/1712485313";
// #else
//             string adUnitId = "unexpected_platform";
// #endif

//         rewardedAd = new RewardedAd(adUnitId);

//         rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
//         // Called when an ad request failed to load.
//         rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
//         // Called when an ad is shown.
//         rewardedAd.OnAdOpening += HandleRewardedAdOpening;
//         // Called when an ad request failed to show.
//         rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
//         // Called when the user should be rewarded for interacting with the ad.
//         rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
//         // Called when the ad is closed.
//         rewardedAd.OnAdClosed += HandleRewardedAdClosed;

//         // Create an empty ad request.
//         AdRequest request = new AdRequest.Builder().Build();
//         // Load the rewarded ad with the request.
//         rewardedAd.LoadAd(request);

//         //보상형 광고가 완료되었을때
//         void HandleRewardedAdLoaded(object sender, EventArgs args)
//         {
//             MonoBehaviour.print("보상형 광고를 로드함");

//             FirebaseAnalytics.LogEvent("RvAdsLoadSuccess");
//         }

//         //보상형 광고 로드 실패함
//         void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
//         {
//             MonoBehaviour.print(
//                 "보상형 광고 로드를 실패하였습니다: "
//                                  + args.LoadAdError);

//             FirebaseAnalytics.LogEvent("RvAdsLoadFailed", "errorCode", "" + args.LoadAdError);
//         }

//         //보상형 광고 표시중
//         void HandleRewardedAdOpening(object sender, EventArgs args)
//         {
//             MonoBehaviour.print("보상형 광고 표시중");
//         }

//         //보상형 광고 표시가 실패하였습니다.
//         void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
//         {
//             MonoBehaviour.print(
//                 "광고 표시를 실패하였습니다: "
//                                  + args.AdError.GetMessage());
//         }

//         //사용자가 보상형 광고를 취소하였을때
//         void HandleRewardedAdClosed(object sender, EventArgs args)
//         {
//             CreateAndLoadRewardedAd();
//             MonoBehaviour.print("사용자가 보상형 광고 시청을 취소하였습니다.");
//         }

//         //보상형 광고를 시청하고 보상을 받아야 할때 실행
//         void HandleUserEarnedReward(object sender, Reward args)
//         {
//             string type = args.Type;
//             double amount = args.Amount;
//             MonoBehaviour.print(
//                 "보상형 광고를 시청하였습니다. 보상을 지급해야 합니다: "
//                             + amount.ToString() + " " + type);
//         }
//     }

//     //부활
//     IEnumerator revive()
//     {
//         touchProjectPanel.SetActive(true);
//         yield return new WaitForSeconds(0.3f);

//         reviveButton.onClick.Invoke();
//         touchProjectPanel.SetActive(false);
//         GameManager.instance.revivedThisGame = true;
//         GameManager.instance.gameStart = true;

//         InterstitialAdCaller.instance.RestartIrAdsCoolTime();

//         FirebaseAnalytics.LogEvent("RvAdsComplete_Revive");
//     }

//     //함선 체험
//     IEnumerator startTrial()
//     {
//         UserDataManager.instance.currentUserData.usingShipTrialTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"); ;
//         //UserDataManager.instance.SaveCurrentDate();
//         GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);

//         touchProjectPanel.SetActive(true);
//         yield return new WaitForSeconds(0.5f);

//         shipTrialButton.onClick.Invoke();
//         touchProjectPanel.SetActive(false);

//         InterstitialAdCaller.instance.RestartIrAdsCoolTime();

//         FirebaseAnalytics.LogEvent("RvAdsComplete_TrialShip");
//     }

//     //크리스탈 두배
//     IEnumerator crystalDouble()
//     {
//         yield return null;

//         HideDoubleCrystalBtn();

//         playerStat.GetCrystalDouble();

//         useCrystalDoubleThisStage = true;

//         InterstitialAdCaller.instance.RestartIrAdsCoolTime();

//         FirebaseAnalytics.LogEvent("RvAdsComplete_DoubleCrystal");
//     }

//     //무료 크리스탈
//     IEnumerator getFreeCrystal()
//     {
//         yield return null;

//         UserDataManager.instance.currentUserData.usingFreeCrystalTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
//         //UserDataManager.instance.SaveCurrentDate();
//         GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);

//         UserDataManager.instance.AddCrystalValue(crystalValue);

//         InterstitialAdCaller.instance.RestartIrAdsCoolTime();

//         FirebaseAnalytics.LogEvent("RvAdsComplete_FreeCrystal");
//     }

//     //모든 업그레이드 획득
//     IEnumerator getAllUpgrade()
//     {
//         yield return null;

//         LevelUpManager.instance.GetAllCurrentUpgrade();

//         FirebaseAnalytics.LogEvent("RvAdsComplete_GetAllUpgarde");
//     }

//     //크리스탈 획득 리워드 광고 호출
//     public void WatchRewardAds_Crytal()
//     {
//         double timeDiff = Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingFreeCrystalTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;

//         if (timeDiff < freeCrystalWaitTime)
//         {
//             //print("무료 크리스탈 리워드 광고 재사용 시간이 지나지 않았습니다. " + timeDiff + " 남은시간 : " + (freeCrystalWaitTime - timeDiff));
//             return;
//         }

//         FirebaseAnalytics.LogEvent("RvAdsCallEvent");

//         if (this.crystallAddRewardedAd.IsLoaded() && !UserDataManager.instance.currentUserData.RemoveAds && !IAPManager.instance.HadPurchased())
//         {
//             if (UserDataManager.instance.currentUserData.usingFreeCrystalTime != null)
//                 print(timeDiff);

//             FirebaseAnalytics.LogEvent("RvAdsCallSuccess_FreeCrystal");

//             this.crystallAddRewardedAd.Show();
//         }
//         else
//         {
//             if (UserDataManager.instance.currentUserData.RemoveAds)
//                 print("광고 제거를 구매해 광고 호출을 안함");
//             else if (!this.crystallAddRewardedAd.IsLoaded())
//                 print("광고가 없습니다");
//             else
//                 print("알수없는 이유로 광고 호출에 실패하였습니다.");

//             FirebaseAnalytics.LogEvent("RvAdsCallFailed_FreeCrystal");

//             crystallAddRewardedAd = CreateAndLoadRewardedAd_Crystal(adUnitId);

//             //StartCoroutine(getFreeCrystal());
//         }
//     }

//     //함선 무료 체험 리워드 광고 호출
//     public void WatchRewardAds_TrialShip()
//     {
//         double timeDiff = Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingShipTrialTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;

//         if (timeDiff < shipTrialWaitTime)
//         {
//             //print("함선 체험 리워드 광고 재사용 시간이 지나지 않았습니다. " + timeDiff + " 남은시간 : " + (freeCrystalWaitTime - timeDiff));
//             return;
//         }

//         FirebaseAnalytics.LogEvent("RvAdsCallEvent");

//         if (this.shipTrialRewardedAd.IsLoaded() && !UserDataManager.instance.currentUserData.RemoveAds && !IAPManager.instance.HadPurchased())
//         {
//             FirebaseAnalytics.LogEvent("RvAdsCallSuccess_trialShip");
//             this.shipTrialRewardedAd.Show();
//         }
//         else
//         {
//             if (UserDataManager.instance.currentUserData.RemoveAds)
//                 print("광고 제거를 구매해 광고 호출을 안함");
//             else if (!this.shipTrialRewardedAd.IsLoaded())
//                 print("광고가 없습니다");
//             else
//                 print("알수없는 이유로 광고 호출에 실패하였습니다.");

//             FirebaseAnalytics.LogEvent("RvAdsCallFailed_trialShip");

//             shipTrialRewardedAd = CreateAndLoadRewardedAd_TrailShip(adUnitId);

//             //StartCoroutine(startTrial());
//         }
//     }

//     //부활 리워드 광고 호출
//     public void WatchRewardAds_Revive()
//     {
//         FirebaseAnalytics.LogEvent("RvAdsCallEvent");

//         if (this.reviveRewardedAd.IsLoaded() && !UserDataManager.instance.currentUserData.RemoveAds && !IAPManager.instance.HadPurchased())
//         {
//             FirebaseAnalytics.LogEvent("RvAdsCallSuccess_revive");
//             this.reviveRewardedAd.Show();
//         }
//         else
//         {
//             if (UserDataManager.instance.currentUserData.RemoveAds)
//                 print("광고 제거를 구매해 광고 호출을 안함");
//             else if (!this.reviveRewardedAd.IsLoaded())
//                 print("광고가 없습니다");
//             else
//                 print("알수없는 이유로 광고 호출에 실패하였습니다.");

//             FirebaseAnalytics.LogEvent("RvAdsCallFailed_revive");

//             reviveRewardedAd = CreateAndLoadRewardedAd_Revive(adUnitId);

//             //StartCoroutine(revive());
//         }
//     }

//     //크리스탈 2배 리워드 광고 호출
//     public void WatchRewardAds_CrystalDouble()
//     {
//         FirebaseAnalytics.LogEvent("RvAdsCallEvent");
//         if (this.crystalDoubleRewardAd.IsLoaded() && !UserDataManager.instance.currentUserData.RemoveAds && !IAPManager.instance.HadPurchased())
//         {
//             FirebaseAnalytics.LogEvent("RvAdsCallSuccess_crystalDouble");
//             this.crystalDoubleRewardAd.Show();
//         }
//         else
//         {
//             if (UserDataManager.instance.currentUserData.RemoveAds)
//                 print("광고 제거를 구매해 광고 호출을 안함");
//             else if (!this.crystalDoubleRewardAd.IsLoaded())
//                 print("광고가 없습니다");
//             else
//                 print("알수없는 이유로 광고 호출에 실패하였습니다.");

//             FirebaseAnalytics.LogEvent("RvAdsCallFailed_crystalDouble");

//             crystalDoubleRewardAd = CreateAndLoadRewardedAd_CrystalDouble(adUnitId);

//             //StartCoroutine(crystalDouble());
//         }
//     }

//     //모든 업그레이드 확득 RV
//     public void WatchRewardAds_GetAllUpgrade()
//     {
//         FirebaseAnalytics.LogEvent("RvAdsCallEvent");
//         if (this.getAllUpgardeAd.IsLoaded() && !UserDataManager.instance.currentUserData.RemoveAds && !IAPManager.instance.HadPurchased())
//         {
//             FirebaseAnalytics.LogEvent("RvAdsCallSuccess_GetAllUpgrade");
//             this.getAllUpgardeAd.Show();
//         }
//         else
//         {
//             if (UserDataManager.instance.currentUserData.RemoveAds)
//                 print("광고 제거를 구매해 광고 호출을 안함");
//             else if (!this.getAllUpgardeAd.IsLoaded())
//                 print("광고가 없습니다");
//             else
//                 print("알수없는 이유로 광고 호출에 실패하였습니다.");

//             FirebaseAnalytics.LogEvent("RvAdsCallFailed_GetAllUpgrade");

//             getAllUpgardeAd = CreateAndLoadRewardedAd_GetAllUpgarde(adUnitId);

//             //StartCoroutine(getAllUpgrade());
//         }
//     }

//     public static void ShowRv(IEnumerator reward)
//     {
//         FirebaseAnalytics.LogEvent("RvAdsCallEvent");
//         if (rewardedAd.IsLoaded() && !UserDataManager.instance.currentUserData.RemoveAds && !IAPManager.instance.HadPurchased())
//         {
//             FirebaseAnalytics.LogEvent("RvAdsCallSuccess");
//             rewardedAd.Show();

//             rewardList.Add(reward);
//         }
//         else
//         {
//             if (UserDataManager.instance.currentUserData.RemoveAds)
//                 print("광고 제거를 구매해 광고 호출을 안함");
//             else if (!rewardedAd.IsLoaded())
//                 print("광고가 없습니다");
//             else
//                 print("알수없는 이유로 광고 호출에 실패하였습니다.");

//             FirebaseAnalytics.LogEvent("RvAdsCallFailed");

//             CreateAndLoadRewardedAd();
//         }
//     }
}
