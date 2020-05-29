using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform cameraTransform;
    public float movementSpeed; 
    public float movementTime;
    public float rotation;
    public float zoomSpeedMultiplier = 100;
    public float maxZoom = 400f;
    public float minZoom = 20f;
    public Vector3 newPosition;
    public Vector3 zoomAmount;
    public Quaternion newRotation;
    public Vector3 newZoom;
    private void Start() {
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    private void Update() {
        movementInput();
        mouseInput();
    }

    void mouseInput() {
        if(Input.mouseScrollDelta.y != 0) {
            newZoom += Input.mouseScrollDelta.y * zoomAmount;
            if(newZoom.y > maxZoom) {
                newZoom.y = maxZoom;
                newZoom.z = -maxZoom/2;
            } else if(newZoom.y < minZoom) {
                newZoom.y = minZoom;
                newZoom.z = -minZoom/2;
            }
            movementSpeed = newZoom.y / zoomSpeedMultiplier;
        }
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
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }   
}