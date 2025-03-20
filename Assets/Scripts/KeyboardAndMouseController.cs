using UnityEngine;
using UnityEngine.InputSystem;

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
    private PlayerActionsAutoGenerated _playerActions;
    private Transform myTransform;
    private Transform cameraTransform;
    private float _xRotation;
    private float _yRotation;
    private Vector3 _moveDirection;
    private bool pauseMovement;



    [SerializeField] 
    private float lookSensitivity;
        
    private void Awake()
    {
        //instantiate our actions class so we can access the input system
        _playerActions = new PlayerActionsAutoGenerated();

        _playerActions.Player.Interact.performed += OnInteract;
        myTransform = transform;
        cameraTransform = mainCamera.transform;

    }
    
    void OnEnable()
    {
        _playerActions.Player.Enable();
        // Lock and disable cursor in non-mobile mode
        if (!WebGLPlatformChecker.IsWebGLMobile())
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
    void OnDisable()
    {
        _playerActions.Player.Disable();
        // Unlock and enable cursor in non-mobile mode
        if (!WebGLPlatformChecker.IsWebGLMobile())
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    
    public void Update()
    {
        HandleMovement();
        HandleLook();        
    }
    
    void OnInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interact!");
    }


    private Vector3 velocity = Vector3.zero;

    private void HandleLook()
    {
        Vector2 lookVector = _playerActions.Player.Look.ReadValue<Vector2>();

        
        _yRotation += lookVector.x * lookSensitivity;
        
        //The xrotation is subtracted to prevent inverted looking
        _xRotation -= lookVector.y * lookSensitivity;

        //Prevent the player from looking too far up or down
        _xRotation = Mathf.Clamp(_xRotation, -90.0f, 90.0f);
       
        //Add the rotation to the camera for up and down, and to the player to spin around
        cameraTransform.localRotation = Quaternion.Euler(_xRotation,0,0);
        myTransform.rotation = Quaternion.Euler(0,_yRotation+180,0);
    }

    public void PauseMovementForAudio(float _pauseTime)
    {
        pauseMovement = true;
        Invoke("UnPauseMovement", _pauseTime);
    }
    void UnPauseMovement()
    {
        pauseMovement = false;
    }
    void HandleMovement(){
        // Get keyboard input values
        if(pauseMovement)
            return;
        Vector2 moveVector = _playerActions.Player.Move.ReadValue<Vector2>();
        _moveDirection = new Vector3(moveVector.x, 0, moveVector.y);
    
        // Apply keyboard movement
        myTransform.Translate(Vector3.forward * (_moveDirection.z * Time.deltaTime * keyboardForwardSpeed));
        myTransform.Translate(Vector3.right * (_moveDirection.x * Time.deltaTime * keyboardHorizontalSpeed));


    }

}