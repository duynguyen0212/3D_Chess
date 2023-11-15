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
    public bool isProcessing;
    public bool isMoving;
    public virtual List<Vector2Int> GetAvailableMove(ref ChessPiece[,] board, int tileCountX, int tileCountY){
        List<Vector2Int> r = new List<Vector2Int>();

        r.Add(new Vector2Int(3,3));
        r.Add(new Vector2Int(3,4));
        r.Add(new Vector2Int(4,3));
        r.Add(new Vector2Int(4,4));
        return r;
    }

    void Update(){
        transform.localScale = Vector3.Lerp(transform.localScale, desiredScale, Time.deltaTime*10);
        StartCoroutine(MoveToPosition(desiredPos, 20f));
    }
    public virtual void SetPos(Vector3 pos){
        desiredPos =pos;
        StartCoroutine(MoveToPosition(desiredPos, 3f));
        
    } 
    public virtual void SetScale(Vector3 scale){
        desiredScale =scale;
        transform.position = desiredScale;
    } 
    private IEnumerator MoveToPosition(Vector3 targetPosition, float duration)
    {
        float elapsedTime = 0f;
        isMoving = true;
        Vector3 startingPosition = transform.position;
        anim.SetBool("walk", true);
        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        anim.SetBool("walk", false);
        transform.position = targetPosition;
        isMoving = false;
    }

    public IEnumerator AttackingCoroutine(){
        anim.SetBool("attack", true);
        yield return new WaitForSeconds(1f);
        anim.SetBool("attack", false);
    }

    public IEnumerator DeathCo(){
        anim.SetTrigger("die");
        yield return new WaitForSeconds(1f);
    }

}
