using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    public GameObject Chessboard;
    public PieceController[] Pieces;
    public PieceController SelectedPiece;
    private List<CaseController> highlightenCases;

    public GameObject whiteTile;
    public GameObject blackTile;
    public CaseController[] Board;
    void Start()
    {
        Pieces = FindObjectsOfType<PieceController>();
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
                obj.transform.parent = Chessboard.transform;
                obj.name = string.Format("{0}x{1}", x, y);
                Board[x + y * 8] = obj.AddComponent<CaseController>();
                obj.GetComponent<CaseController>().Position = new Vector2Int(x, y);
                count++;
            }
            count++;
        }

        // After we created the tiles, we add every pieces to the board
        foreach (var piece in Pieces)
        {
            piece.transform.position = Board[piece.coordinates.x + 8 * piece.coordinates.y].transform.position + Vector3.up * 2.73f;
            Board[piece.coordinates.x + 8 * piece.coordinates.y].Piece = piece;
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
                foreach (var coord in this.SelectedPiece.GetPossiblePositions())
                {
                    // We make sure it is inside the board
                    if (coord.x >= 8 || coord.x < 0 || coord.y >= 8 || coord.y < 0)
                    {
                        continue;
                    }

                    // And not on another piece
                    if(Board[coord.x + 8 * coord.y].Piece != null) {
                        continue;
                    }

                    highlightenCases.Add(Board[coord.x + 8 * coord.y]);
                    if (!Board[coord.x + 8 * coord.y].IsHighlighten)
                    {
                        Board[coord.x + 8 * coord.y].Highlight(Color.red);
                    }
                }

            // If we click a tile
            } else if (hits && hitObj.transform.GetComponent<CaseController>() != null)
            {
                var caseController = hitObj.transform.GetComponent<CaseController>();

                // If we have a piece selected and that the tile is one of the possible moves, we move the piece there
                if (this.SelectedPiece != null && highlightenCases.Contains(caseController))
                {
                    var index = Array.IndexOf(Board, caseController);
                    if (index != -1)
                    {
                        Board[this.SelectedPiece.coordinates.x + 8 * this.SelectedPiece.coordinates.y].Piece = null;
                        this.SelectedPiece.coordinates = new Vector2Int(index % 8, index / 8);
                        StartCoroutine(MovePiece(this.SelectedPiece, caseController.transform.position + Vector3.up * 2.73f, 4f));
                        Board[this.SelectedPiece.coordinates.x + 8 * this.SelectedPiece.coordinates.y].Piece = this.SelectedPiece;
                    }

                    // And then we clear the board, removing highlights and unselecting the piece
                    highlightenCases.ForEach((c) => c.RemoveHighlight());
                    highlightenCases.Clear();
                    if (this.SelectedPiece != null) 
                    {
                        this.SelectedPiece.Selected = false;
                    }
                    this.SelectedPiece = null;
                }

            // If we hit nothing important, we unselect the piece if one was selected, and we remove every highlights
            } else if(!hits || (hits && hitObj.transform.GetComponent<CaseController>() == null && hitObj.transform.GetComponent<PieceController>() == null)) {
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
            while(Vector3.Distance(piece.transform.position, to) > 0.0001f)  {
                piece.transform.position = Vector3.Lerp(piece.transform.position, to, speed * Time.deltaTime);
                yield return null;
            } 
            yield break;
        }
    }
}