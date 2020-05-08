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
    public IntVariable years, currentTurn, currentLevel, actionPoints;
    private int dice;

    [Header("Human Prefabs")]
    public List<GameObject> _protectedHumans;
    public GameObject protectedHuman, regularHuman;

    [Header("UI Settings")]
    public bool gamePaused = false;
    public GameObject pauseMenu, turnCounter, yearsCounter, hoverText, actionCounter;
    private TextMeshProUGUI _turns, _years, _ap, _name, _age, _description;

    public List<Vector2Int> neighbors;
    private void Awake() {
        // Sets the GameManager as a singleton
        if(current == null) {
            current = this;
            DontDestroyOnLoad(gameObject);
        } else {
            DestroyImmediate(gameObject);
            return;
        }
        _protectedHumans = new List<GameObject>();
        // TODO: Move to seperate script.
        // Gets refernce to UI elements
        _turns = turnCounter.GetComponent<TextMeshProUGUI>();
        _years = yearsCounter.GetComponent<TextMeshProUGUI>();
        _ap = actionCounter.GetComponent<TextMeshProUGUI>();
        currentLevel.Value = 0;
        years.Value = 0;      
    }

    private void Start() {
        startLevel();
        setHazard();
        actionPoints.Value = level[currentLevel.Value].maxActionPoints;
        _turns.SetText("Turns Remaining: {0}", GameManager.current.level[currentLevel.Value].turns - currentTurn.Value);
        _ap.SetText("Action Points Remaining: {0}", actionPoints.Value);
    }

    void Update () {
        if(_playerTurn.Value) {
            // Then the LMB is clicked, raycast to check what has been hit.
            if(Input.GetMouseButtonDown(0)) {
                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                // Move the selected human to that tile.
                if(hit && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                    Node tile = hitInfo.transform.gameObject.GetComponent<Node>();
                    if(!_selected) return;
                    if(tile.type == NodeType.Building) {
                        Debug.Log("Invalid Tile Selected");
                        return;
                    }
                    Vector3Int diff = Vector3Int.RoundToInt(_selected.transform.position) - tile.pos;
                    int distance = Mathf.Abs(diff.x) + Mathf.Abs(diff.z);
                    if((actionPoints.Value - distance) < 0) {
                        Debug.Log("Exceeding number of moves!");
                    } else {
                        actionPoints.Value -= distance;
                        _ap.SetText("Action Points Remaining: {0}", actionPoints.Value);
                        _selected.moveHuman(new Vector3(tile.pos.x, 0.6f, tile.pos.z));
                    }
                // Select the human that was clicked.
                } else if(hit && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Human")) {
                    if(!_selected) _selected = hitInfo.transform.gameObject.GetComponent<HumanController>();
                    if(_selected == hitInfo.transform.gameObject) return;
                    selectNewObject(hitInfo);
                } else {
                    Debug.Log("Not it Chief");
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape)) Pause();
    }

    public void Pause() {
        gamePaused = !gamePaused;
        pauseMenu.SetActive(gamePaused);
        if(gamePaused) {
            Time.timeScale = 0.0f;
            Time.fixedDeltaTime = 0f * Time.timeScale;
        } else {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
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
            dice = Random.Range(0,6);
            int hazardType = Random.Range(0,2);
            Vector2Int tilePos;
            // 2/3 chance to spawn a hazard on top of the protected humans.
            if(dice < 3) {
                tilePos = getTileStart();
            } else {
                Debug.Log("Got you now!");
                Transform target= _protectedHumans[Random.Range(0,_protectedHumans.Count)].transform;
                tilePos = new Vector2Int((int)target.position.x, (int)target.position.z);
            }
            GameObject hazard = null;
            // Checking if this type of hazard has exceeded the maximum.
            // if(level[currentLevel.Value].maxHazardType[hazardType] > )
            switch(hazardType) {
                case 0: // Regular 1X1 Tile
                    hazard = Instantiate(hazardTile, new Vector3(tilePos.x, 0.01f, tilePos.y), Quaternion.Euler(90, 0, 0));
                    break;
                case 1: // Long 2x1 Tile
                    neighbors = getSurroundingTiles(tilePos);
                    int tileIndex = Random.Range(0,neighbors.Count);
                    Debug.Log(neighbors[tileIndex]-tilePos);
                    hazard = Instantiate(hazardTile, new Vector3(tilePos.x, 0.01f, tilePos.y), Quaternion.Euler(90, 0, 0));
                    Instantiate(hazardTile, new Vector3(neighbors[tileIndex].x, 0.01f, neighbors[tileIndex].y), Quaternion.Euler(90, 0, 0), hazard.transform);
                    break;
                case 2: // Big 2x2 Tile
                    List<Vector2Int> square = new List<Vector2Int>();
                    neighbors = getSurroundingTiles(tilePos);
                    if(neighbors.Count < 2) {

                    }
                    Debug.Log("BigTile!");
                    break;
            }
            // Adds the newly created hazard to the list.
            activeHazards.Add(hazard);
        }
    }

    void setHuman() {
        _protectedHumans.Clear();
        for (int i = 0; i < level[currentLevel.Value].humans; i++) {
            Vector2Int tilePos = getTileStart();
            dice = Random.Range(0,6);
            if(_protectedHumans.Count <= level[currentLevel.Value].maxProtectedHumans) {
                int dice = Random.Range(0,6);
                if(dice < 3) {
                    GameObject human = Instantiate(protectedHuman, new Vector3(tilePos.x, 0.01f, tilePos.y), Quaternion.identity);
                    _protectedHumans.Add(human);
                } else {
                    Instantiate(regularHuman, new Vector3(tilePos.x, 0.01f, tilePos.y), Quaternion.identity);
                }
            }
        }
    }

    void clearHumans() {
        GameObject[] humans = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject human in humans) {
            Destroy(human);
        }
        Debug.Log("All Humans Cleared!");
    }

    // Looks for a walkable tile to set the initial position.
    private Vector2Int getTileStart() {
        // Set a random number from 0 to the length of the X and Y of graph[,]
        Vector2Int tilePos = new Vector2Int(Random.Range(0,GridManager.current.graph.GetLength(0)), Random.Range(0,GridManager.current.graph.GetLength(1)));
        // If a non walkable tile exists in these coordinates, generate them again.
        while(GridManager.current.walkable[tilePos.x,tilePos.y] == false) {
            tilePos = new Vector2Int(Random.Range(0,GridManager.current.graph.GetLength(0)), Random.Range(0,GridManager.current.graph.GetLength(1)));
        }
        return tilePos;
    }

    // Returns a list of positions of the tiles around a given position.
    private List<Vector2Int> getSurroundingTiles(Vector2Int startPos) {
        List<Vector2Int> availableTiles = new List<Vector2Int>();
        try {
            if(GridManager.current.walkable[startPos.x,startPos.y-1]) availableTiles.Add(new Vector2Int(startPos.x,startPos.y-1));
        } catch (System.IndexOutOfRangeException) {
            Debug.Log("Out of bounds");
        }
        try {
            if(GridManager.current.walkable[startPos.x,startPos.y+1]) availableTiles.Add(new Vector2Int(startPos.x,startPos.y+1));
        } catch (System.IndexOutOfRangeException) {
            Debug.Log("Out of bounds");
        }
        try {
            if(GridManager.current.walkable[startPos.x-1,startPos.y]) availableTiles.Add(new Vector2Int(startPos.x-1,startPos.y));
        } catch (System.IndexOutOfRangeException) {
            Debug.Log("Out of bounds");
        }
        try {
            if(GridManager.current.walkable[startPos.x+1,startPos.y]) availableTiles.Add(new Vector2Int(startPos.x+1,startPos.y));
        } catch (System.IndexOutOfRangeException) {
            Debug.Log("Out of bounds");
        }
        return availableTiles;
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
        if (currentTurn.Value > level[currentLevel.Value].turns) {
            currentLevel.Value++;
            startLevel();
        }
        actionPoints.Value = level[currentLevel.Value].maxActionPoints;
        _playerTurn.Value = true;
        _turns.SetText("Turns Remaining: {0}", level[currentLevel.Value].turns - currentTurn.Value);
        _years.SetText("Years Collected: {0}", years.Value);
        _ap.SetText("Action Points Remaining: {0}", actionPoints.Value);
        setHazard();
    }

    public void startLevel() {
        // Resets the turn counter.
        currentTurn.Value = 0;
        // Resets the map
        GridManager.current.clearMap();
        GridManager.current.Initialize();
        // Resets the list of protected humans.
        clearHumans();
        setHuman();
        actionPoints.Value = level[currentLevel.Value].maxActionPoints;
        _turns.SetText("Turns Remaining: {0}", GameManager.current.level[currentLevel.Value].turns - currentTurn.Value);
        _ap.SetText("Action Points Remaining: {0}", actionPoints.Value);
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
