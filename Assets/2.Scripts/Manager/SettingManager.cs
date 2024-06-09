using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using System.Linq;
using System;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;


public class SettingManager : MonoBehaviour, IUIBase
{
    //public SettingData settingData = new SettingData();

    [Header("MainMenuSetting")]
    public GameObject settingPanel;
    public Slider mainMenuSoundSlider;
    public Slider mainMenuVibrationSlider;
    public Image soundSliderFill;
    public Image vibrationSliderFill;
    public TextMeshProUGUI soundOnOffText;
    public TextMeshProUGUI vibrationOnOffText;
    public TMP_Dropdown LanguageDropdown;
    public Text versionText;

    [BoxGroup("User Info")][SerializeField] Text userID;
    [BoxGroup("User Info")][SerializeField] GameObject icon_Google, icon_Apple, icon_Guest;


    [Space]

    public Color onColor;
    public Color offColor;

    [Space]
    public GameObject goToMainMenuBtn;
    [SerializeField] private GameObject InGame;
    [SerializeField] private Button mainMenuBtn;

    public void TurnSettingPanel()
    {
        settingPanel.SetActive(!settingPanel.activeSelf);

        if (settingPanel.activeSelf)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;

        goToMainMenuBtn.SetActive(InGame.activeSelf);
    }

    public void OnChangeSoundSlider()
    {
        if (mainMenuSoundSlider.value == 0)
        {
            soundSliderFill.color = offColor;
            soundOnOffText.text = "OFF";
            AudioListener.volume = 0;
        }
        else
        {
            soundSliderFill.color = onColor;
            soundOnOffText.text = "ON";
            AudioListener.volume = 1;
        }

        ES3.Save<int>("Sound", (int)mainMenuSoundSlider.value);
    }

    public void OnChangeVibrationSlider()
    {
        if (mainMenuVibrationSlider.value == 0)
        {
            vibrationSliderFill.color = offColor;
            vibrationOnOffText.text = "OFF";
        }
        else
        {
            vibrationSliderFill.color = onColor;
            vibrationOnOffText.text = "ON";
        }

        ES3.Save<int>("Vibration", (int)mainMenuVibrationSlider.value);
    }

    public void OnClickMainMenuBtn()
    {
        SceneManager.LoadScene("MainScene");
        Hide();
        // mainMenuBtn.onClick.Invoke();
    }

    public void LoadSettingData()
    {
        mainMenuSoundSlider.value = ES3.KeyExists("Sound") ? ES3.Load<int>("Sound") : 1;
        mainMenuVibrationSlider.value = ES3.KeyExists("Vibration") ? ES3.Load<int>("Vibration") : 1;

        OnChangeSoundSlider();
        OnChangeVibrationSlider();
    }

    void UpdateUI()
    {
        LoadSettingData();

        StartCoroutine(localizeDropBox());
        StartCoroutine(LocalInit());

        versionText.text = "version : " + Application.version;

        userID.text = "ID : " + FirebaseInit.instance.userID.ToString();
        switch (FirebaseInit.instance.currentLoginType)
        {
            case LoginType.GUEST:
                icon_Guest.SetActive(true);
                break;

            case LoginType.GOOGLE:
                icon_Google.SetActive(true);
                break;

            case LoginType.APPLE:
                icon_Apple.SetActive(true);
                break;
        }
    }

    IEnumerator localizeDropBox()
    {
        // Wait for the localization system to initialize, loading Locales, preloading etc.
        yield return LocalizationSettings.InitializationOperation;

        // Generate list of available Locales
        var options = new List<TMP_Dropdown.OptionData>();
        int selected = 0;
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; ++i)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            if (LocalizationSettings.SelectedLocale == locale)
                selected = i;
            options.Add(new TMP_Dropdown.OptionData(locale.name));
        }
        LanguageDropdown.options = options;

        LanguageDropdown.value = selected;
        LanguageDropdown.onValueChanged.AddListener(LocaleSelected);
    }

    IEnumerator LocalInit()
    {
        yield return LocalizationSettings.InitializationOperation;

        if (Application.systemLanguage == SystemLanguage.English)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }
        else if (Application.systemLanguage == SystemLanguage.Korean)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[1];
        }
        else
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[0];
        }
    }

    static void LocaleSelected(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }

    public void OnClickLogout()
    {
        FirebaseInit.instance.Logout();
    }

    public void Show()
    {
        UpdateUI();

        goToMainMenuBtn.SetActive(GameManager.instance.gameStart);
    }

    public void Hide()
    {
        Destroy(gameObject);

        GameManager.instance.UnPuase();
    }

}

