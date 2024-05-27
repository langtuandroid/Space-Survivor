using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public interface IEquipment
{
    public void UpgradeEquipment(UpgradeModuleList modules);
    public WeaponObject GetWeaponObject();
    public PassiveObject GetPassive();
    public EquipmentType GetEquipmentType();
    public AnyEqupment GetAnyEqupment();
    public List<UpgradeModuleList> GetUpgradeModuleLists();
    public Sprite GetEquipmentIamge();
    public UpgradeModuleList GetUpgradeModuleList();
    public void GenerateWhileParticle(Transform trans);
    public int GetCurrentWeaponLevel();
}

[CreateAssetMenu(fileName = "new Weapon", menuName = "Scriptable Object/Equipment/New Weapon", order = int.MaxValue)]
public class WeaponObject : ScriptableObject, IEquipment
{
    public int id;

    public Stack<GameObject> bulletStack = new Stack<GameObject>();

    public List<GameObject> activeProjectile = new List<GameObject>();

    public GameObject projectilePrefab;
    public Sprite weaponImage;
    public Transform parent;
    public AnyEqupment anyEqupment;
    public EquipmentType type;
    public GameObject whileParticlePrefab;
    public GameObject currenWhileParticle;
    private Transform firePos;
    private Transform fireDir;
    private Vector3 randomDir;
    public string firePosName = "FirePos1";
    public string fireDirName = "FireDir1";

    [Space]

    public string fireSFX = "";

    [Space]

    public Stat currentDamage = new Stat();
    public Vector3 currentSizeVector;
    public Stat currentSize = new Stat();
    public Stat currentTrailSize = new Stat();

    [Space]

    public bool ready = true;
    public Stat coolTime = new Stat();                 //발사 쿨타임

    [Space]

    public Stat damageMultifly = new Stat();

    [Space]

    public int projectileAmount = 1;            //발사 개수
    public Stat firingInterval = new Stat();         //발사 주기

    [Space]

    public Stat FireForce = new Stat();

    [Space]

    public List<UpgradeModule> currentUpgradeModules = new List<UpgradeModule>();
    public List<UpgradeModuleList> UpgradeModulesForLevel = new List<UpgradeModuleList>();

    [Space]

    public int maxWeaponLevel = 6;
    private int currentWeaponLevel = 1;

    public IEnumerator Fire(Transform firepos, PlayerWeapon playerWeapon)
    {
        for (int j = 0; j < projectileAmount; j++)
        {
            var success = DeQueue(firepos.position);

            IProjectileLogic projectileLogic;

            if (success.success)
            {
                projectileLogic = success.Object.GetComponent<IProjectileLogic>();
                projectileLogic.playerWeapon = playerWeapon;
                projectileLogic.weaponObject = this;
                projectileLogic.UpgradeProjectile(success.Object);
                projectileLogic.ResetProjectile();
            }
            else
            {
                var Object = Instantiate(projectilePrefab, firepos.position, Quaternion.identity);

                projectileLogic = Object.GetComponent<IProjectileLogic>();
                projectileLogic.playerWeapon = playerWeapon;
                projectileLogic.weaponObject = this;
                projectileLogic.UpgradeProjectile(Object);
                projectileLogic.ResetProjectile();
                projectileLogic.weaponObject.AddActiveProjectile(Object);
            }

            success.Object.gameObject.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
            projectileLogic.SetSize();
            projectileLogic.Fire(GetFireDir(), FireForce.GetFinalStatValueAsInt());

            if (firingInterval.GetFinalStatValue() > 0)
                yield return new WaitForSecondsRealtime(firingInterval.GetFinalStatValue());

            if (fireSFX != "")
                AudioManager.instance.GenerateAudioAndPlaySFX(fireSFX, firepos.position, Random.Range(0.8f, 1.2f));

        }

        ChangeRandomDir();
    }

    public void UpgradeEquipment(UpgradeModuleList modules)
    {

    }

