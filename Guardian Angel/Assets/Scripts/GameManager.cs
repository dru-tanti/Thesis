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
    public GameObject turnCounter, yearsCounter, hoverText;
    private TextMeshProUGUI _turns, _years, _name, _age, _description;
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
            // Set a random number from 0 to the length of the X and Y of graph[,]
            int randomX = Random.Range(0,GridManager.current.graph.GetLength(0));
            int randomY = Random.Range(0,GridManager.current.graph.GetLength(1));
            Debug.Log(GridManager.current.graph.GetLength(0));
            // If a non walkable tile exists in these coordinates, generate them again.
            while(GridManager.current.walkable[randomX,randomY] == false) {
                randomX = Random.Range(0,GridManager.current.graph.GetLength(0));
                randomY = Random.Range(0,GridManager.current.graph.GetLength(1));
            }
            // Creates the hazard tile and adds it to the list of active hazards.
            GameObject hazard = Instantiate(hazardTile, new Vector3(randomX, 0.01f, randomY), Quaternion.Euler(90, 0, 0));
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
        // Ends the level if the max turn count is exceeded.
        if (currentTurn.Value > level[currentLevel.Value].turns) endLevel();
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

    // Set the text of the hover box, and display it.
    public void showText(string name, int age, string description) {
        hoverText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(name);
        hoverText.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("Age: {0}", age);
        hoverText.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText(description);
        hoverText.SetActive(true);
        hoverText.transform.position = Input.mousePosition - new Vector3(-80, -100, 0);
    }

    public void hideText() {
        hoverText.SetActive(false);
    }
}
