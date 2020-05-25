using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public float movementSpeed; 
    public float movementTime;
    public float rotation;
    public Vector3 newPosition;
    public Quaternion newRotation;
    private void Start() {
        newPosition = transform.position;
        newRotation = transform.rotation;
    }

    private void Update() {
        movementInput();
    }

    void movementInput() {
        if(Input.GetAxisRaw("Vertical") == 1) {
            newPosition += transform.forward * movementSpeed; 
        } else if(Input.GetAxisRaw("Vertical") == -1) {
            newPosition += transform.forward * -movementSpeed; 
        }
        if(Input.GetAxisRaw("Horizontal") == 1) {
            newPosition += transform.right * movementSpeed;
        } else if(Input.GetAxisRaw("Horizontal") == -1) {
            newPosition += transform.right * -movementSpeed;
        } 
        if(Input.GetKey(KeyCode.Q)) {
            newRotation *= Quaternion.Euler(Vector3.up * rotation);
        }
        if(Input.GetKey(KeyCode.E)) {
            newRotation *= Quaternion.Euler(Vector3.up * -rotation);
        }
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
    }   
}
