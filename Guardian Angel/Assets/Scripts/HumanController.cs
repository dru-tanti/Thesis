using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using GlobalEnums;

public class HumanController : MonoBehaviour {
    public Vector2Int gridPos;
    public HumanData human;
    public HumanState state;
    public List<Node> path = null;
    public IntVariable years;
    private MeshRenderer _mesh;
    
    private void Awake() {
        gridPos = new Vector2Int((int) transform.position.x, (int) transform.position.z);
        _mesh = GetComponent<MeshRenderer>();
        human = (HumanData) ScriptableObject.CreateInstance("HumanData");
        this.state = HumanState.Unselected;
    }
    public void MoveNextTile() {
        if(path.Count == 0) return;
    }
    
    void OnMouseOver() {
        if(this.state != HumanState.Selected) this.state = HumanState.Hover;
    }

    void OnMouseExit() {
        if(this.state != HumanState.Selected) this.state = HumanState.Unselected;
    }

    private void Update() {
        // Change the colour of the human if it is selected.
        // NOTE: FOR TESTING PURPOSES.
        if(state == HumanState.Selected) {
            _mesh.material.color = Color.red;
        } else {
            _mesh.material.color = Color.blue;
        }
        if(state == HumanState.Hover) {
            _mesh.material.color = Color.cyan;
        }
    }
    
    // If the current object is selected move it to the designated coordinates
    public void moveHuman(Vector3Int pos) {
        if(this.state != HumanState.Selected) return;
        this.transform.position = pos;
    }

    private void OnDestroy() {
        years.Value += human.lifeExpectancy - human.age;
    }
}
