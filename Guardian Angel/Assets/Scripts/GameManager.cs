using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private Vector2Int gridSize = new Vector2Int(10,10);
    [SerializeField] private GridManager grid = default;
    private HumanController selected;
    private void Awake() {
        grid.Initialize(gridSize);
        grid.ShowGrid = true;
    }

    void OnValidate () {
		if (gridSize.x < 2) gridSize.x = 2;
		if (gridSize.y < 2) gridSize.y = 2;
	}
  
    void Update () {
        if (Input.GetKeyDown(KeyCode.G)) {
            grid.ShowGrid = !grid.ShowGrid;
        }
        if(Input.GetMouseButtonDown(0)) {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if(hit && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Human")) {
                if(!selected) selected = hitInfo.transform.gameObject.GetComponent<HumanController>();
                if(selected == hitInfo.transform.gameObject) return;
                selected.state = State.Unselected;
                selected = hitInfo.transform.gameObject.GetComponent<HumanController>();
                selected.state = State.Selected;
            } else {
                Debug.Log("Not it Chief");
            }
        }
    }
}
