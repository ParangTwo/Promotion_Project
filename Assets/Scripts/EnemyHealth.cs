using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 30;
    private int currentHealth;

    private void Awake()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log($"적 피격! 남은 체력: {currentHealth}");

        // 피격 효과음 또는 반짝임 연출 추가 가능

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        // 사망 효과음 / 드롭 아이템 / 파티클 로직 구현 위치
        Destroy(gameObject);
    }
}