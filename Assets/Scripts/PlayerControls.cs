using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{

    public PlayerInput playerInput;
    public Vector3 currentMovement;
    public bool isMovementPressed;
    public bool isJumpPressed;
    public bool isRunPressed;
    public bool isAttackPressed;

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = new PlayerInput();
        playerInputInit();
    }

    // Update is called once per frame
    void Update()
    {

    }
    /// <summary>
    /// The function "playerInputInit" sets up event handlers for player input related actions such as
    /// moving, running, and jumping.
    /// </summary>
    public void playerInputInit()
    {
        playerInput.CharacterInput.Move.started += OnMove;
        playerInput.CharacterInput.Move.canceled += OnMove;
        playerInput.CharacterInput.Move.performed += OnMove;
        playerInput.CharacterInput.Run.started += OnRun;
        playerInput.CharacterInput.Run.canceled += OnRun;
        playerInput.CharacterInput.Jump.started += OnJump;
        playerInput.CharacterInput.Jump.canceled += OnJump;
        playerInput.CharacterInput.Attack.started += OnAttack;
        playerInput.CharacterInput.Attack.performed += OnAttack;
        playerInput.CharacterInput.Attack.canceled += OnAttack;
    }
    /// <summary>
    /// The function "OnMove" reads input values for movement and updates the current movement vector based
    /// on the input.
    /// </summary>
    /// <param name="context">The context parameter is of type InputAction.CallbackContext. It represents
    /// the context of the input action being performed. It contains information such as the input value,
    /// the phase of the action (e.g. started, performed, canceled), and the time at which the action
    /// occurred.</param>
    void OnMove(InputAction.CallbackContext context)
    {
        Vector2 inputMovement = context.ReadValue<Vector2>();
        isMovementPressed = inputMovement.x != 0 || inputMovement.y != 0;
        currentMovement = new Vector3(inputMovement.x, 0f, inputMovement.y);
    }

    void OnJump(InputAction.CallbackContext context)
    {
        isJumpPressed = context.ReadValueAsButton();
    }

    void OnRun(InputAction.CallbackContext context)
    {
        isRunPressed = context.ReadValueAsButton();
    }

    void OnAttack(InputAction.CallbackContext context){
        isAttackPressed = context.ReadValueAsButton();
    }
    void OnEnable()
    {
        playerInput.CharacterInput.Enable();

    }
    void OnDisable()
    {
        playerInput.CharacterInput.Disable();
    }
}
