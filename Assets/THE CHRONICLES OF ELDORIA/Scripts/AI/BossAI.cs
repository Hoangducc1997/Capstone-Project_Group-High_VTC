// BossAI.cs
using Pathfinding;
using System.Collections.Generic;
using UnityEngine;

public class BossAI : MonoBehaviour
{
    [Header("Boss AI Settings")]
    public Transform playerTransform;
    public Transform[] patrolPoints;
    public EnemyAttack enemyAttack;

    [Header("Boss Specific Settings")]
    public GameObject allyPrefab;
    public Transform summonPoint;
    public Transform healingPoint;
    public Transform enemyRangeMapPoint;
    public float fleeHealthThreshold = 20f;
    public float summonHealthThreshold = 60f;
    public float buffCooldown = 15f;
    public float maxFleeRange = 10f;

    private float lastBuffTime = -Mathf.Infinity;
    private bool hasSummoned = false;
    private EnemyHealth enemyHealth;

    private AIPath aiPath;
    private Animator animator;
    private AINode rootNode;

    void Start()
    {
        aiPath = GetComponent<AIPath>();
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();

        rootNode = new Selector(new List<AINode>
        {
            new FleeIfLowHealth(
                () => enemyHealth.CurrentHealth,
                fleeHealthThreshold,
                playerTransform,
                enemyRangeMapPoint,
                maxFleeRange,
                aiPath,
                animator
            ),
            new BuffSelf(
                transform,
                healingPoint,
                buffCooldown,
                () => lastBuffTime,
                t => lastBuffTime = t,
                enemyHealth,
                allyPrefab,
                summonPoint,
                aiPath,
                animator
            ),
            new SummonAllies(
                summonPoint,
                allyPrefab,
                () => enemyHealth.CurrentHealth,
                summonHealthThreshold,
                () => hasSummoned,
                v => hasSummoned = v,
                animator
            ),
            new Sequence(new List<AINode>
            {
                new CheckPlayerDistance(playerTransform, transform),
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
