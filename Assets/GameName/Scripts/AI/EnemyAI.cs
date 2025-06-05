// EnemyAI.cs
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy AI Settings")]
    public Transform playerTransform;
    public Transform[] patrolPoints;
    public EnemyAttack enemyAttack;

    private AIPath aiPath;
    private Animator animator;
    private Node rootNode;

    void Start()
    {
        // Trong Start()
        var setter = GetComponent<AIDestinationSetter>();
        if (setter != null)
            setter.enabled = false; // hoặc Destroy(setter);

        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();

        rootNode = new Selector(new List<Node>
        {
            new Sequence(new List<Node>
            {
                new CheckPlayerDistance(playerTransform, transform), // Chỉ thành công nếu gần
                new AttackPlayer(playerTransform, transform, enemyAttack, animator, aiPath)
            }),

            new Patrol(transform, patrolPoints, aiPath, animator)
        });
    }

    void Update()
    {
        rootNode.Evaluate();
    }
}