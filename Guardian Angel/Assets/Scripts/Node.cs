using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEnums;

// Will be attached to the tiles on the grid to store properties of the tile.
public class Node : MonoBehaviour {
    public List<Node> neighbours;
    public Vector3Int pos;
    public NodeType type;
    public int x;
    public int y;

    public Node() {
        neighbours = new List<Node>();
    }

    // public float DistanceTo(Node n) {
    //     return Vector2.Distance(new Vector3(x,y), new Vector2(n.x,n.y));
    // }
}
