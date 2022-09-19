using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAPManager : MonoBehaviour
{
    public void PurchaseRemoveAds_Success()
    {
        print("���� ���� ���� ����");

        UserDataManager.instance.currentUserData.RemoveAds = true;

        UserDataManager.instance.SaveUserData(UserDataManager.instance.currentUserData);
    }

    public void PurchaseRemoveAds_Failed()
    {
        Debug.LogError("�������� ���� ����");
    }
}
