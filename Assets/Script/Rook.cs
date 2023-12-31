using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : ChessPiece
{
    // Start is called before the first frame update
    public override List<Vector2Int> GetAvailableMove(ref ChessPiece [,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
        // Down
        for (int i = currentY - 1; i >= 0; i--)
        {
            if (board [currentX, i] == null)
                r.Add(new Vector2Int(currentX, i));

            if (board [currentX, i] != null)
            {
                if(board [currentX, i].team != team)
                r.Add(new Vector2Int (currentX, i));
                break;
            }
        }

        // Up
        for (int i = currentY + 1; i < tileCountY; i++)
        {
            if (board [currentX, i] == null)
                r.Add(new Vector2Int(currentX, i));

            if (board [currentX, i] != null)
            {
                if(board [currentX, i].team != team)
                r.Add(new Vector2Int (currentX, i));
                break;
            }
        }

        // Left
        for (int i = currentX - 1; i >= 0; i--)
        {
            if (board [i, currentY] == null)
                r.Add(new Vector2Int(i, currentY));

            if (board [i, currentY] != null)
            {
                if(board [i, currentY].team != team)
                r.Add(new Vector2Int (i, currentY));
                break;
            }
        }

        // Right
        for (int i = currentX + 1; i < tileCountX; i++)
        {
            if (board [i, currentY] == null)
                r.Add(new Vector2Int(i, currentY));

            if (board [i, currentY] != null)
            {
                if(board [i, currentY].team != team)
                r.Add(new Vector2Int (i, currentY));
                break;
            }
        }
        
        return r;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       
    }

}
