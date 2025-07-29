// --- SummonAllies.cs ---
using UnityEngine;

public class SummonAllies : AINode
{
    private Transform summonPoint;
    private GameObject allyPrefab;
    private System.Func<float> getCurrentHealth;
    private float healthThreshold;
    private System.Func<bool> getSummonState;
    private System.Action<bool> setSummonState;
    private Animator animator;

    public SummonAllies(
        Transform summonPoint,
        GameObject allyPrefab,
        System.Func<float> getCurrentHealth,
        float healthThreshold,
        System.Func<bool> getSummonState,
        System.Action<bool> setSummonState,
        Animator animator = null)
    {
        this.summonPoint = summonPoint;
        this.allyPrefab = allyPrefab;
        this.getCurrentHealth = getCurrentHealth;
        this.healthThreshold = healthThreshold;
        this.getSummonState = getSummonState;
        this.setSummonState = setSummonState;
        this.animator = animator;
    }

    public override NodeState Evaluate()
    {
        float currentHP = getCurrentHealth();

        if (getSummonState())
            return NodeState.FAILURE;

        if (currentHP < healthThreshold)
        {
            if (allyPrefab == null || summonPoint == null)
            {
                Debug.LogError("❌ allyPrefab hoặc summonPoint chưa được gán!");
                return NodeState.FAILURE;
            }

            GameObject.Instantiate(allyPrefab, summonPoint.position, Quaternion.identity);
            animator?.SetTrigger("Summon");
            setSummonState(true);
            return NodeState.SUCCESS;
        }

        return NodeState.FAILURE;
    }
}
