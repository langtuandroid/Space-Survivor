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
using DG.Tweening;
using Sirenix.OdinInspector;

public class RewardedInterstitialAdCaller : MonoBehaviour
{
    public static RewardedInterstitialAdCaller instance;

    //Test ID ca-app-pub-3940256099942544/5224354917
    public static string androidAdUnitId = "ca-app-pub-5179254807136480/1031690389";
    //Test ID ca-app-pub-3940256099942544/1712485313
    public string iosAdUnitId;

    [Space]

    string adUnitId;

    private static RewardedAd rewardedAd;

    [FoldoutGroup("Reward")][SerializeField] private TextMeshProUGUI crystalValueText;
    [FoldoutGroup("Reward")][SerializeField] private Button shipTrialButton;
    [FoldoutGroup("Reward")][SerializeField] private Button reviveButton;
    [FoldoutGroup("Reward")][SerializeField] private Button[] crystalDoubleButtons;
    [FoldoutGroup("Reward")][field: SerializeField] public bool useCrystalDoubleThisStage { get; set; } = false;

    [FoldoutGroup("Reward")][SerializeField] private GameObject touchProjectPanel;
    [FoldoutGroup("Reward")][SerializeField] private PlayerStat playerStat;

    [Space]

    [FoldoutGroup("Reward")] public int crystalValue = 25;
    [FoldoutGroup("Reward")][SerializeField] private int freeCrystalWaitTime = 1800;
    [FoldoutGroup("Reward")][SerializeField] private int shipTrialWaitTime = 1800;
    [FoldoutGroup("Reward")][SerializeField] private int freeModuleWaitTime = 1800;


    [Space]
    [FoldoutGroup("Reward")][SerializeField] private GameObject freeCrystalButtonImage;
    [FoldoutGroup("Reward")][SerializeField] private TextMeshProUGUI freeCrystalButtonTimeText;

    [FoldoutGroup("Reward")][SerializeField] private GameObject trialShipButtonImage;
    [FoldoutGroup("Reward")][SerializeField] private TextMeshProUGUI trialShipButtonTimeText;

    [FoldoutGroup("Reward")] private static List<IEnumerator> rewardList = new List<IEnumerator>();

    [FoldoutGroup("Reward")] public GameObject crystalBonusRVBtn = null;

    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        crystalValueText.text = crystalValue.ToString();

        CreateAndLoadRewardedAd();

#if UNITY_ANDROID
        adUnitId = androidAdUnitId;
#elif UNITY_IPHONE
             adUnitId = iosAdUnitId;
#else
             adUnitId = "unexpected_platform";
#endif

