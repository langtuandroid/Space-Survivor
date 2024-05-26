using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.UI;


public class AdTestBtn : MonoBehaviour
{

    private void Start()
    {
        GetComponent<Button>().onClick.AddListener(OnButtonClick);
    }
    public void OnButtonClick()
    {
        MobileAds.OpenAdInspector(error =>
        {
            // Error will be set if there was an issue and the inspector was not displayed.

            Debug.LogError(error);
        });
    }
}
