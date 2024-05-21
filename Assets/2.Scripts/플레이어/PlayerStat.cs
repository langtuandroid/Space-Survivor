using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
// using Firebase.Analytics;
using Sirenix.OdinInspector;

public class PlayerStat : MonoBehaviour
{
    [BoxGroup("플레이어 스텟")] private int maxHp = 1000;
    [BoxGroup("플레이어 스텟")] private int currentHp = 1000;
    [BoxGroup("플레이어 스텟")][SerializeField] private int maxExp = 50;
    [BoxGroup("플레이어 스텟")][SerializeField] private int maxExpIncreaseValue = 70;
    [BoxGroup("플레이어 스텟")] private int currentExp = 0;
    [BoxGroup("플레이어 스텟")] public Stat getMineralBouse = new Stat();
    [BoxGroup("플레이어 스텟")] private int playerLevel = 1;
    [BoxGroup("플레이어 스텟")] public int currentCrystal = 0;
    [BoxGroup("플레이어 스텟")] private int lastGameCurrentCrystal = 0;
    [BoxGroup("플레이어 스텟")][SerializeField] public Stat moveSpeed;
    [BoxGroup("플레이어 스텟")][SerializeField] public Stat rotationSpeed;

    [Space]

    [FoldoutGroup("쉴드 관련")] public int currentShieldStack = 0;
    [FoldoutGroup("쉴드 관련")] public int maxShieldStack = 0;
    [FoldoutGroup("쉴드 관련")] public bool shieldInvinsible = false;
    [FoldoutGroup("쉴드 관련")] public Stat shieldInvinsibleTime = new Stat();
    [FoldoutGroup("쉴드 관련")] public Stat shieldReloadTime = new Stat();
    [FoldoutGroup("쉴드 관련")] public GameObject shieldImage;
    [FoldoutGroup("쉴드 관련")] public Color oneShieldStackColor;
    [FoldoutGroup("쉴드 관련")] public Color twoShieldStackColor;
    [FoldoutGroup("쉴드 관련")] public Color thereShieldStackColor;
    [FoldoutGroup("쉴드 관련")] private Coroutine shieldReloadCoroutine;

    [Space]

    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI playerLevelText;

    [Space]

    [Space]
    [FoldoutGroup("참조")][SerializeField] private ParticleSystem HitEffect;

    [Space]
    [FoldoutGroup("참조")][SerializeField] private HpBar hpBar;
    [FoldoutGroup("참조")][SerializeField] private Slider expSlider;
    [FoldoutGroup("참조")][SerializeField] private Image expFill;
    [FoldoutGroup("참조")][SerializeField] private Color originExpColor;
    [FoldoutGroup("참조")][SerializeField] private Color fadeOutExpColor;

    [FoldoutGroup("참조")][SerializeField] private PlayerWeapon playerWeapon;
    [FoldoutGroup("참조")][SerializeField] private PlayerUpgradeModule playerUpgradeModule;
    [FoldoutGroup("참조")][SerializeField] private PlayerMovement playerMovement;
    [FoldoutGroup("참조")][SerializeField] private TextMeshProUGUI crystalText;
    [FoldoutGroup("참조")][SerializeField] private Transform weaponSlotParent;
    [FoldoutGroup("참조")][SerializeField] private Transform passiveSlotParent;
    [FoldoutGroup("참조")][SerializeField] private CircleCollider2D resourcePullCollider;

    [Space]
    [FoldoutGroup("참조")][SerializeField] GameObject dieVFX;

    [BoxGroup("플레이어 스텟")] public bool playerDie = false;
    [field: BoxGroup("플레이어 스텟")][field: SerializeField] public bool whileLevelUp { get; set; } = false;
    [FoldoutGroup("참조")] private GameObject currentShipBody;
    [BoxGroup("이벤트")] public UnityEvent startGameEvent;
    [BoxGroup("이벤트")] public UnityEvent playerDieEvent;
    [BoxGroup("이벤트")] public UnityEvent playerLevelUpEvent;

    private ShipObject selectedShipObject = null;

    private ShipObjectData currentShipData;

    [Space]

    [BoxGroup("플레이어 스텟")][SerializeField] public bool invinsible = false;

    [SerializeField] private List<EnemyStat> enteredEnemyList = new List<EnemyStat>();

    private Sequence mySequence;

    private void OnEnable()
    {
        playerLevelText.text = "Level " + playerLevel.ToString();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Enemy"))
        {
            var enemy = collision.transform.GetComponent<EnemyStat>();
            TakeDamage(enemy.GetDamage(), enemy.GetEnemyType().ToString());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Resource"))
        {
            other.transform.GetComponent<Resource>().StartPull(transform);
        }
    }

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            GetExp(1000000);
        }
    }

