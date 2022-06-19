using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[CreateAssetMenu(fileName = "new Weapon", menuName = "Scriptable Object/Weapon Data", order = int.MaxValue)]
public class WeaponObject : ScriptableObject
{
    public Stack<GameObject> bulletStack = new Stack<GameObject>();

    public List<GameObject> activeProjectile = new List<GameObject>();

    public GameObject projectilePrefab;
    public Sprite weaponImage;
    public Transform parent;
    public WeaponType type;
    private Transform firePos;
    private Transform fireDir;
    private Vector3 randomDir;
    public string firePosName = "FirePos1";
    public string fireDirName = "FireDir1";

    public bool ready = true;
    public Stat coolTime = new Stat();                 //�߻� ��Ÿ��

    [Space]

    public Stat damageMultifly = new Stat();

    [Space]

    public int projectileAmount = 1;            //�߻� ����
    public Stat firingInterval = new Stat();         //�߻� �ֱ�

    [Space]

    public Stat FireForce = new Stat();

    [Space]

    public List<UpgradeModule> currentUpgradeModules = new List<UpgradeModule>();
    public List<UpgradeModuleList> UpgradeModulesForLevel = new List<UpgradeModuleList>();

    [Space]

    public int maxWeaponLevel = 6;
    private int currentWeaponLevel = 1;

    public void UpgradeWeapon(UpgradeModuleList modules)
    {
        for(int i = 0; i < modules.upgradeModules.Count; i++)
        {
            switch (modules.upgradeModules[i].upgradeModuleType)
            {
                case upgradeModuleType.DamageIntIncrease:

                    foreach(GameObject projectile in bulletStack)
                    {
                        if(projectile.TryGetComponent<ProjectileLogic>(out ProjectileLogic logic))
                        {
                            DamageIncreaseInt(logic, Utility.RountToInt(modules.upgradeModules[i].value1));
                        }
                    }

                    for(int j = 0; j < activeProjectile.Count; j++)
                    {
                        if (activeProjectile[j].TryGetComponent<ProjectileLogic>(out ProjectileLogic logic))
                        {
                            DamageIncreaseInt(logic, Utility.RountToInt(modules.upgradeModules[i].value1));
                        }
                    }
                    break;

                case upgradeModuleType.CoolTimeDecrease:
                    coolTime.AddPercentModifier(modules.upgradeModules[i].value1);
                    break;

                case upgradeModuleType.IncreasedProjectiles:
                    projectileAmount += Mathf.RoundToInt(modules.upgradeModules[i].value1);
                    break;

                case upgradeModuleType.IncreaseFireForce:
                    FireForce.AddPercentModifier(modules.upgradeModules[i].value1);
                    break;

                case upgradeModuleType.IncreaseSize:
                    foreach (GameObject projectile in bulletStack)
                    {
                        IncreseSize(projectile, modules.upgradeModules[i].value1);
                    }
                    break;

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

                    if(type != VFXType.none)
                        VFXGenerator.instance.AddParticleSize(type, 0.1f);

                    break;


            }

            currentUpgradeModules.Add(modules.upgradeModules[i]);
        }

        currentWeaponLevel++;
        
    }

    private void DamageIncreaseInt(ProjectileLogic logic, int value)
    {
        logic.AddDamage(Utility.RountToInt(value));
    }

    private void IncreseSize(GameObject projectile, float percent)
    {
        projectile.transform.localScale *= percent;
    }

    private void IncreseRotateSpeed(ThronSpike spike, int rotate)
    {
        spike.AddRotateSpeed(rotate);
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

                case upgradeModuleType.IncreaseSize:
                    IncreseSize(projectile, currentUpgradeModules[i].value1);
                    break;

                case upgradeModuleType.IncreseRotateSpeed:
                    if (projectile.TryGetComponent<ThronSpike>(out ThronSpike spike))
                    {
                        IncreseRotateSpeed(spike, Utility.RountToInt(currentUpgradeModules[i].value1));
                    };
                    break;

                case upgradeModuleType.IncreseExplodeRadius:

                    if (projectile.TryGetComponent<Firecracker>(out Firecracker script))
                    {
                        script.AddExplodeRadius(currentUpgradeModules[i].value1);
                    }

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
}

[System.Serializable]
public class UpgradeModuleList
{
    public List<UpgradeModule> upgradeModules = new List<UpgradeModule>();
}

public enum WeaponType { SquareCannon, SmallShotCannon, HomingMissile, MeteoriteFlak, FireworkRocket, ThornSatellite
}