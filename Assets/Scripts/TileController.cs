using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    private GameController gameController;
    private int x, y;
    private bool hasShip;
    public GameObject chipPrefab;

    private GameObject placedChip = null;

    private GameObject hoverChip = null;
    private static bool isBlackTurn = true;

    private static Dictionary<Vector2Int, TileController> tileMap = new Dictionary<Vector2Int, TileController>();
    private static List<GameObject> placedChips = new List<GameObject>();
    public void Init(GameController controller, int xCoord, int yCoord, bool shipPresent)
    {
        gameController = controller;
        x = xCoord;
        y = yCoord;
        hasShip = shipPresent;
        tileMap[new Vector2Int(x, y)] = this;
    }

    public static void ClearAllPlacedChips()
    {
        foreach (var chip in placedChips)
        {
            if (chip != null)
            {
                Destroy(chip);
            }
        }
        placedChips.Clear();
    }


    private void OnMouseEnter()
    {
        if (placedChip == null && chipPrefab != null)
        {
            Vector3 chipPosition = transform.position + new Vector3(0, 0, -0.5f);
            hoverChip = Instantiate(chipPrefab, chipPosition, Quaternion.identity);
            hoverChip.transform.SetParent(transform);
            hoverChip.transform.localScale = new Vector3(0.8f, 0.8f, 1);

            SpriteRenderer hoverRenderer = hoverChip.GetComponent<SpriteRenderer>();
            if (hoverRenderer != null)
            {
                Color hoverColor = isBlackTurn ? new Color(0, 0, 0, 0.3f) : new Color(1, 1, 1, 0.3f);
                hoverRenderer.color = hoverColor;
            }
        }
    }

    private void OnMouseExit()
    {
        if (hoverChip != null)
        {
            Destroy(hoverChip);
        }
    }

    private void OnMouseDown()
    {
        Debug.Log($"Tile clicked at ({x}, {y}) - Ship Present: {hasShip}");


        if (hasShip)
        {
            GetComponent<SpriteRenderer>().color = Color.red;
        }


        if (placedChip == null && chipPrefab != null)
        {
            Vector3 chipPosition = transform.position + new Vector3(0, 0, -1f);
            placedChip = Instantiate(chipPrefab, chipPosition, Quaternion.identity);
            placedChip.transform.SetParent(transform);
            placedChip.transform.localScale = new Vector3(0.8f, 0.8f, 1);


            SpriteRenderer chipRenderer = placedChip.GetComponent<SpriteRenderer>();
            if (chipRenderer != null)
            {
                chipRenderer.color = isBlackTurn ? Color.black : Color.white;
            }
            placedChips.Add(placedChip);
            FlipOpponentChips(isBlackTurn);
            isBlackTurn = !isBlackTurn;
            GameController.ChangeTurn(gameController);
        }
    }

    public static void ResetTileMap()
    {
        tileMap.Clear();
    }

    private void FlipOpponentChips(bool isBlack)
    {
        Vector2Int[] directions = {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1),
            new Vector2Int(1, 1), new Vector2Int(-1, -1),
            new Vector2Int(1, -1), new Vector2Int(-1, 1)
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

                if (chipColor == currentTurnColor)
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
