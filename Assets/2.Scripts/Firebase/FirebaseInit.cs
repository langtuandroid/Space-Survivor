using System.Collections;
using AppleAuth;
using AppleAuth.Enums;
using AppleAuth.Interfaces;
using AppleAuth.Native;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Crashlytics;
using UnityEngine.SceneManagement;
// using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.Auth;
using Firebase.Database;
using Newtonsoft.Json;
using Google;
using System.Threading.Tasks;
using System;
using System.Text;
using System.Security.Cryptography;


public class FirebaseInit : MonoBehaviour
{
    // Start is called before the first frame update

    FirebaseApp _app;
    FirebaseAuth firebaseAuth;
    public FirebaseDatabase firebaseDatabase;

    IAppleAuthManager appleAuthManager;

    public string userID;

    string googleIdToken = "YOUR_GOOGLE_ID_TOKEN";
    string googleAccessToken = "YOUR_GOOGLE_ACCESS_TOKEN";

    public Action onLogin;
    public Action onCancel;


    bool ready = false;
    public LoginType currentLoginType;

    public static FirebaseInit instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        this.TaskWaitUntil(() => SceneManager.LoadScene("MainScene"), () => ready == true);

        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                _app = FirebaseApp.DefaultInstance;

                // FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);

                firebaseAuth = FirebaseAuth.DefaultInstance;

                Crashlytics.IsCrashlyticsCollectionEnabled = true;

                firebaseDatabase = FirebaseDatabase.DefaultInstance;
                firebaseDatabase.SetPersistenceEnabled(false);

                print("Firebase dependencies 연동 선공 + " + task.Result);

