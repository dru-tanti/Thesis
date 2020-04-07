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
        this.state = State.Hover;
    }

    void OnMouseExit() {
        this.state = State.Unselected;
    }

    private void Update() {
        moveX = Input.GetAxisRaw("Horizontal");
        moveY = Input.GetAxisRaw("Vertical");

        Debug.Log(transform.position.x);
        if(Input.GetMouseButtonDown(0)) {
            RaycastHit hitInfo = new RaycastHit();
            bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
            if(hit && hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Human")) {
                hitInfo.transform.gameObject.GetComponent<HumanController>().state = State.Selected;
            } else {
                Debug.Log("Not it Chief");
            }
        }
        
        if(state == State.Selected) {
            m_Renderer.material.color = Color.red;
        } else {
            m_Renderer.material.color = Color.blue;
        }
    }
}
