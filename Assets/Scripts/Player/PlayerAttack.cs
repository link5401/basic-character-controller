using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    // Start is called before the first frame update
    PlayerControls playerControls;
    CharacterController controller;
    Animator animator;
    private int isAttackingHash;
    private int attackHash;

    enum Attack { Idle, Attacking, Waiting }
    float[] attackCDR = { 1.3f, 0.8f, 1.4f };
    float waitTime = 0.2f;
    float attackTimer = 0.0f;
    Attack attackState = Attack.Idle;
    void Awake()
    {
        playerControls = GetComponent<PlayerControls>();
        animator = GetComponent<Animator>();
        isAttackingHash = Animator.StringToHash("isAttacking");
        attackHash = Animator.StringToHash("Attack");
        controller = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {
        AttackUpdate();
        Vector3 rootMotion = animator.deltaPosition;
        Debug.Log("Root Motion: " + rootMotion);

        controller.Move(rootMotion);
    }
    void AttackUpdate()
    {
        bool isAttackPressed = playerControls.isAttackPressed;
        bool isMovementPressed = playerControls.IsMovementPressed;
        int currentAttackAnimation = animator.GetInteger(attackHash);
        switch (attackState)
        {
            case Attack.Idle:
                if (isAttackPressed)
                {
                    attackState = Attack.Attacking;
                    attackTimer = attackCDR[0];
                    animator.SetBool(isAttackingHash, true);
                    animator.SetInteger(attackHash, 1);
                }
                else if(isMovementPressed){
                    animator.SetBool(isAttackingHash, false);
                    attackTimer = 0.0f;
                    attackState = Attack.Idle;
                    animator.SetInteger(attackHash, 0);
                }
                break;
            case Attack.Attacking:
                attackTimer -= Time.deltaTime;
                if (attackTimer < 0)
                {
                    attackTimer = waitTime;
                    attackState = Attack.Waiting;
                }
                break;
            case Attack.Waiting:
                attackTimer -= Time.deltaTime;
                if (attackTimer < 0)
                {
                    attackState = Attack.Idle;
                    animator.SetInteger(attackHash, 0);
                }
                if (isAttackPressed)
                {
                    attackState = Attack.Attacking;
                    if (currentAttackAnimation == 3)
                    {
                        animator.SetInteger(attackHash, 1);
                        attackTimer = attackCDR[0];
                    }
                    else
                    {
                        animator.SetInteger(attackHash, currentAttackAnimation + 1);
                        attackTimer = attackCDR[currentAttackAnimation];
                    }

                }
                break;
        }
        currentAttackAnimation = animator.GetInteger(attackHash);
        // if(currentAttackAnimation == 1) controller.Move(transform.forward * Time.deltaTime);
        // if(currentAttackAnimation == 2) controller.Move(transform.forward * Time.deltaTime);

        // if(currentAttackAnimation == 3) controller.Move(transform.forward * 3 * Time.deltaTime);
    }
}
