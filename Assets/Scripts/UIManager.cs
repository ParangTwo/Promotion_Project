using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("플레이어 체력 UI")]
    [SerializeField] private Slider healthSlider;

    [Header("적 남은 수 UI")]
    [SerializeField] private TextMeshProUGUI remainingEnemyText;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// 
    /// 체력 바의 최대값과 현재값을 갱신합니다.
    /// 
    public void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthSlider == null) return;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    /// 
    /// 우측 상단 남은 적 수 텍스트를 갱신합니다.
    /// 
    public void UpdateRemainingEnemyText(int remainingCount)
    {
        if (remainingEnemyText == null) return;
        remainingEnemyText.text = $"Remaining : {remainingCount}";
    }
}