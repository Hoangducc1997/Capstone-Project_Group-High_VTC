// Node_CheckPlayerDistance.cs
using UnityEngine;

public class CheckPlayerDistance : Node
{
    private Transform playerTransform;
    private Transform robotTransform;
    private float distanceRange = 8f;

    public CheckPlayerDistance(Transform playerTransform, Transform robotTransform)
    {
        this.playerTransform = playerTransform;
        this.robotTransform = robotTransform;
    }

    public override NodeState Evaluate()
    {
        float distance = Vector3.Distance(playerTransform.position, robotTransform.position);
        return distance < distanceRange ? NodeState.SUCCESS : NodeState.FAILURE;
    }
}
