using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Chessboard : MonoBehaviour
{
    private const int TILE_COUNT_X = 8;
    private const int TILE_COUNT_Y = 8;
    private GameObject[,] tiles;
    private Camera currentCam;
    private Vector2Int currentHover;
    private Vector3 bounds;
    private ChessPiece[,] chessPieces;
    private ChessPiece currentPiece;
    public bool isClicked = false;


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

        RaycastHit info;
        Ray ray = currentCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out info, 100, LayerMask.GetMask("Tile","Hover") ))
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
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = hitPosition;
                tiles[hitPosition.x, hitPosition.y].layer = LayerMask.NameToLayer("Hover");
            }
        
            if(Input.GetMouseButtonDown(0)){
                if(chessPieces[hitPosition.x, hitPosition.y] != null && chessPieces[hitPosition.x, hitPosition.y].team == currentPlayer){
                    //player's turn
                    isClicked = true;
                    currentPiece = chessPieces[hitPosition.x, hitPosition.y];
                    return;                   
                }

            }
            if(isClicked && Input.GetMouseButtonDown(0)){
                    Vector2Int previousPos = new Vector2Int(currentPiece.currentX, currentPiece.currentY);
                    bool validMove = MoveTo(currentPiece, hitPosition.x, hitPosition.y);
                    if(!validMove){
                        return;
                    }
                    
                    currentPlayer = (currentPlayer+1)%2;
                    isClicked = false;
                    
            }

        }
        else
        {
            if (currentHover != -Vector2Int.one)
            {
                tiles[currentHover.x, currentHover.y].layer = LayerMask.NameToLayer("Tile");
                currentHover = -Vector2Int.one;
            }
        }



    }

    private bool MoveTo(ChessPiece cp, int x, int y)
    {
        Vector2Int prePos = new Vector2Int(cp.currentX, cp.currentY);
        if(chessPieces[x, y] != null && chessPieces[x, y].team != cp.team)
        {
            ChessPiece capturedPiece = chessPieces[x, y];
            StartCoroutine(AttackChessPiece(cp, capturedPiece, x, y, prePos));
            return true;
        }

        chessPieces[x, y] = cp;
        chessPieces[prePos.x, prePos.y] = null;
        
        MoveToCo(x, y);
        
        return true;
    }

    private IEnumerator AttackChessPiece(ChessPiece attacker, ChessPiece target, int x, int y, Vector2Int prePos){
        // UP
        if(attacker.currentY - y > 1 && attacker.currentX == x) SetAttackOffset(0,1);
        // DOWN
        else if (attacker.currentY - y < -1 && attacker.currentX == x) SetAttackOffset(0,-1);
        // LEFT
        else if(attacker.currentX - x < -1 && attacker.currentY == y) SetAttackOffset(1,0);
        //RIGHT
        else if(attacker.currentX - x > 1 && attacker.currentY == y) SetAttackOffset(-1,0);
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
            yield return new WaitForSeconds(.5f);
            Destroy(target.gameObject);
            chessPieces[x + attackOffsetX, y + attackOffsetY] = attacker;
            chessPieces[prePos.x, prePos.y] = null;
            MoveToCo(x, y);
            yield break;
        }
        
        chessPieces[x + attackOffsetX, y + attackOffsetY] = attacker;
        chessPieces[prePos.x, prePos.y] = null;
        MoveToCo(x + attackOffsetX, y+ attackOffsetY);
        yield return new WaitForSeconds(3f);
        StartCoroutine(attacker.AttackingCoroutine());
        yield return new WaitForSeconds(1f);
        StartCoroutine(target.DeathCo());
        yield return new WaitForSeconds(.5f);
        Destroy(target.gameObject);
        chessPieces[x, y] = attacker;
        chessPieces[prePos.x, prePos.y] = null;
        
        MoveToCo(x, y);
        
    }

    private void MoveToCo(int x, int y){
        chessPieces[x,y].currentX = x;
        chessPieces[x,y].currentY = y;
        chessPieces[x,y].SetPos(GetTileCenter(x,y));

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
        chessPieces[3,0] = SpawningSinglePiece(ChessPieceType.Queen, blueTeam, blueTeamPrefab);
        chessPieces[4,0] = SpawningSinglePiece(ChessPieceType.King, blueTeam, blueTeamPrefab);
        chessPieces[5,0] = SpawningSinglePiece(ChessPieceType.Bishop, blueTeam, blueTeamPrefab);
        chessPieces[6,0] = SpawningSinglePiece(ChessPieceType.Knight, blueTeam, blueTeamPrefab);
        chessPieces[7,0] = SpawningSinglePiece(ChessPieceType.Rook, blueTeam, blueTeamPrefab);
        for( int i = 0; i< TILE_COUNT_X;i++){
            chessPieces[i,1] = SpawningSinglePiece(ChessPieceType.Pawn, blueTeam, blueTeamPrefab);
        }

        chessPieces[0,7] = SpawningSinglePiece(ChessPieceType.Rook, redTeam, redTeamPrefab);
        chessPieces[1,7] = SpawningSinglePiece(ChessPieceType.Knight, redTeam, redTeamPrefab);
        chessPieces[2,7] = SpawningSinglePiece(ChessPieceType.Bishop, redTeam, redTeamPrefab);
        chessPieces[3,7] = SpawningSinglePiece(ChessPieceType.Queen, redTeam, redTeamPrefab);
        chessPieces[4,7] = SpawningSinglePiece(ChessPieceType.King, redTeam, redTeamPrefab);
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

    private Vector3 GetTileCenter(int x, int y){
        return new Vector3(x*tileSize, yOffset, y*tileSize) -bounds + new Vector3(tileSize/2,0,tileSize/2);
    }

}
