using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStat : EnemyStat
{
    [Space]
    [Header("Boss---------------------------")]
    [SerializeField] private Transform damageTextGeneartePosition;

    [Space]

    [SerializeField] private DropTable bossDropTable;

    [Space]

    [SerializeField] private Transform HP_parent;
    [SerializeField] private Transform HP_Bar;
    public Vector3 Hp_offset;

    public bool finalBoss = false;

    private void OnEnable() {
        HP_Bar.localScale = new Vector3(1,1,1);
    }

    private void FixedUpdate()
    {
        HP_parent.transform.position = transform.position + Hp_offset;
        HP_parent.transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
    }

    public override void Die()
    {
        base.Die();

        base.ResourceDrop(bossDropTable);

        if (finalBoss)
        {
            Item.HyperAtomicExplosion(transform.position);
            GameManager.instance.ClearStage();
        }
        else
        {
            EnemyGenerator.instance.bossFighting = false;
            CinemachinShake.instance.ShakeCamera(5, 1f);
        }

        EnemyGenerator.instance.deleteBossWall();
    }

    public override void TakeDamage(int damage, bool damageText)
    {
        if (damage <= 0)
            return;

        currentHp -= damage;

        if (damageText)
            TextGenerator.instance.DequeueText(damageTextGeneartePosition.position, damage, textGenerateOffset);

        OnChangeHp();

        HP_Bar.localScale = new Vector3((float)currentHp / (float)maxHp, 1, 1);
    }
}
