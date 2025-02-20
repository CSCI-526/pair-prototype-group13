using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro; 
public class TileController : MonoBehaviour
{
    private GameController gameController;

    private int x, y;
    private bool hasShip;
    public GameObject chipPrefab;
    public static Color color1 = new Color(145f / 255f, 77f / 255f, 4f / 255f); 
    public static Color color2 = new Color(247f / 255f, 183f / 255f, 99f / 255f);
    public GameObject placedChip = null;

    private GameObject hoverChip = null;
    private static bool iscolor1Turn = true;


    public static Dictionary<Vector2Int, TileController> tileMap = new Dictionary<Vector2Int, TileController>();
    private static List<GameObject> placedChips = new List<GameObject>();
    private static GameObject InstructionsPanel;


    private int scorePlayer1 = 0;
    private int scorePlayer2 = 0;

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

    private void UpdateScoreUI()
{
    TextMeshProUGUI scoreText = GameObject.Find("ScoreText")?.GetComponent<TextMeshProUGUI>();

    if (scoreText != null)
    {
        scoreText.text = $"Player 1: {scorePlayer1} | Player 2: {scorePlayer2}";
    }
    else
    {
        Debug.LogError("ScoreText UI element not found!");
    }
}
    private void UpdateScore()
{
    int newScorePlayer1 = 0;
    int newScorePlayer2 = 0;
    Debug.Log("UpdateScore() called!");
    Debug.Log($"Total Ship Groups: {gameController.Realgroups.Count}");
    foreach (var group in gameController.Realgroups)
    {
        
        Debug.Log($"Checking Group: {group}");

        int player1Count = 0;
        int player2Count = 0;

        foreach (var pos in group)
        {
            Debug.Log($"Checking Pos: {pos}");

            if (tileMap.ContainsKey(pos)&& tileMap[pos].placedChip != null)
            {
                SpriteRenderer chipRenderer = tileMap[pos].placedChip.GetComponent<SpriteRenderer>();
                if (chipRenderer == null)
                {
                    Debug.LogError($"Tile at {pos} has no SpriteRenderer!");
                    continue;
                }

                if (chipRenderer.color == color1)
                {
                    player1Count++;
                    Debug.Log($"Tile at {pos} belongs to Player 1");
                }
                else if (chipRenderer.color == color2)
                {
                    player2Count++;
                    Debug.Log($"Tile at {pos} belongs to Player 2");
                }
            }
            else
            {
                Debug.LogWarning($"Tile at {pos} has NO CHIP or is missing in tileMap!");
            }
        }

        if (player1Count > player2Count)
        {
            newScorePlayer1 += 1;
        }
        else if (player2Count > player1Count)
        {
            newScorePlayer2 += 1;
        }
    }

    scorePlayer1 = newScorePlayer1;
    scorePlayer2 = newScorePlayer2;

    Debug.Log($"FINAL Updated Scores -> Player 1: {scorePlayer1}, Player 2: {scorePlayer2}");

    UpdateScoreUI();
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
            //gameController.CalculateScores();
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
            tileMap[new Vector2Int(x, y)] = this;
            Debug.Log($"Tile ({x},{y}) now has a chip for Player {(iscolor1Turn ? 1 : 2)}");

            placedChips.Add(placedChip);
            FlipOpponentChips(iscolor1Turn);

            UpdateScore();

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

