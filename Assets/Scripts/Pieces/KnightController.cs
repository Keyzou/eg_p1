using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightController : PieceController
{

    new void Update()
    {
        base.Update();
    }

    public override List<Vector2Int> GetPossiblePositions(CaseController[] board)
    {
        var possiblePositions = new List<Vector2Int>();
        
        possiblePositions.Add (new Vector2Int (coordinates.x - 1, coordinates.y + 2));
        possiblePositions.Add (new Vector2Int (coordinates.x + 1, coordinates.y + 2));
        possiblePositions.Add (new Vector2Int (coordinates.x - 1, coordinates.y - 2));
        possiblePositions.Add (new Vector2Int (coordinates.x + 1, coordinates.y - 2));
        possiblePositions.Add (new Vector2Int (coordinates.x + 2, coordinates.y + 1));
        possiblePositions.Add (new Vector2Int (coordinates.x + 2, coordinates.y - 1));
        possiblePositions.Add (new Vector2Int (coordinates.x - 2, coordinates.y + 1));
        possiblePositions.Add (new Vector2Int (coordinates.x - 2, coordinates.y - 1));

        return possiblePositions;
    }
}
