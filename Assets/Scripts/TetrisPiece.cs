using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TetrisPiece : MonoBehaviour
{
    private TetrisGrid grid;
    private float dropInterval = 1f;
    private float dropTimer;
    private void Start()
    {
        grid = FindObjectOfType<TetrisGrid>();
        dropTimer = dropInterval;
    }
    private void Update()
    {
        HandleAutomaticDrop();
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move(Vector3.left);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move(Vector3.right);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            Move(Vector3.down);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Rotate();
        }
    }
    void Move(Vector3 direction)
    {
        transform.position += direction;

        if (!IsValidPosition())
        {
            transform.position -= direction; // revert invalid moves
            if (direction == Vector3.down)
            {
                LockPiece();
            }
        }
    }

    bool IsValidPosition()
    {
        foreach (Transform block in transform)
        {
            Vector2Int position = Vector2Int.RoundToInt(block.position);

            if (grid.IsCellOccupied(position))
            {
                return false;

            }
        }
        return true;
    }

    void Rotate()
    {
        transform.Rotate(0, 0, 90);
        if (!IsValidPosition())
        {
            transform.Rotate(0, 0, -90);//revert invalid rotations 
        }
    }

    private void HandleAutomaticDrop()
    {
        dropTimer -= Time.deltaTime;

        if (dropTimer <= 0)
        {
            Move(Vector3.down);
            dropTimer = dropInterval;//reset timer
        }
    }

    private void LockPiece()
    {
        foreach(Transform block in transform)
        {
            Vector2Int position = Vector2Int.RoundToInt(block.position);
            grid.AddBlockToGrid(block, position); // add block to grid 
        }
        grid.ClearFullLines();//check for full lines 
        FindObjectOfType<TetrisSpawner>().SpawnPiece();
        Destroy(this); //remove this script
    }
}           
