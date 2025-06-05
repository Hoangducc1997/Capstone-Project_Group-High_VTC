using Pathfinding;
using UnityEngine;

public class CheckPlayerDistance : Node
{
    private Transform playerTransform;
    private Transform robotTransform;
    private float distanceRange = 6f;
    private AIDestinationSetter setter;

    public CheckPlayerDistance(Transform playerTransform, Transform robotTransform)
    {
        this.playerTransform = playerTransform;
        this.robotTransform = robotTransform;
        setter = robotTransform.GetComponent<AIDestinationSetter>();
    }

    public override NodeState Evaluate()
    {
        float distance = Vector3.Distance(playerTransform.position, robotTransform.position);

        if (distance < distanceRange)
        {
            if (setter != null)
                setter.enabled = true;

            return NodeState.SUCCESS;
        }
        else
        {
            if (setter != null)
                setter.enabled = false;

            return NodeState.FAILURE;
        }
    }
}
