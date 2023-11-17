using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : ChessPiece
{
    public override List<Vector2Int> GetAvailableMove(ref ChessPiece [,] board, int tileCountX, int tileCountY)
    {
        List<Vector2Int> r = new List<Vector2Int>();
    
        //right
        if(currentX+1<tileCountX){
            //right
            if(board[currentX+1,currentY]==null)
                r.Add(new Vector2Int(currentX+1,currentY));
            else if(board[currentX+1,currentY].team != team)
                r.Add(new Vector2Int(currentX+1,currentY));

            //top right
            if(currentY+1<tileCountY)
                if(board[currentX+1,currentY+1]==null)
                r.Add(new Vector2Int(currentX+1,currentY+1));
            else if(board[currentX+1,currentY+1].team != team)
                r.Add(new Vector2Int(currentX+1,currentY+1));

            //bottom right
            if(currentY-1>=0)
                if(board[currentX+1,currentY-1]==null)
                r.Add(new Vector2Int(currentX+1,currentY-1));
            else if(board[currentX+1,currentY-1].team != team)
                r.Add(new Vector2Int(currentX+1,currentY-1));
        }
        //left
        if(currentX-1>=0){
            //left
            if(board[currentX-1,currentY]==null)
                r.Add(new Vector2Int(currentX-1,currentY));
            else if(board[currentX-1,currentY].team != team)
                r.Add(new Vector2Int(currentX-1,currentY));

            //top left
            if(currentY+1<tileCountY)
                if(board[currentX-1,currentY+1]==null)
                r.Add(new Vector2Int(currentX-1,currentY+1));
            else if(board[currentX-1,currentY+1].team != team)
                r.Add(new Vector2Int(currentX-1,currentY+1));

            //bottom left
            if(currentY-1>=0)
                if(board[currentX-1,currentY-1]==null)
                r.Add(new Vector2Int(currentX-1,currentY-1));
            else if(board[currentX-1,currentY-1].team != team)
                r.Add(new Vector2Int(currentX-1,currentY-1));
        }
        //up
        if(currentY+1<tileCountY)
            if(board[currentX,currentY+1]==null || board[currentX, currentY+1].team != team)
                r.Add(new Vector2Int(currentX, currentY+1));
        //down  
        if(currentY-1 >=0)
            if(board[currentX,currentY-1]==null || board[currentX, currentY-1].team != team)
                r.Add(new Vector2Int(currentX, currentY-1));
        return r;
    }
    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves){
        SpecialMove r = SpecialMove.None;

        var kingMove = moveList. Find (m => m[0].x == 3 && m[0].y == ((team == 0) ? 0 : 7));
        var leftRook = moveList. Find (m => m[0].x == 0 && m[0].y == ((team == 0) ? 0 : 7));
        var rightRook = moveList. Find (m => m[0].x == 7 && m[0].y == ((team == 0) ? 0 : 7));

        if (kingMove == null && currentX == 3)
        {
            // White team
            if (team == 0)
            {
                // Left Rook
                if (leftRook == null)
                    if (board [0, 0].type == ChessPieceType. Rook)
                        if (board [0, 0].team == 0)
                            if (board [3, 0] == null)
                                if (board [2, 0] == null)
                                    if (board [1, 0] == null)
                                    {
                                        availableMoves.Add(new Vector2Int (2, 0));
                                        r = SpecialMove.Castling;
                                    }
                
                // Left Rook
                if (rightRook == null)
                    if (board [7, 0].type == ChessPieceType. Rook)
                        if (board [7, 0].team == 0)
                            if (board [6, 0] == null)
                                if (board [5, 0] == null)
                                    {
                                        availableMoves.Add(new Vector2Int (6, 0));
                                        r = SpecialMove.Castling;
                                    }
            }
            else{
                // Left Rook
                if (leftRook == null)
                    if (board [0, 7].type == ChessPieceType. Rook)
                        if (board [0, 7].team == 1)
                            if (board [2, 7] == null)
                                if (board [1, 7] == null)
                                {
                                    availableMoves.Add(new Vector2Int (1, 7));
                                    r = SpecialMove.Castling;
                                }

                if (rightRook == null)
                    if (board [7, 7].type == ChessPieceType. Rook)
                        if (board [7, 7].team == 1)
                            if (board [6, 7] == null)
                                if (board [5, 7] == null)
                                    if (board [4, 7] == null)
                                    {
                                        availableMoves.Add(new Vector2Int (5, 7));
                                        r = SpecialMove.Castling;
                                    }
            }
        }
        return r;
        
        //return base.GetSpecialMoves(ref board, moveList, availableMoves);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
     void Update()
    {
       
    }
}
