using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using TMPro;
using GlobalEnums;
public class GameManager : MonoBehaviour {
    public static GameManager current;
    private HumanController _selected;
    [Header("Level Settings")]
    public LevelSettings[] level; 
    [Header("Hazard Management")]
    public List<GameObject> activeHazards;
    public GameObject hazardTile;
    [Header("Game State Information")]
    public BoolVariable _playerTurn;
    public IntVariable years, currentTurn, currentLevel;
    [Header("UI Settings")]
    public GameObject turnCounter;
    public GameObject yearsCounter;
    private TextMeshProUGUI _turns;
    private TextMeshProUGUI _years;
    private void Awake() {
        if(current == null) {
            current = this;
            DontDestroyOnLoad(gameObject);
        } else {
            DestroyImmediate(gameObject);
            return;
        }
        _turns = turnCounter.GetComponent<TextMeshProUGUI>();
        _years = yearsCounter.GetComponent<TextMeshProUGUI>();
        GridManager.current.Initialize();
        setHazard();
        years.Value = 0;
        currentLevel.Value = 0;
        currentTurn.Value = 0;
        _turns.SetText("Turns Remaining: {0}", GameManager.current.level[currentLevel.Value].turns - currentTurn.Value);
    }

    void Update () {
        if(_playerTurn.Value) {
            // Then the LMB is clicked, raycast to check what has been hit.
            if(Input.GetMouseButtonDown(0)) {
                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                if(hit && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                    Node tile = hitInfo.transform.gameObject.GetComponent<Node>();
                    if(!_selected) return;
                    if(tile.type == NodeType.Building) {
                        Debug.Log("Invalid Tile Selected");
                        return;
                    }
                    _selected.moveHuman(new Vector3Int(tile.pos.x, 1, tile.pos.z));
                } else if(hit && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Human")) {
                    if(!_selected) _selected = hitInfo.transform.gameObject.GetComponent<HumanController>();
                    if(_selected == hitInfo.transform.gameObject) return;
                    selectNewObject(hitInfo);
                } else {
                    Debug.Log("Not it Chief");
                }
            }
        }
    }

    // Unselects the current human, and selects the new one.
    private void selectNewObject(RaycastHit hitInfo) {
        _selected.state = HumanState.Unselected;
        _selected = hitInfo.transform.gameObject.GetComponent<HumanController>();
        _selected.state = HumanState.Selected;
    }

    // Selects a random position and places a Hazard tile.
    private void setHazard() {
        for (int i = 0; i < Random.Range(1, level[currentLevel.Value].maxHazards); i++) {
            int randomX = Random.Range(0, 10);
            int randomY = Random.Range(0,10);
            while(GridManager.current.walkable[randomX,randomY] == false) {
                randomX = Random.Range(0, 10);
                randomY = Random.Range(0,10);
            }
            GameObject hazard = Instantiate(hazardTile, new Vector3(Random.Range(0, 10), 0.01f, Random.Range(0, 10)), Quaternion.Euler(90, 0, 0));
            activeHazards.Add(hazard);    
        }
    }
    
    public void endTurn() {
        _playerTurn.Value = false;
        // Loops through the hazards and checks if a human is standing on top of it.
        foreach(GameObject hazard in activeHazards) {
            RaycastHit hitInfo = new RaycastHit();
            if (Physics.Raycast(hazard.transform.position, transform.TransformDirection(Vector3.up), out hitInfo, 20.0f, LayerMask.GetMask("Human"))) {
                if(hitInfo.transform.gameObject.GetComponent<HumanController>()._protected) Debug.LogError("Protected Humn Killed!");
                Destroy(hitInfo.transform.gameObject);
                Destroy(hazard);
            } else {
                Destroy(hazard);
            }
        }
        activeHazards.Clear();
        currentTurn.Value++;
        if (currentTurn.Value > level[currentLevel.Value].turns) {
            endLevel();
        }
        _playerTurn.Value = true;
        _turns.SetText("Turns Remaining: {0}", level[currentLevel.Value].turns - currentTurn.Value);
        _years.SetText("Years Collected: {0}", years.Value);
        setHazard();
    }

    public void endLevel() {
        currentLevel.Value++;
        currentTurn.Value = 0;
        GridManager.current.clearGrid();
        GridManager.current.Initialize();
    }
}
