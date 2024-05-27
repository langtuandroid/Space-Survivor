using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnqueueWall : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.TryGetComponent<EnemyStat>(out EnemyStat enemy))
        {
            if (other.CompareTag("Enemy") && enemy.GetComponent<BossStat>() == null)
            {
                enemy.EnQueueThisEnemy();
            }

        }
    }
}
