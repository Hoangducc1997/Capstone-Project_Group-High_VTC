// --- FleeIfLowHealth.cs ---
using UnityEngine;
using Pathfinding;

public class FleeIfLowHealth : Node
{
    private System.Func<float> getHealth;
    private float threshold;
    private Transform player;
    private Transform centerPoint;
    private float maxRange;
    private AIPath aiPath;
    private Animator animator;
    private float fleeSpeed = 4f;
    private float normalSpeed;
    private bool isFleeing = false;

    public FleeIfLowHealth(System.Func<float> getHealth, float threshold, Transform player, Transform centerPoint, float maxRange, AIPath aiPath, Animator animator)
    {
        this.getHealth = getHealth;
        this.threshold = threshold;
        this.player = player;
        this.centerPoint = centerPoint;
        this.maxRange = maxRange;
        this.aiPath = aiPath;
        this.animator = animator;
        this.normalSpeed = aiPath.maxSpeed;
    }

    public override NodeState Evaluate()
    {
        if (getHealth() < threshold)
        {
            if (!isFleeing)
            {
                aiPath.maxSpeed = fleeSpeed;
                isFleeing = true;
                animator?.SetBool("Run", true);
            }

            Vector3 fleeDir = (aiPath.transform.position - player.position).normalized;
            Vector3 target = aiPath.transform.position + fleeDir * 10f;
            Vector3 offset = target - centerPoint.position;
            if (offset.magnitude > maxRange)
            {
                offset = offset.normalized * maxRange;
                target = centerPoint.position + offset;
            }

            aiPath.destination = target;
            return NodeState.RUNNING;
        }

        if (isFleeing)
        {
            aiPath.maxSpeed = normalSpeed;
            isFleeing = false;
            animator?.SetBool("Run", false);
            animator?.SetTrigger("Common");
        }

        return NodeState.FAILURE;
    }
}
