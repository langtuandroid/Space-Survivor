using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using Sirenix.OdinInspector;


public class EnemyStat : MonoBehaviour
{
    [SerializeField] private EnemyType type;
    public EnemyType GetEnemyType() => type;

    [SerializeField] protected int maxHp = 10;
    protected int currentHp = 10;

    [Space]
    //[SerializeField] private int damage = 5;
    [SerializeField] private Stat damage = new Stat();
    [SerializeField] private bool hitEffect = true;
    [SerializeField] private float hitEffectTime = 0.1f;
    [SerializeField] private Material hitMat;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private Material originalMat;

    [Space]
    [SerializeField] private Rigidbody2D rigid;
    [SerializeField] private DropTable dropTable;
    [SerializeField] private VFXType dieVFXType;
    [SerializeField] private EnemyAI enemyAi;
    [field: SerializeField] public string dieSound { get; private set; } = "kill1";

    public TextGenerateOffset textGenerateOffset;

    [field: SerializeField] public Transform target { get; private set; }

    private void Start()
    {
        currentHp = maxHp;
        originalMat = spriteRenderer.material;
    }

    public virtual void TakeDamage(int damage, bool damageText = true)
    {
        if (damage <= 0)
            return;

        currentHp -= damage;

        if (damageText)
            TextGenerator.instance.DequeueText(transform.position, damage, textGenerateOffset);

        OnChangeHp();
    }

    protected void OnChangeHp()
    {
        if (currentHp <= 0)
        {
            Die();
        }
        else
        {
            if (hitEffect)
                StartCoroutine(HitEffect());
        }
    }

    public virtual void Die()
    {
        EnQueueThisEnemy();

        ResourceDrop(dropTable);

        //��� ��ƼŬ ����
        if (dieVFXType != VFXType.none)
        {
            VFXGenerator.instance.GenerateVFX(dieVFXType, transform.position);
        }

        GameManager.instance.AddKillCount();

        spriteRenderer.material = originalMat;

        AudioManager.instance.GenerateAudioAndPlaySFX(dieSound, transform.position);

        switch (type)
        {
            case EnemyType.goldGoblin1:
                EventManager.instance.CustomEvent(AnalyticsType.GAME, "GoldGoblin1KilledEvent");
                break;

            case EnemyType.goldGoblin2:
                EventManager.instance.CustomEvent(AnalyticsType.GAME, "GoldGoblin2KilledEvent");
                break;
        }
    }

    protected void ResourceDrop(DropTable dropTable)
    {
        //����ġ ���
        if (dropTable.expType != resourceType.none)
            ResourceGenerator.instance.DeQueueResource(dropTable.expType, transform.position, dropTable);
        //Instantiate(expObject,transform.position,Quaternion.identity);

        //ũ����Ż ���
        if (Utility.PercentageCalculator(dropTable.crystalDropPercent, 100))
        {
            ResourceGenerator.instance.DeQueueResource(dropTable.crystalType, transform.position, dropTable);
        }

        //������ ���
        if (Utility.PercentageCalculator(dropTable.ItemDropPercent, 1000))
        {
            ItemGenerator.instance.GenerateRandomItem(transform.position);
        }

        if (dropTable.moduleDropTable != null)
        {
            if (Utility.PercentageCalculator(dropTable.moduleDropTable.dropPercent, UpgradeModuleDropTable.dropMaxPercent))
            {
                UpgradeModuleDropManager.instance.GenerateRandomModuleDrops(transform.position);
            }
        }
    }

    public new EnemyType GetType()
    {
        return type;
    }

    public void ResetStat()
    {
        currentHp = maxHp;
    }

    public void EnQueueThisEnemy()
    {
        EnemyGenerator.instance.EnQueueEnemy(this);
        gameObject.SetActive(false);

        enemyAi.moveStrate = false;
        enemyAi.moveSpeed.ClearPercentModifier();
    }

    public void Knockback(Vector2 hitPoint, int force)
    {
        rigid.AddForce(Utility.GetDirection(transform.position, hitPoint) * force, ForceMode2D.Impulse);
    }

    private IEnumerator HitEffect()
    {
        spriteRenderer.material = hitMat;

        yield return new WaitForSeconds(hitEffectTime);

        spriteRenderer.material = originalMat;
    }

    public int GetDamage()
    {
        return damage.GetFinalStatValueAsInt();
    }

    [ContextMenu("CustomMenu/InitValue")]
    private void InitValue()
    {
        if (TryGetComponent<EnemyAI>(out EnemyAI ai))
            enemyAi = ai;
    }

    [Button]
    public void DieInvoke()
    {
        Die();
    }
}

[System.Serializable]
public class DropTable
{
    public resourceType expType;

    [Space]

    public resourceType crystalType;
    [Range(0, 100)]
    public int crystalDropPercent;

    [Space]
    [Range(0, 1000)]
    public int ItemDropPercent;

    [Space]
    public float minDropForce;
    public float maxDropForce;

    [Space]

    public int dropExpMinAmount = 1;
    public int dropExpmaxAmount = 1;

    [Space]

    public int dropCrystalMinAmount = 1;
    public int dropCrystalmaxAmount = 1;

    [Space]

    [Range(0, 1000)]
    public int ModuleDropPercent = 3;

    public UpgradeModuleDropTable moduleDropTable;
}
