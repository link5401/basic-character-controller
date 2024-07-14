using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerControls), typeof(CharacterController))]
public class PlayerMovement : AnimationBrain
{
    // Start is called before the first frame update
    private CharacterController m_CharacterController;
    private PlayerControls m_PlayerControls;
    public float m_MovementSpeed = 2f;
    public float m_RunMultiplier = 2f;
    public Transform m_MainCameraTransform;

    //Variables received from callbacks
    //movement
    public Vector3 m_AppliedMovement;

    void Awake()
    {
        m_PlayerControls = GetComponent<PlayerControls>();
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
        m_MainCameraTransform = GameObject.Find("Main Camera").transform;
        Cursor.lockState = CursorLockMode.Locked;
        StartCoroutine(MoveUpdate());
    }
    void Update()
    {
        //GravityUpdate();
        //JumpUpdate();
    }




    /// <summary>
    /// The MoveUpdate function updates the movement of an object based on user input and applies
    /// animations.
    /// </summary>
    private IEnumerator MoveUpdate()
    {
        while (true)
        {
            Vector3 movementVector = HandleMovementPhysics();

            if (m_PlayerControls.IsMovementPressed)
            {
                float speedMultiplier = RunUpdate();
                Move(speedMultiplier);
            }
            else
            {
                m_PlayerControls.CurrentMovement = Vector3.zero;
                yield return DetermineMovementAnimationState(Vector3.zero);
            }
            yield return DetermineMovementAnimationState(movementVector);

        }
    }
    private float RunUpdate()
    {
        float speedMultiplier = 1.0f;
        if (m_PlayerControls.isRunPressed && m_AppliedMovement.z != -1)
        {
            m_AppliedMovement *= m_RunMultiplier;
            speedMultiplier = m_RunMultiplier;
        }
        return speedMultiplier;
    }
    private Vector3 HandleMovementPhysics()
    {
        m_AppliedMovement = m_PlayerControls.CurrentMovement;
        return m_AppliedMovement;
    }
    private void Move(float speed)
    {
        transform.rotation = Quaternion.Euler(0f, m_MainCameraTransform.eulerAngles.y, 0f);
        float targetAngle = Mathf.Atan2(m_AppliedMovement.x, m_AppliedMovement.z) * Mathf.Rad2Deg + m_MainCameraTransform.eulerAngles.y;
        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        m_CharacterController.Move(moveDir * m_MovementSpeed * speed * Time.deltaTime);
    }
    /// <summary>
    ///         x
    ///         1
    ///         |
    ///         |
    ///-1 - - - 0 - - - 1 z
    ///         |
    ///         |
    ///         -1
    /// </summary>
    /// <param name="movement">Player input movement vector</param>
    private IEnumerator DetermineMovementAnimationState(Vector3 movement)
    {
        //if (m_PlayerControls.IsMovementPressed && m_CurrentAnimationState == PlayerAnimation.movement.Idle)
        //{
        //    yield return new WaitUntil(() => IsAnimationDone(m_CurrentAnimationState));
        //}
        if (movement.x > 0f && movement.z == 0f)
        {
            ChangeAnimationState(PlayerAnimation.movement.WalkRight);
        }
        else if (movement.x < 0f && movement.z == 0f)
        {
            ChangeAnimationState(PlayerAnimation.movement.WalkLeft);
        }
        else if (movement.x == 0f && movement.z > 0f)
        {
            ChangeAnimationState(PlayerAnimation.movement.WalkFront);
        }
        else if (movement.x == 0f && movement.z < 0f)
        {
            ChangeAnimationState(PlayerAnimation.movement.WalkBack);
        }
        else
        {
            ChangeAnimationState(PlayerAnimation.movement.Idle);
        }
        yield return null;
    }
}
