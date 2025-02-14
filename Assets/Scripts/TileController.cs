using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileController : MonoBehaviour
{
    private GameController gameController;

    private int x, y;
    private bool hasShip;
    public GameObject chipPrefab;
    public Color color1 = new Color(145f / 255f, 77f / 255f, 4f / 255f); 
    public Color color2 = new Color(247f / 255f, 183f / 255f, 99f / 255f);
    private GameObject placedChip = null;

    private GameObject hoverChip = null;
    private static bool iscolor1Turn = true;


    private static Dictionary<Vector2Int, TileController> tileMap = new Dictionary<Vector2Int, TileController>();
    private static List<GameObject> placedChips = new List<GameObject>();
    private static GameObject InstructionsPanel;

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
                Color hoverColor = iscolor1Turn ? new Color(0, 0, 0, 0.3f) : new Color(1, 1, 1, 0.3f);
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

        if (EventSystem.current.IsPointerOverGameObject())
        {
            Debug.Log("Clicked on UI, ignoring tile click.");
            return;
        }

        if (InstructionsPanel != null && InstructionsPanel.activeSelf)
        {
            Debug.Log("Tile click ignored because instructions panel is active.");
            return;
        }


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
                chipRenderer.color = iscolor1Turn ? color1 : color2;
            }

            placedChips.Add(placedChip);
            FlipOpponentChips(iscolor1Turn);
            iscolor1Turn = !iscolor1Turn;
            GameController.ChangeTurn(gameController);
        }
    }

    public static void ResetTileMap()
    {
        tileMap.Clear();
    }

    private void FlipOpponentChips(bool iscolor1)
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
                Color currentTurnColor = iscolor1 ? color1 : color2;

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
