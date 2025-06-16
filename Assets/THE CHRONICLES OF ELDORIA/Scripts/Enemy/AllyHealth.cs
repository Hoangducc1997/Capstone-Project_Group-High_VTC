using UnityEngine;
using UnityEngine.UI;

public class AllyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI - Health Bar")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Canvas healthCanvas;
    private Animator animator;
    public int CurrentHealth => currentHealth;

    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Update()
    {
        // Quay thanh máu về phía camera
        if (healthCanvas != null && Camera.main != null)
        {
            healthCanvas.transform.LookAt(Camera.main.transform);
            healthCanvas.transform.Rotate(0, 180, 0);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthUI();
        Debug.Log($"{gameObject.name} bị trúng đòn! Máu còn: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        UpdateHealthUI();
        Debug.Log($"{gameObject.name} hồi {amount} máu");
    }

    private void UpdateHealthUI()
    {
        if (healthSlider != null)
        {
            healthSlider.value = (float)currentHealth / maxHealth;
        }
    }
    private void Die()
    {
        if (animator != null)
        {
            animator.SetTrigger("Die"); // 👈 Giả sử có trigger anim chết tên "Die"
        }

        AudioManager.Instance.PlayVFX("EnemyDeath");

        Debug.Log($"{gameObject.name} đã chết!");
        Invoke(nameof(DestroySelf), 3f);
    }
    private void DestroySelf()
    {
        Destroy(gameObject);
    }

}
