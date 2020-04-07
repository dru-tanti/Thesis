using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum State {Selected, Unselected, Hover, Tired}
public class HumanController : MonoBehaviour {
    public Vector3Int gridPos;
    public Human human;
    public State state;
    public float moveX;
    public float moveY;
    MeshRenderer m_Renderer;    
    
    private void Awake() {
        this.state = State.Unselected;
        m_Renderer = GetComponent<MeshRenderer>();
    }
    
    void OnMouseOver() {
        if(this.state != State.Selected) this.state = State.Hover;
    }

    void OnMouseExit() {
        if(this.state != State.Selected) this.state = State.Unselected;
    }

    private void Update() {
        if(state == State.Selected) {
            m_Renderer.material.color = Color.red;
        } else {
            m_Renderer.material.color = Color.blue;
        }
    }

    public void moveHuman(Vector3Int loc) {
        
    }
}
