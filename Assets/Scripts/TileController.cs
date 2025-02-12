using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    // Start is called before the first frame update
    private GameController gameController;
    private int x, y;
    private bool hasShip;

    public void Init(GameController controller, int xCoord, int yCoord, bool shipPresent)
    {
        gameController = controller;
        x = xCoord;
        y = yCoord;
        hasShip = shipPresent;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseDown()
    {
        Debug.Log($"Tile clicked at ({x}, {y}) - Ship Present: {hasShip}");
        if (hasShip)
        {
            GetComponent<SpriteRenderer>().color = Color.red; // Change color if ship is present
        }
        else
        {
            GetComponent<SpriteRenderer>().color = Color.blue; // Change color if empty
        }
    }
}
