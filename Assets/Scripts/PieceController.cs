using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Outline))]
public abstract class PieceController : MonoBehaviour {

    public Team team;
    public Vector2Int coordinates;
    public Color hoverColor = new Color(112 / 255f, 229 / 255f, 1f);
    public Color selectedColor = new Color(107 / 255f, 221 / 255f, 41 / 255f);
    public bool Selected { get; set; }

    private bool hover;

    public abstract List<Vector2Int> GetPossiblePositions(CaseController[] board);

    public virtual void OnPieceMoved() {

    }
    void OnMouseOver () {
        this.hover = true;
    }

    void OnMouseExit () {
        this.hover = false;
    }

    // Update is called once per frame
    protected void Update () {
        this.GetComponent<Outline> ().enabled = Selected || hover;
        this.GetComponent<Outline> ().OutlineColor = Selected ? selectedColor : hoverColor;
    }

    public enum Team {
        WHITE,
        BLACK
    }
}