using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject Chessboard;
    private PieceController[] whitePieces;
    private PieceController[] blackPieces;

    public int turnCount;

    public PieceController.Team currentTeam = PieceController.Team.WHITE;

    public PieceController SelectedPiece;
    private List<CaseController> highlightenCases;
    public CaseController[] Board;

    [Header("Board Generation")]
    public GameObject whiteTile;
    public GameObject blackTile;
    [Space(10f)]
    public Material whitePieceMaterial;
    public Material blackPieceMaterial;
    [Space(10f)]
    public GameObject PawnPrefab;
    public GameObject RookPrefab;
    public GameObject KnightPrefab;
    public GameObject BishopPrefab;
    public GameObject QueenPrefab;
    public GameObject KingPrefab;
    void Start()
    {
        whitePieces = new PieceController[16];
        blackPieces = new PieceController[16];
        Board = new CaseController[8 * 8];
        highlightenCases = new List<CaseController>();
        int count = 0;
        // We programatically generate tiles from two alternating prefabs, better for in-world placement and to add components
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 8; x++)
            {
                var tile = count % 2 == 0 ? whiteTile : blackTile;
                var obj = Instantiate(tile, Vector3.right * x * 10 + Vector3.forward * y * 10, Quaternion.identity);
                obj.transform.parent = Chessboard.transform.Find("Tiles");
                obj.name = string.Format("{0}x{1}", x, y);
                Board[x + y * 8] = obj.AddComponent<CaseController>();
                obj.GetComponent<CaseController>().Position = new Vector2Int(x, y);
                count++;
            }
            count++;
        }

        // After we created the tiles, we add every pieces to the board

        // TODO: Better generation
        AddPiecesToBoard();

        var pieces = Chessboard.transform.Find("Pieces");
        var whiteCount = 0;
        var blackCount = 0;

        for (int i = 0; i < pieces.childCount; i++) {
            var piece = pieces.GetChild(i).gameObject.GetComponent<PieceController>();
            if(piece.team == PieceController.Team.WHITE) {
                whitePieces[whiteCount] = piece;
                whiteCount++;
            }

            if(piece.team == PieceController.Team.BLACK) {
                piece.GetComponent<MeshCollider>().enabled = false;
                blackPieces[blackCount] = piece;
                blackCount++;
            }
        }

    }

    void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitObj;
            var hits = Physics.Raycast(ray, out hitObj, Mathf.Infinity);

            // If we click a piece
            if (hits && hitObj.transform.GetComponent<PieceController>() != null)
            {
                // If the piece isn't the one currently selected, we remove every highlighten tile and "unselect" the piece
                if (this.SelectedPiece != hitObj.transform.GetComponent<PieceController>() && this.SelectedPiece != null)
                {
                    highlightenCases.ForEach((c) => c.RemoveHighlight());
                    highlightenCases.Clear();
                    this.SelectedPiece.Selected = false;
                }
                this.SelectedPiece = hitObj.transform.GetComponent<PieceController>();
                this.SelectedPiece.Selected = true;
                // We check every possible move for the piece, excluding impossible ones like going off the board or on an ally piece
                foreach (var coord in this.SelectedPiece.GetPossiblePositions(Board))
                {
                    if(coord.x >= 8 || coord.x < 0 || coord.y >= 8 || coord.y < 0)
                        continue;
                    highlightenCases.Add(Board[coord.x + 8 * coord.y]);
                    if (!Board[coord.x + 8 * coord.y].IsHighlighten)
                    {
                        Color highlightColor = new Color (0.15f, 1f, 0.7f);
                        if(Board[coord.x + 8 * coord.y].Piece != null && Board[coord.x + 8 * coord.y].Piece.team != this.SelectedPiece.team) {
                            highlightColor = Color.red;
                        }
                        Board[coord.x + 8 * coord.y].Highlight(highlightColor);
                    }
                }

            // If we click a tile
            } 
            else if (hits && hitObj.transform.GetComponent<CaseController>() != null)
            {
                var caseController = hitObj.transform.GetComponent<CaseController>();

                // If we have a piece selected and that the tile is one of the possible moves, we move the piece there
                if (this.SelectedPiece != null && highlightenCases.Contains(caseController))
                {
                    var index = Array.IndexOf(Board, caseController);
                    if (index != -1)
                    {
                        Board[this.SelectedPiece.coordinates.x + 8 * this.SelectedPiece.coordinates.y].Piece = null;
                        StartCoroutine(MovePiece(this.SelectedPiece, caseController.transform.position + Vector3.up * .5f, 4f));
                        this.SelectedPiece.coordinates = new Vector2Int(index % 8, index / 8);
                        if(caseController.Piece != null && caseController.Piece.team != this.SelectedPiece.team) {
                            // TODO: Magic will happen here !
                            Destroy(caseController.Piece.gameObject);
                        }
                        Board[this.SelectedPiece.coordinates.x + 8 * this.SelectedPiece.coordinates.y].Piece = this.SelectedPiece;
                        this.SelectedPiece.OnPieceMoved();


                        // If the player has made a move, we finish the turn.
                        // First, we disable mesh colliders so we can't click the current team's pieces
                        // Then we enable mesh colliders of the other's team, so it can play.
                        // And then we "officially" change the current team to be the other team.
                        
                        switch(currentTeam) {
                            case PieceController.Team.BLACK:
                                for(int i = 0; i < blackPieces.Length; i++) {
                                    blackPieces[i].GetComponent<MeshCollider>().enabled = false;
                                }
                                for(int i = 0; i < whitePieces.Length; i++) {
                                    whitePieces[i].GetComponent<MeshCollider>().enabled = true;
                                }
                            break;
                            default:
                            case PieceController.Team.WHITE:
                                for(int i = 0; i < blackPieces.Length; i++) {
                                    blackPieces[i].GetComponent<MeshCollider>().enabled = true;
                                }
                                for(int i = 0; i < whitePieces.Length; i++) {
                                    whitePieces[i].GetComponent<MeshCollider>().enabled = false;
                                }
                            break;
                        }
                        currentTeam = currentTeam == PieceController.Team.WHITE ? PieceController.Team.BLACK : PieceController.Team.WHITE;
                    }

                }
                // And then we clear the board, removing highlights and unselecting the piece
                highlightenCases.ForEach((c) => c.RemoveHighlight());
                highlightenCases.Clear();
                if (this.SelectedPiece != null) 
                {
                    this.SelectedPiece.Selected = false;
                }
                this.SelectedPiece = null;

            // If we hit nothing important, we unselect the piece if one was selected, and we remove every highlights
            } 
            else if(!hits || (hits && hitObj.transform.GetComponent<CaseController>() == null && hitObj.transform.GetComponent<PieceController>() == null)) {
                if (this.SelectedPiece != null) 
                {
                    this.SelectedPiece.Selected = false;
                }
                this.SelectedPiece = null;
                highlightenCases.ForEach((c) => c.RemoveHighlight());
                highlightenCases.Clear();
            }

        }

        // Coroutine used to move a piece to another point smoothly
        IEnumerator MovePiece(PieceController piece, Vector3 to, float speed) {
            var startingPos = piece.transform.position;
            var elaspedTime = 0f;
            var time = 0.5f;
            while(elaspedTime < time)  {
                piece.transform.position = Vector3.Lerp(startingPos, to, Mathf.SmoothStep(0, 1, elaspedTime / time));
                elaspedTime += Time.deltaTime;
                yield return null;
            }
            yield break;
        }
    }


    // FIXME: Better way to generate pieces on the board!
    private void AddPiecesToBoard() {
        GameObject go;
        
        ///////////////////////////////
        // We generate white team first
        ///////////////////////////////

        // Rooks
        go = Instantiate(RookPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");
        go.GetComponent<PieceController>().team = PieceController.Team.WHITE;
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        
        go = Instantiate(RookPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");
        go.GetComponent<PieceController>().team = PieceController.Team.WHITE;
        go.GetComponent<PieceController>().coordinates = new Vector2Int(7 - go.GetComponent<PieceController>().coordinates.x, go.GetComponent<PieceController>().coordinates.y);
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        
        // Knights
        go = Instantiate(KnightPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");
        go.GetComponent<PieceController>().team = PieceController.Team.WHITE;
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        
        go = Instantiate(KnightPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");
        go.GetComponent<PieceController>().team = PieceController.Team.WHITE;
        go.GetComponent<PieceController>().coordinates = new Vector2Int(7 - go.GetComponent<PieceController>().coordinates.x, go.GetComponent<PieceController>().coordinates.y);
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        

        // Bishops
        go = Instantiate(BishopPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");
        go.GetComponent<PieceController>().team = PieceController.Team.WHITE;
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        
        go = Instantiate(BishopPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");
        go.GetComponent<PieceController>().team = PieceController.Team.WHITE;
        go.GetComponent<PieceController>().coordinates = new Vector2Int(7 - go.GetComponent<PieceController>().coordinates.x, go.GetComponent<PieceController>().coordinates.y);
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        
        // King
        go = Instantiate(KingPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");
        go.GetComponent<PieceController>().team = PieceController.Team.WHITE;
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        
        // Queen
        go = Instantiate(QueenPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");
        go.GetComponent<PieceController>().team = PieceController.Team.WHITE;
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        
        // Pawns
        for(int i = 0; i < 8; i++) {
            go = Instantiate(PawnPrefab, Vector3.zero, Quaternion.identity);
            go.transform.parent = Chessboard.transform.Find("Pieces");
            go.GetComponent<PieceController>().team = PieceController.Team.WHITE;
            go.GetComponent<PawnController>().coordinates = new Vector2Int(i, 1);
            go.transform.position = Board[i + 8 * 1].transform.position + Vector3.up * 0.5f;
            Board[i + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        }
        
        ///////////////////////////////
        // We generate black team then
        ///////////////////////////////


        // Rooks =========================
        
        go = Instantiate(RookPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");
        
        // We set the material and the team to the black one since it's the black team
        go.GetComponent<MeshRenderer>().material = blackPieceMaterial;
        go.GetComponent<PieceController>().team = PieceController.Team.BLACK;

        go.GetComponent<PieceController>().coordinates.y = 7 - go.GetComponent<PieceController>().coordinates.y;
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        
        // Second one

        go = Instantiate(RookPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");

        go.GetComponent<MeshRenderer>().material = blackPieceMaterial;
        go.GetComponent<PieceController>().team = PieceController.Team.BLACK;

        go.GetComponent<PieceController>().coordinates.y = 7 - go.GetComponent<PieceController>().coordinates.y;
        go.GetComponent<PieceController>().coordinates = new Vector2Int(7 - go.GetComponent<PieceController>().coordinates.x, go.GetComponent<PieceController>().coordinates.y);
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        
        // Knights =========================

        go = Instantiate(KnightPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");
        go.GetComponent<MeshRenderer>().material = blackPieceMaterial;
        go.GetComponent<PieceController>().team = PieceController.Team.BLACK;
        go.GetComponent<PieceController>().coordinates.y = 7 - go.GetComponent<PieceController>().coordinates.y;
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        
        go = Instantiate(KnightPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");
        go.GetComponent<MeshRenderer>().material = blackPieceMaterial;
        go.GetComponent<PieceController>().team = PieceController.Team.BLACK;
        go.GetComponent<PieceController>().coordinates.y = 7 - go.GetComponent<PieceController>().coordinates.y;
        go.GetComponent<PieceController>().coordinates = new Vector2Int(7 - go.GetComponent<PieceController>().coordinates.x, go.GetComponent<PieceController>().coordinates.y);
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        
        // Bishops =========================

        go = Instantiate(BishopPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");
        go.GetComponent<MeshRenderer>().material = blackPieceMaterial;
        go.GetComponent<PieceController>().team = PieceController.Team.BLACK;
        go.GetComponent<PieceController>().coordinates.y = 7 - go.GetComponent<PieceController>().coordinates.y;
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        
        go = Instantiate(BishopPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");
        go.GetComponent<MeshRenderer>().material = blackPieceMaterial;
        go.GetComponent<PieceController>().team = PieceController.Team.BLACK;
        go.GetComponent<PieceController>().coordinates.y = 7 - go.GetComponent<PieceController>().coordinates.y;
        go.GetComponent<PieceController>().coordinates = new Vector2Int(7 - go.GetComponent<PieceController>().coordinates.x, go.GetComponent<PieceController>().coordinates.y);
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        
        // King =========================
        
        go = Instantiate(KingPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");
        go.GetComponent<MeshRenderer>().material = blackPieceMaterial;
        go.GetComponent<PieceController>().team = PieceController.Team.BLACK;
        go.GetComponent<PieceController>().coordinates.y = 7 - go.GetComponent<PieceController>().coordinates.y;
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        
        // Queen =========================

        go = Instantiate(QueenPrefab, Vector3.zero, Quaternion.identity);
        go.transform.parent = Chessboard.transform.Find("Pieces");
        go.GetComponent<MeshRenderer>().material = blackPieceMaterial;
        go.GetComponent<PieceController>().team = PieceController.Team.BLACK;
        go.GetComponent<PieceController>().coordinates.y = 7 - go.GetComponent<PieceController>().coordinates.y;
        go.transform.position = Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
        Board[go.GetComponent<PieceController>().coordinates.x + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        
        // Pawns =========================
        
        for(int i = 0; i < 8; i++) {
            go = Instantiate(PawnPrefab, Vector3.zero, Quaternion.identity);
            go.transform.parent = Chessboard.transform.Find("Pieces");
        go.GetComponent<MeshRenderer>().material = blackPieceMaterial;
            go.GetComponent<PieceController>().team = PieceController.Team.BLACK;
            go.GetComponent<PawnController>().coordinates = new Vector2Int(i, 1);
            go.GetComponent<PieceController>().coordinates.y = 7 - go.GetComponent<PieceController>().coordinates.y;
            go.transform.position = Board[i + 8 * go.GetComponent<PieceController>().coordinates.y].transform.position + Vector3.up * 0.5f;
            Board[i + 8 * go.GetComponent<PieceController>().coordinates.y].Piece = go.GetComponent<PieceController>();
        }
    }
}