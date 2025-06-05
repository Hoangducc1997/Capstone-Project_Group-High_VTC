// Node_AttackPlayer.cs
using UnityEngine;
using Pathfinding;

public class AttackPlayer : Node
{
    private Transform player;
    private Transform self;
    private float attackRange = 2.6f;
    private EnemyAttack enemyAttack;
    private Animator animator;
    private AIPath aiPath;

    public AttackPlayer(Transform player, Transform self, EnemyAttack enemyAttack, Animator animator, AIPath aiPath)
    {
        this.player = player;
        this.self = self;
        this.enemyAttack = enemyAttack;
        this.animator = animator;
        this.aiPath = aiPath;
    }

    public override NodeState Evaluate()
    {
        float dist = Vector3.Distance(player.position, self.position);

        if (dist > attackRange)
        {
            aiPath.destination = player.position;
            animator?.SetBool("Walk", true);
            return NodeState.RUNNING;
        }

        animator?.SetTrigger("Attack");
        animator?.SetBool("Walk", false);
        enemyAttack.TryAttack(player);
        return NodeState.SUCCESS;
    }
}
