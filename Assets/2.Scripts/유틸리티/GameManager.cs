using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using Cinemachine;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;
using DG.Tweening;
// using Firebase.Analytics;
using Sirenix.OdinInspector;
using System.Linq;

public class GameManager : MonoBehaviour
{

    [FoldoutGroup("참조")][SerializeField] private GameObject inGameMenu;
    [FoldoutGroup("참조")][SerializeField] private GameObject MainMenu;
    [FoldoutGroup("참조")][SerializeField] private GameObject DieMenu;
    [FoldoutGroup("참조")][SerializeField] private Image dieBackground;
    [FoldoutGroup("참조")][SerializeField] public Transform MainUIParent;
    [FoldoutGroup("참조")][SerializeField] private GameObject reviveButton;
    [FoldoutGroup("참조")][SerializeField] private GameObject player;
    [FoldoutGroup("참조")][SerializeField] private GameObject hpBar;
    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI timer;
    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI killCount;
    //[TitleGroup("참조")][SerializeField] private TextMeshProUGUI getCrystalCountText;
    //[TitleGroup("참조")][SerializeField] private TextMeshProUGUI killCountText;
    [FoldoutGroup("참조")][SerializeField] public PlayerStat playerStat;
    [FoldoutGroup("참조")][SerializeField] private PlayerWeapon playerWeapon;
    [FoldoutGroup("참조")][SerializeField] private PlayerUpgradeModule playerUpgradeModule;
    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI shipNameText;
    [FoldoutGroup("참조")][SerializeField] private GameObject shipBuyBtn;
    [FoldoutGroup("참조")][SerializeField] private GameObject shipTrialBtn;
    [FoldoutGroup("참조")][SerializeField] private Color buyBtnDisableColor;
    [FoldoutGroup("참조")][SerializeField] private Color buyBtnEnableColor;
    [FoldoutGroup("참조")][SerializeField] private CinemachineVirtualCamera cmvc;
    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI clearStage_getCrystalCountText;
    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI clearStage_killCountText;
    [FoldoutGroup("참조")][SerializeField] private GameObject clearStageMenu;
    [FoldoutGroup("참조")][SerializeField] private GameObject console;
    [FoldoutGroup("참조")][SerializeField] private GameObject inGameDebug;
    [FoldoutGroup("참조")][SerializeField] private Transform gameEndMenuParent;
    [FoldoutGroup("참조")] public Transform playerTrans;
    [FoldoutGroup("참조")][SerializeField] private GameObject[] editmodeUI;
    [FoldoutGroup("참조")][SerializeField] private StageSelect stageSelect;
    [FoldoutGroup("참조")][field: SerializeField] public GameObject savingIcon { get; private set; }

    [FoldoutGroup("End Menu")][SerializeField] private GameObject retryBtn;
    [FoldoutGroup("End Menu")][SerializeField] private GameObject mainMenuBtn;

    public Sequence reviveAnimationSequence { get; private set; }

    [Space]

    [SerializeField] public ShipObject currentShip;
    [field: SerializeField] public ShipList shipList { get; private set; }
    [FoldoutGroup("참조")][SerializeField] private GameObject tiralBtn;
    [FoldoutGroup("참조")][SerializeField] private GameObject buyShipBtn;
    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI shipCostText;
    [FoldoutGroup("참조")][SerializeField] private GameObject shipUpgradeSlot;
    [SerializeField] private int currentShipNumber;
    [FoldoutGroup("참조")][SerializeField] private Button previusBtn, nextBtn;
    [FoldoutGroup("참조")][SerializeField] private Slider stageProgressBar;
    [FoldoutGroup("참조")][SerializeField] private GameObject stageProgressMark;


    public UnityEvent PlayGameEvent;

    public bool gameStart = false;
    [field: SerializeField] public bool gameclear { get; private set; } = false;
    public bool inMainMenu = true;
    public bool editmode = false;

    private int currentTime;
    public float currentKillCount { get; private set; }
    private Coroutine timerCoroutine;
    public bool revivedThisGame = false;
    private float revivedTimer = 1f;

    private GameObject endGameMenu;

    public static GameManager instance;

    private void Awake()
    {
        instance = this;

#if UNITY_EDITOR
        //console.SetActive(true);
        //inGameDebug.SetActive(true);
#endif

        if (editmode)
        {
            for (int i = 0; i < editmodeUI.Length; i++)
            {
                editmodeUI[i].SetActive(true);
            }
        }

        if (!ES3.KeyExists("TryStage"))
        {
            ES3.Save("TryStage", 0);
            EventManager.instance.CustomEvent(AnalyticsType.GAME, "TryStage - " + ES3.Load<int>("TryStage").ToString());
        }

    }

