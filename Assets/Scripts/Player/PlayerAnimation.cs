using System;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private AnimationClip dodgeAnim;
    [SerializeField] private AnimationClip attackAnim;
    [SerializeField] private AnimationClip damagedFrontAnim;
    [SerializeField] private AnimationClip damagedBackAnim;

    [Header("Actions")]
    public Action OnAttacked;
    public Action OnAttackEnded;

    #region Move Animations
    public void SetRunState(bool value)
    {
        animator.SetBool("Run", value);
    }

    public void SetWalkState(bool value)
    {
        animator.SetBool("Walk", value);
    }

    public void SetJumpStart()
    {
        animator.ResetTrigger("JumpEnd");
        animator.SetTrigger("JumpStart");
    }

    public void SetJumpEnd()
    {
        animator.SetTrigger("JumpEnd");
    }

    #endregion

    #region DodgeAnimations

    public void SetDodgeStart()
    {
        animator.ResetTrigger("DodgeEnd");
        animator.SetTrigger("DodgeFront");
    }

    public void SetDodgeEnd()
    {
        animator.SetTrigger("DodgeEnd");
    }

    public void SetDodgeMulti(float multiplier)
    {
        animator.SetFloat("DodgeMultiplier", multiplier);
    }

    public float GetDodgeDuration()
    {
        return dodgeAnim.length;
    }

    #endregion

    #region Damaged

    public void SetDamagedTrigger(bool isFront)
    {
        if (isFront)
        {
            animator.SetTrigger("DamagedFront");
        }
        else
        {
            animator.SetTrigger("DamagedBack");
        }
    }

    public void SetDamageMulti(float multiplier)
    {
        animator.SetFloat("DamagedMultiplier", multiplier);
    }

    public float GetDamagedDuration(bool isFront)
    {
        if (isFront)
        {
            return damagedFrontAnim.length;
        }
        else
        {
            return damagedBackAnim.length;
        }
    }

    public void SetDiedTrigger()
    {
        animator.SetTrigger("Died");
    }

    #endregion

    #region Air/Wall Animations

    public void SetInAir(bool value)
    {
        animator.SetBool("InAir", value);
    }

    public void SetHoldWall(bool value)
    {
        animator.SetBool("HoldWall", value);
    }

    #endregion

    #region Attack Animations

    public void SetAttackTrigger()
    {
        animator.SetTrigger("Attack");
    }

    public void StartAttackAction()
    {
        OnAttacked?.Invoke();
    }

    public void EndAttackAction()
    {
        OnAttackEnded?.Invoke();
    }

    public void SetAttackMulti(float multiplier)
    {
        animator.SetFloat("AttackMultiplier", multiplier);
    }

    public float GetAttackDuration()
    {
        return attackAnim.length;
    }

    #endregion
}
