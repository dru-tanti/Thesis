using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    [SerializeField] private Vector2Int gridSize = new Vector2Int(10,10);
    [SerializeField] private GridManager grid = default;
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
	}
}
