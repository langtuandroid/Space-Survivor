using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new Wave", menuName = "Scriptable Object/Wave Data", order = int.MaxValue)]
public class WaveObject : ScriptableObject
{
    //[Range(0,1800)]
    //public int StartWaveTime;
    //[Range(0, 1800)]
    //public int StopWaveTime;
    [Space]
    public waveType waveType;
    [Space]
    public float summonCycleTime = 1f;

    public EnemyObject enemyObject;

    public Coroutine waveCoroutine;

    public IEnumerator SummonPreiodically()
    {
        while (true)
        {
            EnemyGenerator.instance.GenerateEnemy2(enemyObject);

            yield return new WaitForSeconds(summonCycleTime);
        }
        
    }
}

public enum waveType { summonedPeriodically, summonBoss, blobBurstSummon, hyperBurstSummon }
