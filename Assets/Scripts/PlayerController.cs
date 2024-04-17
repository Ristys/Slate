using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private const float _gravity = -9.81f;

    [Header("Mouse-look options")]
    [SerializeField] private float mouseSensitivity = 0.3f;
    [SerializeField] private float minViewDistance = 50f;

    [Header("Movement options")]
    [SerializeField] private float movementSpeed = 5f;
    
    [Header("Jumping options")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private float gravityMultiplier = 0.66f;

    [Header("Crouching options")]
    [SerializeField] private float crouchHeight = 0.5f;
    [SerializeField] private float standHeight = 2f;
    [SerializeField] private float timeToCrouch = 0.25f;
    [SerializeField] private Vector3 characterCenterWhenCrouched = new Vector3(0, 0.5f, 0);
    [SerializeField] private Vector3 characterCenterWhenStanding = new Vector3(0, 0, 0);
    [SerializeField] private bool isCrouched;
    [SerializeField] private bool isCrouching;

    [Header("Selection options")]
    [SerializeField] private float interactionRange = 5f;
    [SerializeField] private LayerMask interactableLayer;
    [SerializeField] private GameObject selectedObject;

    private float fallingVelocity;
    private float mouseX, mouseY;
    private float horizontalInput, verticalInput;
    private Vector3 forwardDirection, rightDirection;
    private Vector3 movementVector;
    private Vector3 currentRotation;
    private float rotationX;

    public Camera playerCamera;
    public Canvas hudCanvas;
    private TextMeshProUGUI hudSelectedText;
    private CharacterController characterController;
    private PlayerInput playerInput;
    private InputActionMap actionMap;
    private InputAction movement, mousePos, interact, crouch, jump;



    private void OnEnable()
    {
        actionMap.Enable();
        movement.Enable();
        mousePos.Enable();
        interact.Enable();
        crouch.Enable();
        jump.Enable();
    }

    private void OnDisable()
    {
        actionMap.Disable();
        movement.Disable();
        mousePos.Disable();
        interact.Disable();
        crouch.Disable();
        jump.Disable();
    }

    private void Awake()
    {
        playerInput = GetComponent<PlayerInput>();

        actionMap = playerInput.actions.FindActionMap("PlayerActions");
        movement = actionMap.FindAction("Movement");
        mousePos = actionMap.FindAction("MousePosition");
        interact = actionMap.FindAction("Interact");
        crouch = actionMap.FindAction("Crouch");
        jump = actionMap.FindAction("Jump");


        if (movement != null)
        {
            movement.performed += MoveAction;
            Debug.Log("Movement action contextualized.");
        }
        else { Debug.LogWarning("Movement action not found!"); }

        //PROBABLY DON'T NEED TO HAVE A METHOD FOR MOUSE POSITION AT ALL TIMES.
        if (mousePos != null)
        {
            Debug.Log("Mouse action contextualized.");
        }
        else { Debug.LogWarning("Mouse action not found!"); }

        if (interact != null)
        {
            interact.performed += Interact;
            Debug.Log("Interact action contextualized.");
        }
        else { Debug.LogWarning("Interact action not found!"); }

        if (crouch != null)
        {
            crouch.performed += Crouch;
            Debug.Log("Crouch action contextualized.");
        }
        else { Debug.LogWarning("Crouch action not found!"); }

        if (jump != null)
        {
            jump.performed += Jump;
            Debug.Log("Jump action contextualized.");
        }
        else { Debug.LogWarning("Jump action not found!"); }

    }

    // Start is called before the first frame update
    void Start()
    {
        isCrouched = false;

        hudSelectedText = hudCanvas.GetComponentInChildren<TextMeshProUGUI>();
        
        characterController = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        CalculateGravity();
        ApplyRotation();
        ProcessMovement();
        ApplyMovement();
        GetSelection();
    }

    private GameObject GetSelection()
    {
        RaycastHit hit;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out hit, interactionRange, interactableLayer))
        {
            if (hit.collider.gameObject.layer == 3)
            {
                Debug.Log("Selecting: " + hit.collider.gameObject.name);
                hudSelectedText.text = hit.collider.gameObject.name;
                return hit.collider.gameObject;
            } else { hudSelectedText.text = ""; }
        }
        hudSelectedText.text = "";
        return null;
    }
    private void CalculateGravity()
    {

        if (characterController.isGrounded && fallingVelocity < 0f)
        {
            fallingVelocity = -1f;
        }
        else
        {
            fallingVelocity += _gravity * gravityMultiplier * Time.deltaTime;
        }
        
    }
    private void ApplyRotation()
    {
        // Get mouse input.
        mouseX = Mouse.current.delta.x.ReadValue() * mouseSensitivity;
        mouseY = Mouse.current.delta.y.ReadValue() * mouseSensitivity;

        // Rotate camera based on mouse input and mouse sensitivity.
        transform.Rotate(Vector3.up * mouseX * mouseSensitivity);

        currentRotation = transform.eulerAngles;
        currentRotation.x -= mouseY * mouseSensitivity;
        transform.eulerAngles = currentRotation;

        rotationX = transform.eulerAngles.x;
        rotationX = (rotationX > 180) ? rotationX - 360 : rotationX;
        rotationX = Mathf.Clamp(rotationX, -90f, minViewDistance);

        transform.eulerAngles = new Vector3(rotationX, transform.eulerAngles.y, 0f);
    }

    private void ProcessMovement()
    {
        // Get keyboard (WASD) input.
        horizontalInput = Keyboard.current.dKey.ReadValue() - Keyboard.current.aKey.ReadValue();
        verticalInput = Keyboard.current.wKey.ReadValue() - Keyboard.current.sKey.ReadValue();

        // Get current direction so movement can be relative to where the player is looking.
        forwardDirection = transform.forward;
        rightDirection = transform.right;

        // Zero the Y value to prevent "flying".
        forwardDirection.y = 0f;
        rightDirection.y = 0f;

        // Normalize the camera-relative vectors.
        forwardDirection.Normalize();
        rightDirection.Normalize();

        // Calculate how to move based on inputs, relative to the camera.
        movementVector = forwardDirection * verticalInput + rightDirection * horizontalInput;


    }

    private void ApplyMovement()
    {
        
        if (!characterController.isGrounded)
        { 
            // Apply the falling velocity
            movementVector.y = fallingVelocity;
        }

        // Apply movement and rotation to the character controller.
        characterController.Move(movementVector * movementSpeed * Time.deltaTime);
        
    }


    /*          *
     * ACTIONS  *
     *          */
    protected void MoveAction(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Move performed.");
        }
    }

    protected void Interact(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (GetSelection() != null) 
            {
                IInteractable interactable = GetSelection().GetComponent<IInteractable>();
                if (interactable != null)
                {
                    interactable.Interact();
                }

            }
            
            Debug.LogWarning("Interact performed, but no selection.");
        }
    }

    protected void Crouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (!isCrouching)
            {
                StartCoroutine(ToggleCrouch());
                Debug.Log("Crouch performed");
            }
            else
            {
                Debug.Log("Crouching ignored because crouching is already in progress.");
            }
        }
    }

    protected void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Vector3 jumped = movementVector;
            jumped.y = jumpForce;
            characterController.Move(jumped);
            Debug.Log("Jump performed.");
        }
    }


    /*             *
     * COROUTINES  *
     *             */
    private IEnumerator ToggleCrouch()
    {
        if (isCrouched && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;

        isCrouching = true;

        float timeElapsed = 0;
        float targetHeight = isCrouched ? standHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouched ? characterCenterWhenStanding : characterCenterWhenCrouched;
        Vector3 currentCenter = characterController.center;

        while (timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed/timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed/timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouched = !isCrouched;

        isCrouching = false;
    }

    /*         *
     * GIZMOS  *
     *         */
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 selectionRay = playerCamera.transform.TransformDirection(Vector3.forward) * interactionRange;
        Gizmos.DrawRay(playerCamera.transform.position, selectionRay);
    }
}
