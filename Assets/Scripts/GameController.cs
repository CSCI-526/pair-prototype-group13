using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject tilePrefab;
    public TextMeshProUGUI turnText;
    public Button resetButton;

    private int boardSize = 10;
    private int[,] grid; // 0 = Empty, 1 = Ship
    private static bool isBlackTurn = true;

    void Start()
    {
        grid = new int[boardSize, boardSize];
        PlaceShips();
        GenerateBoard();
        PrintGrid();
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

    void AdjustCamera()
    {
        Camera.main.orthographicSize = 6.5f;
    }

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
                    bool hasShip = grid[x, y] == 1;
                    tileController.Init(this, x, y, hasShip);
                }
            }
        }
    }

    public void ResetGame()
    {
        Debug.Log("Reset button clicked!");

        isBlackTurn = true;
        grid = new int[boardSize, boardSize];

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        TileController.ClearAllPlacedChips();
        TileController.ResetTileMap();
        PlaceShips();
        GenerateBoard();
        PrintGrid();
        UpdateTurnText();
    }

    void PlaceShips()
    {
        List<int> shipSizes = new List<int> { 2, 3, 4, 6, 8 };

        foreach (int shipSize in shipSizes)
        {
            bool placed = false;

            while (!placed)
            {
                int startX = Random.Range(0, boardSize);
                int startY = Random.Range(0, boardSize);
                bool isHorizontal = Random.Range(0, 2) == 0;

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
            if (startX + 4 > boardSize - 1 || startY + 1 >= boardSize - 1) return false;
        }
        else
        {
            if (startY + 4 > boardSize - 1 || startX + 1 >= boardSize - 1) return false;
        }

        for (int i = -1; i <= 4; i++)
        {
            int x1 = isHorizontal ? startX + i : startX;
            int y1 = isHorizontal ? startY : startY + i;
            int x2 = isHorizontal ? startX + i : startX + 1;
            int y2 = isHorizontal ? startY + 1 : startY + i;

            if (IsOutOfBounds(x1, y1) || IsOutOfBounds(x2, y2)) continue;

            if (grid[x1, y1] == 1 || grid[x2, y2] == 1 || IsAdjacentOccupied(x1, y1) || IsAdjacentOccupied(x2, y2))
                return false;
        }

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
            if (startX + size > boardSize - 1) return false;
        }
        else
        {
            if (startY + size > boardSize - 1) return false;
        }

        for (int i = -1; i <= size; i++)
        {
            int x = isHorizontal ? startX + i : startX;
            int y = isHorizontal ? startY : startY + i;

            if (IsOutOfBounds(x, y)) continue;

            if (grid[x, y] == 1 || IsAdjacentOccupied(x, y)) return false;
        }

        return true;
    }

    void PlaceShip(int startX, int startY, int size, bool isHorizontal)
    {
        for (int i = 0; i < size; i++)
        {
            int x = isHorizontal ? startX + i : startX;
            int y = isHorizontal ? startY : startY + i;
            grid[x, y] = 1;
        }
    }

    bool IsAdjacentOccupied(int x, int y)
    {
        int[][] directions = {
        new int[] { -1, -1 }, new int[] { -1, 0 }, new int[] { -1, 1 },
        new int[] { 0, -1 }, new int[] { 0, 1 },
        new int[] { 1, -1 }, new int[] { 1, 0 }, new int[] { 1, 1 }
    };

        foreach (int[] dir in directions)
        {
            int newX = x + dir[0];
            int newY = y + dir[1];

            if (!IsOutOfBounds(newX, newY) && grid[newX, newY] == 1)
            {
                return true;
            }
        }
        return false;
    }

    bool IsOutOfBounds(int x, int y)
    {
        return x < 0 || y < 0 || x >= boardSize || y >= boardSize;
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
        Debug.Log("Final Grid:\n" + gridString);
    }
}
