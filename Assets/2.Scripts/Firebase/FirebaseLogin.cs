using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public enum LoginType
{
    NONE,
    GUEST,
    GOOGLE,
    APPLE
}

public class FirebaseLogin : MonoBehaviour
{
    [SerializeField] private Button guestBtn;
    [SerializeField] private Button googleBtn;
    [SerializeField] private Button appleBtn;
    [SerializeField] private GameObject loginParent;
    [SerializeField] private GameObject progressParent;

    private void Start()
    {
#if UNITY_ANDROID
        googleBtn.gameObject.SetActive(true);
        appleBtn.gameObject.SetActive(false);
#endif

#if UNITY_IOS
googleBtn.gameObject.SetActive(true);
        appleBtn.gameObject.SetActive(true);
#endif

        FirebaseInit.instance.onLogin += DeactiveLoginButtons;
        FirebaseInit.instance.onCancel += ActiveLoginButtons;


        if (ES3.KeyExists("FastLogin") && ES3.KeyExists("FastLoginType"))
            Login(ES3.Load<LoginType>("FastLoginType"));
        else
            loginParent.SetActive(true);

        guestBtn.onClick.AddListener(() => Login(LoginType.GUEST));
        googleBtn.onClick.AddListener(() => Login(LoginType.GOOGLE));
        appleBtn.onClick.AddListener(() => Login(LoginType.APPLE));

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            // 인터넷에 연결되어 있지 않음
            Message.GeneateMessage(() => SceneManager.LoadScene("Login"), transform, "UI", "Message_internetNotConnect");
        }
    }

    void Login(LoginType type)
    {
        FirebaseInit.instance.onLogin?.Invoke();

        progressParent.gameObject.SetActive(true);

        if (type == LoginType.GUEST)
            FirebaseInit.instance.Login_Anonymouse();

        if (type == LoginType.GOOGLE)
            FirebaseInit.instance.Login_Google();

        if (type == LoginType.APPLE)
            FirebaseInit.instance.Login_Apple();

        guestBtn.interactable = false;
        googleBtn.interactable = false;
        appleBtn.interactable = false;
    }

    public void ActiveLoginButtons()
    {
        guestBtn.interactable = true;
        googleBtn.interactable = true;
        appleBtn.interactable = true;
    }

    public void DeactiveLoginButtons()
    {
        guestBtn.interactable = false;
        googleBtn.interactable = false;
        appleBtn.interactable = false;
    }
}
