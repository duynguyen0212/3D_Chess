using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum SpecialMove{
    None = 0,
    Castling,
    Promotion
}
public class Chessboard : MonoBehaviour
{
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCam;
    private Vector2Int currentHover;
    private Vector3 bounds;
    public ChessPiece[,] chessPieces;
    private ChessPiece currentPiece;
    public bool isClicked = false;
    //private bool isAttacking = false;
    private List<Vector2Int> availableMoves = new List<Vector2Int>();

    [SerializeField] private GameObject PWScreen;
    private bool isPause;
    private SpecialMove specialMove;
    private List<Vector2Int[]> moveList = new List<Vector2Int[]>();
    [Header("Tiles")]
    [SerializeField] private Material tileMaterial;
    [SerializeField] private float tileSize = 1.0f;
    [SerializeField] private float yOffset = 0.2f;
    [SerializeField] private Vector3 boardCenter = Vector3.zero;

    [Header("Team's Prefab")]
    [SerializeField] private GameObject[] redTeamPrefab;
    [SerializeField] private GameObject[] blueTeamPrefab;
    public int currentPlayer = 1;
    private int attackOffsetX, attackOffsetY;
    public RandomAI randomAI;
    public MinimaxChess minimaxAI;
    public bool playerAction;
    
    private void Awake() {
        GenerateAllTiles(tileSize, TILE_COUNT_X, TILE_COUNT_Y);
        SpawnAllPieces();
        PositionAllPieces();
    }

