using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAI : MonoBehaviour
{
    public ChessPiece[,] chessPieces; // Your chessboard array
    // Other necessary variables and functions...
    private List<Vector2Int> availableMoves = new List<Vector2Int>();
    private List<ChessPiece> chessPieceCanMove = new List<ChessPiece>();
    [SerializeField] private Chessboard chessboard;
   
    // Function to get a random piece with available moves
    public ChessPiece GetRandomPiece()
    {
        Debug.Log("waiting for pieces");
        ChessPiece cp;
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                cp = chessboard.chessPieces[i, j];
                if (cp != null && cp.team == 0)
                {
                    availableMoves = cp.GetAvailableMove(ref chessboard.chessPieces, 8, 8);

                    if (availableMoves.Count > 0)
                    {
                        chessPieceCanMove.Add(cp);
                    }
                }
                if(cp == null)
                    break;
            }
        }

        cp = chessPieceCanMove[Random.Range(0, chessPieceCanMove.Count)];
        chessPieceCanMove.Clear();
        availableMoves.Clear();
        return cp;
    }
}
