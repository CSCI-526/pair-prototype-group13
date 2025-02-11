using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileController : MonoBehaviour
{
    // Start is called before the first frame update
    private GameController gameController;
    private int x, y;
    public void Init(GameController controller, int xCoord, int yCoord)
    {
        gameController = controller;
        x = xCoord;
        y = yCoord;
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
        Debug.Log($"Tile clicked at ({x}, {y})");
    }
}
