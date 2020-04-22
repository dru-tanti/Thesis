using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEnums;

// Will be attached to the tiles on the grid to store properties of the tile.
public class Node : MonoBehaviour {
    public List<Node> neighbours;
    public Vector3Int pos;
    public NodeType type;

    public Node() {
        neighbours = new List<Node>();
    }

}
