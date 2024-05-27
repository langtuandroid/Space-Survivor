using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;

public class BottomBanner : MonoBehaviour
{
    private BannerView bannerView;

    //Test ID ca-app-pub-3940256099942544/6300978111
    [SerializeField] private string Aos_bannerAdUnitId = "ca-app-pub-5179254807136480/2536343741";
    //Test ID ca-app-pub-3940256099942544/2934735716
    [SerializeField] private string Ios_bannerAdUnitId = "";

    public static BottomBanner instance;


    private void Awake()
    {
        instance = this;
    }

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(initStatus => { });

        StartCoroutine(requestBanner());

        IEnumerator requestBanner()
        {
            while (true)
            {
                yield return null;

                if (IAPManager.instance.initialized)
                {
                    if (!UserDataManager.instance.currentUserData.RemoveAds && !IAPManager.instance.HadPurchased())
                    {
                        this.RequestBanner();

                        print("NoAds가 없기 때문에 배너 광고 시작");
                    }
                    else
                    {
                        print("NoAds가 있기 때문에 배너광고가 호출되지 않습니다.");
                    }

                    break;
                }
            }
        }
    }

    private void RequestBanner()
    {
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-5179254807136480/2536343741";
#elif UNITY_IPHONE
            string adUnitId = Ios_bannerAdUnitId;
#else
            string adUnitId = "unexpected_platform";
#endif

        // Clean up banner ad before creating a new one.
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }

        AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        this.bannerView = new BannerView(adUnitId, adaptiveSize, AdPosition.Bottom);

        // Create an empty ad request.
        // AdRequest request = new AdRequest.Builder().Build();
        AdRequest request = new AdRequest();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }

    public void DestoryBanner()
    {
        if (this.bannerView != null)
        {
            this.bannerView.Destroy();
        }
    }
}
