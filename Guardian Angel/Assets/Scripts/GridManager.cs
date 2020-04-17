using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GlobalEnums;

 [System.Serializable]
public class TileSettings {
    public Color color; // What we will compare the current pixel to.
    public GameObject tile; // What tile will be placed on the pixel.
}

public class GridManager : MonoBehaviour {
    public static GridManager current;
   
    [Header("Level Settings")]
    public LevelSettings level;
    
    [Header("Map Generation")]
    [SerializeField] private TileSettings[] _nodeSettings = null;
    public GameObject[,] activeTiles;

    [Header("Pathfinding")]
    public List<Node> currentPath = null;
    public Node[,] graph;

    private void Awake() {
        if(current == null) {
            current = this;
            DontDestroyOnLoad(gameObject);
        } else {
            DestroyImmediate(gameObject);
            return;
        }
    }

    // Generates the map depending on the Level Settings Scriptable Object.
    public void Initialize() {
        if(level.map == null) {
            Debug.LogError("No map was selected for this level");
            return;
        }
        int width = level.map.width;
        int height = level.map.height;
        generateMap(width, height);
    }

    public void generateMap(int width, int height) {
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                Color pixel = level.map.GetPixel(x,y);
                IEnumerable<TileSettings> select = _nodeSettings.Where(t => t.color == pixel);
                if (select.Count() == 0) continue;
                GameObject tileToSpawn = select.ElementAt(0).tile;
                GameObject node = (GameObject)Instantiate(tileToSpawn, new Vector3Int(x - (width/2), 0, y - (height/2)), Quaternion.Euler(90, 0, 0), this.transform);
                node.GetComponent<Node>().pos = new Vector3Int(x - (width/2), 0, y - (height/2));
                activeTiles = new GameObject[x, y];
                Debug.Log(activeTiles);
            }
        }
    }

    public void generatePathfindingGraph(int width, int height) {
        graph = new Node[width, height];
        // Initialising the graph
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                graph[x, y] = new Node();
                graph[x, y].x = x;
                graph[x, y].y = y;           
            }
        }
        // Adding the neighbors to the graph.
        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                if(x > 0) graph[x,y].neighbours.Add(graph[x-1,y]);
                if(x < width - 1) graph[x,y].neighbours.Add(graph[x+1,y]);
                if(y > 0) graph[x,y].neighbours.Add(graph[x,y-1]);
                if(y < height - 1) graph[x,y].neighbours.Add(graph[x,y+1]);            
            }
        }
    }

    public void generatePath(Vector2Int targetPos, HumanController selected) {
        selected.path = null;
        currentPath = null;
        Dictionary<Node, float> dist = new Dictionary<Node, float>();
        Dictionary<Node, Node> prev = new Dictionary<Node, Node>();    
        Node source = graph[selected.gridPos.x, selected.gridPos.y];
        Node target = graph[targetPos.x, targetPos.y];
        dist[source] = 0;
        prev[source] = null;
        List<Node> unvisited = new List<Node>();

        foreach(Node n in graph) {
            // For now initialize infinity.
            if(n != source) {
                dist[n] = Mathf.Infinity;
                prev[n] = null;
            }
            unvisited.Add(n);
        }
        // As long as a node is listed as unvisited, keep runnning this funciton.
        while(unvisited.Count > 0) {
            Node u = null;
            foreach(Node possibleU in unvisited) {
                if(u == null || dist[possibleU] < dist[u]) {
                    u = possibleU;
                }
            }
            if(u == target) break;
            unvisited.Remove(u);
            foreach(Node n in u.neighbours) {
                float alt = dist[u] + tileCost(new Vector2Int(n.x, n.y));
                if(alt < dist[n]) {
                    dist[n] = alt;
                    prev[n] = u;
                }
            }
        }
        // Either we found the shortest path, or no path exists.
        if(prev[target] == null) return;
        currentPath = new List<Node>();
        Node curr = target;
        while(curr) {
            currentPath.Add(curr);
            curr = prev[curr];
        }
        // Currently the list is backwards, so we reverse it to get the correct order.
        currentPath.Reverse();
        selected.path = currentPath;
    }

    public float tileCost(Vector2Int tilePos) {
        float cost = (float) activeTiles[tilePos.x, tilePos.y].GetComponent<Node>().type;
        if(cost < 1) return Mathf.Infinity;
        Debug.Log(cost);
        return cost;
        // activeTiles[tilePos.x, tilePos.y].GetComponent<Node>().type
    }
}
