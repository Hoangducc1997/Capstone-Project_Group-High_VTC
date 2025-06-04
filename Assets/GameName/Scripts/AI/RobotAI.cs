using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Pathfinding;

public class RobotAI : MonoBehaviour
{
    public Transform playerTransform;
    public Transform[] patrolPoints;
    public GameObject allyPrefab;
    public Transform summonPoint;
    public Transform healingPoint;
    public Transform enemyRangeMapPoint;

    public float fleeHealthThreshold = 20f;
    public float summonHealthThreshold = 60f;
    public float buffCooldown = 15f;
    public float maxFleeRange = 10f;

    private float lastBuffTime = -Mathf.Infinity;
    private Node rootNode;
    private bool hasSummoned = false;

    private EnemyHealth enemyHealth;
    public EnemyAttack enemyAttack;

    private AIPath aiPath;
    private AIDestinationSetter destinationSetter;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyHealth = GetComponent<EnemyHealth>();
        aiPath = GetComponent<AIPath>();
        destinationSetter = GetComponent<AIDestinationSetter>();

        rootNode = new Selector(new List<Node>
        {
            new FleeIfLowHealth(() => enemyHealth.CurrentHealth, fleeHealthThreshold, playerTransform, enemyRangeMapPoint, maxFleeRange, aiPath, animator),
            new BuffSelf(transform, healingPoint, buffCooldown, () => lastBuffTime, t => lastBuffTime = t, enemyHealth, allyPrefab, summonPoint, aiPath, animator),
            new SummonAllies(summonPoint, allyPrefab, () => enemyHealth.CurrentHealth, summonHealthThreshold, () => hasSummoned, v => hasSummoned = v),
            new Sequence(new List<Node>
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

    // --- FleeIfLowHealth Node ---
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
                    if (animator != null) animator.SetBool("Run", true);
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
                if (animator != null)
                {
                    animator.SetBool("Run", false);
                    animator.SetTrigger("Common");
                }
            }

            return NodeState.FAILURE;
        }
    }
    public class SummonAllies : Node
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
            Animator animator = null) // optional
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
            Debug.Log($"🩸 [Summon] Máu hiện tại: {currentHP}, Đã gọi ally? {getSummonState()}");

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
                Debug.Log("✅ Ally đã được triệu hồi!");

                if (animator != null)
                {
                    animator.SetTrigger("Summon"); // nếu có animation
                }

                setSummonState(true);
                return NodeState.SUCCESS;
            }

            return NodeState.FAILURE;
        }
    }

    // --- BuffSelf Node ---
    public class BuffSelf : Node
    {
        private Transform self;
        private Transform healPoint;
        private float cooldown;
        private System.Func<float> getLastTime;
        private System.Action<float> setLastTime;
        private EnemyHealth enemyHealth;
        private AIPath aiPath;
        private Animator animator;

        private float healPerTick = 10f;
        private float interval = 1f;
        private float nextHealTime;
        private bool isHealing = false;
        private float normalSpeed;
        private float runSpeed = 4f;

        private GameObject allyPrefab;
        private Transform summonPoint;
        private bool hasSummoned = false;

        public BuffSelf(Transform self, Transform healPoint, float cooldown, System.Func<float> getLastTime, System.Action<float> setLastTime,
                        EnemyHealth enemyHealth, GameObject allyPrefab, Transform summonPoint, AIPath aiPath, Animator animator)
        {
            this.self = self;
            this.healPoint = healPoint;
            this.cooldown = cooldown;
            this.getLastTime = getLastTime;
            this.setLastTime = setLastTime;
            this.enemyHealth = enemyHealth;
            this.allyPrefab = allyPrefab;
            this.summonPoint = summonPoint;
            this.aiPath = aiPath;
            this.animator = animator;
            this.normalSpeed = aiPath.maxSpeed;
        }

        public override NodeState Evaluate()
        {
            if (Time.time - getLastTime() < cooldown || (!isHealing && enemyHealth.CurrentHealth >= 50))
                return NodeState.FAILURE;

            float dist = Vector3.Distance(self.position, healPoint.position);

            if (dist > 1f)
            {
                if (!isHealing)
                {
                    aiPath.maxSpeed = runSpeed;
                    animator?.SetBool("Run", true);
                }
                aiPath.destination = healPoint.position;
                return NodeState.RUNNING;
            }

            if (!isHealing)
            {
                isHealing = true;
                nextHealTime = Time.time + interval;
                aiPath.maxSpeed = normalSpeed;
                animator?.SetBool("Run", false);
                animator?.SetTrigger("Common");
                if (!hasSummoned && allyPrefab && summonPoint)
                {
                    GameObject.Instantiate(allyPrefab, summonPoint.position, Quaternion.identity);
                    hasSummoned = true;
                }
            }

            if (Time.time >= nextHealTime)
            {
                if (enemyHealth.CurrentHealth < enemyHealth.maxHealth)
                {
                    enemyHealth.Heal((int)healPerTick);
                    nextHealTime = Time.time + interval;
                    return NodeState.RUNNING;
                }
                else
                {
                    setLastTime(Time.time);
                    isHealing = false;
                    return NodeState.SUCCESS;
                }
            }

            return NodeState.RUNNING;
        }
    }

    // --- Patrol Node ---
    public class Patrol : Node
    {
        private Transform self;
        private Transform[] points;
        private int index = 0;
        private AIPath aiPath;
        private Animator animator;

        public Patrol(Transform self, Transform[] points, AIPath aiPath, Animator animator)
        {
            this.self = self;
            this.points = points;
            this.aiPath = aiPath;
            this.animator = animator;
        }

        public override NodeState Evaluate()
        {
            if (points.Length == 0) return NodeState.FAILURE;

            if (aiPath.reachedDestination)
            {
                index = (index + 1) % points.Length;
                aiPath.destination = points[index].position;
            }

            animator?.SetBool("Walk", true);
            return NodeState.RUNNING;
        }
    }

    // --- AttackPlayer Node ---
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


    // --- Distance Check ---
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
}
