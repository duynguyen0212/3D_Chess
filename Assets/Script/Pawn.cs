using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : ChessPiece
{
    public override List<Vector2Int> GetAvailableMove(ref ChessPiece[,] board, int tileCountX, int tileCountY){
        List<Vector2Int> r = new List<Vector2Int>();

        int direction = (team == 1) ? -1 : 1;

        // One in front
        if(board[currentX, currentY + direction] == null)
            r.Add(new Vector2Int(currentX, currentY + direction));

        // Two in front
        if(board[currentX, currentY + direction] == null){
            // red team
            if(team == 1 && currentY == 6 && board[currentX, currentY + (direction*2)] == null)
                r.Add(new Vector2Int(currentX, currentY + direction*2));
            
            // blue team
            if(team == 0 && currentY == 1 && board[currentX, currentY + (direction*2)] == null)
                r.Add(new Vector2Int(currentX, currentY + direction*2));

        }

        // Kill move
        if(currentX != tileCountX-1)
            if(board[currentX+1, currentY + direction] != null && board[currentX+1, currentY + direction].team != team)
                r.Add(new Vector2Int(currentX+1, currentY + direction));
                
         if(currentX != 0)
            if(board[currentX-1, currentY + direction] != null && board[currentX-1, currentY + direction].team != team)
                r.Add(new Vector2Int(currentX-1, currentY + direction));

        return r;
    }

    public override SpecialMove GetSpecialMoves(ref ChessPiece[,] board, ref List<Vector2Int[]> moveList, ref List<Vector2Int> availableMoves){
        SpecialMove r = SpecialMove.None;

        int direction = (team==0) ? 1:-1;
        if((team==1 && currentY==1) || (team==0 && currentY==6))
            return SpecialMove.Promotion;
        return r;
    
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
     void Update()
    {
       
    }
}
