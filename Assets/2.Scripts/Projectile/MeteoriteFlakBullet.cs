using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteoriteFlakBullet : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private ProjectileLogic projectileLogic;


    [SerializeField] private float sizeRange = 0.5f;

    private Vector3 originSize;
    private float rotationSpeed = 0;

    private void Awake()
    {
        originSize = transform.localScale;
    }

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 0, rotationSpeed) * Time.deltaTime);
    }

    private void OnEnable()
    {
        ChangeSprite();
        ChangeSize();
        rotationSpeed = Random.Range(50, 200);
    }

    private void OnDisable()
    {
        transform.localScale = originSize;
    }


    private void ChangeSprite()
    {
        spriteRenderer.sprite = sprites[Random.Range(0, sprites.Length)];
    }

    private void ChangeSize()
    {
        var range = Random.Range(1f - sizeRange, 1f + sizeRange);
        transform.localScale = new Vector3(originSize.x * range, originSize.y * range);
    }
}