    private void Update() {
        if(!currentCam){
            currentCam = Camera.main;
            return;
        }

        if(Input.GetKeyDown(KeyCode.Escape)){
            if(isPause){
                ResumeGame();
            }
            else{
                PauseGame();
            }
        }

        RaycastHit info;
        Ray ray = currentCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile","Hover", "Highlight") ))
        {
            // Get the indexes of the tile i've hit
            Vector2Int hitPosition = LookupTileIndex(info.transform.gameObject);
            // If we're hovering a tile after not hovering any tiles
            if (currentHover == -Vector2Int.one)
            {
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            // If we were already hovering a tile, change the previous one
            if (currentHover != hitPosition)
            {
                tiles[currentHover.x, currentHover.y].layer = ContainsValidMove(ref availableMoves, currentHover) ?  LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }

            if(currentPlayer == 0 && currentPiece.isMoving == false && playerAction == false){
                //random ai
                ChessPiece AIChesspiece = randomAI.GetRandomPiece(ref chessPieces);
                Vector2Int AIMove;
                availableMoves = AIChesspiece.GetAvailableMove(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);
                AIMove = availableMoves[UnityEngine.Random.Range(0, availableMoves.Count)];
                MoveTo(AIChesspiece, AIMove.x, AIMove.y);
                RevHighlightTiles(); 
                currentPlayer = (currentPlayer+1)%2;
                // randomAI.testingfunc(chessPieces);

                //minimax ai
                //var bestPieceMove = minimaxAI.GetBestPieceMove(chessPieces, 3, true);
                // ChessPiece bestPiece = bestPieceMove.piece;
                // Vector2Int bestMove = bestPieceMove.move;
                // MoveTo(bestPiece, bestMove.x, bestMove.y);
                // Debug.Log("moving " +bestPiece.type+" " +bestPiece.team+ " " +bestMove.x +", " +bestMove.y);
                // RevHighlightTiles(); 
                // currentPlayer = (currentPlayer+1)%2;
            }
            if(Input.GetMouseButtonDown(0)){
                if(chessPieces[hitPosition.x, hitPosition.y] != null && chessPieces[hitPosition.x, hitPosition.y].team == 1 && currentPlayer == 1){
                //if(chessPieces[hitPosition.x, hitPosition.y] != null && chessPieces[hitPosition.x, hitPosition.y].team==currentPlayer ){
                    //player's turn
                    isClicked = true;
                    if(currentPiece != chessPieces[hitPosition.x, hitPosition.y]) RevHighlightTiles();
                    
                    currentPiece = chessPieces[hitPosition.x, hitPosition.y];
                    //Get avaialable move and highlight it
                    availableMoves = currentPiece.GetAvailableMove(ref chessPieces, TILE_COUNT_X, TILE_COUNT_Y);

                    //Get special move
                    specialMove = currentPiece.GetSpecialMoves(ref chessPieces, ref moveList, ref availableMoves);
                    HighlightTiles();
                    return;                   
                }

            }
            if(isClicked && Input.GetMouseButtonDown(0)){
                    bool validMove = MoveTo(currentPiece, hitPosition.x, hitPosition.y);
                    if(!validMove){
                        return;
                    }
                    else {
                        currentPlayer = (currentPlayer+1)%2;
                        isClicked = false;
                    }
                    RevHighlightTiles();
                    
            }

        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = ContainsValidMove(ref availableMoves, currentHover) ?  LayerMask.NameToLayer("Highlight") : LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
        }
    }

    private bool MoveTo(ChessPiece cp, int x, int y)
    {
        if(!ContainsValidMove(ref availableMoves, new Vector2(x,y)))
            return false;
        Vector2Int prePos = new Vector2Int(cp.currentX, cp.currentY);

        //attack other piece
        if(chessPieces[x, y] != null && chessPieces[x, y].team != cp.team)
        {
            playerAction = true;
            ChessPiece capturedPiece = chessPieces[x, y];
            randomAI.RemoveChessPiece(x, y);
            StartCoroutine(AttackChessPiece(cp, capturedPiece, x, y, prePos));
            if(capturedPiece.type == ChessPieceType.King){
                if(capturedPiece.team == 1)
                    CheckMate(0);
                else
                    CheckMate(1);
            }
            moveList.Add(new Vector2Int[] { prePos, new Vector2Int(x,y)});
            return true;
        }

        chessPieces[x, y] = cp;
        chessPieces[prePos.x, prePos.y] = null;

        chessPieces[x,y].currentX = x;
        chessPieces[x,y].currentY = y;
        chessPieces[x,y].SetPos(GetTileCenter(x,y));
        moveList.Add(new Vector2Int[] { prePos, new Vector2Int(x,y)});
        ProcessSpecialMove();
        return true;
    }

    private IEnumerator AttackChessPiece(ChessPiece attacker, ChessPiece target, int x, int y, Vector2Int prePos){
        // UP
        if(attacker.currentY - y > 1 && attacker.currentX == x) SetAttackOffset(0,1);
        // DOWN
        else if (attacker.currentY - y < -1 && attacker.currentX == x) SetAttackOffset(0,-1);
        // LEFT
        else if(attacker.currentX - x < -1 && attacker.currentY == y) SetAttackOffset(-1,0);
        //RIGHT
        else if(attacker.currentX - x > 1 && attacker.currentY == y) SetAttackOffset(1,0);
        // DOWN LEFT
        else if (attacker.currentY - y < -1 && attacker.currentX - x < -1) SetAttackOffset(-1,-1);
        //DOWN RIGHT
        else if(attacker.currentY - y < -1 && attacker.currentX - x > 1) SetAttackOffset(1,-1);
        //UP RIGHT
        else if(attacker.currentY - y > 1 && attacker.currentX - x > 1) SetAttackOffset(1,1);
        //UP LEFT
        else if(attacker.currentY - y > 1 && attacker.currentX - x < -1) SetAttackOffset(-1,1);
        // OTHER, CHESS PIECE DOESN'T NEED TO MOVE TO ATTACK
        else {
            SetAttackOffset(0,0);
            StartCoroutine(attacker.AttackingCoroutine()); 
            yield return new WaitForSeconds(1f);
            StartCoroutine(target.DeathCo());
            yield return new WaitForSeconds(1f);
            Destroy(target.gameObject);
            chessPieces[x + attackOffsetX, y + attackOffsetY] = attacker;
            chessPieces[prePos.x, prePos.y] = null;
            MoveToCo(x, y);
            playerAction = false;
            yield break;
        }
        chessPieces[x + attackOffsetX, y + attackOffsetY] = attacker;
        chessPieces[prePos.x, prePos.y] = null;

        //move to pos that close to the chess piece to attack
        MoveToCo(x + attackOffsetX, y+ attackOffsetY);
        yield return new WaitForSeconds(3f);
        StartCoroutine(attacker.AttackingCoroutine());
        yield return new WaitForSeconds(1f);
        StartCoroutine(target.DeathCo());
        chessPieces[x + attackOffsetX, y + attackOffsetY] = null;
        yield return new WaitForSeconds(1.5f);
        Destroy(target.gameObject);
        chessPieces[x, y] = attacker;
        chessPieces[prePos.x, prePos.y] = null;

        //Move chess piece to the enemy pos after kill it
        MoveToCo(x,y);
        playerAction = false;
        
    }

    private void MoveToCo(int x, int y){
        chessPieces[x,y].currentX = x;
        chessPieces[x,y].currentY = y;
        chessPieces[x,y].SetPos(GetTileCenter(x,y));

    }
    //special move
    private void ProcessSpecialMove(){
        if(specialMove == SpecialMove.Castling){
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            // Left Rook
            if (lastMove [1].x == 1)
            {
                if (lastMove[1].y == 0) // blue side
                {
                    ChessPiece rook = chessPieces [0, 0];
                    chessPieces [2, 0] = rook;
                    MoveToCo(2, 0);
                    chessPieces [0, 0] = null;
                }
                else if (lastMove[1].y == 7) //red side
                {
                    ChessPiece rook = chessPieces [0, 7];
                    chessPieces [2, 7] = rook;
                    MoveToCo(2, 7);
                    chessPieces [0, 7] = null;
                }
            }
            // right rook
            else if(lastMove[1].x == 5){
                if (lastMove[1].y == 0) // blue side
                {
                    ChessPiece rook = chessPieces [7, 0];
                    chessPieces [4, 0] = rook;
                    MoveToCo(4, 0);
                    chessPieces [7, 0] = null;
                }
                else if (lastMove[1].y == 7) //red side
                {
                    ChessPiece rook = chessPieces [7, 7];
                    chessPieces [4, 7] = rook;
                    MoveToCo(4, 7);
                    chessPieces [7, 7] = null;
                }

            }
        }

        if(specialMove == SpecialMove.Promotion){
            Vector2Int[] lastMove = moveList[moveList.Count - 1];
            ChessPiece targetPawn= chessPieces [lastMove [1].x, lastMove[1].y];

            if(targetPawn.type == ChessPieceType.Pawn){
                if (targetPawn.team == 0 && lastMove [1].y == 7)
                {
                    ChessPiece newQueen = SpawningSinglePiece(ChessPieceType.Queen, 0, blueTeamPrefab);
                    Destroy(chessPieces[lastMove [1].x, lastMove [1].y].gameObject);
                    chessPieces[lastMove [1].x, lastMove [1].y] = newQueen;
                    PositionSinglePiece(lastMove [1].x, lastMove [1].y, true);
                }

                if (targetPawn.team == 1 && lastMove [1].y == 0)
                {
                    ChessPiece newQueen = SpawningSinglePiece(ChessPieceType.Queen, 1, redTeamPrefab);
                    Destroy(chessPieces[lastMove [1].x, lastMove [1].y].gameObject);
                    chessPieces[lastMove [1].x, lastMove [1].y] = newQueen;
                    PositionSinglePiece(lastMove [1].x, lastMove [1].y);
                }
                currentPiece.isMoving = false;
                playerAction = false;
            }
        }
    }
    //Checkmate
    private void CheckMate(int team){
        DisplayVictory(team);
    }

    private void DisplayVictory(int winningTeam){
        PWScreen.SetActive(true);
        PWScreen.transform.GetChild(winningTeam).gameObject.SetActive(true);
    }

    public void OnMenuButton(){

    }

    public void OnExitButton(){
        Application.Quit();
    }

    public void PauseGame(){
        PWScreen.SetActive(true);
        PWScreen.transform.GetChild(2).gameObject.SetActive(true);
        Time.timeScale = 0f;
        isPause = true;
        
    }
    public void ResumeGame(){
        PWScreen.SetActive(false);
        PWScreen.transform.GetChild(2).gameObject.SetActive(false);
        Time.timeScale = 1f;
        isPause = false;

    }

    private void SetAttackOffset(int x, int y){
        attackOffsetX = x;
        attackOffsetY = y;
    }

    private void GenerateAllTiles(float tileSize, int tileCountX, int tileCountY){
        yOffset += transform.position.y;
        bounds = new Vector3((tileCountX/2) *tileSize, 0, (tileCountX/2)*tileSize)+boardCenter;
        tiles = new GameObject[tileCountX, tileCountY];
        for (int x = 0; x < tileCountX; x++)
            for (int y = 0; y < tileCountY; y++)
                tiles[x,y] = GenerateSingleTiles(tileSize, x, y);
    }

    private GameObject GenerateSingleTiles(float tileSize, int x, int y){
        GameObject tileObject = new GameObject(string.Format("X:{0}, Y:{1}", x, y));
        tileObject.transform.parent = transform;

        Mesh mesh = new Mesh();
        tileObject.AddComponent<MeshFilter>().mesh = mesh;
        tileObject.AddComponent<MeshRenderer>().material = tileMaterial;

        Vector3[] vertices = new Vector3[4];
        vertices[0] = new Vector3(x *tileSize, yOffset, y*tileSize) - bounds;
        vertices[1] = new Vector3(x *tileSize, yOffset, (y+1)*tileSize) - bounds;
        vertices[2] = new Vector3((x+1) *tileSize, yOffset, y*tileSize) - bounds;
        vertices[3] = new Vector3((x+1) *tileSize, yOffset, (y+1)*tileSize) - bounds;

        int[] tris = new int[]{0, 1, 2, 1, 3, 2};

        mesh.vertices = vertices;
        mesh.triangles = tris;
        mesh.RecalculateNormals();
        tileObject.layer = LayerMask.NameToLayer("Tile");
        tileObject.AddComponent<BoxCollider>();

        return tileObject;
    }

    // Operations
    private Vector2Int LookupTileIndex(GameObject hitInfo){
         for (int x = 0; x < TILE_COUNT_X; x++)
            for (int y = 0; y < TILE_COUNT_Y; y++)
                if(tiles[x,y] == hitInfo)
                    return new Vector2Int(x, y);

        return -Vector2Int.one;  //Invalid
    }

    // Spawning chess piece
    private void SpawnAllPieces(){
        chessPieces = new ChessPiece[TILE_COUNT_X, TILE_COUNT_Y];

        int blueTeam = 0, redTeam = 1;
        chessPieces[0,0] = SpawningSinglePiece(ChessPieceType.Rook, blueTeam, blueTeamPrefab);
        chessPieces[1,0] = SpawningSinglePiece(ChessPieceType.Knight, blueTeam, blueTeamPrefab);
        chessPieces[2,0] = SpawningSinglePiece(ChessPieceType.Bishop, blueTeam, blueTeamPrefab);
        chessPieces[3,0] = SpawningSinglePiece(ChessPieceType.King, blueTeam, blueTeamPrefab);
        chessPieces[4,0] = SpawningSinglePiece(ChessPieceType.Queen, blueTeam, blueTeamPrefab);
        chessPieces[5,0] = SpawningSinglePiece(ChessPieceType.Bishop, blueTeam, blueTeamPrefab);
        chessPieces[6,0] = SpawningSinglePiece(ChessPieceType.Knight, blueTeam, blueTeamPrefab);
        chessPieces[7,0] = SpawningSinglePiece(ChessPieceType.Rook, blueTeam, blueTeamPrefab);
        for( int i = 0; i< TILE_COUNT_X;i++){
            chessPieces[i,1] = SpawningSinglePiece(ChessPieceType.Pawn, blueTeam, blueTeamPrefab);
        }

        chessPieces[0,7] = SpawningSinglePiece(ChessPieceType.Rook, redTeam, redTeamPrefab);
        chessPieces[1,7] = SpawningSinglePiece(ChessPieceType.Knight, redTeam, redTeamPrefab);
        chessPieces[2,7] = SpawningSinglePiece(ChessPieceType.Bishop, redTeam, redTeamPrefab);
        chessPieces[3,7] = SpawningSinglePiece(ChessPieceType.King, redTeam, redTeamPrefab);
        chessPieces[4,7] = SpawningSinglePiece(ChessPieceType.Queen, redTeam, redTeamPrefab);
        chessPieces[5,7] = SpawningSinglePiece(ChessPieceType.Bishop, redTeam, redTeamPrefab);
        chessPieces[6,7] = SpawningSinglePiece(ChessPieceType.Knight, redTeam, redTeamPrefab);
        chessPieces[7,7] = SpawningSinglePiece(ChessPieceType.Rook, redTeam, redTeamPrefab);
        for( int i = 0; i< TILE_COUNT_X;i++){
            chessPieces[i,6] = SpawningSinglePiece(ChessPieceType.Pawn, redTeam, redTeamPrefab);
        }
    }

    private ChessPiece SpawningSinglePiece(ChessPieceType type, int team, GameObject[] prefabs){
        ChessPiece cp = Instantiate(prefabs[(int)type-1],transform).GetComponent<ChessPiece>();
        cp.type = type;
        cp.team = team;
        
        
        return cp;

    }   

    private void HighlightTiles(){
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Highlight");
        }
    }

    private void RevHighlightTiles(){
        for (int i = 0; i < availableMoves.Count; i++)
        {
            tiles[availableMoves[i].x, availableMoves[i].y].layer = LayerMask.NameToLayer("Tile");
        }
        availableMoves.Clear();
    }

    private void PositionAllPieces(){
        for (int x = 0; x < TILE_COUNT_X; x++)
        {
            for (int y = 0; y < TILE_COUNT_Y; y++)
            {
                if(chessPieces[x,y] != null)
                    PositionSinglePiece(x, y, true);
            }
        }
    }

    private void PositionSinglePiece(int x, int y, bool force = false){
        chessPieces[x,y].currentX = x;
        chessPieces[x,y].currentY = y;
        chessPieces[x,y].transform.position = GetTileCenter(x, y);
    }

    public Vector3 GetTileCenter(int x, int y){
        return new Vector3(x*tileSize, yOffset, y*tileSize) -bounds + new Vector3(tileSize/2,0,tileSize/2);
    }

    private bool ContainsValidMove(ref List<Vector2Int> moves, Vector2 pos){
        for (int i = 0; i < moves.Count; i++)
        {
            if(moves[i].x == pos.x && moves[i].y == pos.y ) return true;
        }

        return false;
    }
}
