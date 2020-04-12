using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour {
    public static GridManager current;

    private void Awake() {
        if(current == null) {
            current = this;
            DontDestroyOnLoad(gameObject);
        } else {
            DestroyImmediate(gameObject);
            return;
        }
    }
    [SerializeField] private Transform ground = default;
    public Vector2Int size;
    [SerializeField] Texture2D gridTexture = default;
    bool showGrid;
    public bool ShowGrid {
        get => showGrid;
        set {
            showGrid = value;
            Material m = ground.GetComponent<MeshRenderer>().material;
            if(showGrid) {
                m.mainTexture = gridTexture;
                m.SetTextureScale("_MainTex", size);
            } else {
                m.mainTexture = null;
            }
        }
    }
    public void Initialize(Vector2Int size) {
        this.size = size;
        ground.localScale = new Vector3(size.x, size.y, 1f);
    }
}
