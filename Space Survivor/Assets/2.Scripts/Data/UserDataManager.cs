using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class UserDataManager : MonoBehaviour
{
    private const string userDataName = "UserData";

    public static UserDataManager instance;

    public UserData currentUserData = new UserData();

    private void Awake()
    {
        instance = this;

        currentUserData = LoadUserData();

        //DontDestroyOnLoad(gameObject);
    }


    public UserData LoadUserData()
    {
        string filePath = Application.persistentDataPath + userDataName;

        //�ҷ����� ����
        if(File.Exists(filePath))
        {
            print("UserData �ҷ����� ����");
            string JsonData = File.ReadAllText(filePath);
            UserData userData = JsonUtility.FromJson<UserData>(JsonData);

            return userData;

        }
        //�ҷ��� ������ ������
        else
        {
            print("UserData�� ��� ���ο� ������ �����մϴ�."); 
            UserData userData = new UserData();

            return userData;
        }
    }

    public void SaveUserData(UserData data)
    {
        string filePath = Application.persistentDataPath + userDataName;

        string JsonData = JsonUtility.ToJson(data);

        File.WriteAllText(filePath, JsonData);
    }

    public void AddCrystalValue(int value)
    {
        /*
        print("1");
        var data = GoogleCloud.instance.LoadUserDataWithCloud();
        print("2 " + data.crystal + data.testString);
        data.crystal += value;
        print("3");
        GoogleCloud.instance.SaveUserDataWithCloud(data);
        //SaveUserData(data);
        print("4");
        */

        var data = LoadUserData();

        data.crystal += value;

        CrystalDisplay.instance.ChangeCrystalText(data.crystal);
        SaveUserData(data);
    }

    public UserData LoadCurrentUserDataFromLocal()
    {
        currentUserData = LoadUserData();

        return currentUserData;
    }

    public void SaveCurrentUserDataToLocal()
    {
        SaveUserData(currentUserData);
    }

    private void OnApplicationPause(bool pause)
    {
        if(pause)
        {
            //GoogleCloud.instance.SaveUserDataWithCloud(currentUserData, (suc, str)=> { print("������ �Ͻ������Ǿ� ���� ������ ����"); });
        }
    }

    private void OnApplicationQuit()
    {
        //GoogleCloud.instance.SaveUserDataWithCloud(currentUserData, (suc, str) => { print("������ ����Ǿ� ���� ������ ����"); });
    }
}
