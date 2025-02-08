using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisGrid : MonoBehaviour
{
    public int width = 10;
    public int height = 26;
    private Transform[,] grid;
    public Transform[,] debugGrid;
    TetrisManager tetrisManager;
    TetrisSpawner spawner;
     void Awake()
    {
        tetrisManager = FindObjectOfType <TetrisManager>();
        grid = new Transform[width, height];
        debugGrid = new Transform[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject cell = new GameObject($"cell({i},{j})");
                cell.transform.position = new Vector3(i, j, 0);
                debugGrid[i,j] = cell.transform;
            }
        }
    }
    public void AddBlockToGrid(Transform block, Vector2Int position)
    {
        grid[position.x, position.y] = block;
    }

    //checks to see if a piece can move to a spot
    public bool IsCellOccupied(Vector2Int position)
    {
        if (position.x < 0 || position.x >= width || position.y < 0 || position.y >= height)
        {
            return true; //out of bounds 
        }
       return grid [position.x, position.y] !=null;
    }

    //checks cells in a line to see if they are all occupied 
    public bool IsLineFull(int rowNumber)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x,rowNumber] == null)
            {
                return false;
            }
        }
        return true;
    }

    public void SpecialClear()
    {
        int linesCleared = 0;

        // loops through all rows in the grid
        for (int y = 0; y < height; y++)
        {
            bool containsSpecialPiece = false;

            for (int x = 0; x < width; x++)
            {
                if (grid[x, y] != null && grid[x, y].CompareTag("SpecialPiece"))//checks if special piece tag is implemented
                {
                    containsSpecialPiece = true;
                    
                    break;
                }
            }

            if (containsSpecialPiece) //if special piece tag then does the clear
            {
                
                ClearLine(y);
                ShiftRowsDown(y);
                y--; // recheck the current row after shifting
                linesCleared++;
            }
        }

        if (linesCleared > 0)
        {
           
            tetrisManager.CalculateScore(linesCleared);
        }
    }
    public void ClearLine(int rowNumber)
    {
        for (int x = 0; x < width; x++)
        {
            if (grid[x, rowNumber] != null) 
            {
                Destroy(grid[x, rowNumber].gameObject); 
                grid[x, rowNumber] = null; 
            }
        }
    }

    public void ClearFullLines()
    {
        int linesCleared = 0 ;
        for(int y = 0; y < height; y++)
        {
            if (IsLineFull(y))
            {
                ClearLine(y);
                ShiftRowsDown(y);
                y--;//recheck the current row after shifting 
                linesCleared++;
            }
        }
        if(linesCleared>0)
        tetrisManager.CalculateScore(linesCleared);
    }


    //removes blocks that are above a line being cleared down 1
    public void ShiftRowsDown(int clearedRow) 
    {
        for(int y = clearedRow; y < height -1; y++)
        {
            for (int x = 0; x < width; x++)
            {
                grid[x,y] = grid[x,y +1];
                if (grid[x,y] != null)
                {
                    grid[x, y].position += Vector3.down;
                    
                }
                grid[x, y + 1] = null;
            }
        }
    }
     void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if(debugGrid !=null )
        {
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Gizmos.DrawWireCube(debugGrid[i, j].position, Vector2.one);
                }
            }
        }
    }
}
