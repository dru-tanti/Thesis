using UnityEngine;
using GlobalEnums;

// Will be attached to the tiles on the grid to store properties of the tile.
public class Node : MonoBehaviour {
    public Vector3 pos;
    public NodeType type;

    private void Start() {
        pos = this.transform.position;
    }
    // public Node(Vector2Int position, NodeType nodetype) {
    //     pos = position;
    //     type = nodetype;
    // }
}
