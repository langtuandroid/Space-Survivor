using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class PlayerWeapon : MonoBehaviour
{
    [FoldoutGroup("참조")][SerializeField] private PlayerMovement playerMovement;
    [FoldoutGroup("참조")][SerializeField] public PlayerStat playerStat;
    [FoldoutGroup("참조")][SerializeField] public PlayerUpgradeModule playerModule;

    public List<WeaponObject> weaponPool = new List<WeaponObject>();
    public List<PassiveObject> passivePool = new List<PassiveObject>();

    public Stat additionalDamageStat = new Stat();
    public Stat additionalCoolTimeStat = new Stat();
    public ShipObjectData playerShipData;

    [Space]
    public List<EquipmentSlot> weaponSlotList = new List<EquipmentSlot>();
    public List<EquipmentSlot> passiveSlotList = new List<EquipmentSlot>();


    [Space]

    public bool allowFire = false;

    public static List<int> activeSpiralVortexPositions = new List<int>();


    private void Update()
    {
        if (playerStat.GetPlayerDie() || !allowFire || !GameManager.instance.gameStart)
            return;

        for (int i = 0; i < weaponPool.Count; i++)
        {
            if (weaponPool[i].ready)
            {
                weaponSlotList[i].weaponCoolTimeImage.StartCoolTime(weaponPool[i].coolTime.GetFinalStatValue());
                StartCoroutine(ReloadProjectile(weaponPool[i]));

                StartCoroutine(weaponPool[i].Fire(weaponPool[i].GetFirePos(), this));
            }
        }
    }

    public IEnumerator ReloadProjectile(WeaponObject pool)
    {
        pool.ready = false;

        // print(pool.GetWeaponObject().GetWeaponObject().GetEquipmentType().ToString() + " /  " + pool.coolTime.GetFinalStatValue() + " / " + additionalCoolTimeStat.GetFinalStatValue() + " / " + playerModule.module_Firerate.GetFinalStatValue() + " / " + (pool.coolTime.GetFinalStatValue() * additionalCoolTimeStat.GetFinalStatValue()) * playerModule.module_Firerate.GetFinalStatValue());
        // print(pool.coolTime.GetFinalStatValue() + " " + additionalCoolTimeStat.GetFinalStatValue() + " " + playerModule.module_Firerate.GetFinalStatValue());

        yield return new WaitForSeconds(pool.coolTime.GetFinalStatValue() * additionalCoolTimeStat.GetFinalStatValue() * playerModule.module_Firerate.GetFinalStatValue());
        pool.ready = true;
    }

    public void EnQueueBullet(ProjectileLogic bullet)
    {
        foreach (WeaponObject pool in weaponPool)
        {
            if (pool.type == bullet.type)
            {
                pool.EnQueue(bullet.gameObject);
            }
        }
    }

    public bool CheckProjectileContainQueue(ProjectileLogic bullet)
    {
        foreach (WeaponObject pool in weaponPool)
        {
            if (pool.type == bullet.type)
            {
                return pool.IsProjectileContain(bullet.gameObject);
            }
        }

        return false;
    }

    public void ResetPlayerWeapon()
    {
        for (int i = 0; i < weaponPool.Count; i++)
        {
            weaponPool[i].ResetThisWeapon();
        }
    }

    public void AddNewWeapon(IEquipment equipment)
    {
        switch (equipment.GetAnyEqupment())
        {
            case AnyEqupment.Weapon:
                if (LevelUpManager.maxWeaponCount <= weaponPool.Count)
                    break;

                var newWeapon = Instantiate(equipment.GetWeaponObject());
                weaponPool.Add(newWeapon);
                FindFirePos(newWeapon);
                FindFireDir(newWeapon);
                newWeapon.GenerateWhileParticle(transform);

                LevelUpManager.instance.AddNewWeaponImage(equipment, weaponSlotList);
                break;

            case AnyEqupment.Passive:
                if (LevelUpManager.maxPassiveCount <= passivePool.Count)
                    break;

                var newPassive = Instantiate(equipment.GetPassive());
                passivePool.Add(newPassive);
                newPassive.GenerateWhileParticle(transform);

                switch (newPassive.GetEquipmentType())
                {
                    case EquipmentType.EnergyShield:
                        newPassive.passiveStat = Instantiate(Resources.Load<EnergyShield>("PassiveStat/EnergyShieldStat"));
                        break;

                    case EquipmentType.SelfRepair:
                        newPassive.passiveStat = Instantiate(Resources.Load<SelfRepairStat>("PassiveStat/SelfRepairStat"));
                        break;

                    case EquipmentType.MagneticGenerator:
                        newPassive.passiveStat = Instantiate(Resources.Load<MagneticGeneratorStat>("PassiveStat/MagneticGeneratorStat"));
                        break;

                    case EquipmentType.ResourceRefiner:
                        newPassive.passiveStat = Instantiate(Resources.Load<ResourceRefinerStat>("PassiveStat/ResourceRefinerStat"));
                        break;

                    case EquipmentType.Thruster:
                        newPassive.passiveStat = Instantiate(Resources.Load<ThrusterStat>("PassiveStat/ThrusterStat"));
                        break;

                    case EquipmentType.EnhancedSiege:
                        newPassive.passiveStat = Instantiate(Resources.Load<EnhancedSiegeStat>("PassiveStat/EnhancedSiegeStat"));
                        break;

                    case EquipmentType.ReloadingDevice:
                        newPassive.passiveStat = Instantiate(Resources.Load<ReloadingDeviceStat>("PassiveStat/ReloadingDeiveStat"));
                        break;
                }

                LevelUpManager.instance.AddNewWeaponImage(equipment, passiveSlotList, newPassive.passiveStat);

                newPassive.passiveStat.GetPassiveEffect(playerStat);
                newPassive.passiveStat.SetCoroutine(StartCoroutine(newPassive.passiveStat.StartWhilePassiveEffect()));
                break;
        }

    }

    public void FindFirePos(WeaponObject weapon)
    {
        weapon.SetFirePos(playerStat.GetShipBody().Find(weapon.firePosName));
    }

    public void FindFireDir(WeaponObject weapon)
    {
        weapon.SetFireDir(playerStat.GetShipBody().Find(weapon.fireDirName));
    }

    public void ClearAllWeapon()
    {
        for (int i = 0; i < weaponPool.Count; i++)
        {
            if (weaponPool[i].currenWhileParticle != null)
                Destroy(weaponPool[i].currenWhileParticle);
        }

        weaponPool.Clear();
    }

    public void ClearAllPassive()
    {
        for (int i = 0; i < passivePool.Count; i++)
        {
            if (passivePool[i].currenWhileParticle != null)
                Destroy(passivePool[i].currenWhileParticle);
        }

        passivePool.Clear();
    }

    public List<WeaponObject> GetMaxLevelWeaponList()
    {
        List<WeaponObject> weaponObjects = new List<WeaponObject>();

        for (int i = 0; i < weaponPool.Count; i++)
        {
            if (weaponPool[i].GetCurrentWeaponLevel() > weaponPool[i].maxWeaponLevel)
                weaponObjects.Add(weaponPool[i]);
        }

        return weaponObjects;
    }

    public List<IEquipment> GetMaxLevelEquipments()
    {
        List<IEquipment> EquipmentObejcts = new List<IEquipment>();

        for (int i = 0; i < weaponPool.Count; i++)
        {
            if (weaponPool[i].GetCurrentWeaponLevel() > weaponPool[i].maxWeaponLevel)
                EquipmentObejcts.Add(weaponPool[i]);
        }

        for (int i = 0; i < passivePool.Count; i++)
        {
            if (passivePool[i].GetCurrentWeaponLevel() > passivePool[i].maxWeaponLevel)
                EquipmentObejcts.Add(passivePool[i]);
        }

        return EquipmentObejcts;
    }

    public void UpgradeWeapon(IEquipment weaponObject)
    {
        switch (weaponObject.GetAnyEqupment())
        {
            case AnyEqupment.Weapon:
                for (int i = 0; i < weaponPool.Count; i++)
                {
                    if (weaponObject.GetEquipmentType() == weaponPool[i].GetEquipmentType())
                    {
                        weaponPool[i].UpgradeWeapon(weaponPool[i].GetUpgradeModuleList());
                        LevelUpManager.instance.AddUpgradeNode(weaponObject);
                        return;
                    }
                }

                AddNewWeapon(weaponObject);
                break;

            case AnyEqupment.Passive:
                for (int i = 0; i < passivePool.Count; i++)
                {
                    if (weaponObject.GetEquipmentType() == passivePool[i].GetEquipmentType())
                    {
                        passivePool[i].UpgradeEquipment(passivePool[i].GetUpgradeModuleList());
                        LevelUpManager.instance.AddUpgradeNode(weaponObject);
                        return;
                    }
                }

                AddNewWeapon(weaponObject);
                break;
        }
    }

    public WeaponObject GetCurrentWeapon(EquipmentType type)
    {
        for (int i = 0; i < weaponPool.Count; i++)
        {
            if (weaponPool[i].type == type)
                return weaponPool[i];
        }

        return null;
    }

    public int GetWeaponLevel(EquipmentType type)
    {
        List<IEquipment> equipmentPool = new List<IEquipment>();

        equipmentPool.AddRange(weaponPool);
        equipmentPool.AddRange(passivePool);

        for (int i = 0; i < equipmentPool.Count; i++)
        {
            if (equipmentPool[i].GetEquipmentType() == type)
                return equipmentPool[i].GetCurrentWeaponLevel();
        }

        return 0;
    }

    public int GetMaxWeaponLevel(EquipmentType type)
    {
        for (int i = 0; i < weaponPool.Count; i++)
        {
            if (weaponPool[i].type == type)
                return weaponPool[i].maxWeaponLevel;
        }

        return 0;
    }

    public bool GetIsWeaponHave(EquipmentType type)
    {
        for (int i = 0; i < weaponPool.Count; i++)
        {
            if (weaponPool[i].type == type)
            {
                //Debug.LogError(type);
                return true;
            }
        }

        return false;
    }

    public bool GetIsPassiveHave(EquipmentType type)
    {
        for (int i = 0; i < passivePool.Count; i++)
        {
            if (passivePool[i].type == type)
            {
                //Debug.LogError(type);
                return true;
            }
        }

        return false;
    }

    public void MissileBurst(Collider2D player)
    {
        StartCoroutine(Missile());

        IEnumerator Missile()
        {
            for (int i = 0; i < 20; i++)
            {

                yield return new WaitForSeconds(.15f);

                if (playerStat.GetPlayerDie())
                    break;

                var missile = ProjectileGenerator.instance.DeQueueProjectile(ProjectileType.BurstMissile, player.transform.position);

                if (missile.TryGetComponent<BurstMissile>(out BurstMissile burstMissile))
                {
                    burstMissile.InitMissile(player.GetComponentInParent<PlayerStat>());
                }

                if (missile.TryGetComponent<ProjectileLogic>(out ProjectileLogic projectile))
                {
                    projectile.Fire(null, 0);
                }

                AudioManager.instance.GenerateAudioAndPlaySFX("missile1", transform.position);
            }
        }
    }

    public void ClearWeaponSlotList()
    {
        weaponSlotList.Clear();
    }

    public void ClearPassiveSlotList()
    {
        passiveSlotList.Clear();
    }

    public float GetPlayerStatDamage()
    {
        return (playerModule.module_Damage.GetFinalStatValue() + playerShipData.baseDamage.GetFinalStatValueAsInt()) * additionalDamageStat.GetFinalStatValue();
    }

    /// <summary>
    /// 모듈 스탯들 모두 초기화
    /// </summary>
    public void ClearModuleStat()
    {

    }
}

/*public class WeaponPool
{
    public Stack<GameObject> bulletStack = new Stack<GameObject>();

    public GameObject projectilePrefab;
    public Transform parent;
    public WeaponType type;
    public Transform firePos;

    public bool ready = true;
    public float coolTime = 1f;

    public void EnQueue(GameObject enemy)
    {
        bulletStack.Push(enemy);
    }

    public Return DeQueue(Vector2 position)
    {
        Return ret = new Return();

        if (bulletStack.Count > 0)
        {
            var bullet = bulletStack.Pop();

            bullet.transform.position = position;
            bullet.SetActive(true);

            var projectileLogic = bullet.GetComponent<ProjectileLogic>();
            projectileLogic.ResetProjectile();

            ret.Object = bullet;
            ret.success = true;

            return ret;
        }
        else
        {
            ret.Object = projectilePrefab;
            ret.success = false;

            return ret;
        }
    }

    public void ResetThisWeapon()
    {
        ready = true;
    }

    public class Return
    {
        public GameObject Object;
        public bool success = false;
    }
}*/

