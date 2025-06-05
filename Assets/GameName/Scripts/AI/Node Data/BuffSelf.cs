// --- BuffSelf.cs ---
using UnityEngine;
using Pathfinding;

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

    public BuffSelf(
        Transform self,
        Transform healPoint,
        float cooldown,
        System.Func<float> getLastTime,
        System.Action<float> setLastTime,
        EnemyHealth enemyHealth,
        GameObject allyPrefab,
        Transform summonPoint,
        AIPath aiPath,
        Animator animator)
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
