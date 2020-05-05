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
    // public GameObject[,] activeTiles;

    [Header("Pathfinding")]
    public List<Node> currentPath = null;
    public Node[,] graph;
    public bool[,] walkable;

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
        graph = new Node[width,height];
        walkable = new bool[width,height];
        generateMap(width, height);
    }

    public void clearGrid() {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("Tile");
        foreach(GameObject tile in tiles) {
            Destroy(tile);
        }
        Debug.Log("All Tiles Cleared!");
    }

    public void generateMap(int width, int height) {
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                Color pixel = level.map.GetPixel(x,y);
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
}