        StartCoroutine(RewardAdsTimeChecking());
    }

    private void Update()
    {
        if (rewardList.Count > 0)
        {
            StartCoroutine(rewardList[0]);
            rewardList.Clear();
        }
    }

    public void CallRV_FreeCrystal()
    {
        if (IsFreeCrystalReady())
            CallRV(getFreeCrystal(), "FreeCrystal");
    }

    public void CallRV_TrialShip()
    {
        if (IsShipTrialReady())
            CallRV(startTrial(), "TrialShip");
    }

    public void CallRV_Revive()
    {
        CallRV(revive(), "Revive");
    }

    public void CallRV_CrystalDouble()
    {
        CallRV(crystalDouble(), "CrystalDouble");
    }

    public void CallRV_GetAllUpgrade()
    {
        CallRV(getAllUpgrade(), "GetAllUpgrade");
    }

    //부활 보상획득
    IEnumerator revive()
    {
        if (GameManager.instance.reviveAnimationSequence != null)
            GameManager.instance.reviveAnimationSequence.Kill();

        touchProjectPanel.SetActive(true);
        yield return new WaitForSeconds(0.3f);

        // reviveButton.onClick.Invoke();
        touchProjectPanel.SetActive(false);
        GameManager.instance.revivedThisGame = true;
        GameManager.instance.gameStart = true;
        GameManager.instance.Resurrection();
        GameManager.instance.StartTimer();
        EnemyGenerator.instance.StartSpawnEnemy();
        playerStat.Resurrection();

        InterstitialAdCaller.instance.RestartIrAdsCoolTime();

        FirebaseAnalytics.LogEvent("ADS_RvAdsComplete_Revive");
    }

    //함선 체험 보상획득
    IEnumerator startTrial()
    {
        UserDataManager.instance.currentUserData.usingShipTrialTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        //UserDataManager.instance.SaveCurrentDate();
        // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
        UserDataManager.instance.Save();

        touchProjectPanel.SetActive(true);
        yield return new WaitForSeconds(0.5f);

        shipTrialButton.onClick.Invoke();
        touchProjectPanel.SetActive(false);

        InterstitialAdCaller.instance.RestartIrAdsCoolTime();

        FirebaseAnalytics.LogEvent("ADS_RvAdsComplete_TrialShip");

        FirebaseAnalytics.LogEvent("TrialShip", "shipName", GameManager.instance.currentShip.shipObjectData.shipCode);
    }

    //크리스탈 두배 보상획득
    IEnumerator crystalDouble()
    {
        yield return null;

        HideDoubleCrystalBtn();

        playerStat.GetCrystalDouble();

        useCrystalDoubleThisStage = true;

        InterstitialAdCaller.instance.RestartIrAdsCoolTime();

        crystalBonusRVBtn?.SetActive(false);

        FirebaseAnalytics.LogEvent("ADS_RvAdsComplete_DoubleCrystal");
    }

    //무료 크리스탈 보상획득
    IEnumerator getFreeCrystal()
    {
        yield return null;

        UserDataManager.instance.currentUserData.usingFreeCrystalTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
        //UserDataManager.instance.SaveCurrentDate();
        // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
        UserDataManager.instance.Save();

        UserDataManager.instance.AddCrystalValue(crystalValue);

        InterstitialAdCaller.instance.RestartIrAdsCoolTime();

        FirebaseAnalytics.LogEvent("ADS_RvAdsComplete_FreeCrystal");
    }

    //모든 업그레이드 획득 보상획득
    IEnumerator getAllUpgrade()
    {
        yield return null;
        LevelUpManager.instance.GetAllCurrentUpgrade();

        FirebaseAnalytics.LogEvent("ADS_RvAdsComplete_GetAllUpgarde");
    }

    public bool IsFreeCrystalReady()
    {
        try
        {
            double timeDiff = Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingFreeCrystalTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;

            //print(timeDiff + " " + freeCrystalWaitTime);
            if (timeDiff < freeCrystalWaitTime)
                return false;
            else
                return true;

        }
        catch (FormatException e)
        {
            Debug.LogError("Date Time Parse Error : / " + UserDataManager.instance.currentUserData.usingFreeCrystalTime + " / " + e);

            UserDataManager.instance.currentUserData.usingFreeCrystalTime = "2000-01-01 01:01:01";

            // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
            UserDataManager.instance.Save();

            return true;
        }
    }

    public bool IsShipTrialReady()
    {
        try
        {
            double timeDiff = Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingShipTrialTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;

            if (timeDiff < shipTrialWaitTime)
                return false;
            else
                return true;

        }
        catch (FormatException e)
        {
            Debug.LogError("Date Time Parse Error : " + UserDataManager.instance.currentUserData.usingShipTrialTime + " / " + e);

            UserDataManager.instance.currentUserData.usingShipTrialTime = "2000-01-01 01:01:01";

            // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
            UserDataManager.instance.Save();

            return true;
        }
    }

    public bool IsFreeModuleReady()
    {
        try
        {
            double timeDiff = Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingFreeModuleTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;

            //print(timeDiff + " " + freeCrystalWaitTime);
            if (timeDiff < freeCrystalWaitTime)
                return false;
            else
                return true;

        }
        catch (FormatException e)
        {
            Debug.LogError("Date Time Parse Error : / " + UserDataManager.instance.currentUserData.usingFreeModuleTime + " / " + e);

            UserDataManager.instance.currentUserData.usingFreeModuleTime = "2000-01-01 01:01:01";

            // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
            UserDataManager.instance.Save();

            return true;
        }
    }

    public double GetFreeCrystalLeftTime()
    {
        try
        {
            return freeCrystalWaitTime - (int)Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingFreeCrystalTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;
        }
        catch (FormatException e)
        {
            FirebaseAnalytics.LogEvent("FormatExceptionErrorEvent");

            Debug.LogError("Date Time Parse Error : / " + UserDataManager.instance.currentUserData.usingFreeCrystalTime + " / " + e);

            UserDataManager.instance.currentUserData.usingFreeCrystalTime = "2000-01-01 01:01:01";

            // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
            UserDataManager.instance.Save();

            return freeCrystalWaitTime - (int)Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingFreeCrystalTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;
        }
    }

    public double GetTrialShipLeftTime()
    {
        try
        {
            return shipTrialWaitTime - Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingShipTrialTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;
        }
        catch (FormatException e)
        {
            FirebaseAnalytics.LogEvent("FormatExceptionErrorEvent");

            Debug.LogError("Date Time Parse Error : / " + UserDataManager.instance.currentUserData.usingShipTrialTime + " / " + e);

            UserDataManager.instance.currentUserData.usingShipTrialTime = "2000-01-01 01:01:01";

            // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
            UserDataManager.instance.Save();

            return freeCrystalWaitTime - (int)Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingShipTrialTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;
        }
    }

    public double GetFreeModuleLeftTime()
    {
        try
        {
            return freeModuleWaitTime - (int)Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingFreeModuleTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;
        }
        catch (FormatException e)
        {
            FirebaseAnalytics.LogEvent("FormatExceptionErrorEvent");

            Debug.LogError("Date Time Parse Error : / " + UserDataManager.instance.currentUserData.usingFreeModuleTime + " / " + e);

            UserDataManager.instance.currentUserData.usingFreeModuleTime = "2000-01-01 01:01:01";

            // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
            UserDataManager.instance.Save();

            return freeModuleWaitTime - (int)Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingFreeModuleTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;
        }
    }

    private void HideDoubleCrystalBtn()
    {
        foreach (Button btn in crystalDoubleButtons)
        {
            btn.gameObject.SetActive(false);
        }
    }

    public void ShowDoubleCrystalBtn()
    {
        if (useCrystalDoubleThisStage)
            return;

        foreach (Button btn in crystalDoubleButtons)
        {
            btn.gameObject.SetActive(true);
        }
    }

    IEnumerator RewardAdsTimeChecking()
    {
        while (true)
        {
            if (RewardedInterstitialAdCaller.instance.IsFreeCrystalReady())
            {
                freeCrystalButtonImage.SetActive(true);
                freeCrystalButtonTimeText.gameObject.SetActive(false);
            }
            else
            {
                freeCrystalButtonImage.SetActive(false);
                freeCrystalButtonTimeText.text = Utility.GetFormatedStringFromSecond((int)RewardedInterstitialAdCaller.instance.GetFreeCrystalLeftTime());
                freeCrystalButtonTimeText.gameObject.SetActive(true);
            }

            if (RewardedInterstitialAdCaller.instance.IsShipTrialReady())
            {
                trialShipButtonImage.SetActive(true);
                trialShipButtonTimeText.gameObject.SetActive(false);
            }
            else
            {
                trialShipButtonImage.SetActive(false);
                trialShipButtonTimeText.text = Utility.GetFormatedStringFromSecond((int)RewardedInterstitialAdCaller.instance.GetTrialShipLeftTime());
                trialShipButtonTimeText.gameObject.SetActive(true);
            }

            yield return new WaitForSeconds(1f);
        }
    }

    public static void CreateAndLoadRewardedAd()
    {

        string adUnitId;

#if UNITY_ANDROID
        adUnitId = androidAdUnitId;
#elif UNITY_IPHONE
             adUnitId = iosAdUnitId;
#else
             adUnitId = "unexpected_platform";
#endif

        // var adRequest = new AdRequest();

        // // send the request to load the ad.
        // RewardedAd.Load(adUnitId, adRequest,
        //     (RewardedAd ad, LoadAdError error) =>
        //     {
        //         // if error is not null, the load request failed.
        //         if (error != null || ad == null)
        //         {
        //             Debug.LogError("Rewarded ad failed to load an ad " +
        //                            "with error : " + error);
        //             return;
        //         }

        //         Debug.Log("Rewarded ad loaded with response : "
        //                   + ad.GetResponseInfo());

        //         rewardedAd = ad;
        //     });

        //보상형 광고가 완료되었을때
        void HandleRewardedAdLoaded(object sender, EventArgs args)
        {
            MonoBehaviour.print("보상형 광고를 로드함");

            FirebaseAnalytics.LogEvent("ADS_RvAdsLoadSuccess");
        }

        //보상형 광고 로드 실패함
        void HandleRewardedAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            MonoBehaviour.print(
                "보상형 광고 로드를 실패하였습니다: "
                                 + args.LoadAdError);

            FirebaseAnalytics.LogEvent("ADS_RvAdsLoadFailed", "errorCode", "" + args.LoadAdError);

            CreateAndLoadRewardedAd();
        }

        //보상형 광고 표시중
        void HandleRewardedAdOpening(object sender, EventArgs args)
        {
            MonoBehaviour.print("보상형 광고 표시중");

            FirebaseAnalytics.LogEvent("ADS_RvAdsOpening");
        }

        //보상형 광고 표시가 실패하였습니다.
        // void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
        // {
        //     MonoBehaviour.print(
        //         "광고 표시를 실패하였습니다: "
        //                          + args.AdError.GetMessage());
        // }

        //사용자가 보상형 광고를 취소하였을때
        void HandleRewardedAdClosed(object sender, EventArgs args)
        {
            CreateAndLoadRewardedAd();
            MonoBehaviour.print("사용자가 보상형 광고 시청을 취소하였습니다.");
        }

        //보상형 광고를 시청하고 보상을 받아야 할때 실행
        void HandleUserEarnedReward(object sender, Reward args)
        {

        }
    }

    public static void CallRV(IEnumerator _reward, string rvAdsType)
    {
        // FirebaseAnalytics.LogEvent("ADS_RvAdsCallEvent");
        FirebaseAnalytics.LogEvent("ADS_RvAdsCallEvent", "RvAdsType", rvAdsType);
        // FirebaseAnalytics.LogEvent("ADS_RvAdsCallEvent" + "_" + rvAdsType);

        if (!UserDataManager.instance.currentUserData.RemoveAds && !IAPManager.instance.HadPurchased())
        {
            // FirebaseAnalytics.LogEvent("ADS_RvAdsCallSuccess");
            FirebaseAnalytics.LogEvent("ADS_RvAdsCallSuccess", "RvAdsType", rvAdsType);
            // FirebaseAnalytics.LogEvent("ADS_RvAdsCallSuccess" + "_" + rvAdsType);

            // rewardedAd.OnUserEarnedReward -= HandleUserEarnedReward;
            // rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;

            // rewardedAd.OnAdOpening -= HandleRewardedAdOpening;
            // rewardedAd.OnAdOpening += HandleRewardedAdOpening;

            void HandleUserEarnedReward()
            {
                print("11");
                rewardList.Add(_reward);

                CreateAndLoadRewardedAd();
            }

            void HandleRewardedAdOpening(object sender, EventArgs args)
            {
                MonoBehaviour.print("보상형 광고 표시중");

                FirebaseAnalytics.LogEvent("ADS_RvAdsOpening", "RvAdsOpenType", rvAdsType);
            }

            rewardedAd.Show((Reward reward) =>
            {
                print("11");
                rewardList.Add(_reward);

                CreateAndLoadRewardedAd();
            });
        }
        else
        {
            // if (UserDataManager.instance.currentUserData.RemoveAds)
            // {
            //     FirebaseAnalytics.LogEvent("ADS_RvAdsCallFailed_hadNoads", "RvAdsType2", rvAdsType);
            //     print("광고 제거를 구매해 광고 호출을 안함");
            //     rewardList.Add(reward);
            // }
            // else 
            if (!rewardedAd.CanShowAd())
            {
                FirebaseAnalytics.LogEvent("ADS_RvAdsCallFailed_NotLoadedAd", "RvAdsType3", rvAdsType);
                CreateAndLoadRewardedAd();

                print("광고가 없습니다");
            }
            else
            {
                FirebaseAnalytics.LogEvent("ADS_RvAdsCallFailed_UnknownError", "RvAdsType4", rvAdsType);

                print("알수없는 이유로 광고 호출에 실패하였습니다.");
            }

            // FirebaseAnalytics.LogEvent("ADS_RvAdsCallFailed");
            FirebaseAnalytics.LogEvent("ADS_RvAdsCallFailed", "RvAdsType", rvAdsType);
            // FirebaseAnalytics.LogEvent("ADS_RvAdsCallFailed" + "_" + rvAdsType);

        }
    }
}


