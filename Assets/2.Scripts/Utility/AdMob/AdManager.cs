using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using TMPro;
using System;

public class AdManager : MonoBehaviour
{
    public static AdManager instance;

    private RewardedAd rewardedAd;
    private BannerView bannerView;
    private InterstitialAd interstitial;

    [FoldoutGroup("Reward")] public int crystalValue = 25;
    [FoldoutGroup("Reward")][SerializeField] private int freeCrystalWaitTime = 1800;
    [FoldoutGroup("Reward")][SerializeField] private int shipTrialWaitTime = 1800;
    [FoldoutGroup("Reward")][SerializeField] private int freeModuleWaitTime = 1800;
    [Space]
    [FoldoutGroup("Reward")][SerializeField] private TextMeshProUGUI crystalValueText;
    [FoldoutGroup("Reward")][SerializeField] public Button shipTrialButton;
    [FoldoutGroup("Reward")][SerializeField] private Button reviveButton;
    [FoldoutGroup("Reward")][SerializeField] private Button[] crystalDoubleButtons;
    [FoldoutGroup("Reward")][SerializeField] public bool useCrystalDoubleThisStage { get; set; } = false;
    [FoldoutGroup("Reward")][SerializeField] public GameObject touchProjectPanel;
    [FoldoutGroup("Reward")][SerializeField] private PlayerStat playerStat;
    [FoldoutGroup("Reward")][SerializeField] private GameObject freeCrystalButtonImage;
    [FoldoutGroup("Reward")][SerializeField] private TextMeshProUGUI freeCrystalButtonTimeText;
    [FoldoutGroup("Reward")][SerializeField] private GameObject trialShipButtonImage;
    [FoldoutGroup("Reward")][SerializeField] private TextMeshProUGUI trialShipButtonTimeText;
    [FoldoutGroup("Reward")] private static List<IEnumerator> rewardList = new List<IEnumerator>();
    [FoldoutGroup("Reward")] public GameObject crystalBonusRVBtn = null;


    [FoldoutGroup("Institial")] public float IrAdsCallTime = 10f;
    [FoldoutGroup("Institial")] public bool IrAdsReady = false;


    public void Awake()
    {
        if (instance == null)
            instance = this;
    }

    private void Start() {
        AdmobInit();
    }

