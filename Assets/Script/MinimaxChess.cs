using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimaxChess: MonoBehaviour
{
    private const int pawnValue = 100;
    private const int knightValue = 320;
    private const int bishopValue = 330;
    private const int rookValue = 500;
    private const int queenValue = 900;
    private const int kingValue = 20000;
    [SerializeField] private Chessboard cb;
     private static readonly int[,] bestPawnPositions = new int[,] {
            {0,  0,  0,  0,  0,  0,  0,  0},
            {50, 50, 50, 50, 50, 50, 50, 50},
            {10, 10, 20, 30, 30, 20, 10, 10},
            {5,  5, 10, 25, 25, 10,  5,  5},
            {0,  0,  0, 20, 20,  0,  0,  0},
            {5, -5,-10,  0,  0,-10, -5,  5},
            {5, 10, 10,-20,-20, 10, 10,  5},
            {0,  0,  0,  0,  0,  0,  0,  0}
    };

    private static readonly int[,] bestKnightPositions = new int[,] {
        {-50,-40,-30,-30,-30,-30,-40,-50},
        {-40,-20,  0,  0,  0,  0,-20,-40},
        {-30,  0, 10, 15, 15, 10,  0,-30},
        {-30,  5, 15, 20, 20, 15,  5,-30},
        {-30,  0, 15, 20, 20, 15,  0,-30},
        {-30,  5, 10, 15, 15, 10,  5,-30},
        {-40,-20,  0,  5,  5,  0,-20,-40},
        {-50,-40,-30,-30,-30,-30,-40,-50}
    };

    private static readonly int[,] bestBishopPositions = new int[,]{
        {-20,-10,-10,-10,-10,-10,-10,-20},
        {-10,  0,  0,  0,  0,  0,  0,-10},
        {-10,  0,  5, 10, 10,  5,  0,-10},
        {-10,  5,  5, 10, 10,  5,  5,-10},
        {-10,  0, 10, 10, 10, 10,  0,-10},
        {-10, 10, 10, 10, 10, 10, 10,-10},
        {-10,  5,  0,  0,  0,  0,  5,-10},
        {-20,-10,-10,-10,-10,-10,-10,-20}
    };

    private static readonly int[,] bestRookPositions = new int [,]{
            {0,  0,  0,  0,  0,  0,  0,  0},
            {5, 10, 10, 10, 10, 10, 10,  5},
            {-5,  0,  0,  0,  0,  0,  0, -5},
            {-5,  0,  0,  0,  0,  0,  0, -5},
            {-5,  0,  0,  0,  0,  0,  0, -5},
            {-5,  0,  0,  0,  0,  0,  0, -5},
            {-5,  0,  0,  0,  0,  0,  0, -5},
            {0,  0,  0,  5,  5,  0,  0,  0}
    };

    private static readonly int[,] bestQueenPositions = new int[,]{
            {-20,-10,-10, -5, -5,-10,-10,-20},
            {-10,  0,  0,  0,  0,  0,  0,-10},
            {-10,  0,  5,  5,  5,  5,  0,-10},
            {-5,  0,  5,  5,  5,  5,  0, -5},
            {0,  0,  5,  5,  5,  5,  0, -5},
            {-10,  5,  5,  5,  5,  5,  0,-10},
            {-10,  0,  5,  0,  0,  0,  0,-10},
            {-20,-10,-10, -5, -5,-10,-10,-20}
    };

    private static readonly int[,] bestKingPositions = new int[,] {
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-30,-40,-40,-50,-50,-40,-40,-30},
        {-20,-30,-30,-40,-40,-30,-30,-20},
        {-10,-20,-20,-20,-20,-20,-20,-10},
        {20, 20,  0,  0,  0,  0, 20, 20},
        {20, 30, 10,  0,  0, 10, 30, 20}
    };
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    
    // Define a basic chess evaluation function (this could be more sophisticated in a real scenario)
    public int EvaluateBoard(ChessPiece[,] board)
    {
        int totalEvaluation = 0;

        // Evaluate based on piece values and positional heuristics
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                ChessPiece piece = board[x, y];
                if (piece != null)
                {
                    int pieceValue = GetPieceValue(board,x,y); // Consider piece value heuristic
                    int positionalValue = GetPositionalValue(piece, x, y);
                    totalEvaluation += pieceValue + positionalValue;
                }
            }
        }

        return totalEvaluation;
    }

     private int GetPieceValue(ChessPiece[,] board,int x, int y)
    {
        
        if (board[x,y].type == ChessPieceType.Pawn)
        {
            return pawnValue;
        }
        else if (board[x,y].type == ChessPieceType.Rook)
        {
            return rookValue;
        }
        else if (board[x,y].type == ChessPieceType.Knight)
        {
            return knightValue;
        }
        else if (board[x,y].type == ChessPieceType.Bishop)
        {
            return bishopValue;
        }
        else if (board[x,y].type == ChessPieceType.Queen)
        {
            return queenValue;
        }
        else if (board[x,y].type == ChessPieceType.King)
        {
            return kingValue;
        }

        return 0;
    }

    private int GetPositionalValue(ChessPiece piece, int x, int y)
    {
        int positionalValue = 0;

        switch (piece.type)
        {
            case ChessPieceType.Pawn:
                positionalValue = bestPawnPositions[x, y];
                break;
            case ChessPieceType.Knight:
                positionalValue = bestKnightPositions[x, y];
                break;
            case ChessPieceType.Bishop:
                positionalValue = bestBishopPositions[x, y];
                break;
            case ChessPieceType.Rook:
                positionalValue = bestRookPositions[x, y];
                break;
            case ChessPieceType.Queen:
                positionalValue = bestQueenPositions[x, y];
                break;
            case ChessPieceType.King:
                positionalValue = bestKingPositions[x, y];
                break;
            default:
                break;
        }

        return positionalValue;
    }
    public int Minimax(ChessPiece[,] board, int depth, bool isMaximizingPlayer)
    {
        if (depth == 0 /*or game over*/)
        {
            return EvaluateBoard(board);
        }

        if (isMaximizingPlayer)
        {
            int maxEval = int.MinValue;
            foreach (ChessPiece piece in GetAllPieces(board, 0)) // Assuming team 0 is AI's team
            {
                List<Vector2Int> availableMoves = piece.GetAvailableMove(ref board, 8, 8);
                foreach (Vector2Int move in availableMoves)
                {
                    ChessPiece[,] newBoard = SimulateMove(ref board, piece, move);
                    int eval = Minimax(newBoard, depth - 1, false);
                    maxEval = Mathf.Max(maxEval, eval);
                }
            }
            return maxEval;
        }
        else
        {
            int minEval = int.MaxValue;
            foreach (ChessPiece piece in GetAllPieces(board, 1)) // Assuming team 1 is opponent's team
            {
                List<Vector2Int> availableMoves = piece.GetAvailableMove(ref board, 8, 8);
                foreach (Vector2Int move in availableMoves)
                {
                    ChessPiece[,] newBoard = SimulateMove(ref board, piece, move);
                    int eval = Minimax(newBoard, depth - 1, true);
                    minEval = Mathf.Min(minEval, eval);
                }
            }
            return minEval;
        }
    }


     public ChessPiece[,] SimulateMove(ref ChessPiece[,] board, ChessPiece piece, Vector2Int move)
    {
        
        int startX = piece.currentX;
        int startY = piece.currentY;
        int targetX = move.x;
        int targetY = move.y;

        // Make a copy of the board
        ChessPiece[,] newBoard = new ChessPiece[board.GetLength(0), board.GetLength(1)];
        
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 8; j++)
            {
                
                if (board[i,j] == null) newBoard[i,j] = null;
                else
                {
                    if (board[i,j].team == 0)
                    {
                        newBoard[i,j].team = 0;
                        // Debug.Log("found team " + board[i,j].team + " of " +board[i,j].type);
                        if (board[i,j].type == ChessPieceType.Pawn)
                            newBoard[i,j].type = ChessPieceType.Pawn;         
                        
                        else if (board[i,j].type == ChessPieceType.Knight)
                            newBoard[i,j].type = ChessPieceType.Knight;             
                        
                        else if (board[i,j].type == ChessPieceType.Bishop)
                            newBoard[i,j].type = ChessPieceType.Bishop;
                           
                        else if (board[i,j].type == ChessPieceType.Rook)                        
                            newBoard[i,j].type = ChessPieceType.Rook;
                                                    
                        else if (board[i,j].type == ChessPieceType.Queen)
                            newBoard[i,j].type = ChessPieceType.Queen;                           
                        
                        else if (board[i,j].type == ChessPieceType.King)
                            newBoard[i,j].type = ChessPieceType.King;
                    }
                    else if (board[i,j].team == 1)
                    {
                       
                        newBoard[i,j].team = 1;
                        if (board[i,j].type == ChessPieceType.Pawn)                        
                            newBoard[i,j].type = ChessPieceType.Pawn;  

                        else if (board[i,j].type == ChessPieceType.Knight)                        
                            newBoard[i,j].type = ChessPieceType.Knight;                            
                        
                        else if (board[i,j].type == ChessPieceType.Bishop)                        
                            newBoard[i,j].type = ChessPieceType.Bishop;                           
                        
                        else if (board[i,j].type == ChessPieceType.Rook)                        
                            newBoard[i,j].type = ChessPieceType.Rook;                            
                        
                        else if (board[i,j].type == ChessPieceType.Queen)
                            newBoard[i,j].type = ChessPieceType.Queen;                            
                        
                        else if (board[i,j].type == ChessPieceType.King)
                            newBoard[i,j].type = ChessPieceType.King;
                    }
                }
            }
        }

        // Simulate the move on the new board
        newBoard[targetX, targetY].type = board[startX, startY].type;
        newBoard[targetX, targetY].team = board[startX, startY].team;
        newBoard[targetX, targetY].currentX = targetX;
        newBoard[targetX, targetY].currentY = targetY;
       
        newBoard[startX, startY] = null; // Clear the original position
        return newBoard;
    }

    public List<ChessPiece> GetAllPieces(ChessPiece[,] board, int team)
    {
        List<ChessPiece> pieces = new List<ChessPiece>();
        for (int x = 0; x < 8; x++)
        {
            for (int y = 0; y < 8; y++)
            {
                ChessPiece piece = board[x, y];
                if (piece != null && piece.team == team)
                {
                    pieces.Add(piece);
                }
            }
        }
        return pieces;
    }

    public (ChessPiece piece, Vector2Int move) GetBestPieceMove(ChessPiece[,] board, int depth, bool isMaximizingPlayer)
{
    // Get all pieces for the current player
    List<ChessPiece> allPieces = GetAllPieces(board, isMaximizingPlayer ? 0 : 1);
    
    ChessPiece bestPiece = null;
    Vector2Int bestMove = Vector2Int.zero;
    int bestMoveValue = int.MinValue;

    foreach (ChessPiece piece in allPieces)
    {
        // Get legal moves for the piece
        List<Vector2Int> availableMoves = piece.GetAvailableMove(ref board, 8, 8);
        
        foreach (Vector2Int move in availableMoves)
        {
            // Simulate the move
            ChessPiece[,] newBoard = SimulateMove(ref board, piece, move);

            // Evaluate the move using Minimax or other evaluation techniques
            int moveValue = Minimax(newBoard, depth - 1, !isMaximizingPlayer);

            // Store the best move found so far
            if (moveValue > bestMoveValue)
            {
                bestMoveValue = moveValue;
                bestPiece = piece;
                bestMove = move;
            }
        }
    }
    return (bestPiece, bestMove);
}

}

