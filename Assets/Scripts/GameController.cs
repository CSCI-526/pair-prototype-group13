using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public GameObject tilePrefab;
    public TextMeshProUGUI turnText;
    //public Button resetButton;
    public TextMeshProUGUI winnerText;

    private int boardSize = 10;
    private int[,] grid; // 0 = Empty, 1 = Ship
    //private static bool iscolor1Turn = true;
    private List<Vector2Int> ships = new List<Vector2Int>(); // Track ship positions
    public List<List<Vector2Int>> Realgroups = new List<List<Vector2Int>>();

    //private int scorePlayer1 = 0;
    //private int scorePlayer2 = 0;

    void Start()
    {
        winnerText.gameObject.SetActive(false);

        grid = new int[boardSize, boardSize];
        PlaceShips();
        GenerateBoard();
        PrintGrid();
        AdjustCamera();
        UpdateTurnText();
        Realgroups = GroupShips(ships);

        ////if (resetButton != null)
        //{
            //resetButton.onClick.AddListener(ResetGame);
        //}

    }

    public void UpdateTurnText()
    {
        //iscolor1Turn = TileController.iscolor1Turn;
        turnText.text = TileController.iscolor1Turn ? "Player 1's Turn" : "Player 2's Turn";
    }

    public static void ChangeTurn(GameController gameController)
    {
        TileController.iscolor1Turn = !TileController.iscolor1Turn;
        gameController.UpdateTurnText();
        //gameController.CalculateScores(); 
        gameController.CheckGameEnd();
        //TileController.UpdateScore();

    }
    

    void AdjustCamera()
    {
        Camera.main.orthographicSize = 6.5f;
    }

    void GenerateBoard()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

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


    /*public void ResetGame()
    {
        Debug.Log("Reset button clicked!");

        TileController.iscolor1Turn = true;
        grid = new int[boardSize, boardSize];

        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        System.GC.Collect();
        Resources.UnloadUnusedAssets();         

        ships.Clear();
        Realgroups.Clear();
            
        TileController.ClearAllPlacedChips();
        TileController.ResetTileMap();
        TileController.ResetScores();
        winnerText.gameObject.SetActive(false);

        PlaceShips();
        GenerateBoard();
        PrintGrid();
        UpdateTurnText();
        Realgroups = GroupShips(ships);
    }*/

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
            ships.Add(new Vector2Int(x1, y1));
            ships.Add(new Vector2Int(x2, y2));
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
            ships.Add(new Vector2Int(x, y));
        }
    }
    void CheckGameEnd()
    {
        if (TileController.AllTilesFilled()) // Condition to end the game
        {

            if (TileController.scorePlayer1 > TileController.scorePlayer2)
            {
                winnerText.text = "Player 1 Wins!\n RELOAD to Play Again!";
            }
            else if (TileController.scorePlayer2 > TileController.scorePlayer1)
            {
                winnerText.text = "Player 2 Wins!\n RELOAD to Play Again!";
            }
            else
            {
                winnerText.text = "It's a Tie!\n RELOAD to Play Again!";
            }
            winnerText.gameObject.SetActive(true);
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
    void PrintShips()
    {
        string shipPositions = "Ships Positions:\n";
        foreach (Vector2Int position in ships)
        {
            shipPositions += $"({position.x}, {position.y})\n";
        }
        Debug.Log(shipPositions);
    }

List<List<Vector2Int>> GroupShips(List<Vector2Int> shipPositions)
{
    List<List<Vector2Int>> groups = new List<List<Vector2Int>>();
    HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

    // Include all 8 possible adjacency directions
    Vector2Int[] directions = new Vector2Int[]
    {
        new Vector2Int(1, 0), new Vector2Int(-1, 0), // Left & Right
        new Vector2Int(0, 1), new Vector2Int(0, -1), // Up & Down
        new Vector2Int(1, 1), new Vector2Int(-1, -1), // Diagonal 
        new Vector2Int(1, -1), new Vector2Int(-1, 1),  // Diagonal 

       
    };

    //Debug.Log($"Grouping Ships... Total Ship Positions: {shipPositions.Count}");

    foreach (Vector2Int ship in shipPositions)
    {
        if (!visited.Contains(ship))
        {
            List<Vector2Int> group = new List<Vector2Int>();
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            queue.Enqueue(ship);
            visited.Add(ship);

            while (queue.Count > 0)
            {
                Vector2Int current = queue.Dequeue();
                group.Add(current);

                foreach (Vector2Int dir in directions) // consider extra row shifts
                {
                    Vector2Int neighbor = current + dir;
                    if (shipPositions.Contains(neighbor) && !visited.Contains(neighbor))
                    {
                        queue.Enqueue(neighbor);
                        visited.Add(neighbor);
                    }
                }
            }

            groups.Add(group);
            //Debug.Log($"New Group Formed: {group.Count} ships.");
        }
    }

    Debug.Log($"Total Groups Formed: {groups.Count}");
    return groups;
}


    

        void PrintShipGroups(List<List<Vector2Int>> groups)
    {
        List<List<Vector2Int>> Realgroups = GroupShips(ships);

        string output = "Grouped Ships:\n";
        int groupNumber = 1;
        foreach (var group in Realgroups)
        {
            output += $"Group {groupNumber}: ";
            foreach (var pos in group)
            {
                output += $"({pos.x}, {pos.y}) ";
            }
            output += "\n";
            groupNumber++;
        }
        Debug.Log(output);
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
        PrintShips();
        PrintShipGroups(Realgroups);
    }
}
