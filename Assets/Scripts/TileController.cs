using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    private GameController gameController;
    private int x, y;
    private bool hasShip;
    public GameObject chipPrefab; 

    private GameObject placedChip = null; //track placed chip
    private static bool isBlackTurn = true;  //track turns

    private static Dictionary<Vector2Int, TileController> tileMap = new Dictionary<Vector2Int, TileController>(); // Stores all tiles

    public void Init(GameController controller, int xCoord, int yCoord, bool shipPresent)
    {
        gameController = controller;
        x = xCoord;
        y = yCoord;
        hasShip = shipPresent;
        tileMap[new Vector2Int(x, y)] = this; 
    }

    private void OnMouseDown()
    {
        Debug.Log($"Tile clicked at ({x}, {y}) - Ship Present: {hasShip}");

        
        if (hasShip)
        {
            GetComponent<SpriteRenderer>().color = Color.red; //change tile color if  ship is present
        }

        //place chip only if one isn't already placed
        if (placedChip == null && chipPrefab != null)
        {
            Vector3 chipPosition = transform.position + new Vector3(0, 0, -1f); 
            placedChip = Instantiate(chipPrefab, chipPosition, Quaternion.identity);
            placedChip.transform.SetParent(transform); //attach chip to tile
            placedChip.transform.localScale = new Vector3(0.8f, 0.8f, 1);

            //change chip color based on turn
            SpriteRenderer chipRenderer = placedChip.GetComponent<SpriteRenderer>();
            if (chipRenderer != null)
            {
                chipRenderer.color = isBlackTurn ? Color.black : Color.white; 
            }

            FlipOpponentChips(isBlackTurn);
            isBlackTurn = !isBlackTurn; //switch turn
        }
    }
    //flipping mechanics
    private void FlipOpponentChips(bool isBlack)
    {
        Vector2Int[] directions = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0), //horizontal
            new Vector2Int(0, 1), new Vector2Int(0, -1), //vertical
            new Vector2Int(1, 1), new Vector2Int(-1, -1), //diagonal - \
            new Vector2Int(1, -1), new Vector2Int(-1, 1) //diagonal - /
        };

        foreach (var dir in directions)
        {
            List<TileController> toFlip = new List<TileController>();
            Vector2Int checkPos = new Vector2Int(x, y) + dir;

            while (tileMap.ContainsKey(checkPos) && tileMap[checkPos].placedChip != null)
            {
                SpriteRenderer chipRenderer = tileMap[checkPos].placedChip.GetComponent<SpriteRenderer>();
                if (chipRenderer == null) break;

                Color chipColor = chipRenderer.color;
                Color currentTurnColor = isBlack ? Color.black : Color.white;

                if (chipColor == currentTurnColor) //flip all stored if a matching color is found
                {
                    foreach (var tile in toFlip)
                    {
                        tile.placedChip.GetComponent<SpriteRenderer>().color = currentTurnColor;
                    }
                    break;
                }
                else
                {
                    toFlip.Add(tileMap[checkPos]); 
                }

                checkPos += dir;
            }
        }
    }
}
