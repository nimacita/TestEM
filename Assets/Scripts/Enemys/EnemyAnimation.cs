using System;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Clips")]
    [SerializeField] private AnimationClip dieClip;
    [SerializeField] private AnimationClip attackClip;
    [SerializeField] private AnimationClip damageFrontClip;
    [SerializeField] private AnimationClip damageBackClip;

    [Header("Actions")]
    public Action onAttacked;

    #region Move / Run
    public void SetWalkState(bool value)
    {
        animator.SetBool("Walk", value);
    }

    public void SetRunState(bool value)
    {
        animator.SetBool("Run", value);
    }
    #endregion

    #region Attacked

    public void SetAttackTrigger()
    {
        animator.SetTrigger("Attack");
    }

    public float GetAttackDuration()
    {
        return attackClip.length;
    }

    public void StartAttackAction()
    {
        onAttacked?.Invoke();
    }

    public void SetAttackMulti(float multi)
    {
        animator.SetFloat("AttackedMulti", multi);
    }

    #endregion

    #region Taked Damage / Die

    public void SetTakeDamageTrigger(bool isFront)
    {
        if (isFront)
        {
            animator.SetTrigger("FrontDamage");
        }
        else
        {
            animator.SetTrigger("BackDamage");
        }      
    }

    public float GetDamagedDuration(bool isFront)
    {
        if (isFront)
        {
            return damageFrontClip.length;
        }
        else
        {
            return damageBackClip.length;
        }
    }

    public void SetTakeDamageMulti(float multi)
    {
        animator.SetFloat("DamagedMulti", multi);
    }

    public void SetDieTrigger()
    {
        animator.SetTrigger("Die");
    }

    public float GetDieDuration()
    {
        return dieClip.length;
    }
    #endregion

}
