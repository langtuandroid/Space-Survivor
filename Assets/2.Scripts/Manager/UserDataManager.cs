using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using System;
using Firebase;
using Firebase.Database;
using Google.MiniJSON;
using System.Threading.Tasks;
using Firebase.Extensions;
using System.Linq;
using Unity.VisualScripting;


public enum SaveType
{
    LocalOnly,
    LocalAndFirebase,
    FirebaseOnly
}

public class UserDataManager : MonoBehaviour
{
    private const string userDataName = "UserData";

    [field: SerializeField] public ShipObject startShip { get; private set; }
    [field: SerializeField] public ShipList shipList { get; private set; }

    public static UserDataManager instance;

    public UserData currentUserData = new UserData();

    public List<TextMeshProUGUI> crystalTextList = new List<TextMeshProUGUI>();

    public delegate void OnChangeCrystalValue();
    public static OnChangeCrystalValue onChangeCrystalValue;

    FirebaseDatabase reference;

    public SaveType currentSaveType;

    private void Awake()
    {
        instance = this;

        //currentUserData = LoadUserData();
        //DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        //StartCoroutine(RewardAdsTimeChecking());

        reference = FirebaseDatabase.DefaultInstance;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
            AddCrystalValue(100);

        if (Input.GetKeyDown(KeyCode.D))
        {
            DeleteUserData();
            currentUserData = LoadUserData();

            AddCrystalValue(0);
        }
        else if (Input.GetKeyDown(KeyCode.P))
        {
            SaveToFirebase(currentUserData);
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            LoadFromFirebase();
        }

    }

    //���� ������ �ҷ�����
    public UserData LoadUserData()
    {
        string filePath = Application.persistentDataPath + userDataName;

        //�ҷ����� ����
        if (File.Exists(filePath))
        {
            print("로컬 세이브가 존재하여 불러옵니다. " + filePath);
            string JsonData = File.ReadAllText(filePath);
            UserData userData = JsonUtility.FromJson<UserData>(JsonData);

            return userData;
        }
        //�ҷ��� ������ ������
        else
        {
            print("로컬 세이브가 존재하지 않아 새로 생성합니다. " + filePath);
            UserData userData = new UserData();
            userData.Init();

            var newShipData = Instantiate(startShip);

            // userData.playerHaveShip.Add(newShipData.shipObjectData);

            AddNewShip(newShipData.shipObjectData.shipCode);

            // SaveUserData(userData);

            return userData;
        }
    }

    /// <summary>
    /// 로컬세이브가 존재하는지 확인
    /// </summary>
    public bool CheckLocalSaveExist()
    {
        string filePath = Application.persistentDataPath + userDataName;

        if (File.Exists(filePath))
        {
            return true;
        }
        else
            return false;
    }

    //���� ������ ���̺�
    public void SaveUserData(UserData data)
    {
        string filePath = Application.persistentDataPath + userDataName;

        string JsonData = JsonUtility.ToJson(data);

        File.WriteAllText(filePath, JsonData);
    }

    public void SaveCurrentDate()
    {
        string filePath = Application.persistentDataPath + userDataName;

        string JsonData = JsonUtility.ToJson(currentUserData);

        File.WriteAllText(filePath, JsonData);
    }

    //���� ������ ����
    public void DeleteUserData()
    {
        string filePath = Application.persistentDataPath + userDataName;

        // check if file exists
        if (!File.Exists(filePath))
        {
            print("로컬데이터가 존재하지 않아 삭제를 실패하였습니다.");
        }
        else
        {
            print("로컬데이터를 삭제하였습니다.");

            File.Delete(filePath);

            RefreshEditorProjectWindow();
        }
    }

