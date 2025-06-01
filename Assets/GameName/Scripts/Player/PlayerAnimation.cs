using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [SerializeField] Animator playerAnimator;

    void PlayAnimation()
    {
        /* playerAnimator.set*/
    }

    public void PlayAnimation(string AnimationName, bool isActive)
    {
        playerAnimator.SetBool(AnimationName, isActive);
    }

    public bool CheckCurrentAnimation(string animtionName)
    {
        return playerAnimator.GetCurrentAnimatorStateInfo(0).IsName(animtionName);
    }

    public float GetAnimationTimeNormalize()
    {
        return playerAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime;
    }
}
