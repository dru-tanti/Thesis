using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Pathfinding;
using UnityAtoms.BaseAtoms;
using GlobalEnums;

 [System.Serializable]
public class TileSettings {
    public Color color; // What we will compare the current pixel to.
    public GameObject tile; // What tile will be placed on the pixel.
}

public class GridManager : MonoBehaviour {
    public static GridManager current;
   
    [Header("Level Settings")]
    public IntVariable currentLevel;
    
    [Header("Map Generation")]
    [SerializeField] private TileSettings[] _nodeSettings = null;

    [Header("Pathfinding")]
    public Node[,] graph;
    public bool[,] walkable;
    private Seeker seeker;
    private void Awake() {
        if(current == null) {
            current = this;
            DontDestroyOnLoad(gameObject);
        } else {
            DestroyImmediate(gameObject);
            return;
        }
        seeker = GetComponent<Seeker>();
    }

    // Generates the map depending on the Level Settings Scriptable Object.
    public void Initialize() {
        if(GameManager.current.level[currentLevel.Value].map == null) {
            Debug.LogError("No map was selected for this level");
            return;
        }
        int width = GameManager.current.level[currentLevel.Value].map.width;
        int height = GameManager.current.level[currentLevel.Value].map.height;
        // GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        // _mesh.name = "Map";
        graph = new Node[width,height];
        walkable = new bool[width,height];
        generateMap(width, height);
        generateGridGraph(width, height);
    }

    public void clearMap() {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach(GameObject tile in tiles) {
            Destroy(tile);
        }
        Debug.Log("All Tiles Cleared!");
    }

    // Generates a grid graph for the A* Pathfinding system.
    private void generateGridGraph(int width, int height) {
        AstarData data = AstarPath.active.data;
        GridGraph gg = data.AddGraph(typeof(GridGraph)) as GridGraph;
        float nodeSize = 1f;
        gg.center = new Vector3((width/2)-0.5f, 0, (height/2)-0.5f);
        gg.SetDimensions(width, height,nodeSize);
        gg.neighbours = NumNeighbours.Four;
        AstarPath.active.Scan();
        AstarPath.active.AddWorkItem(new AstarWorkItem(ctx => {
            for(int x = 0; x < width; x++) {
                for(int y = 0; y < height; y++) {
                    var node = gg.GetNode(x,y);
                    node.Walkable = walkable[x,y];
                }
            }
            gg.GetNodes(node => gg.CalculateConnections((GridNodeBase)node));
        }));
    }

    // Generates the tiles that will form the playable map.
    public void generateMap(int width, int height) {
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                Color pixel = GameManager.current.level[currentLevel.Value].map.GetPixel(x,y);
                IEnumerable<TileSettings> select = _nodeSettings.Where(t => t.color == pixel);
                if (select.Count() == 0) continue;
                GameObject tileToSpawn = select.ElementAt(0).tile;
                GameObject node = (GameObject)Instantiate(tileToSpawn, new Vector3Int(x, 0, y), Quaternion.Euler(90, 0, 0), this.transform);
                graph[x,y] = node.GetComponent<Node>();
                graph[x,y].pos = new Vector3Int(x, 0, y);
                walkable[x,y] = (graph[x,y].type == NodeType.Building) ? false : true;
            }
        }
    }
    
    public int findDistance(Vector3 currPos, Vector3 targetPos) {
        // if(AstarPath.active == null) return;
        int distance = 0;
        Path path = seeker.StartPath(currPos, targetPos);
        path.BlockUntilCalculated();
        float length = path.GetTotalLength();
        distance = Mathf.RoundToInt(length);
        Debug.Log("Total distance of path: "+distance);
        return distance;
    }
}
