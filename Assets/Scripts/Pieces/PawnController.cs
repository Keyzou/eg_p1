using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnController : PieceController
{

    public bool hasMoved = false;

    new void Update()
    {
        base.Update();
    }

    public override List<Vector2Int> GetPossiblePositions(CaseController[] board)
    {
        var possiblePositions = new List<Vector2Int>();

        var tile = board[this.coordinates.x + 1 + 8 * (this.coordinates.y + (this.team == PieceController.Team.BLACK ? -1 : 1))];
        if(tile.Piece != null && tile.Piece.team != this.team) {
            possiblePositions.Add(new Vector2Int(this.coordinates.x + 1, this.coordinates.y + (this.team == PieceController.Team.BLACK ? -1 : 1)));
        }
        tile = board[this.coordinates.x - 1 + 8 * (this.coordinates.y + (this.team == PieceController.Team.BLACK ? -1 : 1))];
        if(tile.Piece != null && tile.Piece.team != this.team) {
            possiblePositions.Add(new Vector2Int(this.coordinates.x - 1, this.coordinates.y + (this.team == PieceController.Team.BLACK ? -1 : 1)));
        }
        
        if(board[this.coordinates.x + 8 * (this.coordinates.y + (this.team == PieceController.Team.BLACK ? -1 : 1))].Piece != null) {
            return possiblePositions;
        }

        possiblePositions.Add(new Vector2Int(this.coordinates.x, this.coordinates.y + (this.team == PieceController.Team.BLACK ? -1 : 1)));

        if(!hasMoved && board[this.coordinates.x + 8 * (this.coordinates.y + (this.team == PieceController.Team.BLACK ? -2 : 2))].Piece == null) {
            possiblePositions.Add(new Vector2Int(this.coordinates.x, this.coordinates.y + (this.team == PieceController.Team.BLACK ? -2 : 2)));
        }

        return possiblePositions;
    }


    public override void OnPieceMoved() {
        hasMoved = true;
    }
}
