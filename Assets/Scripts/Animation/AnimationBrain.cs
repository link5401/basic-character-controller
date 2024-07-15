using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimationBrain : MonoBehaviour
{
    public string m_CurrentAnimationState;
    protected Animator m_Animator;
    public void ChangeAnimationState(string animState, float crossFadeDuration = 0.05f, float waitTime = 0f)
    {
        if (waitTime > 0f)
        {
            StartCoroutine(Wait(waitTime));
        }
        else
        {
            ValidateAnimation();
        }
        void ValidateAnimation()
        {
            if (m_CurrentAnimationState != animState)
            {
                m_Animator.CrossFade(animState, crossFadeDuration);
                m_CurrentAnimationState = animState;
            }
        }
        IEnumerator Wait(float duration)
        {
            yield return new WaitForSeconds(duration - crossFadeDuration);
            ValidateAnimation();
        }
    }
    protected bool IsAnimationDone(string animState)
    {
        return m_Animator.GetCurrentAnimatorStateInfo(0).IsName(animState);
    }
}