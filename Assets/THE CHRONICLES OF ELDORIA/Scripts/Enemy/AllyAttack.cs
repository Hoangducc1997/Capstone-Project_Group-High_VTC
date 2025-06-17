using UnityEngine;
using UnityEngine.AI;

public class AllyAttack : MonoBehaviour
{
    [Header("Combat Settings")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1.2f;
    [SerializeField] private int damage = 10;
    [SerializeField] private LayerMask playerLayer;

    private float lastAttackTime;
    private Animator animator;
    private NavMeshAgent agent;
    private Transform player;

    private void Start()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
        }
        else
        {
            Debug.LogError("Không tìm thấy Player có tag 'Player'");
        }
    }

    private void Update()
    {
        if (player == null) return;

        float distance = Vector3.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            agent.isStopped = true;

            if (Time.time - lastAttackTime >= attackCooldown)
            {
                lastAttackTime = Time.time;
                Attack();
            }
        }
        else
        {
            agent.isStopped = false;
            agent.SetDestination(player.position);

            if (animator != null)
            {
                animator.SetBool("Walk", true);
            }
        }
    }

    private void Attack()
    {
        if (animator != null)
        {
            animator.SetTrigger("Attack");
            animator.SetBool("Walk", false);
        }
        AudioManager.Instance.PlayVFX("EnemyAttack");
        Collider[] hitPlayers = Physics.OverlapSphere(transform.position, attackRange, playerLayer);
        foreach (var hit in hitPlayers)
        {
            var playerHealth = hit.GetComponent<PlayerInfo>();
            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                Debug.Log($"🤖 Ally tấn công {hit.name}, gây {damage} sát thương.");
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
