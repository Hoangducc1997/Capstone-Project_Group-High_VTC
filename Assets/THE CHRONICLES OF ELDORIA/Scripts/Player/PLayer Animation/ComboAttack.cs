using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class ComboAttack
{
    float lastAttackTime;
    public float comboDelay = 1f;
    bool inputBuffered = false;
    private int MaxCombo = 4;
    int comboStep = 0;
    private int currentActiveStep = 0;
    private string lastAnimation = "";
    PlayerController playerController;
    PlayerAnimation playerAnimation;
    public ComboAttack(PlayerController playerController)
    {
        this.playerController = playerController;
        this.playerController.GetPlayerAnimation().SetActionComboReset(this.ResetCombo);
    }

    public void ComboUpdate()
    {
        /*if (Time.time - lastAttackTime > comboDelay)
        {
            ResetCombo();
        }*/

        HandleBufferedCombo();
        CheckAndStopCurrentAttack();
    }

    public void OnAttackInput()
    {
        if (comboStep == 0)
        {
            comboStep = 1;
            playerController.GetPlayerAnimation().SetAnimationType("isAttack", true);
            PlayComboAnimation(comboStep);
            lastAttackTime = Time.time;
        }
        else if (comboStep < MaxCombo)
        {
            inputBuffered = true;
        }
    }

    void HandleBufferedCombo()
    {
        if (!inputBuffered) return;

        string currentState = $"Attack_0{comboStep}";
        if (comboStep >= MaxCombo)
        {
            ResetCombo();
            return;
        }
        if (playerController.GetPlayerAnimation().CheckCurrentAnimation(currentState) &&
                playerController.GetPlayerAnimation().GetCurrentAnimatorStateInfo().normalizedTime > 0.7f)
        {
            comboStep++;
            if (comboStep > MaxCombo)
            {
                ResetCombo();
                return;
            }
            inputBuffered = false;
            PlayComboAnimation(comboStep);
            lastAttackTime = Time.time;
        }

        if (playerController.GetPlayerAnimation().GetCurrentAnimatorStateInfo().normalizedTime >= 1)
        {
            inputBuffered = false;
        }
    }

    void PlayComboAnimation(int step)
    {
        currentActiveStep = step;
        playerController.GetPlayerAnimation().SetAnimationType("isAttack_0" + step, true);
        if (step > 1)
        {
            playerController.GetPlayerAnimation().SetAnimationType("isAttack_0" + (step - 1), false);
        }

        this.lastAnimation = "Attack_0" + step;
    }



    void CheckAndStopCurrentAttack()
    {
        for (int i = 1; i <= MaxCombo; i++)
        {
            string stateName = $"Attack_0{i}";
            if (playerController.GetPlayerAnimation().CheckCurrentAnimation(stateName) &&
                playerController.GetPlayerAnimation().GetCurrentAnimatorStateInfo().normalizedTime > 0.95f)
            {
                StopComboAnimation(i);

                if (i == MaxCombo)
                {
                    ResetCombo(); // Ensures it doesn’t get stuck
                }
            }
        }
    }


    void StopComboAnimation(int step)
    {
        playerController.GetPlayerAnimation().SetAnimationType("isAttack_0" + step, false);
    }

    public void ResetCombo()
    {
        AnimatorStateInfo stateInfo = playerController.GetPlayerAnimation().GetCurrentAnimatorStateInfo();

        bool isCorrectState = stateInfo.IsName("Attack_0" + currentActiveStep);
        bool isFinished = stateInfo.normalizedTime >= 1f;

        // If we're at the last combo and animation has finished, force reset
        if ((comboStep >= MaxCombo && isCorrectState && isFinished) || (!inputBuffered && isCorrectState && isFinished))
        {
            Debug.Log("Resetting Combo");

            comboStep = 0;
            currentActiveStep = 0;
            inputBuffered = false;

            for (int i = 1; i <= MaxCombo; i++)
            {
                playerController.GetPlayerAnimation().SetAnimationType("isAttack_0" + i, false);
            }

            playerController.GetPlayerAnimation().SetAnimationType("isAttack", false);
        }
    }


}
