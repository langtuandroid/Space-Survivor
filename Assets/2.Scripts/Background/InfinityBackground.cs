using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfinityBackground : MonoBehaviour
{
    [field: SerializeField] public Stage stage {get; private set;}

    [SerializeField] Transform playerPosition;
    [SerializeField] Vector2Int currentPlayerTilePosition;
    [Space]
    [SerializeField] Vector2 tileSize = new Vector2(20f, 20f);
    [SerializeField] GameObject[,] Tiles;

    [Space]
    [SerializeField] int backgroundHeight = 4;
    [SerializeField] int backgroundWidth = 4;


    private void Awake()
    {
        Tiles = new GameObject[backgroundHeight, backgroundWidth];

        Application.targetFrameRate = 60;
    }

    // Update is called once per frame
    void Update()
    {
        var tempCurrentPlayerTilePosition = new Vector2Int((int)Mathf.Round(playerPosition.position.x / tileSize.x), (int)Mathf.Round(playerPosition.position.y / tileSize.y));

        if(currentPlayerTilePosition != tempCurrentPlayerTilePosition)
        {
            currentPlayerTilePosition = tempCurrentPlayerTilePosition;
            OnChangeTilePosition();
        }
    }

    private void OnChangeTilePosition()
    {
        for(int i = 0; i < backgroundHeight; i++)
        {
            for (int j = 0; j < backgroundWidth; j++)
            {
                Tiles[i, j].transform.position = new Vector2(((currentPlayerTilePosition.x + i - (backgroundWidth / 2)) * tileSize.x)
                    , ((currentPlayerTilePosition.y + j - (backgroundWidth / 2)) * tileSize.y));
            }
        }
    }

    public void addBackgroundTile(Vector2Int tilePosition, GameObject tileObject)
    {
        Tiles[tilePosition.x + (backgroundWidth/2), tilePosition.y + (backgroundHeight / 2)] = tileObject;
    }
}
