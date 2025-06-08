using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float delayHitTime = 0.5f; // delay trước khi gây sát thương
    [SerializeField] private string playerTag = "Player";

    private bool canAttack = true;
    private Transform currentTarget;

    public void TryAttack(Transform target)
    {
        if (!canAttack) return;

        currentTarget = target;
        canAttack = false;

        Invoke(nameof(DealDamage), delayHitTime);
        Invoke(nameof(ResetAttack), attackCooldown);
    }

    private void DealDamage()
    {
        if (currentTarget == null) return;

        float distance = Vector3.Distance(transform.position, currentTarget.position);
        if (distance > 2f) return;

        PlayerInfo playerInfo = currentTarget.GetComponent<PlayerInfo>();
        if (playerInfo != null)
        {
            playerInfo.TakeDamage(attackDamage);
            Debug.Log($"{gameObject.name} gây sát thương cho người chơi!");
        }
    }

    private void ResetAttack()
    {
        canAttack = true;
        currentTarget = null;
    }
}
