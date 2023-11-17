// using System;
// using System.IO;
// using System.Collections.Generic;
// using System.Text;
// using UnityEngine;

// public class AI:MonoBehaviour
// {
//     /// <summary>
//     /// The "evaluate" depth of minimax algorithm
//     /// </summary>
//     private int depth;

//     /// <summary>
//     /// Piece value
//     /// </summary>
//     private const int pawnValue = 100;
//     private const int knightValue = 320;
//     private const int bishopValue = 330;
//     private const int rookValue = 500;
//     private const int queenValue = 900;
//     private const int kingValue = 20000;
//     [SerializeField] private Chessboard cb;

//     /// <summary>
//     /// Position point
//     /// It didn't work as well as I expected
//     /// source : https://www.chessprogramming.org/Simplified_Evaluation_Function
//     /// </summary>
//     private static readonly int[,] bestPawnPositions = new int[,] {
//             {0,  0,  0,  0,  0,  0,  0,  0},
//             {50, 50, 50, 50, 50, 50, 50, 50},
//             {10, 10, 20, 30, 30, 20, 10, 10},
//             {5,  5, 10, 25, 25, 10,  5,  5},
//             {0,  0,  0, 20, 20,  0,  0,  0},
//             {5, -5,-10,  0,  0,-10, -5,  5},
//             {5, 10, 10,-20,-20, 10, 10,  5},
//             {0,  0,  0,  0,  0,  0,  0,  0}
//     };

//     private static readonly int[,] bestKnightPositions = new int[,] {
//         {-50,-40,-30,-30,-30,-30,-40,-50},
//         {-40,-20,  0,  0,  0,  0,-20,-40},
//         {-30,  0, 10, 15, 15, 10,  0,-30},
//         {-30,  5, 15, 20, 20, 15,  5,-30},
//         {-30,  0, 15, 20, 20, 15,  0,-30},
//         {-30,  5, 10, 15, 15, 10,  5,-30},
//         {-40,-20,  0,  5,  5,  0,-20,-40},
//         {-50,-40,-30,-30,-30,-30,-40,-50}
//     };

//     private static readonly int[,] bestBishopPositions = new int[,]{
//         {-20,-10,-10,-10,-10,-10,-10,-20},
//         {-10,  0,  0,  0,  0,  0,  0,-10},
//         {-10,  0,  5, 10, 10,  5,  0,-10},
//         {-10,  5,  5, 10, 10,  5,  5,-10},
//         {-10,  0, 10, 10, 10, 10,  0,-10},
//         {-10, 10, 10, 10, 10, 10, 10,-10},
//         {-10,  5,  0,  0,  0,  0,  5,-10},
//         {-20,-10,-10,-10,-10,-10,-10,-20}
//     };

//     private static readonly int[,] bestRookPositions = new int [,]{
//             {0,  0,  0,  0,  0,  0,  0,  0},
//             {5, 10, 10, 10, 10, 10, 10,  5},
//             {-5,  0,  0,  0,  0,  0,  0, -5},
//             {-5,  0,  0,  0,  0,  0,  0, -5},
//             {-5,  0,  0,  0,  0,  0,  0, -5},
//             {-5,  0,  0,  0,  0,  0,  0, -5},
//             {-5,  0,  0,  0,  0,  0,  0, -5},
//             {0,  0,  0,  5,  5,  0,  0,  0}
//     };

//     private static readonly int[,] bestQueenPositions = new int[,]{
//             {-20,-10,-10, -5, -5,-10,-10,-20},
//             {-10,  0,  0,  0,  0,  0,  0,-10},
//             {-10,  0,  5,  5,  5,  5,  0,-10},
//             {-5,  0,  5,  5,  5,  5,  0, -5},
//             {0,  0,  5,  5,  5,  5,  0, -5},
//             {-10,  5,  5,  5,  5,  5,  0,-10},
//             {-10,  0,  5,  0,  0,  0,  0,-10},
//             {-20,-10,-10, -5, -5,-10,-10,-20}
//     };

//     private static readonly int[,] bestKingPositions = new int[,] {
//         {-30,-40,-40,-50,-50,-40,-40,-30},
//         {-30,-40,-40,-50,-50,-40,-40,-30},
//         {-30,-40,-40,-50,-50,-40,-40,-30},
//         {-30,-40,-40,-50,-50,-40,-40,-30},
//         {-20,-30,-30,-40,-40,-30,-30,-20},
//         {-10,-20,-20,-20,-20,-20,-20,-10},
//         {20, 20,  0,  0,  0,  0, 20, 20},
//         {20, 30, 10,  0,  0, 10, 30, 20}
//     };

//     public AI(int _depth)
//     {
//         depth = _depth;
//     }

//     /// <summary>
//     /// Calculate the point for evaluate
//     public int CalculatePoint(ChessPiece cp)
//     {
//         int scoreRed = 0;
//         int scoreBlue = 0;
        
