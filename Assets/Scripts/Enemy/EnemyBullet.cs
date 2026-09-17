using UnityEngine;

/// <summary>
/// 적이 발사하는 투사체. 플레이어에게만 피해를 준다.
/// (플레이어 총알 Bullet.cs와 분리해두어 서로의 로직에 영향을 주지 않는다.)
/// </summary>
public class EnemyBullet : MonoBehaviour
{
    [SerializeField] private int damage = 5;
    [SerializeField] private float lifeTime = 3f;

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 적끼리는 오사하지 않는다(발사한 본인 포함).
        if (collision.GetComponent<EnemyHealth>() != null) return;

        if (collision.TryGetComponent(out PlayerHealth player))
        {
            player.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        // 트리거가 아닌 콜라이더 = 벽/지형이므로 소멸
        if (!collision.isTrigger)
        {
            Destroy(gameObject);
        }
    }
}
