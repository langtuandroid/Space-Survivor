using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump;
using GoogleMobileAds.Ump.Api;
using GoogleMobileAds.Common;

public class GDPR_Manager : MonoBehaviour
{
    // private void Awake()
    // {
    //     AdSettings.AddTestDevice("faf3e7af-272a-4a76-8c0a-18d77f00722e");
    //     AudienceNetworkAds.Initialize();
    // }

    // Start is called before the first frame update
    void Start()
    {
        MobileAds.RaiseAdEventsOnUnityMainThread = true;
        //테스트 할때만 활성화

        // ConsentInformation.Reset();
        // var debugSettings = new ConsentDebugSettings
        // {
        //     DebugGeography = DebugGeography.EEA,
        //     TestDeviceHashedIds =
        // new List<string>
        // {
        //     "C15B15B020858BD77C9A255A51089BA2"
        // }
        // };

        ConsentRequestParameters request = new ConsentRequestParameters
        {
            TagForUnderAgeOfConsent = false,

            // 테스트 할때만 활성화
            // ConsentDebugSettings = debugSettings,
        };

        // Check the current consent information status.
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }


    void OnConsentInfoUpdated(FormError consentError)
    {
        if (consentError != null)
        {
            // Handle the error.
            UnityEngine.Debug.LogError(consentError);
            return;
        }


        // If the error is null, the consent information state was updated.
        // You are now ready to check if a form is available.
        ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
        {
            if (formError != null)
            {
                // Consent gathering failed.
                UnityEngine.Debug.LogError(consentError);
                return;
            }

            // UnityEngine.Debug.Log("UMP : ConsentForm.LoadAndShowConsentFormIfRequired");


            // Consent has been gathered.
            if (ConsentInformation.CanRequestAds())
            {
                // UnityEngine.Debug.Log("UMP : ConsentInformation.CanRequestAds");

            }
        });
    }
}