                GoogleMobileAds.Api.Mediation.AppLovin.AppLovin.Initialize();
            }
            else
            {
                Debug.LogError("Firebase dependencies failed + " + task.Result);

            }
        });
    }

    public void Login_Anonymouse()
    {
        firebaseAuth.SignInAnonymouslyAsync().ContinueWith(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogError("SignInAnonymouslyAsync was canceled.");
                onCancel?.Invoke();
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
                onCancel?.Invoke();
                return;
            }
            if (task.IsCompleted)
            {
                Login(task, LoginType.GUEST);
            };
        });
    }

    public void Login_Google()
    {

        if (GoogleSignIn.Configuration == null)
        {
            GoogleSignIn.Configuration = new GoogleSignInConfiguration
            {
                WebClientId = "997553477781-5n1nk4nri21t9q3i4ob0sk2slvjq1rl7.apps.googleusercontent.com",
                RequestEmail = true,
                RequestIdToken = true,
                UseGameSignIn = false
            };
        }

        GoogleSignIn.DefaultInstance.SignIn().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                Debug.LogWarning("[DB] GoogleSignIn was canceled.");
                onCancel?.Invoke();
                return;
            }
            if (task.IsFaulted)
            {
                Debug.LogError("[DB] GoogleSignIn encountered an error: " + task.Exception);
                onCancel?.Invoke();
                return;
            }
            if (task.IsCompleted)
            {
                firebaseAuth.SignInWithCredentialAsync(GoogleAuthProvider.GetCredential(task.Result.IdToken, null)).ContinueWithOnMainThread(task =>
                {
                    Login(task, LoginType.GOOGLE);
                });

            }
        });
    }



    void AppleSign(Action<Credential> credential)
    {
        var rawNonce = GenerateRandomString(32);
        var nonce = GenerateSHA256NonceFromRawNonce(rawNonce);

        var loginArgs = new AppleAuthLoginArgs(LoginOptions.IncludeEmail, nonce);

        appleAuthManager.LoginWithAppleId(loginArgs, appleCredential =>
        {
            var appleIdCredential = appleCredential as IAppleIDCredential;
            var identityToken = Encoding.UTF8.GetString(appleIdCredential.IdentityToken);
            var authorizationCode = Encoding.UTF8.GetString(appleIdCredential.AuthorizationCode);
            var firebaseCredential = OAuthProvider.GetCredential(
                "apple.com",
                identityToken,
                rawNonce,
                authorizationCode);
            credential?.Invoke(firebaseCredential);
        }, (e) => onCancel?.Invoke());
    }

    public void Login_Apple()
    {


        AppleSign(credential =>
                {
                    firebaseAuth.SignInWithCredentialAsync(credential).ContinueWithOnMainThread(task => Login(task, LoginType.APPLE));
                });
    }

    string GenerateRandomString(int length)
    {
        const string charset = "0123456789ABCDEFGHIJKLMNOPQRSTUVXYZabcdefghijklmnopqrstuvwxyz-._";
        var cryptographicallySecureRandomNumberGenerator = new RNGCryptoServiceProvider();
        var result = string.Empty;
        var remainingLength = length;

        var randomNumberHolder = new byte[1];
        while (remainingLength > 0)
        {
            var randomNumbers = new List<int>(16);
            for (var randomNumberCount = 0; randomNumberCount < 16; randomNumberCount++)
            {
                cryptographicallySecureRandomNumberGenerator.GetBytes(randomNumberHolder);
                randomNumbers.Add(randomNumberHolder[0]);
            }

            for (var randomNumberIndex = 0; randomNumberIndex < randomNumbers.Count; randomNumberIndex++)
            {
                if (remainingLength == 0)
                {
                    break;
                }

                var randomNumber = randomNumbers[randomNumberIndex];
                if (randomNumber < charset.Length)
                {
                    result += charset[randomNumber];
                    remainingLength--;
                }
            }
        }

        return result;
    }

    string GenerateSHA256NonceFromRawNonce(string rawNonce)
    {
        var sha = new SHA256Managed();
        var utf8RawNonce = Encoding.UTF8.GetBytes(rawNonce);
        var hash = sha.ComputeHash(utf8RawNonce);

        var result = string.Empty;
        for (var i = 0; i < hash.Length; i++)
        {
            result += hash[i].ToString("x2");
        }

        return result;
    }


    public void Login(Task<FirebaseUser> task, LoginType type)
    {
        var user = task.Result;
        userID = user.UserId;

        ES3.Save("FastLogin", userID);
        ES3.Save("FastLoginType", type);

        currentLoginType = type;

        OnSuccessLogin();

        print($"<color=green>[Firebase Login]</color> {type.ToString()} Authentication User ID : " + userID);
    }

    public void Logout()
    {
        ES3.DeleteKey("FastLogin");
        ES3.DeleteKey("FastLoginType");
        SceneManager.LoadScene("Login");
    }

    void OnSuccessLogin()
    {
        LoadFromFirebase(() => ready = true);
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

        print(userID);

        firebaseDatabase.GetReference("User").Child(userID).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                // Debug.LogError("Load firebase Save Failed");
                Debug.Log("<color=blue>[Firebase]</color> 해당 User의 userID가 없어 로컬데이터를 불러옵니다.");

                UserDataManager.instance.LoadCurrentUserDataFromLocal();
                onLoadComplete?.Invoke();
            }
            else if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;

                // for (int i = 0; i < snapshot.ChildrenCount; i++)
                // Debug.Log(snapshot.Child(i.ToString()).Child("username").Value);

                // print(snapshot.Value.ToString());

                Debug.Log("<color=blue>[Firebase]</color> 해당 User의 데이터를 불러왔습니다.");
                // print(task.Result.Exists);

                foreach (var data in snapshot.Children)
                {
                    print(data.Value.ToString());
                }

                if (task.Result.Exists)
                {
                    onLoadComplete?.Invoke();
                    print(snapshot.Value.ToString());
                    // var userData = JsonConvert.DeserializeObject<UserData>(JsonUtility.ToJson(snapshot.Value.ToString()));
                    UserDataManager.instance.currentUserData = JsonConvert.DeserializeObject<UserData>(snapshot.Value.ToString());

                    // for(int i = 0; i < UserDataManager.instance.currentUserData.equipModuleSaveDatas.Length; i++)
                    // {
                    //     UserDataManager.instance.currentUserData.equipModuleSaveDatas[i].type = (UpgradeModuleType)i + 1;
                    // }

                    // print(JsonUtility.ToJson(userData));
                    // UserDataManager.instance.currentUserData = JsonUtility.FromJson<UserData>(snapshot.Value.ToString());

                    // UserDataManager.instance.LoadDataSnap(snapshot);

                }
                else
                {
                    onLoadComplete?.Invoke();
                }
            }
        });
    }
}
