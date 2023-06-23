using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public List<Block> blockList;
    public Block[,] blockGrid;
    public int blockCount;
    public Vector2 gridSize;

    /// <summary>Fills the blockGrid with the correct blocks</summary>
    public void InitGrid()
    {
        blockGrid = new Block[(int)gridSize.x, (int)gridSize.y];
        for (int i = 0; i < blockList.Count; i++)
        {
            blockGrid[(int)blockList[i].gridCoordinates.x, (int)blockList[i].gridCoordinates.y] = blockList[i];
            Debug.Log(blockGrid[(int)blockList[i].gridCoordinates.x, (int)blockList[i].gridCoordinates.y].blockHealth.ToString());
        }
    }

    /// <summary>Finds out if the given coordinates are in the bounds of the grid, and if that coordinates has a block in it</summary>
    /// <returns>A bool if there is a block in the given coordinates</returns>
    public bool IsValidGridPosition(int x, int y)
    {
        if (((x >= 0 && x < gridSize.x) && (y >= 0 && y < gridSize.y)) && blockGrid[x, y] != null)
        {
            return true;
        }

        return false;        
    }
}