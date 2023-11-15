using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : ChessPiece
{

    public override List<Vector2Int> GetAvailableMove(ref ChessPiece [,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();

        // Top right
        int x = currentX + 1;
        int y = currentY + 2;
        if(x <tileCountX && y < tileCountY)
            if(board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x,y));

        x = currentX + 2;
        y = currentY + 1;
        if(x <tileCountX && y < tileCountY)
            if(board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x,y));

         // Top left
        x = currentX - 1;
        y = currentY + 2;
        if(x >= 0 && y < tileCountY)
            if(board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x,y));
        
        x = currentX - 2;
        y = currentY + 1;
        if(x >= 0 && y < tileCountY)
            if(board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x,y));
        
        // bottom right
        x = currentX + 1;
        y = currentY - 2;
        if(x < tileCountX && y >= 0)
            if(board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x,y));

        x = currentX + 2;
        y = currentY - 1;
        if(x < tileCountX && y >= 0)
            if(board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x,y));

        // bottom left
        x = currentX - 1;
        y = currentY - 2;
        if(x >= 0 && y >= 0)
            if(board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x,y));

        x = currentX - 2;
        y = currentY - 1;
        if(x >= 0 && y >= 0)
            if(board[x, y] == null || board[x, y].team != team)
                r.Add(new Vector2Int(x,y));

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
