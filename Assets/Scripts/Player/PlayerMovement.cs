using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(PlayerControls), typeof(CharacterController), typeof(Rigidbody))]
public class PlayerMovement : AnimationBrain
{
    // Start is called before the first frame update
    private CharacterController m_CharacterController;
    private PlayerControls m_PlayerControls;
    private Rigidbody m_Rigidbody;
    public float m_MovementSpeed = 2f;
    public float m_RunMultiplier = 2f;
    public Transform m_MainCameraTransform;
    public Vector3 m_AppliedMovement;
    public bool IsJumping = false;
    private float m_JumpHeight = 2.0f;
    private float m_GravityValue = -9.81f;
    void Awake()
    {
        m_PlayerControls = GetComponent<PlayerControls>();
        m_CharacterController = GetComponent<CharacterController>();
        m_Animator = GetComponent<Animator>();
        m_Rigidbody = GetComponent<Rigidbody>();
        m_MainCameraTransform = GameObject.Find("Main Camera").transform;
        Cursor.lockState = CursorLockMode.Locked;
    }
    void Update()
    {
        AnimationUpdate();
    }
    void FixedUpdate()
    {
        JumpUpdate();
        MoveUpdate();
    }
    /// <summary>
    /// The MoveUpdate function updates the movement of an object based on user input and applies
    /// animations.
    /// </summary>
    private void MoveUpdate()
    {
        HandleMovementPhysics();
        if (m_PlayerControls.IsMovementPressed && !IsJumping && !IsInJumpAnimation())
        {
            float speedMultiplier = RunUpdate();
            Move(speedMultiplier);
        }
        else
        {
            m_PlayerControls.CurrentMovement = Vector3.zero;
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
    private float m_JumpingElapsed = 0.0f;
    private bool m_IsJumpStarting = false;
    private void JumpUpdate()
    {
        if (IsJumping)
        {
            m_JumpingElapsed += Time.deltaTime;
            m_AppliedMovement.y += m_GravityValue * Time.deltaTime;
            m_AppliedMovement.y += Mathf.Sqrt(m_JumpHeight * -3.0f * m_GravityValue);
            m_CharacterController.Move(m_AppliedMovement * Time.deltaTime);
            return;
        }
        if (m_PlayerControls.isJumpPressed && m_CharacterController.isGrounded && !IsJumping && !m_IsJumpStarting)
        {
            m_IsJumpStarting = true;
            ChangeAnimationState(PlayerAnimation.movement.JumpStart);
        }
        else if (m_CurrentAnimationState == PlayerAnimation.movement.JumpAir)
        {
            m_IsJumpStarting = false;
            IsJumping = true;
        }
    }
    private Vector3 HandleMovementPhysics()
    {
        m_AppliedMovement = m_PlayerControls.CurrentMovement;
        return m_AppliedMovement;
    }
    private bool IsInJumpAnimation()
    {
        return m_CurrentAnimationState == PlayerAnimation.movement.JumpAir
            || m_CurrentAnimationState == PlayerAnimation.movement.JumpStart
            || m_CurrentAnimationState == PlayerAnimation.movement.JumpEnd;
    }

    private void Move(float speed, bool isLockedOn = false)
    {
        if (!m_CharacterController.isGrounded || IsJumping || IsInJumpAnimation()) return;
        Vector3 moveDir = Vector3.zero;
        if (isLockedOn)
        {
            transform.rotation = Quaternion.Euler(0f, m_MainCameraTransform.eulerAngles.y, 0f);
            float targetAngle = Mathf.Atan2(m_AppliedMovement.x, m_AppliedMovement.z) * Mathf.Rad2Deg + m_MainCameraTransform.eulerAngles.y;
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        else
        {
            Quaternion movementRotation = Quaternion.LookRotation(m_AppliedMovement);
            Quaternion cameraRotation = Quaternion.Euler(0f, m_MainCameraTransform.eulerAngles.y, 0f);
            Vector3 targetRotation = movementRotation.eulerAngles + cameraRotation.eulerAngles;
            moveDir = Quaternion.Euler(0f, targetRotation.y, 0f) * Vector3.forward;
            if (targetRotation != transform.rotation.eulerAngles)
            {
                transform.DORotate(targetRotation, 0.2f);
            }
        }
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
    private void AnimationUpdate(bool isLockedOn = false)
    {
        if (m_CurrentAnimationState == PlayerAnimation.movement.JumpStart ||
            m_CurrentAnimationState == PlayerAnimation.movement.JumpEnd)
        {
            return;
        }
        if (m_CurrentAnimationState == PlayerAnimation.movement.JumpAir)
        {
            if (m_CharacterController.isGrounded && m_JumpingElapsed >= 0.3f)
            {
                ChangeAnimationState(PlayerAnimation.movement.JumpEnd);
                IsJumping = false;
                m_JumpingElapsed = 0.0f;
            }
            return;
        }

        if (m_AppliedMovement == Vector3.zero)
        {
            ChangeAnimationState(PlayerAnimation.movement.Idle);
            return;
        }
        if (isLockedOn)
        {
            LockedOnMovementUpdate(m_AppliedMovement);
        }
        else
        {
            NormalMovementUpdate(m_AppliedMovement);
        }
        void LockedOnMovementUpdate(Vector3 movement)
        {
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
        }
        void NormalMovementUpdate(Vector3 movement)
        {
            if (m_PlayerControls.isRunPressed)
            {
                ChangeAnimationState(PlayerAnimation.movement.Run);
            }
            else
            {
                ChangeAnimationState(PlayerAnimation.movement.WalkFront);
            }
        }
    }
}
