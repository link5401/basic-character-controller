using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationBrain : MonoBehaviour
{
    public string m_CurrentAnimationState;
    protected Animator m_Animator;
    protected void ChangeAnimationState(string animState, float crossFadeDuration = 0.05f)
    {
        if (m_CurrentAnimationState != animState)
        {
            m_Animator.CrossFade(animState, crossFadeDuration);
            m_CurrentAnimationState = animState;
        }
    }
    protected bool IsAnimationDone(string animState)
    {
        return m_Animator.GetCurrentAnimatorStateInfo(0).IsName(animState);
    }
}