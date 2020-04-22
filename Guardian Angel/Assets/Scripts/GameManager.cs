using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using GlobalEnums;
public class GameManager : MonoBehaviour {
    public static GameManager current;
    private HumanController _selected;
    public LevelSettings level; 
    public List<GameObject> activeHazards;
    public GameObject hazardTile;
    public IntVariable years;
    public BoolVariable _playerTurn;
    private void Awake() {
        if(current == null) {
            current = this;
            DontDestroyOnLoad(gameObject);
        } else {
            DestroyImmediate(gameObject);
            return;
        }
        GridManager.current.Initialize();
        setHazard();
        years.Value = 0;
    }
    void Update () {
        // Only allow player input if it is the players turn.
        if(Input.GetKeyDown(KeyCode.W)) {
            _playerTurn.Value = false;
        }
        if(_playerTurn.Value) {
            // Then the LMB is clicked, raycast to check what has been hit.
            if(Input.GetMouseButtonDown(0)) {
                RaycastHit hitInfo = new RaycastHit();
                bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                if(hit && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                    Node tile = hitInfo.transform.gameObject.GetComponent<Node>();
                    if(!_selected) return;
                    _selected.moveHuman(new Vector3Int(tile.pos.x, 1, tile.pos.z));
                } else if(hit && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Human")) {
                    if(!_selected) _selected = hitInfo.transform.gameObject.GetComponent<HumanController>();
                    if(_selected == hitInfo.transform.gameObject) return;
                    selectNewObject(hitInfo);
                } else {
                    Debug.Log("Not it Chief");
                }
            }
        } else {
            foreach(GameObject hazard in activeHazards) {
                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(hazard.transform.position, transform.TransformDirection(Vector3.up), out hitInfo, 20.0f, LayerMask.GetMask("Human"))) {
                    Destroy(hitInfo.transform.gameObject);
                    Debug.Log("Fired and hit someone");
                    Destroy(hazard);
                } else {
                    Debug.Log("nope");
                    Destroy(hazard);
                }
            }
            activeHazards.Clear();
            _playerTurn.Value = true;
            setHazard();
        }
    }
    // Unselects the current human, and selects the new one.
    private void selectNewObject(RaycastHit hitInfo) {
        _selected.state = HumanState.Unselected;
        _selected = hitInfo.transform.gameObject.GetComponent<HumanController>();
        _selected.state = HumanState.Selected;
    }

    private void setHazard() {
        for (int i = 0; i < Random.Range(1, level.maxHazards); i++) {
            GameObject hazard = Instantiate(hazardTile, new Vector3(Random.Range(0, 10), 0.01f, Random.Range(0, 10)), Quaternion.Euler(90, 0, 0));
            activeHazards.Add(hazard);    
        }
    }
}
