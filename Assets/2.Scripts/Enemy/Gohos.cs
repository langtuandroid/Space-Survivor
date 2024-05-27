using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gohos : MonoBehaviour
{

    //기체 회전속도
    public float rotateSpeed = 10f;

    [Space]

    [Header("탄막")]
    [SerializeField] private fireInfo[] fireRain_fireInfos;
    public ProjectileType projectileType;
    public float bulletRainFireRate = 0.5f;
    public float bulletRainFireForce = 0.04f;
    private Coroutine bulletRainCoroutine;

    [Space]
    [Header("흩뿌리기")]
    public int starBubbleBurstAmount = 3;
    public int starBubbleBurstAngle = 10;
    public float starBubbleBurstCoolTime = 5;
    public float starBubbleBurstFireForce = 0.04f;
    public Transform burstFirePos;
    private Coroutine fireStarBubbleBurstCoroutine;

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 0, Time.deltaTime * rotateSpeed));
    }

    private void OnEnable()
    {
        StartFireStarBubble();
        FireStarBubbleBurstStart();
    }

    public void StartFireStarBubble()
    {
        bulletRainCoroutine = StartCoroutine(fireingStarBubble());

        IEnumerator fireingStarBubble()
        {
            yield return new WaitForSeconds(5f);
            while (true)
            {
                for (int i = 0; i < fireRain_fireInfos.Length; i++)
                {
                    var projectile = ProjectileGenerator.instance.DeQueueProjectile(projectileType, fireRain_fireInfos[i].firepos.position);
                    projectile.GetComponent<StarBubble>().Fire(fireRain_fireInfos[i].firedir, bulletRainFireForce);
                }

                yield return new WaitForSeconds(bulletRainFireRate);
            }
        }
    }

    private void FireStarBubbleBurstStart()
    {
        fireStarBubbleBurstCoroutine = StartCoroutine(fireingStarBubbleBurst());

        IEnumerator fireingStarBubbleBurst()
        {
            yield return new WaitForSeconds(5f);
            while (true)
            {
                int Angel = 0;

                if (starBubbleBurstAmount > 1)
                    Angel = starBubbleBurstAngle / (starBubbleBurstAmount - 1);

                for (int i = 0; i < starBubbleBurstAmount; i++)
                {
                    var starBubble = ProjectileGenerator.instance.DeQueueProjectile(projectileType, burstFirePos.position);

                    starBubble.GetComponent<StarBubble>().FireToHead(GameManager.instance.playerTrans, starBubbleBurstFireForce, new Vector3(0, 0, (Angel * (i)) - (starBubbleBurstAngle / 2)));
                    starBubble.GetComponentInChildren<TrailRenderer>().enabled = true;
                }

                yield return new WaitForSeconds(starBubbleBurstCoolTime);
            }
        }
    }

    private void OnDisable()
    {
        if (bulletRainCoroutine != null)
            StopCoroutine(bulletRainCoroutine);

        if (fireStarBubbleBurstCoroutine != null)
            StopCoroutine(fireStarBubbleBurstCoroutine);
    }
}

[System.Serializable]
struct fireInfo
{
    [field: SerializeField] public Transform firepos { get; private set; }
    [field: SerializeField] public Transform firedir { get; private set; }
}