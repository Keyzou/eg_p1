using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Outline))]
public class PieceController : MonoBehaviour {

    public Color hoverColor;
    public Color selectedColor;
    public bool Selected { get; set; }

    public Vector2Int coordinates;
    private bool hover;

    // Start is called before the first frame update
    void Start () {

    }

    public List<Vector2Int> GetPossiblePositions () {
        var possiblePositions = new List<Vector2Int> ();
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

    void OnMouseOver () {
        this.hover = true;
    }

    void OnMouseExit () {
        this.hover = false;
    }

    // Update is called once per frame
    void Update () {
        this.GetComponent<Outline> ().enabled = Selected || hover;
        this.GetComponent<Outline> ().OutlineColor = Selected ? selectedColor : hoverColor;
    }
}