using System;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Animator playerAnimator;
    [SerializeField] Action ComboResetAction;
    void PlayAnimation()
    {
        /* playerAnimator.set*/
    }

    public void SetAnimationType(string AnimationName, bool isActive)
    {
        playerAnimator.SetBool(AnimationName, isActive);
    }

    public bool CheckCurrentAnimation(string animtionName)
    {
        return playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(animtionName);
    }

    public AnimatorStateInfo GetCurrentAnimatorStateInfo()
    {
        return playerAnimator.GetCurrentAnimatorStateInfo(0);
    }

    public void SeAnimation(string BooleanAnimationKey, bool BoolAnimationValue)
    {
        playerAnimator.SetBool(BooleanAnimationKey, BoolAnimationValue);
    }

    #region Animation Action

    public void SetActionComboReset(Action actionComboReset)
    {
        this.ComboResetAction = actionComboReset;   
    }

    #endregion

    #region Animation Events

    public void OnComboAttackReset()
    {
        this.ComboResetAction?.Invoke();
    }

    #endregion
}
