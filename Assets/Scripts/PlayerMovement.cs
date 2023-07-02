using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public Animator animator;
    public CharacterController controller;
    private PlayerControls playerControls;
    public float movementSpeed = 10f;
    public float runMultiplier = 3f;
    public Transform cam;
    private Vector3 input;
    //gravity variables
    public float gravity = -9.8f;
    public float groundedGravity = -.05f;
    //jump variables

    //Variables received from callbacks
    //movement
    public Vector3 appliedMovement;
    //run
    public Vector3 currentRunMovement;
    public float initialJumpVelocity;
    public float maxJumpHeight = 4.0f;
    public float maxJumpTime = 0.5f;
    private int isJumpingHash;
    private bool isJumping = false;
    private bool isJumpingAnimating = false;
    void Awake()
    {
        playerControls = GetComponent<PlayerControls>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cam = GameObject.Find("Main Camera").transform;
        Cursor.lockState = CursorLockMode.Locked;
        AnimatorHashVariableInit();
        JumpVariableInit();
    }
    void Update()
    {
        MoveUpdate();
        GravityUpdate();
        JumpUpdate();
    }

    /// <summary>
    /// The function initializes a hash variable for the "isJumping" parameter in an animator.
    /// </summary>
    public void AnimatorHashVariableInit()
    {
        isJumpingHash = Animator.StringToHash("isJumping");
    }




    /// <summary>
    /// The function calculates the gravity and initial jump velocity based on the maximum jump height and
    /// time.
    /// </summary>
    private void JumpVariableInit()
    {
        float timeToApex = maxJumpTime / 2;
        gravity = (-2 * maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        initialJumpVelocity = (2 * maxJumpHeight) / timeToApex;
    }


    /// <summary>
    /// The MoveUpdate function updates the movement of an object based on user input and applies
    /// animations.
    /// </summary>
    private void MoveUpdate()
    {
        bool isAttacking = animator.GetBool("isAttacking");
        float speedMultiplier = 1.0f;
        appliedMovement = playerControls.currentMovement;
        if (playerControls.isRunPressed && appliedMovement.z != -1)
        {
            appliedMovement *= runMultiplier;
            speedMultiplier = runMultiplier;
        }
        if (playerControls.isMovementPressed)
        {
            transform.rotation = Quaternion.Euler(0f, cam.eulerAngles.y, 0f);
            float targetAngle = Mathf.Atan2(appliedMovement.x, appliedMovement.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            if (!isAttacking) controller.Move(moveDir * movementSpeed * speedMultiplier * Time.deltaTime);
        }
        //animation
        if (!isAttacking)
        {
            animator.SetFloat("Horizontal", appliedMovement.x);
            animator.SetFloat("Vertical", appliedMovement.z);
        }
    }



    /// <summary>
    /// The JumpUpdate function checks if the player is grounded and if the jump button is pressed, and
    /// if so, initiates a jump animation and sets the player's vertical movement to the initial jump
    /// velocity.
    /// </summary>
    private void JumpUpdate()
    {
        bool isAttacking = animator.GetBool("isAttacking");

        if (controller.isGrounded && playerControls.isJumpPressed && !isJumping)
        {
            animator.SetBool(isJumpingHash, true);
            isJumpingAnimating = true;
            isJumping = true;
            playerControls.currentMovement.y = initialJumpVelocity;
        }
        else if (isJumping && controller.isGrounded)
        {
            isJumping = false;
        }
        if(!isAttacking)controller.Move(playerControls.currentMovement * Time.deltaTime);

    }

    /// <summary>
    /// The function updates the gravity effect on the character's movement, adjusting the y-axis
    /// movement based on whether the character is grounded, falling, or jumping.
    /// </summary>
    private void GravityUpdate()
    {
        float fallMultiplier = 2.0f;
        bool isFalling = playerControls.currentMovement.y <= 0.0f || !playerControls.isJumpPressed;
        if (controller.isGrounded)
        {
            playerControls.currentMovement.y = groundedGravity;
        }
        else if (isFalling)
        {
            if (isJumpingAnimating)
            {
                animator.SetBool(isJumpingHash, false);
                isJumpingAnimating = false;
            }
            playerControls.currentMovement.y += gravity * Time.deltaTime * fallMultiplier;

        }
        else
        {
            playerControls.currentMovement.y += gravity * Time.deltaTime;
        }
    }
}
