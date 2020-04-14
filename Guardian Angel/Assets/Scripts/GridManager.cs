using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GlobalEnums;

public class GridManager : MonoBehaviour {
    [System.Serializable]
    public class NodeSettings {
        public Color color; // What we will compare the current pixel to.
        public GameObject tile; // What tile will be placed on the pixel.
    }
    public static GridManager current;
    public LevelSettings level;
    [SerializeField] private NodeSettings[] _nodeSettings = null;
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
        // Will cycle through the image in the Level Settings, and will generate a tile depending on the colour of the pixel.
        for(int x = 0; x < width; x++) {
            for(int y = 0; y < height; y++) {
                Color pixel = level.map.GetPixel(x,y);
                IEnumerable<NodeSettings> select = _nodeSettings.Where(t => t.color == pixel);
                if (select.Count() == 0) continue;
                GameObject tile = select.ElementAt(0).tile;
                GameObject node = Instantiate(tile, new Vector3Int(x - (width/2), 0, y - (height/2)), Quaternion.Euler(90, 0, 0), this.transform);
            }
        }
    }
}