    public void UpgradeWeapon(UpgradeModuleList modules)
    {
        for (int i = 0; i < modules.upgradeModules.Count; i++)
        {
            switch (modules.upgradeModules[i].upgradeModuleType)
            {
                /*
                case upgradeModuleType.DamageIntIncrease:

                    currentDamage.AddFloatModifier(modules.upgradeModules[i].value1);

                    foreach (GameObject projectile in bulletStack)
                    {
                        if (projectile.TryGetComponent<ProjectileLogic>(out ProjectileLogic logic))
                        {
                            DamageIncreaseInt(logic, Utility.RountToInt(modules.upgradeModules[i].value1));
                        }
                    }

                    
                    for (int j = 0; j < activeProjectile.Count; j++)
                    {
                        if (activeProjectile[j].TryGetComponent<ProjectileLogic>(out ProjectileLogic logic))
                        {
                            DamageIncreaseInt(logic, Utility.RountToInt(modules.upgradeModules[i].value1));
                        }
                    }
                    
                    break;
                    */

                case upgradeModuleType.CoolTimeDecrease:
                    coolTime.AddPercentModifier(modules.upgradeModules[i].value1);
                    break;

                case upgradeModuleType.IncreasedProjectiles:
                    projectileAmount += Mathf.RoundToInt(modules.upgradeModules[i].value1);
                    break;

                case upgradeModuleType.IncreaseDroneNumber:
                    projectileAmount += Mathf.RoundToInt(modules.upgradeModules[i].value1);
                    break;

                case upgradeModuleType.IncreaseFireForce:
                    FireForce.AddPercentModifier(modules.upgradeModules[i].value1);
                    break;


                case upgradeModuleType.IncreaseSize:
                    currentSize.AddPercentModifier(modules.upgradeModules[i].value1);
                    currentTrailSize.AddPercentModifier(modules.upgradeModules[i].value1);

                    SetSizeWhileParticle();
                    /*
                    foreach (GameObject projectile in bulletStack)
                    {
                        IncreseSize(projectile, modules.upgradeModules[i].value1);
                    }

                    for (int j = 0; j < activeProjectile.Count; j++)
                    {
                        IncreseSize(activeProjectile[j], modules.upgradeModules[i].value1);
                    }
                    */


                    break;

                    /*
                case upgradeModuleType.IncreseRotateSpeed:

                    foreach (GameObject projectile in bulletStack)
                    {
                        if (projectile.TryGetComponent<ThronSpike>(out ThronSpike spike))
                        {
                            IncreseRotateSpeed(spike, Utility.RountToInt(modules.upgradeModules[i].value1));
                        }
                    }

                    
                    for (int j = 0; j < activeProjectile.Count; j++)
                    {
                        if (activeProjectile[j].TryGetComponent<ThronSpike>(out ThronSpike spike))
                        {
                            IncreseRotateSpeed(spike, Utility.RountToInt(modules.upgradeModules[i].value1));
                        }
                    }
                    

                    break;
                    */

                    /*
                case upgradeModuleType.IncreseExplodeRadius:

                    VFXType type = VFXType.none;

                    foreach (GameObject projectile in bulletStack)
                    {
                        if (projectile.TryGetComponent<Firecracker>(out Firecracker script))
                        {
                            script.AddExplodeRadius(modules.upgradeModules[i].value1);

                            type = script.GetVFXType();
                        }
                    }

                    for (int j = 0; j < activeProjectile.Count; j++)
                    {
                        if (activeProjectile[j].TryGetComponent<Firecracker>(out Firecracker script))
                        {
                            script.AddExplodeRadius(modules.upgradeModules[i].value1);

                            type = script.GetVFXType();
                        }
                    }

                    if (type != VFXType.none)
                        VFXGenerator.instance.AddParticleSize(type, 0.1f);

                    break;
                    */


            }

            currentUpgradeModules.Add(modules.upgradeModules[i]);
        }

        currentWeaponLevel++;

    }

    private void DamageIncreaseInt(ProjectileLogic logic, int value)
    {
        logic.AddDamage(Utility.RountToInt(value));
    }

    private void HitCountIncrease(ProjectileLogic logic, int value)
    {
        logic.AddHitCount(Utility.RountToInt(value));
    }

    public void EnQueue(GameObject bullet)
    {
        bulletStack.Push(bullet);

        activeProjectile.Remove(bullet);
    }

