using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private EnemyStat enemyStat;
    [SerializeField] private Rigidbody2D rigid;

    [Space]
    
    private Vector2 movedir;
    private float angle;
    [field: SerializeField] public bool moveStrate { get; set; } = false;
    [field: SerializeField] public Stat moveSpeed {get; private set;} = new Stat();
    [field: SerializeField] public Stat rotationSpeed {get; private set;} = new Stat();

    [field: SerializeField] public Transform target { get; private set; }

    private void FixedUpdate()
    {
        if (!moveStrate)
            movedir = (target.position - transform.position).normalized;

        rigid.velocity += movedir * Time.deltaTime * moveSpeed.GetFinalStatValue();

        angle = Mathf.Atan2((movedir.y + transform.position.y) - transform.position.y,
            (movedir.x + transform.position.x) - transform.position.x) * Mathf.Rad2Deg;

        if (rotationSpeed.GetFinalStatValue() > 0)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.AngleAxis(angle - 90, Vector3.forward), rotationSpeed.GetFinalStatValue() * Time.deltaTime);
    }

    public void SetMoveStrate()
    {
        moveStrate = true;
        movedir = (target.position - transform.position).normalized;
        moveSpeed.AddPercentModifier(1);
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    [ContextMenu("CustomMenu/InitValue")]
    private void InitValue()
    {
        if (TryGetComponent<EnemyStat>(out EnemyStat stat))
            enemyStat = stat;

        if (TryGetComponent<Rigidbody2D>(out Rigidbody2D rigidbody2D))
            rigid = rigidbody2D;

        //angle = enemyStat.angle;

        //moveSpeed = enemyStat.moveSpeed;
        //rotationSpeed = enemyStat.rotationSpeed;
    }

}
