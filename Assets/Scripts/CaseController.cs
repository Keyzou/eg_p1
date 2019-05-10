using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaseController : MonoBehaviour {

    private Color defaultMaterialColor;
    private Color oldMaterialColor;
    public Material HighlightMaterial;
    public Color hoverColor = new Color (0.15f, 0.5f, 0.9f);
    private Color defaultHighlightColor = new Color (1f, 0.2f, 0f);
    public Vector2Int Position;

    public PieceController Piece;

    public bool IsHighlighten;
    // Start is called before the first frame update
    void Start () {
        this.defaultMaterialColor = this.GetComponent<MeshRenderer> ().materials[0].color;
    }

    public void Highlight (Color highlightColor) {
        Debug.Log(this.GetComponent<MeshRenderer>().materials[0].name);
        this.GetComponent<MeshRenderer>().materials[0].color = Color.Lerp(highlightColor, this.GetComponent<MeshRenderer>().materials[0].name == "Black_Case (Instance)" ? Color.black : Color.white, .25f);
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