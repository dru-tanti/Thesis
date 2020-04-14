using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEnums;
public class GameManager : MonoBehaviour {
    public static GameManager current;
    private HumanController _selected;
    [SerializeField] private GridManager _grid = default;

    private void Awake() {
        if(current == null) {
            current = this;
            DontDestroyOnLoad(gameObject);
        } else {
            DestroyImmediate(gameObject);
            return;
        }
        _grid.Initialize();
    }
    void Update () {
        // Then the LMB is clicked, raycast to check what has been hit.
        if(Input.GetMouseButtonDown(0)) {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if(hit && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Ground")) {
                Node tile = hitInfo.transform.gameObject.GetComponent<Node>();
                if(!_selected) return;
                _selected.moveHuman(tile.pos);
            } else if(hit && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Human")) {
                if(!_selected) _selected = hitInfo.transform.gameObject.GetComponent<HumanController>();
                if(_selected == hitInfo.transform.gameObject) return;
                selectNewObject(hitInfo);
            } else {
                Debug.Log("Not it Chief");
            }
        }
    }
    // Unselects the current human, and selects the new one.
    private void selectNewObject(RaycastHit hitInfo) {
        _selected.state = HumanState.Unselected;
        _selected = hitInfo.transform.gameObject.GetComponent<HumanController>();
        _selected.state = HumanState.Selected;
    }
}
