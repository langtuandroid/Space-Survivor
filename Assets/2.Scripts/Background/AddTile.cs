using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTile : MonoBehaviour
{
    [SerializeField] private InfinityBackground infinityBackground;
    [SerializeField] private Vector2Int tilePosition;

    private void Start()
    {
        infinityBackground.addBackgroundTile(tilePosition, gameObject);
    }
}
