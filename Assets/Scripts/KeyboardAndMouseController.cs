using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class KeyboardAndMouseController : MonoBehaviour
{
    [SerializeField] private float keyboardForwardSpeed=2;
    [SerializeField] private float keyboardHorizontalSpeed=2;
    [SerializeField] float mouseMovementSpeed = 6f;
    [SerializeField] float smoothingFactor = 0.8f; 
    [SerializeField] float maxUpAngle=40;
    [SerializeField] float maxDownAngle=40;
    private bool initialized=false;
    [SerializeField] Camera mainCamera;
    private Vector2 smoothedMouseMovement;
    private Vector2 mouseMovement;

    public void Update()
    {
        GetKeyboardAndMouseInput();
    }

    private Vector3 velocity = Vector3.zero;

    void GetKeyboardAndMouseInput(){
        // Get keyboard input values
        float horizontalInput = Input.GetAxis("Horizontal");
        float forwardInput = Input.GetAxis("Vertical");

        // Apply keyboard movement
        transform.Translate(Vector3.forward * forwardInput * Time.deltaTime * keyboardForwardSpeed);
        transform.Translate(Vector3.right * horizontalInput * Time.deltaTime * keyboardHorizontalSpeed);

        float mouseXMovement = Input.GetAxis("Mouse X");
        float mouseYMovement = Input.GetAxis("Mouse Y");

        // Apply smoothing
        mouseMovement.x = Mathf.Lerp(mouseMovement.x, mouseXMovement, 1f / smoothingFactor);
        mouseMovement.y = Mathf.Lerp(mouseMovement.y, mouseYMovement, 1f / smoothingFactor);

        smoothedMouseMovement.x = Mathf.Lerp(smoothedMouseMovement.x, mouseMovement.x, smoothingFactor);
        smoothedMouseMovement.y = Mathf.Lerp(smoothedMouseMovement.y, mouseMovement.y, smoothingFactor);

        // Apply horizontal rotation
        transform.Rotate(0, smoothedMouseMovement.x * mouseMovementSpeed, 0);

        // Get User Rotation
        Quaternion userRotation = transform.rotation;

        // Apply vertical rotation to camera
        mainCamera.transform.Rotate(-smoothedMouseMovement.y * mouseMovementSpeed, 0, 0);

        // Limit upward and downward rotation
        Quaternion cameraRotation = mainCamera.transform.rotation;
        Vector3 cameraRotationDegrees = cameraRotation.eulerAngles;

        if(cameraRotationDegrees.x>=180){
            if(cameraRotationDegrees.x<(360-maxUpAngle)){
                mainCamera.transform.rotation = Quaternion.Euler(320f, userRotation.eulerAngles.y, userRotation.eulerAngles.z);
            }
        }
        if(cameraRotationDegrees.x<=180){
            if(cameraRotationDegrees.x>maxDownAngle){
                mainCamera.transform.rotation = Quaternion.Euler(40f, userRotation.eulerAngles.y, userRotation.eulerAngles.z);
            }
        }
         
    }

}