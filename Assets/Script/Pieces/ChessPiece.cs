using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public enum ChessPieceType{
    None = 0,
    Pawn = 1,
    Rook = 2,
    Knight = 3,
    Bishop = 4,
    Queen = 5,
    King = 6
}
public class ChessPiece : MonoBehaviour
{
    public ChessPieceType type;
    public int team;
    public int currentX, currentY;
    private Vector3 desiredPos, desiredScale = Vector3.one;  
    public Animator anim;

    private void Update(){
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime*20);
        transform.position =Vector3.Lerp(transform.position, desiredPos, Time.deltaTime * 20);
    }
    public virtual void SetPos(Vector3 pos){
        desiredPos =pos;
        transform.position = desiredPos;
    } 
    public virtual void SetScale(Vector3 scale){
        desiredPos =scale;
        transform.position = desiredScale;
    } 

    public void Move(int x, int y){

    }
}
