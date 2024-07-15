using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

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
                yield return coroutineMove(speedMultiplier);
            }
            else
            {
                m_PlayerControls.CurrentMovement = Vector3.zero;
            }
            yield return DetermineMovementAnimationState();
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
    private IEnumerator coroutineMove(float speed, bool isLockedOn = false)
    {
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
        yield return null;

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
    private IEnumerator DetermineMovementAnimationState(bool isLockedOn = false)
    {
        if (m_AppliedMovement == Vector3.zero)
        {
            ChangeAnimationState(PlayerAnimation.movement.Idle);
            yield break;
        }
        if (isLockedOn)
        {
            yield return coroutineHandleLockedOnMovement(m_AppliedMovement);
        }
        else
        {
            yield return coroutineHandleNormalMovement(m_AppliedMovement);
        }
        IEnumerator coroutineHandleLockedOnMovement(Vector3 movement)
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
            yield return null;
        }
        IEnumerator coroutineHandleNormalMovement(Vector3 movement)
        {
            if (m_PlayerControls.isRunPressed)
            {
                ChangeAnimationState(PlayerAnimation.movement.Run);
            }
            else
            {
                ChangeAnimationState(PlayerAnimation.movement.WalkFront);
            }
            yield return null;
        }
    }
}
