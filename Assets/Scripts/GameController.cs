using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject tilePrefab;
    private int boardSize = 10;
    void Start()
    {
        GenerateBoard();
        AdjustCamera();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void AdjustCamera()
    {
        Camera.main.orthographicSize = 6.5f;
    }


    // void GenerateBoard()
    // {
    //     float offset = boardSize / 2.0f - 0.5f;

    //     for (int x = 0; x < boardSize; x++)
    //     {
    //         for (int y = 0; y < boardSize; y++)
    //         {
    //             Vector2 position = new Vector2(x - offset, y - offset);
    //             GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
    //             tile.name = $"Tile {x},{y}";
    //             TileController tileController = tile.GetComponent<TileController>();
    //             if (tileController != null)
    //             {
    //                 tileController.Init(this, x, y);
    //             }
    //         }
    //     }
    // }
    void GenerateBoard()
    {
        float offset = boardSize / 2.0f - 0.5f;
        float spacing = 1.1f;

        for (int x = 0; x < boardSize; x++)
        {
            for (int y = 0; y < boardSize; y++)
            {
                Vector2 position = new Vector2((x - offset) * spacing, (y - offset) * spacing);
                GameObject tile = Instantiate(tilePrefab, position, Quaternion.identity);
                tile.name = $"Tile {x},{y}";
                TileController tileController = tile.GetComponent<TileController>();
                if (tileController != null)
                {
                    tileController.Init(this, x, y);
                }
            }
        }
    }


}
