using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour
{
    private Animator animator;
    public int CurrentHealth => currentHealth;

    public int maxHealth = 100;
    private int currentHealth;

    [Header("UI - Health Bar")]
    [SerializeField] private Slider healthSlider;
    [SerializeField] private Canvas healthCanvas;


    private void Start()
    {
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    private void Update()
    {
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

        Debug.Log(gameObject.name + " bị tấn công! Máu còn lại: " + currentHealth);

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
        Debug.Log("Hồi " + amount + " máu, còn lại: " + currentHealth);
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
            animator.SetTrigger("Die");
        }

        AudioManager.Instance.PlayVFX("EnemyDeath");

        Debug.Log($"{gameObject.name} đã chết!");
    }
  
}
