using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
// using Firebase.Analytics;
// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
// using GooglePlayGames.BasicApi.SavedGame;
// using GooglePlayGames.BasicApi.Events;
using System;
using System.IO;

public class GoogleCloud : MonoBehaviour
{
    public static GoogleCloud instance;

    public delegate void Action<in T>(T obj);

    delegate void callback();

    private bool isSaving = false;
    private Queue<saveData> saveDataQueue = new Queue<saveData>();

    [SerializeField] private Slider sceneLoadingSlider;


    public class saveData
    {
        public UserData userData = null;

        public System.Action<bool, string> callback = null;

        public saveData(UserData userData, System.Action<bool, string> callback = null)
        {
            this.userData = userData;
            this.callback = callback;
        }
    }

    private void Awake()
    {
        instance = this;

        UnityEngine.Rendering.DebugManager.instance.enableRuntimeUI = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            // DelectWithCloud(null);
        }
    }

    public void SaveUserDataWithCloud(UserData userData, System.Action<bool, string> callback = null)
    {
        if (isSaving)
        {
            saveData savedata = new saveData(userData, callback);
            saveDataQueue.Clear();
            saveDataQueue.Enqueue(savedata);
            print(saveDataQueue.Count);
            return;
        }

        print("saving Start");

        isSaving = true;
        

        userData.version = Application.version;

        UserDataManager.instance.SaveToFirebase(userData);

        UserDataManager.instance.SaveCurrentUserDataToLocal();

        //     GPGSManager.Instance.SaveWithCloud("USERDATA", serializedData, (success) =>
        //     { // �����͸� Ŭ���忡 �����մϴ�.
        //         if (success)
        //         {
        //             // ������ ���� ������

        //             print("GPGS Cloud에 저장을 성공하였습니다.");

        //             print(DateTime.Now.ToString());

        //             print(userData.crystal);

        //             if (callback != null)
        //                 callback.Invoke(true, "������ �Ͻ������Ǿ� �������� ������");

        //             FirebaseAnalytics.LogEvent("GPGS_SaveDataFromCloud_Success");

        //             //Time.timeScale = 1f;
        //         }
        //         else
        //         {
        //             // ������ ���� ���н�

        //             print("GPGS Cloud에 저장을 실패하였습니다.");

        //             if (callback != null)
        //                 callback.Invoke(true, "������ �Ͻ������Ǿ� �������� ������");

        //             //Time.timeScale = 1f;

        //             FirebaseAnalytics.LogEvent("GPGS_SaveDataFromCloud_Failed");
        //         }

        //         print("saving End");
        //         isSaving = false;
        //         if (GameManager.instance != null && GameManager.instance.savingIcon != null)
        //             GameManager.instance.savingIcon.SetActive(false);

        //         if (saveDataQueue.Count > 0)
        //         {
        //             print(saveDataQueue.Count);
        //             var data = saveDataQueue.Dequeue();
        //             SaveUserDataWithCloud(data.userData, data.callback);
        //         }

        //     });
        // }

        // public UserData LoadUserDataWithCloud(System.Action<bool, string> callback = null)
        // {
        //     UserData userData = new UserData();
        //     userData.Init();
        //     //Time.timeScale = 0f;

        //     GPGSManager.Instance.LoadWithCloud("USERDATA", (success, serializedData) =>
        //     { // �����͸� Ŭ���忡�� �ҷ��ɴϴ�.
        //         if (success)
        //         {
        //             print("GPGS Cloud에서 불러온 유저 데이터 : " + serializedData);

        //             if (UserDataManager.instance.CheckLocalSaveExist())
        //             {
        //                 string filePath = Application.persistentDataPath + "UserData";
        //                 string JsonData = File.ReadAllText(filePath);
        //                 print("로컬세이브 : " + JsonData);
        //             }

        //             if (serializedData.Equals(""))
        //             {
        //                 print("GPGS Cloud에 세이브가 존재하지 않아 새로 생성합니다.");

        //                 //만약 로컬 세이브가 있다면 로컬세이브를 적용
        //                 if (UserDataManager.instance.CheckLocalSaveExist())
        //                 {
        //                     print("로컬데이터가 존재하여 불러옵니다.");

        //                     var data = UserDataManager.instance.LoadUserData();

        //                     SaveUserDataWithCloud(data);
        //                 }
        //                 else
        //                 {
        //                     print("로컬데이터가 존재하지 않아 새로 세이브를 생성합니다.");
        //                     var newShipData = Instantiate(UserDataManager.instance.startShip);

        //                     userData.playerHaveShip.Add(newShipData.shipObjectData);

        //                     SaveUserDataWithCloud(userData);
        //                 }
        //             }
        //             else
        //             {
        //                 print("GPGS Cloud에서 성공적으로 세이브를 불러왔습니다.");

        //                 userData = JsonUtility.FromJson<UserData>(serializedData);  //JsonConvert.DeserializeObject<UserData>(serializedData);
        //                 print(userData.playerHaveShip);

        //                 foreach (ShipObjectData data in userData.playerHaveShip)
        //                 {
        //                     foreach (ShipUpgradeModules module in data.shipUpgradeModuleList)
        //                     {
        //                         print(data.shipCode + " / " + module.upgradeType + " / " + module.currentUpgrade);
        //                     }
        //                 }

        //                 FirebaseAnalytics.LogEvent("GPGS_LoadDataFromCloud_Success");
        //             }

        //             UserDataManager.instance.currentUserData = userData;

        //             //if (callback != null)
        //             //    callback.Invoke(true, "MainScene");

        //             //메인씬 로딩바 구현
        //             StartCoroutine(LoadAsynchronosly());
        //             IEnumerator LoadAsynchronosly()
        //             {
        //                 AsyncOperation operation = SceneManager.LoadSceneAsync("MainScene");

        //                 while (!operation.isDone)
        //                 {
        //                     float progress = Mathf.Clamp01(operation.progress / .9f);

        //                     sceneLoadingSlider.value = progress;

        //                     yield return null;
        //                 }
        //             }
        //         }
        //         else
        //         {
        //             print("GPGS Cloud에서 유저 데이터를 불러오지 못해 로컬 데이터를 불러옵니다.");
        //             //userData = UserDataManager.instance.LoadUserData();

        //             UserDataManager.instance.LoadCurrentUserDataFromLocal();

        //             FirebaseAnalytics.LogEvent("GPGS_LoadDataFromCloud_Failed");

        //             //Time.timeScale = 1f;
        //         }
        //     });

        //     return userData;
        // }

        // public void DelectWithCloud(Action onComplete)
        // {
        //     GPGSManager.Instance.DelectWithCloud("USERDATA", (success) =>
        //     { // �����͸� Ŭ���忡�� �����մϴ�..
        //         if (success)
        //         {
        //             Debug.Log("구글 클라우드 세이브를 삭제하였습니다.");
        //             // ������ ���� ������

        //             onComplete.Invoke();
        //         }
        //         else
        //         {
        //             Debug.LogWarning("구글 클라우드 세이브를 삭제에 실패하였습니다.");
        //             // ������ ���� ���н�

        //             onComplete.Invoke();
        //         }
        //     });
        // }

        // public void GetGoogleInfo()
        // {
        //     //return PlayGamesPlatform.Instance.GetUserId();
        // }

        // private void OnApplicationQuit()
        // {
        //     GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
        // }


        //================================================================================================================

        // public void TestUserDataSave()
        // {
        //     UserData userData = new UserData();

        //     userData.testString = inputField.text;

        //     SaveUserDataWithCloud(userData);
        // }

        // public void TestUserDataLoad()
        // {
        //     UserData userData = LoadUserDataWithCloud();

        //     print(userData.testString);

        //     testText.text = userData.testString;
        // }
    }
}