    private void Start()
    {
        SelectShip(currentShipNumber);

        AudioManager.instance.FindBGM("menu2");

        UserDataManager.onChangeCrystalValue += InitShipBuyButton;

        ShipChangeBtn();

        AudioListener.volume = ES3.KeyExists("Sound") ? ES3.Load<int>("Sound") : 1;
    }

    public void ResetTime()
    {
        currentTime = 0;
    }


    public void PlayGame()
    {
        //player.GetComponent<PlayerStat>().MakeThisShip(currentShip);

        playerWeapon.playerShipData = UserDataManager.instance.GetShipData(currentShip.shipObjectData.shipCode);

        if (playerWeapon.playerShipData == null)
            playerWeapon.playerShipData = shipList.GetShipObject(currentShip.shipObjectData.shipCode).shipObjectData;


        inGameMenu.SetActive(true);
        MainMenu.SetActive(false);
        DieMenu.SetActive(false);
        clearStageMenu.SetActive(false);
        player.SetActive(true);
        //EnemyGenerator.instance.StartEnemySpawn();
        hpBar.SetActive(true);

        gameStart = true;
        inMainMenu = false;
        gameclear = false;

        playerWeapon.allowFire = true;
        CameraManager.instance.ChangeCamera_PlayCamera();

        EnemyGenerator.instance.bossFighting = false;
        revivedThisGame = false;

        playerStat.invinsible = false;

        InterstitialAdCaller.instance.StartIrAdsCoolTime();

        // EventManager.instance.CustomEvent(AnalyticsType.GAME, "Start Stage "FirebaseAnalytics.EventLevelStart);

        FPSDisplay.instance.ResetMinFPS();

        AudioManager.instance.FindBGM("inGame2");

        playerStat.AddBasicEquipment(UserDataManager.instance.GetShipData_currentVersion(currentShip.shipObjectData.shipCode));

        print("현재 함선 데미지 업그레이드 : " + currentShip.shipObjectData.shipUpgradeModuleList[1].currentUpgrade);
        print("현재 함선 데미지 : " + currentShip.shipObjectData.baseDamage.GetFinalStatValueAsInt());
        print("현재 함선 데미지 업그레이드 : " + currentShip.shipObjectData.shipUpgradeModuleList[0].currentUpgrade);
        print("현재 함선 체력 : " + currentShip.shipObjectData.baseMaxHp.GetFinalStatValueAsInt());

        LevelUpManager.getAllUpgradeCount = 0;

        playerStat.ReloadShipStat();

        playerUpgradeModule.UnapplyModuleStat();
        playerUpgradeModule.AddUpgradeModulesToPlayerStat();

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "Stage Start - " + EnemyGenerator.instance.currentEnemySpawnWaveObject.name);

        playerUpgradeModule.StartMineral();
        PlayGameEvent.Invoke();

        ES3.Save("TryStage", ES3.Load<int>("TryStage") + 1);
        // print(ES3.Load<int>("TryStage"));
        EventManager.instance.CustomEvent(AnalyticsType.GAME, "TryStage - " + ES3.Load<int>("TryStage").ToString());

        stageProgressBar.value = 0;