//         scoreRed += GetScoreFromExistingPieces(1, cb);
//         scoreBlue += GetScoreFromExistingPieces(0, cb);

//         int evaluation = scoreBlue - scoreRed;

//         int prespective = (cb.currentPlayer == 1) ? -1 : 1;
//         return evaluation * prespective;
//     }

//     private static int GetScoreFromExistingPieces(int player, Chessboard cb)
//     {
//         int material = 0;
//         ChessPiece cp;
        

//         for (int x = 0; x < 8; x++)
//         {
//             for(int y = 0; y < 8; y++){
//                 cp = cb.chessPieces[x,y];
//                 if (cp != null)
//                 {
//                     if (cp.type == ChessPieceType.Pawn && cb.currentPlayer == player)
//                     {
//                         material += (pawnValue); // plus "+ bestPawnPositions[i]" if you want, but it doesn't work well
//                     }
//                     if (cp.type == ChessPieceType.Knight && cb.currentPlayer == player)
//                     {
//                         material += (knightValue); // plus "+ bestKnightPositions[i]" if you want, but it doesn't work well
//                     }
//                     if (cp.type == ChessPieceType.Bishop && cb.currentPlayer == player)
//                     {
//                         material += (bishopValue); // plus "+ bestBishopPositions[i]" if you want, but it doesn't work well
//                     }
//                     if (cp.type == ChessPieceType.Rook && cb.currentPlayer == player)
//                     {
//                         material += (rookValue); // plus "+ bestRookPositions[i]" if you want, but it doesn't work well
//                     }
//                     if (cp.type == ChessPieceType.Queen && cb.currentPlayer == player)
//                     {
//                         material += (queenValue); // plus "+ bestQueenPositions[i]" if you want, but it doesn't work well
//                     }
//                     if (cp.type == ChessPieceType.King && cb.currentPlayer == player)
//                     {
//                         material += (kingValue); // plus "+ bestKingPositions[i]" if you want, but it doesn't work well
//                     }
//                 }
//             }
//         }
//         return material;
//     }

//     //+++++++++++++++++++++++++++++++++++++ MINIMAX ALGORITHM ++++++++++++++++++++++++++++++++++++

//     /// <summary>
//     /// Copy the current board, then make move
//     /// </summary>
//     /// <param name="oldBoard"></param>
//     /// <param name="move"></param>
//     /// <returns></returns>
//     private Chessboard GenerateMovedBoard(Chessboard oldBoard, Move move)
//     {
//         Chessboard newBoard = new Chessboard();
//         newBoard = ObjectExtensions.Copy(oldBoard);
//         Board.MovePiece(newBoard, move.Tile, move.Next);
//         return newBoard;
//     }

//     /// <summary>
//     /// Get the piece value
//     /// </summary>
//     /// <param name="board"></param>
//     /// <param name="index"></param>
//     /// <returns></returns>
//     private int GetPieceValue(int x, int y)
//     {
//         if (cb.chessPieces[x,y].type == ChessPieceType.Pawn)
//         {
//             return pawnValue;
//         }
//         else if (cb.chessPieces[x,y].type == ChessPieceType.Rook)
//         {
//             return rookValue;
//         }
//         else if (cb.chessPieces[x,y].type == ChessPieceType.Knight)
//         {
//             return knightValue;
//         }
//         else if (cb.chessPieces[x,y].type == ChessPieceType.Bishop)
//         {
//             return bishopValue;
//         }
//         else if (cb.chessPieces[x,y].type == ChessPieceType.Queen)
//         {
//             return queenValue;
//         }
//         else if (cb.chessPieces[x,y].type == ChessPieceType.King)
//         {
//             return kingValue;
//         }

//         return 0;
//     }

//     /// <summary>
//     /// Sort the list to reduce the runtime of the algorithm
//     /// </summary>
//     /// <param name="moveList"></param>
//     /// <param name="board"></param>
//     private void OrderMoves(List<Move> moveList, Board board)
//     {
//         int[] moveScore = new int[moveList.Count];

//         for (int i = 0; i < moveList.Count; i++)
//         {
//             moveScore[i] = 0;

//             if (board.Pieces[moveList[i].Next] != null )
//             {
//                 moveScore[i] += 10 * GetPieceValue(board, moveList[i].Next) - GetPieceValue(board, moveList[i].Tile);
//             }

//             if (Board.PawnPromoted(board.Pieces, moveList[i].Tile))
//             {
//                 moveScore[i] += queenValue;
//             }
            
//         }

//         for (int sorted = 0; sorted < moveList.Count; sorted++)
//         {
//             int bestScore = int.MinValue;
//             int bestScoreIndex = 0;

//             for (int i = sorted; i < moveList.Count; i++)
//             {
//                 if (moveScore[i] > bestScore)
//                 {
//                     bestScore = moveScore[i];
//                     bestScoreIndex = i;
//                 }
//             }

//             // swap

//             Move bestMove = moveList[bestScoreIndex];
//             moveList[bestScoreIndex] = moveList[sorted];
//             moveList[sorted] = bestMove;
//         }
//     }

