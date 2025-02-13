using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject tilePrefab;
    public TextMeshProUGUI turnText;
    public Button resetButton;

    private int boardSize = 10;
    private int[,] grid; // 0 = Empty, 1 = Ship
    private List<int> shipSizes = new List<int> { 3, 2, 5, 8, 6 };
    private static bool isBlackTurn = true;


    void Start()
    {
        grid = new int[boardSize, boardSize];
        PlaceShips();
        GenerateBoard();
        PrintGrid(); // For Debugging only
        AdjustCamera();
        UpdateTurnText();

        if (resetButton != null)
        {
            resetButton.onClick.AddListener(ResetGame);
        }
    }

    public void UpdateTurnText()
    {
        turnText.text = isBlackTurn ? "Black's Turn" : "White's Turn";
    }

    public static void ChangeTurn(GameController gameController)
    {
        isBlackTurn = !isBlackTurn;
        gameController.UpdateTurnText();
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
                    bool hasShip = grid[x, y] == 1; // Check if this tile has a ship
                    tileController.Init(this, x, y, hasShip);
                }
            }
        }
    }

    public void ResetGame()
    {
        Debug.Log("Reset button clicked!"); // Debugging log

        isBlackTurn = true;
        grid = new int[boardSize, boardSize]; // Reset grid

        // Destroy all existing tile objects (tiles and placed chips)
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // Destroy all chips placed on the board
        TileController.ClearAllPlacedChips();

        TileController.ResetTileMap(); // Clear tile references
        PlaceShips(); // Reposition ships
        GenerateBoard(); // Recreate board
        UpdateTurnText();
    }



    void PlaceShips()
    {
        foreach (int shipSize in shipSizes)
        {
            bool placed = false;

            while (!placed)
            {
                int startX = Random.Range(0, boardSize);
                int startY = Random.Range(0, boardSize);
                bool isHorizontal = Random.Range(0, 2) == 0; // Random orientation

                if (shipSize == 8)
                {
                    placed = TryPlaceSplitShip(startX, startY, isHorizontal);
                }
                else
                {
                    if (CanPlaceShip(startX, startY, shipSize, isHorizontal))
                    {
                        PlaceShip(startX, startY, shipSize, isHorizontal);
                        placed = true;
                    }
                }
            }
        }
    }

    bool TryPlaceSplitShip(int startX, int startY, bool isHorizontal)
    {
        if (isHorizontal)
        {
            if (startX + 4 > boardSize || startY + 1 >= boardSize) return false; // Ensure space for 2 rows
        }
        else
        {
            if (startY + 4 > boardSize || startX + 1 >= boardSize) return false; // Ensure space for 2 columns
        }

        // Check for overlap
        for (int i = 0; i < 4; i++)
        {
            int x1 = isHorizontal ? startX + i : startX;
            int y1 = isHorizontal ? startY : startY + i;
            int x2 = isHorizontal ? startX + i : startX + 1;
            int y2 = isHorizontal ? startY + 1 : startY + i;

            if (grid[x1, y1] == 1 || grid[x2, y2] == 1) return false; // Overlap detected
        }

        // Place the split ship
        for (int i = 0; i < 4; i++)
        {
            int x1 = isHorizontal ? startX + i : startX;
            int y1 = isHorizontal ? startY : startY + i;
            int x2 = isHorizontal ? startX + i : startX + 1;
            int y2 = isHorizontal ? startY + 1 : startY + i;

            grid[x1, y1] = 1;
            grid[x2, y2] = 1;
        }

        return true;
    }

    bool CanPlaceShip(int startX, int startY, int size, bool isHorizontal)
    {
        if (isHorizontal)
        {
            if (startX + size > boardSize) return false;
        }
        else
        {
            if (startY + size > boardSize) return false;
        }

        for (int i = 0; i < size; i++)
        {
            int x = isHorizontal ? startX + i : startX;
            int y = isHorizontal ? startY : startY + i;

            if (grid[x, y] == 1) return false; // Already occupied
        }

        return true;
    }

    void PlaceShip(int startX, int startY, int size, bool isHorizontal)
    {
        for (int i = 0; i < size; i++)
        {
            int x = isHorizontal ? startX + i : startX;
            int y = isHorizontal ? startY : startY + i;
            grid[x, y] = 1; // Mark as a ship
        }
    }

    void PrintGrid()
    {
        string gridString = "";
        for (int y = 0; y < boardSize; y++)
        {
            for (int x = 0; x < boardSize; x++)
            {
                gridString += grid[x, y] + " ";
            }
            gridString += "\n";
        }
        Debug.Log(gridString); // Prints the grid in the Unity Console
    }
}