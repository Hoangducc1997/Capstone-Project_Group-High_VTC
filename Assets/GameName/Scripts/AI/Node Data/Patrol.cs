// Node_Patrol.cs
using Pathfinding;
using UnityEngine;

public class Patrol : Node
{
    private Transform self;
    private Transform[] points;
    private int index = 0;
    private AIPath aiPath;
    //private Animator animator;

    public Patrol(Transform self, Transform[] points, AIPath aiPath, Animator animator)
    {
        this.self = self;
        this.points = points;
        this.aiPath = aiPath;
        //this.animator = animator;
        Debug.Log($"Patrol initialized with {points.Length} points.");
    }

    public override NodeState Evaluate()
    {
        if (points.Length == 0) return NodeState.FAILURE;

        if (aiPath.reachedDestination)
        {
            index = (index + 1) % points.Length;
            aiPath.destination = points[index].position;
        }

        //animator?.SetBool("Walk", true);
        return NodeState.RUNNING;
    }
}
