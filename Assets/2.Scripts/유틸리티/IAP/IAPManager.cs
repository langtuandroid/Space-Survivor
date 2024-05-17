using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Purchasing;
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using TMPro;
// using Firebase.Analytics;
using System;
using UnityEngine.UI;

public class IAPManager : MonoBehaviour, IStoreListener
{
    IStoreController m_StoreController; // The Unity Purchasing system.
    IExtensionProvider m_Extension;

    //Your products IDs. They should match the ids of your products in your store.
    //com.TeroGames.spacesurvivor.
    public string removeAdsId = "removeads";
    public string starterPackId = "starterpack";
    public string megaPackId = "megapack";
    public string ultraPackId = "ultrapack";

    public TextMeshProUGUI removeAdsText;
    public TextMeshProUGUI starterPackText;
    public TextMeshProUGUI megaPackText;
    public TextMeshProUGUI ultraPackText;

    [Space]

    public GameObject[] removeAdsButtons;

    [field: SerializeField] public bool initialized { get; private set; } = false;

    public static IAPManager instance;

    private void Awake()
    {
        instance = this;
    }


    private void Start()
    {
        //UpdateUI();

        InitializePurchasing();

        StartCoroutine(InitIAP());

        IEnumerator InitIAP()
        {
            while (true)
            {
                yield return null;

                print("iap 동기화 시도중");
                if (m_StoreController != null)
                {
                    initialized = true;
                    HadPurchased();
                    UpdateUI2();
                    break;
                }
                else
                {

                }
            }
        }

    }

    public void PurchaseRemoveAds()
    {
        m_StoreController.InitiatePurchase(removeAdsId);
    }

    public void PurchageStarterPack()
    {
        m_StoreController.InitiatePurchase(starterPackId);
    }

    public void PurchageMegaPack()
    {
        m_StoreController.InitiatePurchase(megaPackId);
    }

    public void PurchageUltraPack()
    {
        m_StoreController.InitiatePurchase(ultraPackId);
    }

    public void PurchageStarterPack_Success()
    {
        UserDataManager.instance.currentUserData.RemoveAds = true;

        UserDataManager.instance.AddCrystalValue(1000);

        EventManager.instance.CustomEvent(AnalyticsType.IAP, "IAP_StarterPackPurchaseSuccess");

        CheckRemoveAdsHasPurchase();

        UpdateUI2();
    }

    public void PurchageMegaPack_Success()
    {
        UserDataManager.instance.currentUserData.RemoveAds = true;

        UserDataManager.instance.AddCrystalValue(2500);

        EventManager.instance.CustomEvent(AnalyticsType.IAP, "IAP_MegaPackPurchaseSuccess");

        CheckRemoveAdsHasPurchase();

        UpdateUI2();

    }

    public void PurchageUltraPack_Success()
    {
        UserDataManager.instance.currentUserData.RemoveAds = true;

        UserDataManager.instance.AddCrystalValue(5000);

        EventManager.instance.CustomEvent(AnalyticsType.IAP, "IAP_UltraPackPurchaseSuccess");

        CheckRemoveAdsHasPurchase();

        UpdateUI2();

    }

    public void PurchaseRemoveAds_Success()
    {
        UserDataManager.instance.currentUserData.RemoveAds = true;

        // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
        UserDataManager.instance.Save();

        EventManager.instance.CustomEvent(AnalyticsType.IAP, "IAP_RemoveAdsPurchaseSuccess");

        CheckRemoveAdsHasPurchase();

        UpdateUI2();
    }

    private void CheckRemoveAdsHasPurchase()
    {
        var product = m_StoreController.products.WithID(removeAdsId);

        if (!product.hasReceipt)
        {
            //PurchaseRemoveAds();
            print("스타터팩 이상을 구매하였고 광고제거를 구매한적이 없기 때문에 광고제거 상품도 함께 구매");
        }

        UpdateUI2();
    }



    void InitializePurchasing()
    {
        var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

        //Add products that will be purchasable and indicate its type.
        // builder.AddProduct(removeAdsId, ProductType.NonConsumable);
        // builder.AddProduct(starterPackId, ProductType.Consumable);
        // builder.AddProduct(megaPackId, ProductType.Consumable);
        // builder.AddProduct(ultraPackId, ProductType.Consumable);

        builder.AddProduct(removeAdsId, ProductType.NonConsumable,
        new IDs()
        {
            {"removeads", GooglePlay.Name}
        });

        builder.AddProduct(starterPackId, ProductType.Consumable,
        new IDs()
        {
            {"starterpack", GooglePlay.Name}
        });

        builder.AddProduct(ultraPackId, ProductType.Consumable,
        new IDs()
        {
            {"ultrapack", GooglePlay.Name}
        });

        builder.AddProduct(megaPackId, ProductType.Consumable,
        new IDs()
        {
            {"megapack", GooglePlay.Name}
        });

        UnityPurchasing.Initialize(this, builder);

        //HadPurchased();
    }

    // public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    // {
    //     Debug.Log("In-App Purchasing successfully initialized");

    //     print("IAP 연동 성공");
    //     m_StoreController = controller;

    // }

    public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
    {
        Debug.Log("In-App Purchasing successfully initialized");

        this.m_StoreController = controller;
        this.m_Extension = extensions;

        //UpdateUI();
    }

    public void OnInitializeFailed(InitializationFailureReason error)
    {
        Debug.Log($"In-App Purchasing initialize failed: {error}");

        print("IAP 연동 실패 + " + error);
    }