//     /// <summary>
//     /// Main algorithm: minimax
//     /// </summary>
//     /// <param name="board"></param>
//     /// <param name="depth"></param>
//     /// <param name="alpha"></param>
//     /// <param name="beta"></param>
//     /// <param name="isMaximizingPlayer"></param>
//     /// <returns></returns>
//     private int Minimax(Board board, int depth, int alpha, int beta, bool isMaximizingPlayer)
//     {
//         if (depth == 0)
//             return CalculatePoint(board);

//         if (isMaximizingPlayer)
//         {
//             int bestValue = int.MinValue;

//             List<Move> possibleMoves = Board.GetAllLegalMoves(Player.Black, board);

//             OrderMoves(possibleMoves, board);
//             foreach (var move in possibleMoves)
//             {
//                 Board newBoard = GenerateMovedBoard(board, move);

//                 int value = Minimax(newBoard, depth - 1, alpha, beta, false);

//                 bestValue = Math.Max(value, bestValue);

//                 alpha = Math.Max(alpha, value);

//                 if (beta <= alpha)
//                 {
//                     break;
//                 }
//             }

//             return bestValue;
//         }
//         else
//         {
//             int bestValue = int.MaxValue;

//             List<Move> possibleMoves = Board.GetAllLegalMoves(Player.White, board);

//             OrderMoves(possibleMoves, board);
//             foreach (var move in possibleMoves)
//             {
//                 Board newBoard = GenerateMovedBoard(board, move);

//                 int value = Minimax(board, depth - 1, alpha, beta, true);

//                 bestValue = Math.Min(value, bestValue);

//                 beta = Math.Min(beta, value);

//                 if (beta <= alpha)
//                 {
//                     break;
//                 }
//             }

//             return bestValue;
//         }
//     }

//     /// <summary>
//     /// Get the result after evaluate using minimax
//     /// </summary>
//     /// <param name="board"></param>
//     /// <returns></returns>
//     public Move GetBestMove(Board board)
//     {
//         int bestValue = int.MinValue;
//         Move bestMove = null;
//         bool turn;
//         if (board.Turn == Player.Black)
//         {
//             turn = false;
//         }
//         else
//         {
//             turn = true;
//         }

//         List<Move> possibleMoves = Board.GetAllLegalMoves(board.Turn, board);

//         OrderMoves(possibleMoves, board);
//         foreach (var move in possibleMoves)
//         {
//             Board newBoard = GenerateMovedBoard(board, move);

//             int value = Minimax(newBoard, depth, int.MinValue, int.MaxValue, turn);

//             if (value >= bestValue)
//             {
//                 bestValue = value;
//                 bestMove = move;
//             }
//         }

//         return bestMove;
//     }
//     //+++++++++++++ END +++++++++++++++++++ MINIMAX ALGORITHM ++++++++++++++++++++++++++++++++++++               




//     //+++++++++++++++++++++++++++++++++++++ RANDOM ALGORITHM +++++++++++++++++++++++++++++++++++++

//     /// <summary>
//     /// Random a move in legal moves
//     /// </summary>
//     /// <param name="board"></param>
//     /// <returns></returns>
//     public Move RandomMove(Board board)
//     {
//         Random rand = new Random();
//         List<Move> legalMoves = Board.GetAllLegalMoves(Player.Black, board);

//         if (board.CheckMate == true) return null;
//         return legalMoves[rand.Next(0, legalMoves.Count)];
//     }
//     //+++++++++++++++++ END +++++++++++++++ RANDOM ALGORITHM +++++++++++++++++++++++++++++++++++++                     





//     //===================================== USING EVALUATE =======================================

//     /// <summary>
//     /// AI generates random movement
//     /// </summary>
//     /// <param name="board"></param>
//     public void EvaluateRandom(Board board)
//     {

//         Move move = RandomMove(board);

//         if (move == null) return;

//         Board.MovePiece(board, move.Tile, move.Next);
//         board.Save(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName + @"\book\learn.txt", move.Tile, move.Next, board);


//     }

//     /// <summary>
//     /// AI generates evaluated movement
//     /// </summary>
//     /// <param name="board"></param>
//     public void EvaluateAI(Board board)
//     {
//         // uncomment this to use negamax algorithm

//         //Move move = CalculateBestMove(board, 3).Move;
//         //if (move == null) return;
//         //Board.MovePiece(board, move.Tile, move.Next);
//         //board.Save(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName + @"\book\learn.txt", move.Tile, move.Next, board);


//         // uncomment this to use minimax algorithm
//         // This doesn't work as I expected, as you increase the depth (Board.cs line 286)
//         // The AI will take too long to make a "stupid" movement =(((

//         Move move = GetBestMove(board);
//         if (move == null) return;
//         Board.MovePiece(board, move.Tile, move.Next);
//         board.Save(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName + @"\book\learn.txt", move.Tile, move.Next, board);

//     }


// }






