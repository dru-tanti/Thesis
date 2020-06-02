using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityAtoms.BaseAtoms;
using TMPro;
using GlobalEnums;

// [System.Serializable]
// public class NameList {
//     public List<string> names = new List<string>();
//     public List<string> surnames = new List<string>();
// }

public class GameManager : MonoBehaviour {
    
    public static GameManager current;
    public CameraController camera;
    [HideInInspector] public HumanController _selected;
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
    public GameObject protectedHuman, regularHuman, deadHuman;
    public NameList list;

    [Header("UI Settings")]
    public LifeForce yearsCounter;
    public Color safe, danger;
    public bool gamePaused, gameOver = false;
    public GameObject pauseMenu, gameoverScreen,turnCounter, listText, hoverText, actionCounter;
    private TextMeshProUGUI _turns, _years, _ap, _name, _age, _description, _list, _gameOverText;

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
        _ap = actionCounter.GetComponent<TextMeshProUGUI>();
        _list = listText.GetComponent<TextMeshProUGUI>();
        _gameOverText = gameoverScreen.GetComponentInChildren<TextMeshProUGUI>();
    }

    private void Start() {
        startLevel();
        currentLevel.Value = 0;
        years.Value = 0;
        actionPoints.Value = level[currentLevel.Value].maxActionPoints;
        _turns.SetText("Turns Remaining: {0}", GameManager.current.level[currentLevel.Value].turns - currentTurn.Value);
        _ap.SetText("Moves Remaining: {0}", actionPoints.Value);
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
                    if(GridManager.current.graph[tile.pos.x,tile.pos.z].occupied) {
                        Debug.Log("There is already a human on this tile.");
                        return;
                    }
                    // Retrieves the distance using the A* Pathfinding project.
                    int distance = GridManager.current.findDistance(_selected.transform.position, tile.pos);
                    if((actionPoints.Value - distance) < 0) {
                        Debug.Log("Exceeding number of moves!");
                    } else {
                        actionPoints.Value -= distance;
                        _ap.SetText("Moves Remaining: {0}", actionPoints.Value);
                        // Sets the current tile to unoccupied and sets the new tile as occipied.
                        GridManager.current.graph[(int)_selected.transform.position.x,(int)_selected.transform.position.z].occupied = false;
                        _selected.moveHuman(new Vector3(tile.pos.x, 0.6f, tile.pos.z));
                        GridManager.current.graph[tile.pos.x,tile.pos.z].occupied = true;
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
        if(Input.GetKeyDown(KeyCode.Escape) && !gameOver) Pause();
        if(gameOver) StopAllCoroutines();
    }

    // Unselects the current human, and selects the new one.
    private void selectNewObject(RaycastHit hitInfo) {
        _selected.state = HumanState.Unselected;
        _selected = hitInfo.transform.gameObject.GetComponent<HumanController>();
        _selected.state = HumanState.Selected;
    }

    // Selects a random position and places a Hazard tile.
    private void setHazard() {
        for (int i = 0; i < Random.Range(level[currentLevel.Value].minHazards, level[currentLevel.Value].maxHazards); i++) {
            dice = Random.Range(0,6);
            int hazardType = Random.Range(0,2);
            Vector2Int tilePos;
            // 2/3 chance to spawn a hazard on top of the protected humans.
            if(dice < 3) {
                tilePos = getTileStart();
                while(GridManager.current.graph[tilePos.x, tilePos.y].hazardous) {
                    tilePos = getTileStart();
                }
            } else {
                Transform target= _protectedHumans[Random.Range(0,_protectedHumans.Count)].transform;
                tilePos = new Vector2Int((int)target.position.x, (int)target.position.z);
                while(GridManager.current.graph[tilePos.x, tilePos.y].hazardous) {
                    tilePos = getTileStart();
                }
            }
            GameObject hazard = null;
            // Checking if this type of hazard has exceeded the maximum.
            // if(level[currentLevel.Value].maxHazardType[hazardType] > )
            switch(hazardType) {
                case 0: // Regular 1X1 Tile
                    hazard = Instantiate(hazardTile, new Vector3(tilePos.x, 0.01f, tilePos.y), Quaternion.Euler(90, 0, 0));
                    GridManager.current.graph[tilePos.x,tilePos.y].hazardous = true;
                    break;
                case 1: // Long 2x1 Tile
                    neighbors = getSurroundingTiles(tilePos);
                    int tileIndex = Random.Range(0,neighbors.Count);
                    hazard = Instantiate(hazardTile, new Vector3(tilePos.x, 0.01f, tilePos.y), Quaternion.Euler(90, 0, 0));
                    Instantiate(hazardTile, new Vector3(neighbors[tileIndex].x, 0.01f, neighbors[tileIndex].y), Quaternion.Euler(90, 0, 0), hazard.transform);
                    GridManager.current.graph[tilePos.x,tilePos.y].hazardous = true;
                    // GridManager.current.graph[tilePos.x,tilePos.y].hazardous = true;
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
    
    // Instantiates the Humans and sets their positions
    void setHuman() {
        _protectedHumans.Clear();
        for (int i = 0; i < level[currentLevel.Value].humans; i++) {
            Vector2Int tilePos = getTileStart();
            // If the starting tile is already occupied, then find another tile
            while(GridManager.current.graph[tilePos.x, tilePos.y].occupied) {
                tilePos = getTileStart();
            }
            dice = Random.Range(0,6);
            if(_protectedHumans.Count <= level[currentLevel.Value].maxProtectedHumans) {
                int dice = Random.Range(0,6);
                GridManager.current.graph[tilePos.x, tilePos.y].occupied = true;
                if(dice < 3) {
                    GameObject human = Instantiate(protectedHuman, new Vector3(tilePos.x, 0.6f, tilePos.y), Quaternion.identity);
                    _protectedHumans.Add(human);
                    human.name = list.names[Random.Range(0,list.names.Length)]+" "+list.surnames[Random.Range(0,list.surnames.Length)];
                    human.GetComponent<HumanController>().traits = new List<string>();
                    for (int t = 0; t < Random.Range(2,4); t++) {
                        string selectedTrait = list.traits[Random.Range(0,list.traits.Length)];
                        while(human.GetComponent<HumanController>().traits.Contains(selectedTrait)) {
                            selectedTrait = list.traits[Random.Range(0,list.traits.Length)];
                        }
                        human.GetComponent<HumanController>().traits.Add(selectedTrait);
                    } 
                } else {
                    GameObject human = Instantiate(regularHuman, new Vector3(tilePos.x, 0.6f, tilePos.y), Quaternion.identity);
                    human.name = list.names[Random.Range(0,list.names.Length)]+" "+list.surnames[Random.Range(0,list.surnames.Length)];
                    // human.GetComponent<HumanController>().traits = new string[numTraits];
                    human.GetComponent<HumanController>().traits = new List<string>();
                    for (int t = 0; t < Random.Range(2,4); t++) {
                        string selectedTrait = list.traits[Random.Range(0,list.traits.Length)];
                        while(human.GetComponent<HumanController>().traits.Contains(selectedTrait)) {
                            selectedTrait = list.traits[Random.Range(0,list.traits.Length)];
                        }
                        human.GetComponent<HumanController>().traits.Add(selectedTrait);
                    }
                    // for (int t = 0; t < numTraits; t++) {
                    //     human.GetComponent<HumanController>().traits[t] = list.traits[Random.Range(0,list.traits.Length)];
                    // }
                }
            }
        }
    }

    // Destroys any active humans to reset the level.
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
        if(_selected) {
            _selected.state = HumanState.Unselected;
            _selected = null;
        }
        // Loops through the hazards and checks if a human is standing on top of it.
        StartCoroutine(triggerHazards());        
    }

    public void startLevel() {
        // Resets the turn counter.
        activeHazards.Clear();
        years.Value = 0;
        currentTurn.Value = 0;
        gameOver = false;
        gameoverScreen.SetActive(false);
        // Resets the map
        GridManager.current.clearMap();
        GridManager.current.Initialize();
        // Resets the list of protected humans.
        clearHumans();
        setHuman();
        actionPoints.Value = level[currentLevel.Value].maxActionPoints;
        _turns.SetText("Turns Remaining: {0}", GameManager.current.level[currentLevel.Value].turns - currentTurn.Value);
        _ap.SetText("Moves Remaining: {0}", actionPoints.Value);
        yearsCounter.SetValue(years.Value);
        // _years.SetText("Years Collected: {0}", years.Value);
        // string nameList = "";
        // for (int i = 0; i < _protectedHumans.Count; i++) {
        //     nameList += _protectedHumans[i].name+"\n";
        // }
        // _list.SetText(nameList);
        setHazard();
        updateList();
        _playerTurn.Value = true;
        AudioManager.current.Play("Ambient");
    }

    // Set the text of the hover box, and display it.
    public void showText(string name, int age, string description) {
        hoverText.transform.GetChild(0).GetComponent<TextMeshProUGUI>().SetText(name);
        hoverText.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText("Age: {0}", age);
        hoverText.transform.GetChild(2).GetComponent<TextMeshProUGUI>().SetText(description);
        hoverText.SetActive(true);
    }
    // Hides the text of the hover box.
    public void hideText() {
        hoverText.SetActive(false);
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

    public void quitGame() {
        Application.Quit();
    }

    public IEnumerator triggerHazards() {
        foreach(GameObject hazard in activeHazards) {       
            // Move the camera to every hazard before executing the rest of the code.
            camera.newPosition = new Vector3(hazard.transform.position.x,0f, hazard.transform.position.z);
            RaycastHit hitInfo = new RaycastHit();
            yield return new WaitForSeconds(1.5f);
            GridManager.current.graph[(int)hazard.transform.position.x,(int)hazard.transform.position.z].hazardous = false;
            if (Physics.Raycast(hazard.transform.position, transform.TransformDirection(Vector3.up), out hitInfo, 20.0f, LayerMask.GetMask("Human"))) {
                destroyHuman(hitInfo, hazard);
                if(hazard.transform.childCount > 0) {
                    if(Physics.Raycast(hazard.transform.GetChild(0).position, transform.GetChild(0).TransformDirection(Vector3.up), out hitInfo, 20.0f, LayerMask.GetMask("Human"))) {
                        destroyHuman(hitInfo, hazard);
                    }
                }
                yield return new WaitForSeconds(1f);
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
            if(years.Value < 500) {
                setGameOver("You didn't collect enough life force. Looks like you have more work to do...");
            } else {
                setGameOver("Well done, you managed to pay me back. Hope the others don't find out...");
            }
            startLevel();
        }
        // Reset the values for the turn and spawn new hazards.
        actionPoints.Value = level[currentLevel.Value].maxActionPoints;
        _playerTurn.Value = true;
        _turns.SetText("Turns Remaining: {0}", level[currentLevel.Value].turns - currentTurn.Value);
        // _years.SetText("Years Collected: {0}", years.Value);
        _ap.SetText("Moves Remaining: {0}", actionPoints.Value);
        setHazard();
        updateList();
    }

    public void setGameOver(string gameOverText) {
        gameoverScreen.SetActive(true);
        _gameOverText.SetText(gameOverText);
        gamePaused = true;
        gameOver = true;
    }

    public void destroyHuman(RaycastHit hitInfo, GameObject hazard) {
        bool protectedHumanKilled = false;
        Vector3 humanPos = hitInfo.transform.position;
        if(hitInfo.transform.gameObject.GetComponent<HumanController>()._protected) protectedHumanKilled = true;
        Destroy(hitInfo.transform.gameObject);
        AudioManager.current.Play("Kill");
        yearsCounter.SetValue(years.Value);
        // If a human is killed, spawn a broken version of the human
        GameObject dead = Instantiate(deadHuman, humanPos, Quaternion.identity);
        Rigidbody[] shards = dead.GetComponentsInChildren<Rigidbody>();
        // Apply a force to the broken pieces in a random direction
        foreach(Rigidbody shard in shards) {
            shard.velocity = new Vector3(Random.Range(-5, 5), Random.Range(2, 5), Random.Range(-5, 5));
        }
        Destroy(dead, 2f);
        if(protectedHumanKilled) {
            Debug.LogError("Protected Human Killed!");
            AudioManager.current.Play("GameOver");
            setGameOver("One of the humans you were meant to protect died. Can't pay me back if the others find out...");
        }
    }

    public void updateList() {
        for (int i = 0; i < _protectedHumans.Count; i++) {
            TextMeshProUGUI lineText = listText.transform.GetChild(i).GetComponent<TextMeshProUGUI>();
            lineText.SetText(_protectedHumans[i].name);
            if(GridManager.current.graph[(int)_protectedHumans[i].transform.position.x,(int)_protectedHumans[i].transform.position.z].hazardous) {
                lineText.fontStyle = FontStyles.Bold;
                lineText.color = danger;
            } else {
                lineText.fontStyle = FontStyles.Normal;
                lineText.color = safe;
            }
        }
    }
}