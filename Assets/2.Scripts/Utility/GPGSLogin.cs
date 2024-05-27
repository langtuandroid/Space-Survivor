using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
// using Firebase.Analytics;

public class GPGSLogin : MonoBehaviour
{

    public static GPGSLogin instance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Login((suc, str) => SceneManager.LoadScene("MainScene"));
    }

    public static void Login(System.Action<bool, string> callback = null)
    {
        // GPGSManager.Instance.Login(bool autoLogin, Action<bool, ILocalUser> callback = null) 
        // autoLogin : false = 사용자 직접 로그인, true = 자동 로그인

        GPGSBinder.Inst.Login((success, localUser) =>
            { // 로그인을 시도합니다.
                if (success)
                {
                    // 로그인 성공

                    //ilocalUser.userName = 플레이어 이름
                    //ilocalUser.id = 플레이어 아이디
                    //ilocalUser.state = 플레이어 상태
                    //ilocalUser.underage = 플레이어 미성년자 여부

                    //text2.text = "GPGS 로그인 성공 + \n" + ilocalUser.userName + "\n" + ilocalUser.id + "\n" + ilocalUser.state + "\n" + ilocalUser.underage;

                    // FirebaseAnalytics.LogEvent("GPGS_Login_Success");

                    // GoogleCloud.instance.LoadUserDataWithCloud(callback);

                    print("GPGS 로그인 성공.");

                    //SceneManager.LoadScene("MainScene");
                }
                else
                {
                    // 로그인 실패

                    // FirebaseAnalytics.LogEvent("GPGS_Login_Failed");

                    print("GPGS 로그인 실패하여 로컬 데이터를 불러옵니다.");

                    //UserDataManager.instance.LoadCurrentUserDataFromLocal();

                    //text2.text = "GPGS 로그인 실패 + ";

                    UserDataManager.instance.LoadCurrentUserDataFromLocal();

                    // SceneManager.LoadScene("MainScene");
                }
            }); ;

        // GPGSManager.Instance.Login(false, (success, ilocalUser) =>
        // { // 로그인을 시도합니다.
        //     if (success)
        //     {
        //         // 로그인 성공

        //         //ilocalUser.userName = 플레이어 이름
        //         //ilocalUser.id = 플레이어 아이디
        //         //ilocalUser.state = 플레이어 상태
        //         //ilocalUser.underage = 플레이어 미성년자 여부

        //         //text2.text = "GPGS 로그인 성공 + \n" + ilocalUser.userName + "\n" + ilocalUser.id + "\n" + ilocalUser.state + "\n" + ilocalUser.underage;

        //         GoogleCloud.instance.LoadUserDataWithCloud(callback);

        //         print("GPGS 로그인 성공.");

        //         //SceneManager.LoadScene("MainScene");
        //     }
        //     else
        //     {
        //         // 로그인 실패

        //         print("GPGS 로그인 실패하여 로컬 데이터를 불러옵니다.");

        //         //UserDataManager.instance.LoadCurrentUserDataFromLocal();

        //         //text2.text = "GPGS 로그인 실패 + ";

        //         UserDataManager.instance.LoadCurrentUserDataFromLocal();

        //         SceneManager.LoadScene("MainScene");
        //     }
        // });
    }

    public void checkLogin()
    {
        if (GPGSManager.Instance.IsAuthenticated)
        { // 로그인 여부를 확인합니다.
          // Google Play Game Service 로그인 상태

            //text1.text = "GPGS 로그인 성공 ";
        }
        else
        {

            //text1.text = "GPGS 로그인 실패 ";
            // Google Play Game Service 비로그인 상태
        }
    }
}
