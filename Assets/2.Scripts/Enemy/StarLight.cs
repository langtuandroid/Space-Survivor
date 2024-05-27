using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StarLight : MonoBehaviour
{
    [SerializeField] private Transform firePos1, fireDir1, firePos2, fireDir2, firePos3, fireDir3;
    [Space]

    public float rotateSpeed = 10f;

    [Space]

    public ProjectileType starBubbleType;
    public float starBubbleFireRate = 0.5f;
    public float starBubbleFireForce = 0.04f;
    private Coroutine fireStarBubbleCoroutine;

    [Space]
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
        fireStarBubbleCoroutine = StartCoroutine(fireingStarBubble());

        IEnumerator fireingStarBubble()
        {
            yield return new WaitForSeconds(5f);
            while (true)
            {
                var starBubble1 = ProjectileGenerator.instance.DeQueueProjectile(starBubbleType, firePos1.position);
                var starBubble2 = ProjectileGenerator.instance.DeQueueProjectile(starBubbleType, firePos2.position);
                var starBubble3 = ProjectileGenerator.instance.DeQueueProjectile(starBubbleType, firePos3.position);
                starBubble1.GetComponent<StarBubble>().Fire(fireDir1, starBubbleFireForce);
                starBubble1.GetComponentInChildren<TrailRenderer>().enabled = false;
                starBubble2.GetComponent<StarBubble>().Fire(fireDir2, starBubbleFireForce);
                starBubble2.GetComponentInChildren<TrailRenderer>().enabled = false;
                starBubble3.GetComponent<StarBubble>().Fire(fireDir3, starBubbleFireForce);
                starBubble3.GetComponentInChildren<TrailRenderer>().enabled = false;

                yield return new WaitForSeconds(starBubbleFireRate);
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
                    var starBubble = ProjectileGenerator.instance.DeQueueProjectile(starBubbleType, burstFirePos.position);

                    starBubble.GetComponent<StarBubble>().FireToHead(GameManager.instance.playerTrans, starBubbleBurstFireForce, new Vector3(0, 0, (Angel * (i)) - (starBubbleBurstAngle / 2)));
                    starBubble.GetComponentInChildren<TrailRenderer>().enabled = true;
                }

                yield return new WaitForSeconds(starBubbleBurstCoolTime);
            }
        }
    }

    private void OnDisable()
    {
        if (fireStarBubbleCoroutine != null)
            StopCoroutine(fireStarBubbleCoroutine);

        if (fireStarBubbleBurstCoroutine != null)
            StopCoroutine(fireStarBubbleBurstCoroutine);
    }
}
