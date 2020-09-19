using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityAtoms.BaseAtoms;
using GlobalEnums;

public class HumanController : MonoBehaviour {
    public HumanData human;
    public HumanState state;
    public List<Node> path = null;
    public IntVariable years;
    private MeshRenderer _mesh;
    public bool _protected;
    public List<string> traits;
    public string description;
    public int yearsToCollect;
    private void Awake() {
        _mesh = GetComponent<MeshRenderer>();
        human = (HumanData) ScriptableObject.CreateInstance("HumanData");
        this.state = HumanState.Unselected; 
    }
    private void Start() {
        yearsToCollect = human.lifeExpectancy - human.age;
        description = "";
        foreach(string trait in traits) {
            description += trait+"\n\n";
        }
    }
    public void MoveNextTile() {
        if(path.Count == 0) return;
    }

    void OnMouseEnter() {
        if(this.state != HumanState.Selected) this.state = HumanState.Hover;
        GameManager.current.showText(this.gameObject.name, human.age, this.description);
    }

    void OnMouseExit() {
        if(this.state != HumanState.Selected) this.state = HumanState.Unselected;
        GameManager.current.hideText();
    }

    private void Update() {
        // Change the colour of the human if it is selected.
        // NOTE: FOR TESTING PURPOSES.
        if(state == HumanState.Selected) {
            _mesh.material.SetColor("_BaseColor",Color.red);
        } else {
            if(_protected) {
                _mesh.material.SetColor("_BaseColor",Color.cyan);
            } else {
                _mesh.material.SetColor("_BaseColor",Color.blue);
            }
        }
        if(state == HumanState.Hover) {
            _mesh.material.SetColor("_BaseColor",Color.yellow);
        }
    }
    
    // If the current object is selected move it to the designated coordinates
    public void moveHuman(Vector3 pos) {
        if(this.state != HumanState.Selected) return;
        this.transform.position = pos;
        GameManager.current.updateList();
        AudioManager.current.Play("Step");
    }

    // private void OnDestroy() {
    //     years.Value += yearsToCollect;
    // }
}