    public void AdmobInit()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
                {
                    // This callback is called once the MobileAds SDK is initialized.

                    Dictionary<string, AdapterStatus> map = initStatus.getAdapterStatusMap();
                    foreach (KeyValuePair<string, AdapterStatus> keyValuePair in map)
                    {
                        string className = keyValuePair.Key;
                        AdapterStatus status = keyValuePair.Value;
                        switch (status.InitializationState)
                        {
                            case AdapterState.NotReady:
                                // The adapter initialization did not complete.
                                MonoBehaviour.print("Adapter: " + className + " not ready.");
                                break;
                            case AdapterState.Ready:
                                // The adapter was successfully initialized.
                                MonoBehaviour.print("Adapter: " + className + " is initialized.");
                                break;
                        }
                    }

                    LoadRewardedAd();
                    this.TaskWaitUntil(() => RegisterRewardEventHandlers(rewardedAd), () => rewardedAd != null);
                    this.TaskWaitUntil(() => RegisterInstitalEventHandlers(interstitial), () => interstitial != null);

                    this.TaskWaitUntil(() =>
                    {
                        // CreateBannerView();
                        // AudienceNetworkAds.Initialize();

                        if (!UserDataManager.instance.currentUserData.RemoveAds && !IAPManager.instance.HadPurchased())
                        {
                            CreateBannerView();
                            // MAX.MaxBannerInstance.ShowBanner();

                            print("NoAds가 없기 때문에 배너 광고 시작");
                        }
                        else
                        {
                            print("NoAds가 있기 때문에 배너광고가 호출되지 않습니다.");
                        }
                    }, () => IAPManager.instance.initialized);

                    RewardAdInit();
                });
    }

    /// <summary>
    /// 보상형 광고 호출
    /// </summary>
    public void ShowReward(System.Action onGainReward, string rewardInfo)
    {
        const string rewardMsg =
        "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        print((rewardedAd != null) + " " + rewardedAd.CanShowAd());

        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                // TODO: Reward the user.
                Debug.Log(System.String.Format(rewardMsg, reward.Type, reward.Amount));

                //딜레이를 안넣어주면 에러가 발생하면서 앱이 다운됨
                StartCoroutine(delay());

                IEnumerator delay()
                {
                    yield return new WaitForEndOfFrame();
                    onGainReward.Invoke();
                    LoadRewardedAd();
                }
            });
        }
        else
            LoadRewardedAd();

        // MAX.MaxRewardInstance.ShowReward((adUnitId, reward, adInfo) =>
        // {
        //     onGainReward.Invoke();
        // });
    }

    /// <summary>
    /// 전면 광고 호출
    /// </summary>
    public void ShowIntisitial()
    {
        if (IrAdsReady && !UserDataManager.instance.currentUserData.RemoveAds && !IAPManager.instance.HadPurchased())
        {
            interstitial.Show();
        }
        else
        {
            if (UserDataManager.instance.currentUserData.RemoveAds || IAPManager.instance.HadPurchased())
            {
                print("광고제거를 구매하여 전면광고가 없습니다");
            }

            else if (!IrAdsReady)
            {
                print("광고가 로드되지 않았습니다");

                LoadInterstitialAd();
            }
        }
    }

    #region Reward

    public void RewardAdInit()
    {
        crystalValueText.text = crystalValue.ToString();
        StartCoroutine(RewardAdsTimeChecking());
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

    public bool IsTimeLimitRewardReady(RewardType type)
    {
        if (GetTimeLimitRewardLeftTime(type) <= 0)
            return true;
        else
            return false;
    }

    public double GetTimeLimitRewardLeftTime(RewardType type)
    {
        switch (type)
        {
            case RewardType.FreeCrystal:
                try
                {
                    return freeCrystalWaitTime - (int)Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingFreeCrystalTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;
                }
                catch (FormatException e)
                {
                    // FirebaseAnalytics.LogEvent("FormatExceptionErrorEvent");
                    Debug.LogError("Date Time Parse Error : / " + UserDataManager.instance.currentUserData.usingFreeCrystalTime + " / " + e);
                    UserDataManager.instance.currentUserData.usingFreeCrystalTime = "2000-01-01 01:01:01";
                    UserDataManager.instance.Save();
                    return freeCrystalWaitTime - (int)Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingFreeCrystalTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;
                }

            case RewardType.FreeModule:
                try
                {
                    return freeModuleWaitTime - (int)Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingFreeModuleTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;
                }
                catch (FormatException e)
                {
                    // FirebaseAnalytics.LogEvent("FormatExceptionErrorEvent");
                    Debug.LogError("Date Time Parse Error : / " + UserDataManager.instance.currentUserData.usingFreeModuleTime + " / " + e);
                    UserDataManager.instance.currentUserData.usingFreeModuleTime = "2000-01-01 01:01:01";
                    UserDataManager.instance.Save();
                    return freeModuleWaitTime - (int)Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingFreeModuleTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;
                }

            case RewardType.TrialShip:
                try
                {
                    return shipTrialWaitTime - Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingShipTrialTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;
                }
                catch (FormatException e)
                {
                    // FirebaseAnalytics.LogEvent("FormatExceptionErrorEvent");
                    Debug.LogError("Date Time Parse Error : / " + UserDataManager.instance.currentUserData.usingShipTrialTime + " / " + e);
                    UserDataManager.instance.currentUserData.usingShipTrialTime = "2000-01-01 01:01:01";
                    // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
                    UserDataManager.instance.Save();
                    return freeCrystalWaitTime - (int)Utility.GetTimeDiff(DateTime.ParseExact(UserDataManager.instance.currentUserData.usingShipTrialTime, "yyyy-MM-dd HH:mm:ss", System.Globalization.CultureInfo.InvariantCulture)).TotalSeconds;
                }

            default:
                return 0;
        }
    }

    public void HideDoubleCrystalBtn()
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

    /// <summary>
    /// 보상형 광고 로드
    /// </summary>
    public void LoadRewardedAd()
    {
        string adUnitId;

#if UNITY_ANDROID
        adUnitId = Resources.Load<AdMobSettings>("AdMobSettings").androidRv;
#elif UNITY_IPHONE
             adUnitId = Resources.Load<AdMobSettings>("AdMobSettings").IosRv;
#else
             adUnitId = "unexpected_platform";
#endif

        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("보상형 광고 로드 시도");

        // create our request used to load the ad.
        var adRequest = new AdRequest();


        // send the request to load the ad.
        RewardedAd.Load(adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("보상형 광고 로드 실패 " +
                                   "에러 코드 : " + error);

                    this.TaskDelay(2f, LoadRewardedAd);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedAd = ad;

                if (!(rewardedAd != null && rewardedAd.CanShowAd()))
                    LoadRewardedAd();
            });
    }

    private void RegisterRewardEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(System.String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            LoadRewardedAd();
        };
    }

    #endregion

    #region Banner
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        string adUnitId;

#if UNITY_ANDROID
        adUnitId = Resources.Load<AdMobSettings>("AdMobSettings").androidBn;
#elif UNITY_IPHONE
             adUnitId = Resources.Load<AdMobSettings>("AdMobSettings").IosBn;
#else
             adUnitId = "unexpected_platform";
#endif

        if (bannerView != null)
        {
            bannerView.Destroy();
        }

        AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);

        // Create an empty ad request.
        AdRequest request = new AdRequest();

        // Load the banner with the request.
        bannerView.LoadAd(request);
    }

    public void DestoryBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
    }

    #endregion

    #region Institial
    public void LoadInterstitialAd()
    {
        string adUnitId;

#if UNITY_ANDROID
        adUnitId = Resources.Load<AdMobSettings>("AdMobSettings").androidIs;
#elif UNITY_IPHONE
             adUnitId = Resources.Load<AdMobSettings>("AdMobSettings").IosIs;
#else
             adUnitId = "unexpected_platform";
#endif

        // Clean up the old ad before loading a new one.
        if (interstitial != null)
        {
            interstitial.Destroy();
            interstitial = null;
        }

        Debug.Log("Loading the interstitial ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        InterstitialAd.Load(adUnitId, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                interstitial = ad;
            });
    }

    private void RegisterInstitalEventHandlers(InterstitialAd interstitialAd)
    {
        // Raised when the ad is estimated to have earned money.
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(System.String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");

            LoadInterstitialAd();
        };
        // Raised when the ad failed to open full screen content.
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            LoadInterstitialAd();
        };
    }
    #endregion
}

public enum RewardType { FreeCrystal, FreeModule, TrialShip, GetAll, Revive, DoubleCrystal }


