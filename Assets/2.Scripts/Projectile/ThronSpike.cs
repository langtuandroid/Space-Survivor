using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThronSpike : ProjectileLogic
{

    [SerializeField] private Transform player;
    [SerializeField] private Stat rotateSpeed = new Stat();
    [SerializeField] private Vector3 rotateDir = new Vector3(0, 0, 1);

    [SerializeField] private TrailRenderer trailRenderer;
    void Update()
    {
        transform.transform.RotateAround(player.position, rotateDir, rotateSpeed.GetFinalStatValue() * Time.deltaTime);
    }

    public override void Fire(Transform target, int fireForce)
    {
        base.Fire(target, fireForce);

        ChangePosition(weaponObject.projectileAmount, weaponObject.activeProjectile.Count);
    }
    public void ChangePosition(int projectileCount, int thisCount)
    {

        player = GetComponent<ProjectileLogic>().GetPlayerWeapon().transform;
        transform.SetParent(player);

        transform.localScale = weaponObject.currentSizeVector * weaponObject.currentSize.GetFinalStatValue();

        transform.localPosition = new Vector3(0, 60, 0);

        int angle = (360 / projectileCount);

        transform.RotateAround(player.position, new Vector3(0, 0, 1), angle * (thisCount + 1));

        trailRenderer.Clear();

        //print(transform.rotation.eulerAngles);
    }

    public void AddRotateSpeed(float speed)
    {
        rotateSpeed.AddFloatModifier(speed);
    }

    private void OnDisable()
    {
        if(!GetPlayerWeapon().CheckProjectileContainQueue(this))
        {
            OffProjectile();
        }
        
    }

    public override void ResetProjectileStat()
    {
        base.ResetProjectileStat();

        rotateSpeed.ClearFloatModifier();
        rotateSpeed.ClearPercentModifier();
    }

    public override void IncreseRotateSpeed(float value)
    {
        base.IncreseRotateSpeed(value);

        AddRotateSpeed(value);
    }
}