    public bool IsProjectileContain(GameObject enemy)
    {
        return bulletStack.Contains(enemy);
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

            AddActiveProjectile(bullet);

            return ret;
        }
        else
        {
            ret.Object = projectilePrefab;
            ret.success = false;

            return ret;
        }
    }

    public void UpgradeProjectile(GameObject projectile)
    {
        for (int i = 0; i < currentUpgradeModules.Count; i++)
        {
            switch (currentUpgradeModules[i].upgradeModuleType)
            {
                case upgradeModuleType.DamageIntIncrease:
                    if (projectile.TryGetComponent<ProjectileLogic>(out ProjectileLogic logic))
                    {
                        DamageIncreaseInt(logic, Utility.RountToInt(currentUpgradeModules[i].value1));
                    };
                    break;

                /*
            case upgradeModuleType.IncreaseSize:
                if (projectile.TryGetComponent<ProjectileLogic>(out ProjectileLogic logic4))
                {
                    logic4.IncreaseProjectileSize(currentUpgradeModules[i].value1);
                };

                //IncreseSize(projectile, currentUpgradeModules[i].value1);


                break;
                */

                case upgradeModuleType.IncreseRotateSpeed:
                    if (projectile.TryGetComponent<ProjectileLogic>(out ProjectileLogic logic3))
                    {
                        logic3.IncreseRotateSpeed(currentUpgradeModules[i].value1);
                    };

                    /*
                    if (projectile.TryGetComponent<ThronSpike>(out ThronSpike spike))
                    {
                        IncreseRotateSpeed(spike, Utility.RountToInt(currentUpgradeModules[i].value1));
                    };
                    */
                    break;

                case upgradeModuleType.IncreseExplodeRadius:
                    if (projectile.TryGetComponent<ProjectileLogic>(out ProjectileLogic logic2))
                    {
                        logic2.IncreaseExplodeRadius(currentUpgradeModules[i].value1);
                        if (type == EquipmentType.ShockWaveGenerator)
                            currentSize.AddPercentModifier(currentUpgradeModules[i].value1 * 1.5f);
                    };

                    /*
                    if (projectile.TryGetComponent<ShockWave>(out ShockWave shockWave))
                    {
                        shockWave.AddRadius(.3f);
                    };

                    if (projectile.TryGetComponent<Firecracker>(out Firecracker script))
                    {
                        script.AddExplodeRadius(currentUpgradeModules[i].value1);
                    }*/

                    break;

                case upgradeModuleType.IncreaseHitCount:
                    if (projectile.TryGetComponent<ProjectileLogic>(out ProjectileLogic logic4))
                    {
                        HitCountIncrease(logic4, (int)currentUpgradeModules[i].value1);
                    };

                    break;


            }
        }
    }


    public void SetFirePos(Transform pos)
    {
        firePos = pos;
    }

    public Transform GetFirePos()
    {
        return firePos;
    }

    public void SetFireDir(Transform dir)
    {
        fireDir = dir;
    }

    public Transform GetFireDir()
    {
        return fireDir;
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

    public int GetCurrentWeaponLevel()
    {
        return currentWeaponLevel;
    }

    public UpgradeModuleList GetUpgradeModuleList()
    {
        return UpgradeModulesForLevel[currentWeaponLevel - 1];
    }

    public void ChangeRandomDir()
    {
        randomDir = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f));
    }

    public Vector3 GetRandomDir()
    {
        return randomDir;
    }

    public void AddActiveProjectile(GameObject Object)
    {
        activeProjectile.Add(Object);
    }

    public void GenerateWhileParticle(Transform trans)
    {
        if (whileParticlePrefab == null)
            return;

        currenWhileParticle = Instantiate(whileParticlePrefab, trans);
    }

    public void SetSizeWhileParticle()
    {
        if (currenWhileParticle == null)
            return;

        currenWhileParticle.transform.localScale = new Vector3(currentSize.GetFinalStatValue(), currentSize.GetFinalStatValue(), currentSize.GetFinalStatValue());


    }

    public WeaponObject GetWeaponObject()
    {
        return this;
    }

    public PassiveObject GetPassive()
    {
        return null;
    }

    public EquipmentType GetEquipmentType()
    {
        return type;
    }

    public AnyEqupment GetAnyEqupment()
    {
        return anyEqupment;
    }

    public List<UpgradeModuleList> GetUpgradeModuleLists()
    {
        return UpgradeModulesForLevel;
    }

    public Sprite GetEquipmentIamge()
    {
        return weaponImage;
    }
}

[System.Serializable]
public class UpgradeModuleList
{
    public List<UpgradeModule> upgradeModules = new List<UpgradeModule>();

    //사람이 읽을 수 있는 언어로 변환뒤 return [예 : -0.1f => 10]
    public string GetValueHumanReadableValue1(int i)
    {
        if (Mathf.Abs(upgradeModules[i].value1) < 1)
            return (Mathf.Abs(upgradeModules[i].value1) * 100).ToString();
        else
            return upgradeModules[i].value1.ToString();
    }

    public string GetValueHumanReadableValue2(int i)
    {
        if (Mathf.Abs(upgradeModules[i].value2) < 1)
            return (Mathf.Abs(upgradeModules[i].value2) * 100).ToString();
        else
            return upgradeModules[i].value2.ToString();
    }

    public string GetValueHumanReadableValue3(int i)
    {
        if (Mathf.Abs(upgradeModules[i].value3) < 1)
            return (Mathf.Abs(upgradeModules[i].value3) * 100).ToString();
        else
            return upgradeModules[i].value3.ToString();
    }
}

public enum AnyEqupment
{
    Weapon, Passive
}

public enum EquipmentType
{
    SquareCannon, SmallShotCannon, HomingMissile, MeteoriteFlak, FireworkRocket, ThornSatellite, ShockWaveGenerator, MachineGun, BurstMissile,
    SelfRepair, RadiationField, EnergyShield, SawBlade, Armor, Thruster, Magnetic, MineralPurifier, MagneticGenerator, ResourceRefiner, EnhancedSiege, ReloadingDevice, Drone,
    SpiralVortex
}