    void RefreshEditorProjectWindow()
    {
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif
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

        //var data = LoadUserData();

        print(value);

        currentUserData.crystal += value;

        GameObserver.Call(GameObserverType.Game.Crystal);

        foreach (var text in crystalTextList)
        {
            text.text = currentUserData.crystal.ToString();
        }

        CrystalDisplay.instance.ChangeCrystalText(currentUserData.crystal);

        // GoogleCloud.instance.SaveUserDataWithCloud(UserDataManager.instance.currentUserData);
        Save();

        onChangeCrystalValue.Invoke();
        //SaveUserData(currentUserData);
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

    public void AddNewShip(string shipCode)
    {
        currentUserData.playerHaveShip.Add(shipCode);

        Save();
    }

    public bool CheckPlayerHaveShip(string code)
    {
        for (int i = 0; i < currentUserData.playerHaveShip.Count; i++)
        {
            if (currentUserData.playerHaveShip[i].Equals(code))
                return true;
        }

        return false;
    }

    /// <summary>
    /// code로 함선 데이터 가져오기
    /// </summary>
    public ShipObjectData GetShipData(string code)
    {
        for (int i = 0; i < currentUserData.playerHaveShip.Count; i++)
        {
            if (currentUserData.playerHaveShip[i].Equals(code))
                return shipList.shipList.Find((n) => n.shipObjectData.shipCode.Equals(code)).shipObjectData;
        }

        Debug.Log("해당 코드의 함선이 로컬데이터에 없습니다.");
        return shipList.GetShipObject(code).shipObjectData;
    }

    /// <summary>
    /// 최신버전의 함선 데이터 가져오기
    /// </summary>
    public ShipObjectData GetShipData_currentVersion(string code)
    {
        for (int i = 0; i < shipList.shipList.Count; i++)
        {
            if (shipList.shipList[i].shipObjectData.shipCode.Equals(code))
                return shipList.shipList[i].shipObjectData;
        }

        Debug.Log("해당 코드의 함선이 최신버전에 없습니다.");
        return shipList.GetShipObject(code).shipObjectData;
    }

    public void ChangeShipData(ShipObjectData data)
    {
        for (int i = 0; i < currentUserData.playerHaveShip.Count; i++)
        {
            if (currentUserData.playerHaveShip[i].Equals(data.shipCode))
            {
                currentUserData.playerHaveShip[i] = shipList.shipList.Find((n) => n.shipObjectData.shipCode.Equals(data.shipCode)).shipObjectData.shipCode;
            }
        }
    }

    public Sprite GetShipImage(string shipCode)
    {
        for (int i = 0; i < shipList.shipList.Count; i++)
        {
            if (shipList.shipList[i].shipObjectData.shipCode == shipCode)
            {
                return shipList.shipList[i].shipObjectData.shipImage;
            }
        }

        return null;
    }

    public void StageClearSaveData(int stageNum)
    {
        if (!currentUserData.clearedStageNumber.Contains(stageNum))
            currentUserData.clearedStageNumber.Add(stageNum);

        SaveCurrentDate();
    }

    public void Save()
    {
        switch (currentSaveType)
        {
            case SaveType.LocalOnly:
                SaveCurrentDate();
                break;

            case SaveType.LocalAndFirebase:
                SaveCurrentDate();
                SaveToFirebase(currentUserData);
                break;

            case SaveType.FirebaseOnly:
                SaveToFirebase(currentUserData);
                break;
        }
    }

    public void Load(SaveType saveType)
    {

    }

    public void SaveToFirebase(UserData userData)
    {
        if (GameManager.instance != null && GameManager.instance.savingIcon != null)
            GameManager.instance.savingIcon.SetActive(true);

        bool complete = false;

        this.TaskWaitUntil(() =>
        {
            if (GameManager.instance != null && GameManager.instance.savingIcon != null)
                GameManager.instance.savingIcon.SetActive(false);
        }, () => complete);

        string os;

#if UNITY_ANDROID
        os = "AOS";
#endif

#if UNITY_IOS
        os = "IOS";
#endif
        // var task = reference.GetReference("User").Child(FirebaseInit.instance.userID).SetRawJsonValueAsync(JsonUtility.ToJson(userData));

        // this.TaskWaitUntil(() =>
        // {
        //     if (GameManager.instance != null && GameManager.instance.savingIcon != null)
        //         GameManager.instance.savingIcon.SetActive(false);

        // }, () => task.IsCompleted);

        // GameManager.instance.savingIcon.SetActive(false);

        print(JsonUtility.ToJson(userData));

        FirebaseInit.instance.firebaseDatabase.GetReference("User").Child(FirebaseInit.instance.userID).SetValueAsync(JsonUtility.ToJson(userData))
          .ContinueWith(task =>
          {
              if (task.IsCompleted)
              {
                  complete = true;
                  Debug.Log("<color=blue>[Firebase]</color> 서버에 저장을 성공했습니다.");
              }
              else
              {
                  complete = true;
                  Debug.Log("<color=blue>[Firebase]</color> 서버에 저장을 실패했습니다.");
              }
          });
    }

    public void LoadFromFirebase(System.Action onLoadComplete = null)
    {
        string os;

#if UNITY_ANDROID
        os = "AOS";
#endif

#if UNITY_IOS
        os = "IOS";
#endif

        print("12315");


        reference.GetReference("User").Child(FirebaseInit.instance.userID).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Load firebase Save Failed");
            }
            else if (task.IsCompleted)
            {
                if (task.Result.Exists)
                {
                    DataSnapshot snapshot = task.Result;

                    print(snapshot.Value.ToString());

                    currentUserData = JsonUtility.FromJson<UserData>(snapshot.Value.ToString());

                    for (int i = 0; i < snapshot.ChildrenCount; i++)
                        Debug.Log(snapshot.Value);

                    onLoadComplete?.Invoke();
                }
                else
                {
                    onLoadComplete?.Invoke();

                }

            }
        });
    }

    public void LoadDataSnap(DataSnapshot dataSnapshot)
    {
        currentUserData.RemoveAds = bool.Parse(dataSnapshot.Child("RemoveAds").Value.ToString());
        currentUserData.Survey = bool.Parse(dataSnapshot.Child("Survey").Value.ToString());
        currentUserData.crystal = int.Parse(dataSnapshot.Child("crystal").Value.ToString());
        currentUserData.userId = dataSnapshot.Child("userId").Value.ToString();

        currentUserData.usingFreeCrystalTime = dataSnapshot.Child("usingFreeCrystalTime").Value.ToString();
        currentUserData.usingFreeModuleTime = dataSnapshot.Child("usingFreeModuleTime").Value.ToString();
        currentUserData.usingShipTrialTime = dataSnapshot.Child("usingShipTrialTime").Value.ToString();

        currentUserData.version = dataSnapshot.Child("version").Value.ToString();



        // currentUserData.equipModuleSaveDatas = dataSnapshot.Child("equipModuleSaveDatas").Value;


    }
}

