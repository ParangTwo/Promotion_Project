using System;
using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    // 스폰 매니저가 적 사망을 감지하기 위한 이벤트
    public event Action OnEnemyDied;

    [Header("체력 설정")]
    [SerializeField] private int maxHealth = 30;
    private int currentHealth;

    [Header("피격 연출 (선택사항)")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color damageColor = Color.red;
    private Color originalColor = Color.white;
    private Coroutine flashCoroutine;

    private void Awake()
    {
        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren(typeof(SpriteRenderer)) as SpriteRenderer;
            }
        }
    }

    private void OnEnable()
    {
        currentHealth = maxHealth;

        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
            flashCoroutine = null;
        }

        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;

        if (spriteRenderer != null)
        {
            if (flashCoroutine != null) StopCoroutine(flashCoroutine);
            flashCoroutine = StartCoroutine(FlashRoutine());
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator FlashRoutine()
    {
        spriteRenderer.color = damageColor;
        yield return new WaitForSeconds(0.1f);
        spriteRenderer.color = originalColor;
        flashCoroutine = null;
    }

    private void Die()
    {
        // 적이 죽을 때 이벤트 발생
        OnEnemyDied?.Invoke();
        Destroy(gameObject);
    }
}