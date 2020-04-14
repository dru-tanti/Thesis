using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalEnums;

public class HumanController : MonoBehaviour {
    public Vector3Int gridPos;
    public Human human;
    public HumanState state;
    private MeshRenderer _mesh;    
    
    private void Awake() {
        this.state = HumanState.Unselected;
        _mesh = GetComponent<MeshRenderer>();
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
    }

    public void moveHuman(Vector3Int loc) {
        
    }
}
