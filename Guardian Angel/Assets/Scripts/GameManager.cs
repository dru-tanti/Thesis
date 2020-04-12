using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using GlobalEnums;
public class GameManager : MonoBehaviour {
    public static GameManager current;
    [SerializeField] private Vector2Int gridSize = new Vector2Int(10,10);
    [SerializeField] private GridManager grid = default;
    // [SerializeField] private Grid grid = default;
    [SerializeField] private Tilemap tilemap = default;

    // [HideInInspector] 
    public Vector3 mousePos;

    private HumanController selected;
    private void Awake() {
        if(current == null) {
            current = this;
            DontDestroyOnLoad(gameObject);
        } else {
            DestroyImmediate(gameObject);
            return;
        }
        grid.Initialize(gridSize);
        grid.ShowGrid = true;
    }

    void OnValidate () {
		if (gridSize.x < 2) gridSize.x = 2;
		if (gridSize.y < 2) gridSize.y = 2;
	}
    

    void Update () {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow, 0, true);
        if(Input.GetMouseButtonDown(0)) {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(ray, out hitInfo);
            Debug.Log(hitInfo.transform.position);
            if(hit && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Human")) {
                if(!selected) selected = hitInfo.transform.gameObject.GetComponent<HumanController>();
                if(selected == hitInfo.transform.gameObject) return;
                selectNewObject(hitInfo);
            } else  {
                Debug.Log("Not it Chief");
            }
        }
    }

    private void selectNewObject(RaycastHit hitInfo) {
        selected.state = State.Unselected;
        selected = hitInfo.transform.gameObject.GetComponent<HumanController>();
        selected.state = State.Selected;
    }

    private void OnDrawGizmos() {
		Gizmos.color = Color.blue;
		Gizmos.DrawWireCube(mousePos, new Vector3Int (1,0,1));
	}
}
