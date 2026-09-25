using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;

    private void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth <= 0)
        {
            // 플레이어 사망 로직
        }
    }

    private void UpdateHealthUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.UpdateHealthUI(currentHealth, maxHealth);
        }
    }
}