using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ComboAttack
{
    int comboStep = 0;
    float lastAttackTime;
    public float comboDelay = 1.2f;
    bool inputBuffered = false;
    PlayerController playerController;
    PlayerAnimation playerAnimation;
    public ComboAttack(PlayerController playerController)
    {
        this.playerController = playerController;
    }

    public void ComboUpdate()
    {
        if (Time.time - lastAttackTime > comboDelay)
        {
            ResetCombo();
        }

        HandleBufferedCombo();
        CheckAndStopCurrentAttack();
    }

    public void OnAttackInput()
    {
        if (comboStep == 0)
        {
            comboStep = 1;
            PlayComboAnimation(comboStep);
            lastAttackTime = Time.time;
        }
        else
        {
            inputBuffered = true;
        }
    }

    void HandleBufferedCombo()
    {
        if (!inputBuffered) return;

        string currentState = $"Anim_Attack{comboStep}";
        if (playerController.GetPlayerAnimation().CheckCurrentAnimation(currentState) &&
                playerController.GetPlayerAnimation().GetAnimationTimeNormalize() > 0.3f)
        {
            comboStep++;
            if (comboStep > 3)
            {
                ResetCombo();
                return;
            }

            PlayComboAnimation(comboStep);
            lastAttackTime = Time.time;
            inputBuffered = false;
        }
    }

    void PlayComboAnimation(int step)
    {
        /*this.playerController.playerWeapon.isAttacking = true;*/
        playerController.GetPlayerAnimation().PlayAnimation("Attack" + step, true);
        if (step > 1)
        {
            playerController.GetPlayerAnimation().PlayAnimation("Attack" + (step - 1), false);
        }
    }

    void CheckAndStopCurrentAttack()
    {
        for (int i = 1; i <= 3; i++)
        {
            string stateName = $"Attack0{i}";
            if (playerController.GetPlayerAnimation().CheckCurrentAnimation(stateName) &&
                playerController.GetPlayerAnimation().GetAnimationTimeNormalize() > 0.5f)
            {
                StopComboAnimation(i);
            }
        }
    }

    void StopComboAnimation(int step)
    {
        playerController.GetPlayerAnimation().PlayAnimation("Attack" + step, false);
    }

    public void ResetCombo()
    {
        comboStep = 0;
        inputBuffered = false;
        for (int i = 1; i <= 3; i++)
        {
            playerController.GetPlayerAnimation().PlayAnimation("Attack" + i, false);
        }
        /*this.playerController.playerWeapon.isAttacking = false;*/
    }
}
