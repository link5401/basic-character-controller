using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnFinish : StateMachineBehaviour
{
    [SerializeField] private string m_AnimationToPlay;
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
            animator.GetComponent<PlayerMovement>().ChangeAnimationState(m_AnimationToPlay, 0.1f, stateInfo.length);
    }
}

