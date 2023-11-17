using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RandomAI : MonoBehaviour
{
    public ChessPiece[,] chessPieces; // Your chessboard array
    // Other necessary variables and functions...
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<ChessPiece> chessPieceCanMove = new List<ChessPiece>();
   
    // Function to get a random piece with available moves
    public ChessPiece GetRandomPiece(ref ChessPiece[,] board)
    {
        
        ChessPiece cp;
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                if (board[x, y] != null && board[x, y].team == 0)
                {
                    cp = board[x, y];
                    availableMoves = cp.GetAvailableMove(ref board, 8, 8);
                    if (availableMoves.Count > 0)
                        chessPieceCanMove.Add(cp);
                    else break;
                }
                else if (board[x, y] == null) break;
            }
        }
    
        int randomIndex = UnityEngine.Random.Range(0, chessPieceCanMove.Count-1);
        cp = chessPieceCanMove[randomIndex];
        
        //chessPieceCanMove.Clear();
        availableMoves.Clear();

        return cp;
    }

    public void RemoveChessPiece(int x, int y){
        for (int i = 0; i < chessPieceCanMove.Count; i++)
        {
            if (chessPieceCanMove[i].currentX == x && chessPieceCanMove[i].currentY == y)
            {
                chessPieceCanMove.RemoveAt(i);
                break; // Exit the loop after removing the captured piece
            }
        }
    }
}