#endif

    public void TakeDamage(int damage, string name = "none")
    {
        if (invinsible || playerDie || shieldInvinsible)
            return;

        if (currentShieldStack > 0)
        {
            StartCoroutine(UseShield());

            return;
        }

        //Vibration.Vibrate((long)15);
        MoreMountains.NiceVibrations.MMVibrationManager.Haptic(MoreMountains.NiceVibrations.HapticTypes.LightImpact);

        damage = (int)((damage * 16f) / (16f + playerUpgradeModule.module_Armor.GetFinalStatValue()));

        currentHp -= damage;

        hpBar.SetState(currentHp, maxHp);

        HitEffect.Play();

        if (currentHp <= 0)
        {
            Die(name);

        }
    }


    [Button("Force Die")]
    private void Die(string name = "none")
    {
        playerDie = true;

        playerDieEvent.Invoke();

        EventManager.instance.CustomEvent(AnalyticsType.GAME, "PlayerDie by - " + name);

        //gameObject.SetActive(false);

        //Instantiate(dieVFX, transform.position, Quaternion.identity);
        VFXGenerator.instance.GenerateVFX(VFXType.playerDie1, transform.position);

        //DeleteShipBody();
        DisableShipBody();

        EZCameraShake.CameraShakeInstance cameraShakeInstance = new EZCameraShake.CameraShakeInstance(4f, 4f, .2f, 1f);

        Utility.Explode(transform.position, 0, 20, 10, VFXType.AtomicExplosion, cameraShakeInstance);

        AudioManager.instance.GenerateAudioAndPlaySFX("explosion1", transform.position);

        if (GameManager.instance.revivedThisGame)
        {
            // FirebaseAnalytics.LogEvent("PlayerDeathTime_Revived", "PlayerDeathTime_Revived_Sec", GameManager.instance.getCurrentTime());
        }
        else
        {
            //             FirebaseAnalytics.LogEvent("PlayerDeathTime", "IntParam", GameManager.instance.getCurrentTime());

            //             Firebase.Analytics.FirebaseAnalytics.LogEvent(
            //   Firebase.Analytics.FirebaseAnalytics.EventSelectContent, new Firebase.Analytics.Parameter(
            //     FirebaseAnalytics.ParameterItemId, GameManager.instance.getCurrentTime()));
        }

    }

    public void GetCurrentCrystal()
    {
        UserDataManager.instance.AddCrystalValue(currentCrystal);
    }

    public void GetCrystalDouble()
    {
        UserDataManager.instance.AddCrystalValue(lastGameCurrentCrystal * 2);
        GameManager.instance.ChangeGetCrystalText(lastGameCurrentCrystal * 2);

        //if (playerDie)
        //    CrystalMotion.instance.StartCrystalMotion(0, lastGameCurrentCrystal);
        //else
        //    CrystalMotion.instance.StartCrystalMotion(0, lastGameCurrentCrystal);

    }

    public void GetExp(int exp)
    {
        if (whileLevelUp || playerDie || !GameManager.instance.gameStart)
            return;

        //print(getMineralBouse.GetFinalStatValue());

        currentExp += Mathf.RoundToInt(exp * getMineralBouse.GetFinalStatValue());

        //Vibration.Vibrate((long)30);

        OnChangeExp();
    }

    private void OnChangeExp()
    {
        if (currentExp >= maxExp)                    //������ ����ġ�� ����������
        {
            LevelUp();
        }

        float state = (float)currentExp;
        state /= maxExp;
        if (state < 0f) { state = 0f; }

        expSlider.value = state;
    }

    public void GetCrystal(int value)
    {
        currentCrystal += value;

        crystalText.text = currentCrystal.ToString();

        lastGameCurrentCrystal = currentCrystal;
    }

    private void LevelUp()
    {
        if (!GameManager.instance.gameStart)
            return;

        whileLevelUp = true;
        playerLevel++;

        LevelUpManager.instance.StartWeaponUpgrade();
        playerLevelText.text = "Level " + playerLevel.ToString();

        AudioManager.instance.PlaySFX("levelup1");

        //mySequence.Restart();
        //expFill.DOColor(fadeOutExpColor, 1f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
    }

    public void AfterUpgrade()
    {
        currentExp = 0;

        maxExp += maxExpIncreaseValue;

        var remainExp = currentExp - maxExp;

        OnChangeExp();

        whileLevelUp = false;
    }

    public bool GetPlayerDie()
    {
        return playerDie;
    }

    //스탯 초기화
    public void ResetPlayerStat()
    {
        transform.position = Vector2.zero;

        SetHp();
        currentHp = maxHp;
        playerDie = false;

        moveSpeed.ClearFloatModifier();
        moveSpeed.ClearPercentModifier();

        rotationSpeed.ClearFloatModifier();
        rotationSpeed.ClearPercentModifier();

        currentShieldStack = 0;
        maxShieldStack = 0;

        playerLevel = 1;
        currentExp = 0;
        currentCrystal = 0;
        crystalText.text = currentCrystal.ToString();
        maxExp = 5;
        getMineralBouse.ClearPercentModifier();

        playerLevelText.text = "Level " + playerLevel.ToString();

        OnChangeExp();
        hpBar.SetState(currentHp, maxHp);

        for (int i = 0; i < playerWeapon.passivePool.Count; i++)
        {
            playerWeapon.passivePool[i].passiveStat.OnEndGame();
        }

        playerWeapon.ResetPlayerWeapon();
        ClearWeaponSlots();

        playerWeapon.additionalDamageStat.ClearPercentModifier();
        shieldInvinsibleTime.ClearPercentModifier();
        shieldReloadTime.ClearPercentModifier();
        if (shieldReloadCoroutine != null)
            StopCoroutine(shieldReloadCoroutine);
        if (shieldImage != null)
            shieldImage.SetActive(false);

        resourcePullCollider.radius = 40f;
    }

    public void PlayGame()
    {
        SetHp();
        currentHp = maxHp;
        playerDie = false;

        hpBar.SetState(currentHp, maxHp);

        resourcePullCollider.radius = 40f + playerUpgradeModule.module_MagnetRange.GetFinalStatValue();
    }

    public void MakeThisShip(ShipObject ship, Quaternion bodyrotation)
    {
        currentShipBody = Instantiate(GameManager.instance.shipList.GetShipBody(ship.shipObjectData.shipCode), transform.position, bodyrotation, transform);

        playerMovement.SetPlayerBody(currentShipBody.transform);

        selectedShipObject = ship;

        //for (int i = 0; i < ship.basicWeapon.Count; i++)
        //{
        //    playerWeapon.AddNewWeapon(ship.basicWeapon[i]);
        //}

        //for (int i = 0; i < ship.basicPassive.Count; i++)
        //{
        //    playerWeapon.AddNewWeapon(ship.basicPassive[i]);
        //}

        //
        // print(1);
        GetShipStat(UserDataManager.instance.currentUserData.GetShipDataFromPlayerHaveShip(ship.shipObjectData.shipCode));
    }

    public void AddBasicEquipment(ShipObjectData shipData)
    {
        for (int i = 0; i < shipData.basicWeapon.Count; i++)
        {
            // playerWeapon.AddNewWeapon(ship.basicWeapon[i]);
            playerWeapon.UpgradeWeapon(shipData.basicWeapon[i]);
        }

        for (int i = 0; i < shipData.basicPassive.Count; i++)
        {
            //playerWeapon.AddNewWeapon(ship.basicPassive[i]);
            playerWeapon.UpgradeWeapon(shipData.basicPassive[i]);
        }
    }

    public void DisableShipBody()
    {
        if (currentShipBody == null)
            return;

        currentShipBody.SetActive(false);
    }

    public void DeleteShipBody()
    {
        if (currentShipBody == null)
            return;

        Destroy(currentShipBody);
    }

    public Transform GetShipBody()
    {
        return currentShipBody.transform;
    }

    public void GetShipStat(ShipObjectData shipObjectData)              //������� �Լ� ������ �÷��̾��� �Լ� ���ȿ� �ٿ��ֱ�
    {
        currentShipData = shipObjectData;
        SetHp();
        // maxHp = shipObjectData.baseMaxHp.GetFinalStatValueAsInt() + playerUpgradeModule.module_MaxHp.GetFinalStatValueAsInt();
        moveSpeed.SetBaseValue(shipObjectData.baseMoveSpeed.GetFinalStatValue());
        rotationSpeed.SetBaseValue(shipObjectData.baseRotationSpeed.GetFinalStatValue());
    }

    public void SetHp()
    {
        maxHp = currentShipData.baseMaxHp.GetFinalStatValueAsInt() + playerUpgradeModule.module_MaxHp.GetFinalStatValueAsInt();
    }

    /// <summary>
    /// 현재 선택되어 있는 함선의 스탯을 다시 가져오기 게임 시작전에 스탯을 다시한번 가져오는 용도
    /// </summary>
    public void ReloadShipStat()
    {
        if (selectedShipObject != null)
            GetShipStat(UserDataManager.instance.currentUserData.GetShipDataFromPlayerHaveShip(selectedShipObject.shipObjectData.shipCode));
    }

    public float GetMoveSpeed()
    {
        return moveSpeed.GetFinalStatValue() + playerUpgradeModule.module_MoveSpeed.GetFinalStatValue();
    }

    public float GetRotationSpeed()
    {
        return rotationSpeed.GetFinalStatValue() + playerUpgradeModule.module_RotationSpeed.GetFinalStatValue();
    }

    public void ChangeInvinsible()
    {
        invinsible = !invinsible;
    }

    public void FullHp()
    {
        currentHp = maxHp;

        hpBar.SetState(currentHp, maxHp);
    }

    public void Resurrection()
    {
        FullHp();
        currentShipBody.SetActive(true);
        playerDie = false;

        ItemGenerator.instance.DeQueueItem(ItemType.AtomicExplosion, transform.position);
    }

    public void ClearWeaponSlots()
    {
        EquipmentSlot[] weaponSlots = weaponSlotParent.GetComponentsInChildren<EquipmentSlot>();

        for (int i = 0; i < weaponSlots.Length; i++)
        {
            Destroy(weaponSlots[i].gameObject);
        }

        playerWeapon.ClearWeaponSlotList();

        EquipmentSlot[] passiveSlots = passiveSlotParent.GetComponentsInChildren<EquipmentSlot>();

        for (int i = 0; i < passiveSlots.Length; i++)
        {
            Destroy(passiveSlots[i].gameObject);
        }

        playerWeapon.ClearPassiveSlotList();
    }

    public void Heal(int value)
    {
        currentHp += value;

        currentHp = Mathf.Clamp(currentHp, 0, maxHp);

        hpBar.SetState(currentHp, maxHp);

        AudioManager.instance.GenerateAudioAndPlaySFX("upgrade2", transform.position);
    }

    public int GetCurrentPlayerLevel()
    {
        return playerLevel;
    }

    public void GetShield(GameObject shieldImage)
    {

        //print(shieldImage);
        this.maxShieldStack++;

        if (maxShieldStack == 1)
        {
            this.shieldImage = shieldImage;
            AddShield();
        }
        else
        {
            if (shieldReloadCoroutine == null)
                shieldReloadCoroutine = StartCoroutine(ReloadShield());
        }
    }

    public void AddShield()
    {
        if (currentShieldStack == maxShieldStack || playerDie)
            return;

        print("쉴드 생성");

        currentShieldStack++;
        OnChangeShieldStack();

        shieldImage.SetActive(true);

        AudioManager.instance.GenerateAudioAndPlaySFX("shield1", transform.position);
    }

    public IEnumerator UseShield()
    {
        print("쉴드 사용 무적 시작");
        shieldInvinsible = true;

        yield return new WaitForSeconds(shieldInvinsibleTime.GetFinalStatValue());

        shieldInvinsible = false;
        currentShieldStack--;

        if (currentShieldStack == 0)
            shieldImage.SetActive(false);

        print("쉴드 무적 끝");

        OnChangeShieldStack();

        if (shieldReloadCoroutine == null)
            shieldReloadCoroutine = StartCoroutine(ReloadShield());
    }

    public IEnumerator ReloadShield()
    {
        print("쉴드 재생성 시작");

        for (int i = 0; i < playerWeapon.passivePool.Count; i++)
        {
            if (playerWeapon.passivePool[i].type == EquipmentType.EnergyShield)
            {
                playerWeapon.passivePool[i].passiveStat.StartPassiveSlotCoolTimeImage(shieldReloadTime.GetFinalStatValue());
                break;
            }
        }
        yield return new WaitForSeconds(shieldReloadTime.GetFinalStatValue());

        AddShield();

        shieldReloadCoroutine = null;

        if (currentShieldStack < maxShieldStack)
            shieldReloadCoroutine = StartCoroutine(ReloadShield());
    }

    public void ResetShield()
    {
        StopCoroutine("ReloadShield");

        currentShieldStack = 0;
        maxShieldStack = 0;
    }

    private void OnChangeShieldStack()
    {
        switch (currentShieldStack)
        {
            case 0:

                break;

            case 1:
                shieldImage.GetComponent<SpriteRenderer>().color = oneShieldStackColor;
                break;

            case 2:
                shieldImage.GetComponent<SpriteRenderer>().color = twoShieldStackColor;
                break;

            case 3:
                shieldImage.GetComponent<SpriteRenderer>().color = thereShieldStackColor;
                break;

        }
    }
}
