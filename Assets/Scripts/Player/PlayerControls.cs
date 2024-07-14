using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControls : MonoBehaviour
{

    public PlayerInput PlayerInput;
    public Vector3 CurrentMovement;
    public bool IsMovementPressed;
    public bool isJumpPressed;
    public bool isRunPressed;
    public bool isAttackPressed;

    // Start is called before the first frame update
    void Awake()
    {
        PlayerInput = new PlayerInput();
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
        PlayerInput.CharacterInput.Move.started += OnMove;
        PlayerInput.CharacterInput.Move.canceled += OnMove;
        PlayerInput.CharacterInput.Move.performed += OnMove;
        PlayerInput.CharacterInput.Run.started += OnRun;
        PlayerInput.CharacterInput.Run.canceled += OnRun;
        PlayerInput.CharacterInput.Jump.started += OnJump;
        PlayerInput.CharacterInput.Jump.canceled += OnJump;
        PlayerInput.CharacterInput.Attack.started += OnAttack;
        PlayerInput.CharacterInput.Attack.performed += OnAttack;
        PlayerInput.CharacterInput.Attack.canceled += OnAttack;
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
        IsMovementPressed = inputMovement.x != 0 || inputMovement.y != 0;
        CurrentMovement = new Vector3(inputMovement.x, 0f, inputMovement.y);
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
        PlayerInput.CharacterInput.Enable();

    }
    void OnDisable()
    {
        PlayerInput.CharacterInput.Disable();
    }
}
