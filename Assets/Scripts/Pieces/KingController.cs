using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KingController : PieceController
{

    new void Update()
    {
        base.Update();
    }

    public override List<Vector2Int> GetPossiblePositions(CaseController[] board)
    {
        var possiblePositions = new List<Vector2Int>();

        // Straight
        possiblePositions.Add(new Vector2Int(this.coordinates.x + 1, this.coordinates.y));
        possiblePositions.Add(new Vector2Int(this.coordinates.x - 1, this.coordinates.y));
        possiblePositions.Add(new Vector2Int(this.coordinates.x, this.coordinates.y + 1));
        possiblePositions.Add(new Vector2Int(this.coordinates.x, this.coordinates.y - 1));

        // Diagonals
        possiblePositions.Add(new Vector2Int(this.coordinates.x + 1, this.coordinates.y + 1));
        possiblePositions.Add(new Vector2Int(this.coordinates.x + 1, this.coordinates.y - 1));
        possiblePositions.Add(new Vector2Int(this.coordinates.x - 1, this.coordinates.y + 1));
        possiblePositions.Add(new Vector2Int(this.coordinates.x - 1, this.coordinates.y - 1));

        return possiblePositions;
    }
}