    public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args)
    {
        //Retrieve the purchased product
        var product = args.purchasedProduct;

        //Add the purchased product to the players inventory
        if (product.definition.id == starterPackId)
        {
            PurchageStarterPack_Success();
        }
        else if (product.definition.id == megaPackId)
        {
            PurchageMegaPack_Success();
        }
        else if (product.definition.id == ultraPackId)
        {
            PurchageUltraPack_Success();
        }

        Debug.Log($"Purchase Complete - Product: {product.definition.id}");

        //We return Complete, informing IAP that the processing on our side is done and the transaction can be closed.
        return PurchaseProcessingResult.Complete;
    }

    public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason)
    {
        Debug.Log($"Purchase failed - Product: '{product.definition.id}', PurchaseFailureReason: {failureReason}");
    }

    bool IsSubscribedTo(Product subscription)
    {
        // If the product doesn't have a receipt, then it wasn't purchased and the user is therefore not subscribed.
        if (subscription.receipt == null)
        {
            return false;
        }

        //The intro_json parameter is optional and is only used for the App Store to get introductory information.
        var subscriptionManager = new SubscriptionManager(subscription, null);

        // The SubscriptionInfo contains all of the information about the subscription.
        // Find out more: https://docs.unity3d.com/Packages/com.unity.purchasing@3.1/manual/UnityIAPSubscriptionProducts.html
        var info = subscriptionManager.getSubscriptionInfo();

        return info.isSubscribed() == Result.True;
    }

    void UpdateUI()
    {
        var subscriptionProduct = m_StoreController.products.WithID(removeAdsId);

        try
        {
            removeAdsText.text = m_StoreController.products.WithID(removeAdsId).metadata.localizedPriceString;
            starterPackText.text = m_StoreController.products.WithID(starterPackId).metadata.localizedPriceString;
            megaPackText.text = m_StoreController.products.WithID(megaPackId).metadata.localizedPrice.ToString();
            ultraPackText.text = m_StoreController.products.WithID(ultraPackId).metadata.localizedPrice.ToString();

            var isSubscribed = IsSubscribedTo(subscriptionProduct);

            string text = isSubscribed ? "You are subscribed" : "You are not subscribed";
            print(text);
        }
        catch (StoreSubscriptionInfoNotSupportedException)
        {
            var receipt = (Dictionary<string, object>)MiniJson.JsonDecode(subscriptionProduct.receipt);
            var store = receipt["Store"];
            print(
                "Couldn't retrieve subscription information because your current store is not supported.\n" +
                    $"Your store: \"{store}\"\n\n" +
                    "You must use the App Store, Google Play Store or Amazon Store to be able to retrieve subscription information.\n\n" +
                    "For more information, see README.md");
        }
    }

    private void UpdateUI2()
    {
        try
        {
            var product = m_StoreController.products.WithID(removeAdsId);
            print(UserDataManager.instance);
            print(UserDataManager.instance.currentUserData);
            print(UserDataManager.instance.currentUserData.RemoveAds);
            print(product.hasReceipt);

            if (UserDataManager.instance.currentUserData.RemoveAds)
            {
                foreach (GameObject btn in removeAdsButtons)
                {
                    btn.SetActive(false);
                }

                print("광고 제거를 구매하였기 때문에 버튼 비활성화");
                BottomBanner.instance.DestoryBanner();
            }
        }
        catch (NullReferenceException e)
        {
            Debug.LogError(e + " \n " + e.HResult + " \n " + e.InnerException + " \n " + e.Message + " \n " + e.Source);
        }

    }

    /// <summary>
    /// 광고 제거 기능이 있는 iap를 구매한적 있는지 반환
    /// </summary>
    public bool HadPurchased()
    {
        var product = m_StoreController.products.WithID(removeAdsId);
        bool purchased = false;

        print(product.receipt);

        if (product.hasReceipt)
        {
            UserDataManager.instance.currentUserData.RemoveAds = true;

            //UserDataManager.instance.SaveUserData(UserDataManager.instance.currentUserData);

            // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
            UserDataManager.instance.Save();

            print("광고제거를 구매한 적이 있는 유저");
            purchased = true;
        }

        product = m_StoreController.products.WithID(starterPackId);

        print(product.receipt);

        if (product.hasReceipt)
        {
            UserDataManager.instance.currentUserData.RemoveAds = true;

            //UserDataManager.instance.SaveUserData(UserDataManager.instance.currentUserData);

            // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
            UserDataManager.instance.Save();

            print("스타터팩을 구매한 적이 있는 유저");
            purchased = true;
        }

        product = m_StoreController.products.WithID(megaPackId);

        print(product.receipt);

        if (product.hasReceipt)
        {
            UserDataManager.instance.currentUserData.RemoveAds = true;

            //UserDataManager.instance.SaveUserData(UserDataManager.instance.currentUserData);

            // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
            UserDataManager.instance.Save();

            print("메가팩을 구매한 적이 있는 유저");
            purchased = true;
        }

        product = m_StoreController.products.WithID(ultraPackId);

        print(product.receipt);

        if (product.hasReceipt)
        {
            UserDataManager.instance.currentUserData.RemoveAds = true;

            //UserDataManager.instance.SaveUserData(UserDataManager.instance.currentUserData);

            // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
            UserDataManager.instance.Save();

            print("울트라팩을 구매한 적이 있는 유저");
            purchased = true;
        }

        return purchased;
    }

    //구매 복원
    public void PurchaseRestore(IStoreController controller, IExtensionProvider extensions)
    {
        extensions.GetExtension<IAppleExtensions>().RestoreTransactions(result =>
        {
            if (result)
            {
                // This does not mean anything was restored,
                // merely that the restoration process succeeded.
            }
            else
            {
                // Restoration failed.
            }
        });
    }

    public void OnInitializeFailed(InitializationFailureReason error, string message)
    {
        throw new NotImplementedException();
    }
}