        GenerateStageProgressBar(EnemyGenerator.instance.currentEnemySpawnWaveObject);
    }

    public void ReplayGame()
    {
        //InterstitialAdCaller.instance.CallIrAds();
        SelectShip(currentShipNumber);

        EnemyGenerator.instance.deleteBossWall();

        InterstitialAdCaller.instance.CallIrAds();

        RewardedInterstitialAdCaller.instance.useCrystalDoubleThisStage = false;
        RewardedInterstitialAdCaller.instance.ShowDoubleCrystalBtn();

        Destroy(endGameMenu);

        DOTween.Kill(reviveAnimationSequence);

        UpgradeModuleDropManager.instance.getUpgradeModuleOnThisStage.Clear();
        InGameObjectManager.Instance.ClearInGameObjects();
    }

    public void GoMainMenu()
    {
        //InterstitialAdCaller.instance.CallIrAds();
        inGameMenu.SetActive(false);
        MainMenu.SetActive(true);
        DieMenu.SetActive(false);
        clearStageMenu.SetActive(false);
        //player.SetActive(false);
        hpBar.SetActive(false);

        gameStart = false;
        inMainMenu = true;
        gameclear = false;

        playerWeapon.allowFire = false;

        SelectShip(currentShipNumber);

        CameraManager.instance.ChangeCamera_MainMenu();

        EnemyGenerator.instance.deleteBossWall();
        InterstitialAdCaller.instance.StopIrAdsCoolTime();

        // EventManager.instance.CustomEvent(FirebaseAnalytics.EventLevelEnd);

        FPSDisplay.instance.SaveFPS();

        InterstitialAdCaller.instance.CallIrAds();

        AudioManager.instance.FindBGM("menu2");

        RewardedInterstitialAdCaller.instance.useCrystalDoubleThisStage = false;
        RewardedInterstitialAdCaller.instance.ShowDoubleCrystalBtn();

        LevelUpManager.getAllUpgradeCount = 0;

        Destroy(endGameMenu);

        DOTween.Kill(reviveAnimationSequence);

        UpgradeModuleDropManager.instance.getUpgradeModuleOnThisStage.Clear();
        InGameObjectManager.Instance.ClearInGameObjects();
    }

    public void StartTrialShip()
    {
        PlayGame();
        StartTimer();
        EnemyGenerator.instance.StartSpawnEnemy();
    }

    public void Resurrection()
    {
        DieMenu.SetActive(false);
    }

    public void PlayerDie()
    {
        //EnemyGenerator.instance.StopEnemySpawn();

        //getCrystalCountText.text = playerStat.currentCrystal.ToString();
        //killCountText.text = currentKillCount.ToString();

        dieBackground.color = new Color32(0, 0, 0, 0);

        dieBackground.DOColor(new Color32(0, 0, 0, 200), 1.5f);

        DieMenu.SetActive(true);

        //CrystalMotion.instance.StartCrystalMotion(0,playerStat.currentCrystal);

        retryBtn.SetActive(false);
        mainMenuBtn.SetActive(false);

        if (!revivedThisGame)
        {
            EventManager.instance.CustomEvent(AnalyticsType.GAME, "Die_EnemyKillCount - " + currentKillCount);
            EventManager.instance.CustomEvent(AnalyticsType.GAME, "Die_GetCrystalCount - " + playerStat.currentCrystal);
            EventManager.instance.CustomEvent(AnalyticsType.GAME, "Die_CurrentLevel - " + playerStat.GetCurrentPlayerLevel());

            revivedTimer = 1f;
            reviveButton.SetActive(true);

            var fill = reviveButton.GetComponent<Image>();

            fill.fillAmount = 1f;

            if (reviveAnimationSequence != null)
                reviveAnimationSequence.Kill();

            reviveAnimationSequence = DOTween.Sequence();

            reviveAnimationSequence.Join(DOTween.To(() => fill.fillAmount, (var) => fill.fillAmount = var, 0, 3.5f).SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                reviveButton.SetActive(false);
                fill.fillAmount = 1f;

                ShowGameEndMenu(true);
            }));
        }
        else
        {
            reviveButton.SetActive(false);
            ShowGameEndMenu(true);
        }

        gameStart = false;

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "player Die Time - " + timer.text);
    }

    [Button]
    public void ClearStage()
    {
        playerStat.invinsible = true;
        //clearStage_getCrystalCountText.text = playerStat.currentCrystal.ToString();
        //clearStage_killCountText.text = currentKillCount.ToString();

        inGameMenu.SetActive(false);
        //clearStageMenu.SetActive(true);

        DieMenu.SetActive(true);

        ShowGameEndMenu(false);

        gameStart = false;

        StartCoroutine(LootAllItem());

        IEnumerator LootAllItem()
        {
            yield return new WaitForSeconds(0.15f);
            Item.CrystalOnlyMagnetic(player.transform);
        }

        //CrystalMotion.instance.StartCrystalMotion(1,playerStat.currentCrystal);

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "stageClearEvent");

        UserDataManager.instance.StageClearSaveData(stageSelect.currentStageNumber);

        gameclear = true;

        RewardedInterstitialAdCaller.instance.useCrystalDoubleThisStage = false;
        RewardedInterstitialAdCaller.instance.ShowDoubleCrystalBtn();
    }

    void ShowGameEndMenu(bool Die)
    {
        print(1234);

        endGameMenu = Instantiate(Utility.GetResource<GameObject>("UI/AnimatedEndMenu"));
        endGameMenu.transform.SetParent(GameManager.instance.MainUIParent);
        endGameMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        endGameMenu.transform.localScale = new Vector3(1, 1, 1);

        endGameMenu.GetComponent<GameEndPanel>().StartEndMenuAnimation(Die);

        retryBtn.SetActive(true);
        mainMenuBtn.SetActive(true);
    }

    public void AddKillCount()
    {
        currentKillCount++;
        killCount.text = currentKillCount.ToString();
    }

    public void ResetKillCount()
    {
        currentKillCount = 0;
        killCount.text = currentKillCount.ToString();
    }

    public void StartTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }

        timerCoroutine = StartCoroutine(StartTimerCoroutine());
    }

    public void StopTimer()
    {
        if (timerCoroutine == null)
            return;

        StopCoroutine(timerCoroutine);
        timerCoroutine = null;
    }

    public IEnumerator StartTimerCoroutine()
    {
        //timer.text = string.Format("{0:0}:{1:00}", 0, 0);

        while (true)
        {
            yield return new WaitForSeconds(1);

            if (!EnemyGenerator.instance.bossFighting)
            {
                currentTime += 1;

                int min = (int)(currentTime / 60f);
                int second = (int)(currentTime % 60f);

                timer.text = string.Format("{0:0}:{1:00}", min, second);

                EnemyGenerator.instance.CheckWave();
            }

            stageProgressBar.value = ((float)currentTime / 900f);
        }
    }

    public void SkipTime(int value)
    {
        currentTime += value;
    }

    //메인 메뉴에서 선택한 함선으로 변경
    public void SelectShip(int currentShipNumber)
    {
        Quaternion bodyRotation = new Quaternion();
        if (playerStat.GetComponentInChildren<PlayerBodyBeacon>() != null)
            bodyRotation = playerStat.GetComponentInChildren<PlayerBodyBeacon>().transform.localRotation;

        playerStat.DeleteShipBody();
        playerStat.ClearWeaponSlots();
        playerWeapon.ClearAllWeapon();
        playerWeapon.ClearAllPassive();

        var shipObject = shipList.shipList[currentShipNumber];

        currentShip = shipObject;

        player.GetComponent<PlayerStat>().MakeThisShip(shipObject, bodyRotation);

        StartCoroutine(ChangeShipNameText());

        //함선이름 불러오기
        IEnumerator ChangeShipNameText()
        {
            var keyName = shipObject.shipObjectData.shipCode;

            var localizedString = new LocalizedString("Ship", keyName);

            var stringOperation = localizedString.GetLocalizedStringAsync();

            while (true)
            {
                if (stringOperation.IsDone && stringOperation.Status == AsyncOperationStatus.Succeeded)
                {
                    string str = stringOperation.Result;
                    shipNameText.text = str;

                    break;
                }
                yield return null;
            }
        }

        InitShipBuyButton();
    }

    //플레이어가 소지중인 함선이 아닐경우 함선 구매버튼 활성화
    private void InitShipBuyButton()
    {
        if (!UserDataManager.instance.CheckPlayerHaveShip(currentShip.shipObjectData.shipCode) && currentShip.shipObjectData.shipCost > 0)
        {
            if (shipBuyBtn != null)
                shipBuyBtn.SetActive(true);
            if (shipTrialBtn != null)
                shipTrialBtn.SetActive(true);
            shipCostText.text = currentShip.shipObjectData.shipCost.ToString();

            // if (currentShip.shipCost > UserDataManager.instance.currentUserData.crystal)
            // {
            //     shipBuyBtn.GetComponent<Image>().color = buyBtnDisableColor;
            // }
            // else
            // {
            //     shipBuyBtn.GetComponent<Image>().color = buyBtnEnableColor;
            // }

            if (currentShip.shipObjectData.shipCost > UserDataManager.instance.currentUserData.crystal)
            {
                if (shipBuyBtn != null)
                    shipBuyBtn.GetComponent<Button>().interactable = false;
            }
            else
            {
                if (shipBuyBtn != null)
                    shipBuyBtn.GetComponent<Button>().interactable = true;
            }
        }
        else
        {
            if (shipBuyBtn != null)
                shipBuyBtn.SetActive(false);
            if (shipTrialBtn != null)
                shipTrialBtn.SetActive(false);
        }
    }

    public void NextShip()
    {
        if (currentShipNumber + 1 < 0 || currentShipNumber + 1 >= shipList.shipList.Count)
            return;

        currentShipNumber++;

        SelectShip(currentShipNumber);

        ShipChangeBtn();
    }

    public void PreviusShip()
    {
        if (currentShipNumber + -1 < 0 || currentShipNumber + -1 > shipList.shipList.Count)
            return;

        currentShipNumber--;

        SelectShip(currentShipNumber);

        ShipChangeBtn();
    }

    private void ShipChangeBtn()
    {
        if (currentShipNumber + 1 < 0 || currentShipNumber + 1 >= shipList.shipList.Count)
            nextBtn.interactable = false;
        else
            nextBtn.interactable = true;

        if (currentShipNumber + -1 < 0 || currentShipNumber + -1 > shipList.shipList.Count)
            previusBtn.interactable = false;
        else
            previusBtn.interactable = true;
    }

    public int getCurrentTime()
    {
        return currentTime;
    }

    public void BuyShip()
    {
        if (currentShip.shipObjectData.shipCost > UserDataManager.instance.currentUserData.crystal
        || UserDataManager.instance.CheckPlayerHaveShip(currentShip.shipObjectData.shipCode))
            return;

        // UserDataManager.instance.currentUserData.playerHaveShip.Add(currentShip.shipObjectData);
        UserDataManager.instance.AddNewShip(currentShip.shipObjectData.shipCode);
        SelectShip(currentShipNumber);

        UserDataManager.instance.AddCrystalValue(-currentShip.shipObjectData.shipCost);

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "BuyShip - " + currentShip.shipObjectData.shipCode);
    }

    public void ShipUpgradeUIOnOff()
    {
        shipUpgradeSlot.SetActive(!shipUpgradeSlot.activeSelf);
    }

    public void ChangeGetCrystalText(int crystal)
    {
        //getCrystalCountText.text = (int.Parse(getCrystalCountText.text) + crystal).ToString();
    }

    private Dictionary<string, ProgressMark> markPoint = new Dictionary<string, ProgressMark>();

    private Dictionary<string, Image> genenartedMarked = new Dictionary<string, Image>();

    struct ProgressMark
    {
        public int time;
        public waveType waveType;
    }

    private void GenerateStageProgressBar(SpawnWaveObject currentSpawnWave)
    {
        float endTime = 900f;

        markPoint.Clear();

        foreach (var waves in currentSpawnWave.enemySpawnWaves2)
        {
            foreach (var wave in waves.enemySpawnWaves)
            {

                if (wave.waveType == waveType.summonBoss || wave.waveType == waveType.hyperBurstSummon)
                {
                    markPoint.Add(wave.guid, new ProgressMark() { time = wave.StartWaveTime, waveType = wave.waveType });
                }

                if (wave.waveType == waveType.summonBoss)
                {
                    if (wave.enemyObject.Object.GetComponentInChildren<BossStat>().finalBoss)
                    {
                        endTime = wave.StopWaveTime;
                    }
                }
            }
        }

        foreach (var mark in genenartedMarked)
        {
            Destroy(mark.Value);
        }

        genenartedMarked.Clear();

        foreach (var point in markPoint)
        {
            var mark = Instantiate(stageProgressMark, new Vector3(550f * ((float)point.Value.time / endTime), 0, 0), Quaternion.identity, stageProgressBar.transform.Find("Marks").transform).GetComponent<Image>();
            mark.GetComponent<RectTransform>().anchoredPosition = new Vector3(550f * ((float)point.Value.time / endTime), 0, 0);
            switch (point.Value.waveType)
            {
                case waveType.hyperBurstSummon:
                    mark.GetComponent<RectTransform>().sizeDelta = new Vector3(25f, 25f, 1);
                    mark.sprite = Resources.Load<Sprite>("UI/HyperBurst");
                    break;

                case waveType.summonBoss:
                    mark.GetComponent<RectTransform>().sizeDelta = new Vector3(35f, 35f, 1);

                    mark.sprite = Resources.Load<Sprite>("UI/Boss");
                    break;


                default:
                    mark.GetComponent<RectTransform>().sizeDelta = new Vector3(25f, 25f, 1);

                    mark.sprite = Resources.Load<Sprite>("UI/HyperBurst");
                    break;
            }

            genenartedMarked.Add(point.Key, mark);
        }
    }

    public void StageProgressBarCompleteTrigger(string guid)
    {
        foreach (var mark in genenartedMarked)
        {
            if (mark.Key == guid)
            {
                mark.Value.color = new Color32(176, 176, 176, 210);
                break;
            }
        }
    }

    public void OnClickSettingBtn()
    {
        var menu = Instantiate(Resources.LoadAll<GameObject>("UI/Setting").First(), MainUIParent).GetComponent<IUIBase>();
        menu.Show();
    }
}
