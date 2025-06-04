using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RobotAI : MonoBehaviour
{
    public NavMeshAgent agent;
    public Transform playerTransform;
    public Transform[] patrolPoints;
    public GameObject allyPrefab;
    public Transform summonPoint;

    public float fleeHealthThreshold = 20f;
    public float summonHealthThreshold = 60f;
    public Transform healingPoint;
    public float buffCooldown = 15f;

    private float lastBuffTime = -Mathf.Infinity;
    private Node rootNode;
    private bool hasSummoned = false;

    private EnemyHealth enemyHealth;
 
    public EnemyAttack enemyAttack;

    public Transform enemyRangeMapPoint; // Gán trong Inspector
    public float maxFleeRange = 10f;

    private Animator animator;
    void Start()
    {
        animator = GetComponent<Animator>();
        if (animator == null)
        {
            Debug.LogError("Không tìm thấy Animator trên RobotAI!");
        }

        enemyHealth = GetComponent<EnemyHealth>(); // <- Gán đúng lúc đầu

        rootNode = new Selector(new List<Node>
        {
            // 1. Nếu máu < 20 → chạy trốn
            new FleeIfLowHealth(
            () => enemyHealth.CurrentHealth,
            fleeHealthThreshold,
            agent,
            playerTransform,
            enemyRangeMapPoint,         // 👈 vị trí trung tâm
            maxFleeRange        // 👈 bán kính tối đa
            ),


            // 2. Nếu máu < 50 → chạy đến điểm hồi máu
            new BuffSelf(
                transform,
                agent,
                healingPoint,
                buffCooldown,
                () => lastBuffTime,
                t => lastBuffTime = t,
                enemyHealth,
                allyPrefab,          // 👈 truyền thêm prefab đồng minh
                summonPoint          // 👈 và vị trí spawn ally
            ),


            // 4. Nếu máu < 50 → gọi đồng minh (nằm sau attack để tránh gọi sớm)
            new SummonAllies(summonPoint, allyPrefab, () => enemyHealth.CurrentHealth, summonHealthThreshold, () => hasSummoned, v => hasSummoned = v),

            // 3. Nếu gần player → tấn công
            new Sequence(new List<Node>
            {
                new CheckPlayerDistance(playerTransform, transform),
                new AttackPlayer(playerTransform, transform, enemyAttack)
            }),

            // 4. Nếu máu < 50 → gọi đồng minh (nằm sau attack để tránh gọi sớm)
            new SummonAllies(summonPoint, allyPrefab, () => enemyHealth.CurrentHealth, summonHealthThreshold, () => hasSummoned, v => hasSummoned = v),

            // 5. Mặc định → tuần tra
            new Patrol(agent, patrolPoints)
        });

    }

    void Update()
    {
        rootNode.Evaluate();
        Debug.Log("Máu hiện tại của boss: " + enemyHealth.CurrentHealth);
    }


    // --- Flee if low health ---
    public class FleeIfLowHealth : Node
    {
        private System.Func<float> getHealth;
        private float threshold;
        private NavMeshAgent agent;
        private Transform player;
        private Transform spawnPoint;
        private float maxFleeRange;

        private float normalSpeed;
        private float fleeSpeed = 3.9f;
        private bool speedBoosted = false;
        private Animator animator;

        public FleeIfLowHealth(System.Func<float> getHealth, float threshold, NavMeshAgent agent,
                               Transform player, Transform spawnPoint, float maxFleeRange)
        {
            this.getHealth = getHealth;
            this.threshold = threshold;
            this.agent = agent;
            this.player = player;
            this.spawnPoint = spawnPoint;
            this.maxFleeRange = maxFleeRange;
            this.normalSpeed = agent.speed;
            this.animator = agent.GetComponent<Animator>();
        }

        public override NodeState Evaluate()
        {
            // Nếu cần trốn
            if (getHealth() < threshold)
            {
                // tăng tốc khi mới bắt đầu trốn
                if (!speedBoosted)
                {
                    agent.speed = fleeSpeed;
                    speedBoosted = true;
                }

                // tính hướng trốn
                Vector3 fleeDir = (agent.transform.position - player.position).normalized;
                Vector3 rawTarget = agent.transform.position + fleeDir * 10f;
                Vector3 offset = rawTarget - spawnPoint.position;
                if (offset.magnitude > maxFleeRange)
                {
                    offset = offset.normalized * maxFleeRange;
                    rawTarget = spawnPoint.position + offset;
                }

                agent.SetDestination(rawTarget);
                Debug.Log("Fleeing to: " + rawTarget);

                // 🔥 Bật Run animation
                if (animator != null)
                {
                    animator.SetBool("Run", true);
                    animator.SetBool("Walk", false);
                }

                return NodeState.RUNNING;
            }

            // Khi không còn sức trốn, reset speed và anim
            if (speedBoosted)
            {
                agent.speed = normalSpeed;
                speedBoosted = false;

                if (animator != null)
                {
                    animator.SetBool("Run", false);
                    animator.SetTrigger("Common"); // trở về Idle/Common
                }
            }

            return NodeState.FAILURE;
        }
    }

    // --- Summon Allies ---
    public class SummonAllies : Node //gọi đồng minh
    {
        private Transform summonPoint;
        private GameObject allyPrefab;
        private System.Func<float> getCurrentHealth;
        private float healthThreshold;
        private System.Func<bool> getSummonState;
        private System.Action<bool> setSummonState;

        public SummonAllies(Transform summonPoint, GameObject allyPrefab, System.Func<float> getCurrentHealth, float healthThreshold, System.Func<bool> getSummonState, System.Action<bool> setSummonState)
        {
            this.summonPoint = summonPoint;
            this.allyPrefab = allyPrefab;
            this.getCurrentHealth = getCurrentHealth;
            this.healthThreshold = healthThreshold;
            this.getSummonState = getSummonState;
            this.setSummonState = setSummonState;
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
                setSummonState(true);
                return NodeState.SUCCESS;
            }

            return NodeState.FAILURE;
        }


    }

    // --- Buff Self --- //Bơm máu
    public class BuffSelf : Node
    {
        private Transform boss;
        private NavMeshAgent agent;
        private Transform healingPoint;
        private float cooldown;
        private System.Func<float> getLastTime;
        private System.Action<float> setLastTime;
        private EnemyHealth enemyHealth;

        private float healAmountPerTick = 10f;
        private float healInterval = 1f;
        private float nextHealTime;
        private bool isHealing = false;

        private float normalSpeed;
        private float runToHealSpeed = 4f;
        private bool speedBoosted = false;

        private Animator animator;

        // 👇 Ally spawn
        private GameObject allyPrefab;
        private Transform summonPoint;
        private bool hasSummonedAlly = false;

        public BuffSelf(
            Transform boss,
            NavMeshAgent agent,
            Transform healingPoint,
            float cooldown,
            System.Func<float> getLastTime,
            System.Action<float> setLastTime,
            EnemyHealth enemyHealth,
            GameObject allyPrefab,
            Transform summonPoint)
        {
            this.boss = boss;
            this.agent = agent;
            this.healingPoint = healingPoint;
            this.cooldown = cooldown;
            this.getLastTime = getLastTime;
            this.setLastTime = setLastTime;
            this.enemyHealth = enemyHealth;

            this.allyPrefab = allyPrefab;
            this.summonPoint = summonPoint;

            normalSpeed = agent.speed;
            animator = agent.GetComponent<Animator>();
        }

        public override NodeState Evaluate()
        {
            if (Time.time - getLastTime() < cooldown)
                return NodeState.FAILURE;

            if (!isHealing && enemyHealth.CurrentHealth >= 50)
                return NodeState.FAILURE;

            float distance = Vector3.Distance(boss.position, healingPoint.position);

            // --- Đang trên đường đến điểm hồi máu ---
            if (distance > 1f)
            {
                if (!speedBoosted)
                {
                    agent.speed = runToHealSpeed;
                    speedBoosted = true;

                    if (animator != null)
                    {
                        animator.SetBool("Run", true);
                        animator.SetBool("Walk", false);
                    }
                }

                agent.SetDestination(healingPoint.position);
                Debug.Log("🏃‍♂️ Đang chạy nhanh đến điểm hồi máu...");
                isHealing = false;
                return NodeState.RUNNING;
            }

            // --- Đã đến điểm hồi máu ---
            if (speedBoosted)
            {
                agent.speed = normalSpeed;
                speedBoosted = false;

                if (animator != null)
                {
                    animator.SetBool("Run", false);
                    animator.SetTrigger("Common");
                }
            }

            if (!isHealing)
            {
                isHealing = true;
                nextHealTime = Time.time + healInterval;
                Debug.Log("❤️ Bắt đầu hồi máu...");

                // 👇 Spawn đồng minh nếu chưa gọi lần nào
                if (!hasSummonedAlly && allyPrefab != null && summonPoint != null)
                {
                    GameObject.Instantiate(allyPrefab, summonPoint.position, Quaternion.identity);
                    Debug.Log("✅ Ally được gọi khi hồi máu!");
                    hasSummonedAlly = true;
                }
            }

            if (Time.time >= nextHealTime)
            {
                if (enemyHealth.CurrentHealth < enemyHealth.maxHealth)
                {
                    enemyHealth.Heal((int)healAmountPerTick);
                    nextHealTime = Time.time + healInterval;
                    return NodeState.RUNNING;
                }
                else
                {
                    Debug.Log("✅ Đã hồi đầy máu!");
                    setLastTime(Time.time);
                    isHealing = false;

                    if (animator != null)
                    {
                        animator.SetTrigger("Common");
                    }

                    return NodeState.SUCCESS;
                }
            }

            return NodeState.RUNNING;
        }
    }

    // --- Patrol ---
    public class Patrol : Node
    {
        private NavMeshAgent agent;
        private Transform[] patrolPoints;
        private int currentIndex = 0;
        private Animator animator;

        public Patrol(NavMeshAgent agent, Transform[] patrolPoints)
        {
            this.agent = agent;
            this.patrolPoints = patrolPoints;
            this.animator = agent.GetComponent<Animator>();
        }

        public override NodeState Evaluate()
        {
            if (patrolPoints.Length == 0)
                return NodeState.FAILURE;

            if (!agent.hasPath || agent.remainingDistance < 0.5f)
            {
                currentIndex = (currentIndex + 1) % patrolPoints.Length;
                agent.SetDestination(patrolPoints[currentIndex].position);
            }

            if (animator != null)
            {
                animator.SetBool("Walk", true); // 👣 play walk anim
            }

            return NodeState.RUNNING;
        }
    }

    // --- Attack ---
    // --- Attack ---
    public class AttackPlayer : Node
    {
        private Transform playerTransform;
        private Transform robotTransform;
        private float attackRange = 2.6f;
        private EnemyAttack enemyAttack;
        private Animator animator;
        private NavMeshAgent agent;

        public AttackPlayer(Transform playerTransform, Transform robotTransform, EnemyAttack enemyAttack)
        {
            this.playerTransform = playerTransform;
            this.robotTransform = robotTransform;
            this.enemyAttack = enemyAttack;
            this.animator = robotTransform.GetComponent<Animator>();
            this.agent = robotTransform.GetComponent<NavMeshAgent>();
        }

        public override NodeState Evaluate()
        {
            float distance = Vector3.Distance(playerTransform.position, robotTransform.position);

            if (distance > attackRange)
            {
                // 👣 Di chuyển đến gần player
                if (agent != null)
                {
                    agent.SetDestination(playerTransform.position);

                    if (animator != null)
                    {
                        animator.SetBool("Walk", true);
                    }
                }

                return NodeState.RUNNING;
            }
            else
            {
                // 🔥 Tấn công
                if (animator != null)
                {
                    animator.SetTrigger("Attack");
           
                    animator.SetBool("Walk", false);
                }

                enemyAttack.TryAttack(playerTransform);
            
                return NodeState.SUCCESS;
            }
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
