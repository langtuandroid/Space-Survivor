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

        DontDestroyOnLoad(gameObject);
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

        var data = LoadUserData();

        data.crystal += value;
        
        SaveUserData(data);
    }

}
