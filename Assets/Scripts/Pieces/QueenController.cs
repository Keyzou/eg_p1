using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QueenController : PieceController
{

    new void Update()
    {
        base.Update();
    }

    public override List<Vector2Int> GetPossiblePositions(CaseController[] board)
    {
        var possiblePositions = new List<Vector2Int>();

        // Diagonals
        for(int i = 1; i < 8; i++) {
            if(this.coordinates.x + i >= 8 || this.coordinates.y + i >= 8) break;
            possiblePositions.Add(new Vector2Int(this.coordinates.x + i, this.coordinates.y + i));
            if(board[this.coordinates.x + i + 8 * (this.coordinates.y + i)].Piece != null)
                break;
        }
        for(int i = 1; i < 8; i++) {
            if(this.coordinates.x - i < 0 || this.coordinates.y + i >= 8) break;
            possiblePositions.Add(new Vector2Int(this.coordinates.x - i, this.coordinates.y + i));
            if(board[this.coordinates.x - i + 8 * (this.coordinates.y + i)].Piece != null)
                break;
        }
        for(int i = 1; i < 8; i++) {
            if(this.coordinates.y - i  < 0 || this.coordinates.x + i >= 8 ) break;
            possiblePositions.Add(new Vector2Int(this.coordinates.x + i, this.coordinates.y - i));
            if(board[this.coordinates.x + i + 8 * (this.coordinates.y - i)].Piece != null)
                break;
        }
        for(int i = 1; i < 8; i++) {
            if(this.coordinates.y - i < 0 || this.coordinates.x - i < 0) break;
            possiblePositions.Add(new Vector2Int(this.coordinates.x - i, this.coordinates.y - i));
            if(board[this.coordinates.x - i + 8 * (this.coordinates.y - i)].Piece != null)
                break;
        }

        // Straight lines
        for(int i = 1; i < 8; i++) {
            if(this.coordinates.x + i >= 8) break;
            possiblePositions.Add(new Vector2Int(this.coordinates.x + i, this.coordinates.y));
            if(board[this.coordinates.x + i + 8 * this.coordinates.y].Piece != null)
                break;
        }
        for(int i = 1; i < 8; i++) {
            if(this.coordinates.x - i < 0) break;
            possiblePositions.Add(new Vector2Int(this.coordinates.x - i, this.coordinates.y));
            if(board[this.coordinates.x - i + 8 * this.coordinates.y].Piece != null)
                break;
        }
        for(int i = 1; i < 8; i++) {
            if(this.coordinates.y + i >= 8) break;
            possiblePositions.Add(new Vector2Int(this.coordinates.x, this.coordinates.y + i));
            if(board[this.coordinates.x + 8 * (this.coordinates.y + i)].Piece != null)
                break;
        }
        for(int i = 1; i < 8; i++) {
            if(this.coordinates.y - i < 0) break;
            possiblePositions.Add(new Vector2Int(this.coordinates.x, this.coordinates.y - i));
            if(board[this.coordinates.x + 8 * (this.coordinates.y - i)].Piece != null)
                break;
        }

        return possiblePositions;
    }
}
