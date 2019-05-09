using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseController : MonoBehaviour {

    private Color defaultMaterialColor;
    private Color oldMaterialColor;
    public Color hoverColor = new Color (0.15f, 0.7f, 1f);
    private Color defaultHighlightColor = new Color (1f, 0.2f, 0f);
    public Vector2Int Position;

    public PieceController Piece;

    public bool IsHighlighten;
    // Start is called before the first frame update
    void Start () {
        this.defaultMaterialColor = this.GetComponent<MeshRenderer> ().materials[0].color;
    }

    public void Highlight (Color highlightColor) {
        this.GetComponent<MeshRenderer> ().materials[0].color = highlightColor;
        this.IsHighlighten = true;
    }

    public void RemoveHighlight () {
        this.IsHighlighten = false;
        this.GetComponent<MeshRenderer> ().materials[0].color = this.defaultMaterialColor;
    }

    void OnMouseOver () {
        if (this.GetComponent<MeshRenderer> ().materials[0].color != hoverColor) {
            this.oldMaterialColor = this.GetComponent<MeshRenderer> ().materials[0].color;
        }
        this.GetComponent<MeshRenderer> ().materials[0].color = hoverColor;
    }

    void OnMouseExit () {
        this.GetComponent<MeshRenderer> ().materials[0].color = this.oldMaterialColor;
    }

    public override string ToString () {
        return string.Format ("Case[{0}, {1}]", this.Position.x, this.Position.y);
    }
    // Update is called once per frame
    void Update () {

    }
}