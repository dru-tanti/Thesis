using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityAtoms.BaseAtoms;
using GlobalEnums;

 [System.Serializable]
public class TileSettings {
    public Color color; // What we will compare the current pixel to.
    public GameObject tile; // What tile will be placed on the pixel.
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GridManager : MonoBehaviour {
    public NavMeshSurface surface;
    public static GridManager current;
   
    [Header("Level Settings")]
    public IntVariable currentLevel;
    
    [Header("Map Generation")]
    [SerializeField] private TileSettings[] _nodeSettings = null;
    private Vector3[] _vertices;
    private Mesh _mesh;
    // public GameObject[,] activeTiles;

    [Header("Pathfinding")]
    public List<Node> currentPath = null;
    public Node[,] graph;
    public bool[,] walkable;
    
    private void Awake() {
        surface = this.GetComponent<NavMeshSurface>();
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
        if(GameManager.current.level[currentLevel.Value].map == null) {
            Debug.LogError("No map was selected for this level");
            return;
        }
        int width = GameManager.current.level[currentLevel.Value].map.width;
        int height = GameManager.current.level[currentLevel.Value].map.height;
        GetComponent<MeshFilter>().mesh = _mesh = new Mesh();
        _mesh.name = "Map";
        graph = new Node[width,height];
        walkable = new bool[width,height];
        generateMap(width, height);
        generateNavMesh(width, height);
    }

    public void clearMap() {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach(GameObject tile in tiles) {
            Destroy(tile);
        }
        Debug.Log("All Tiles Cleared!");
    }

    // Creates a simple 2 trianlge mesh to be used with the Unity NavMesh.
    private void generateNavMesh(int width, int height) {
        _mesh.Clear();
        _vertices = new Vector3[(width+1) * (height+1)];
        _vertices[0] = new Vector3(0,0,0); // Bottom Left
        _vertices[1] = new Vector3(width,0,0); // Bottom Right
        _vertices[2] = new Vector3(0,0,height); // Top Left
        _vertices[3] = new Vector3(width,0,height); // Top Right
        _mesh.vertices = _vertices;
        int[] triangles = new int[6];
        triangles[0] = 0;
        triangles[1] = triangles[4] = 2;
        triangles[2] = triangles[3] = 1;
        triangles[5] = 3;
        _mesh.triangles = triangles;
    }

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
    private void OnDrawGizmos () {
        if(_vertices == null) return;
		Gizmos.color = Color.black;
		for (int i = 0; i < _vertices.Length; i++) {
			Gizmos.DrawSphere(_vertices[i], 0.1f);
		}
	}
}
