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
        foreach (Transform block in transform)
        {
            Vector2Int position = Vector2Int.RoundToInt(block.position);

            // Ensure the block is correctly tagged before adding it to the grid
            if (CompareTag("SpecialPiece"))
            {
                block.tag = "SpecialPiece";  // Force correct tag
            }

            grid.AddBlockToGrid(block, position);
           
        }

        grid.SpecialClear();
        grid.ClearFullLines();
        FindObjectOfType<TetrisSpawner>().SpawnPiece();
        Destroy(this);
    }

    private void RotatePiece()
    {
        //store original position and rotation for rollback

        Vector3 originalPosition = transform.position;
        Quaternion originalRotation = transform.rotation;
        transform.Rotate(0, 0, 90);

        if (!IsValidPosition())
        {
            if (!TryWallKick(transform.position, transform.rotation))
            {
                transform.position = originalPosition;
                transform.rotation = originalRotation;
                Debug.Log("Rotation invalid, reverting rotation/postion");
            }
            else
            {
                Debug.Log("Rotation/position adjusted with wall kick");
            }
        }
    }
    private bool TryWallKick(Vector3 originalPosition, Quaternion originalRotation)
    {
        //define wall kicks (srs guidelines)
        Vector2Int[] wallKickOffsets = new Vector2Int[]
        {
        new Vector2Int(1,0), //Right by 1
         new Vector2Int(-1,0), //Left
          new Vector2Int(0,-1), //Down
           new Vector2Int(1,-1), //Diagonally right-down
            new Vector2Int(-1,-1), //Diagonally left-down

        new Vector2Int(2,0), //Right by 2
         new Vector2Int(-2,0), //Left
          new Vector2Int(0,-2), //Down
           new Vector2Int(2,-1), //Diagonally right-down
            new Vector2Int(2,-2), //Diagonally left-down
             new Vector2Int(-2,-1), //Diagonally right-down
              new Vector2Int(-2,-2), //Diagonally left-down


        new Vector2Int(3,0), //Right by 3
         new Vector2Int(-3,0), //Left
          new Vector2Int(0,-3), //Down
           new Vector2Int(3,-1), //Diagonally right-down
            new Vector2Int(-3,-1), //Diagonally left-down
             new Vector2Int(3,-2), //Diagonally right-down
              new Vector2Int(-3,-2), //Diagonally left-down
               new Vector2Int(3,-3), //Diagonally right-down
                new Vector2Int(-3,-3), //Diagonally left-down
        };

        foreach (Vector2Int offset in wallKickOffsets)
        {
            //apply offset to piece
            transform.position += (Vector3Int)offset;

            if (IsValidPosition())
            {
                return true;
            }
            //revert position if invalid
            transform.position -= (Vector3Int)offset;
        }
        //loop through all offsets to see which one is valid
        return false;
    }
}           
