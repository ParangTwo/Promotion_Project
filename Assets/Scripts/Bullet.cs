using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 3f; // 화면 밖으로 넘어가서 안 파괴되는 것 방지

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 1. 무엇인가와 충돌했는지 확인
        Debug.Log($"총알이 무언가와 부딪힘: {collision.name} (태그: {collision.tag})");

        if (collision.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            if (collision.TryGetComponent(out EnemyHealth enemy))
            {
                enemy.TakeDamage(damage);
            }
            else
            {
                Debug.LogWarning("Enemy 태그는 맞지만, EnemyHealth 스크립트를 찾을 수 없습니다!");
            }
            Destroy(gameObject);
        }
    }
}