using UnityEngine;
using BT;

/// <summary>
/// Behaviour Tree로 동작하는 적 AI.
///
/// 트리 구조
/// Root [Selector]
///  ├─ [Sequence] 교전
///  │   ├─ DetectTargetNode                      (감지 범위 안 + 히스테리시스)
///  │   └─ [Selector]
///  │       ├─ [Sequence] 원거리 공격
///  │       │   ├─ Condition: 거리 <= attackRange
///  │       │   └─ Cooldown(fireCooldown) → RangedAttackNode
///  │       └─ ChaseNode                         (사거리 밖이거나 쿨다운 중이면 접근)
///  └─ IdleNode                                  (기본 상태)
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class EnemyAI : MonoBehaviour
{
    [Header("타겟")]
    [Tooltip("비워두면 씬에서 PlayerMovement를 가진 오브젝트를 자동으로 찾는다.")]
    [SerializeField] private Transform target;

    [Header("거리 설정 (attackRange < detectRange < loseRange)")]
    [SerializeField] private float detectRange = 8f;   // 이 안으로 들어오면 인지
    [SerializeField] private float loseRange = 10f;    // 이 밖으로 나가야 인지 해제
    [SerializeField] private float attackRange = 6f;   // 이 안이면 발사

    [Header("이동")]
    [SerializeField] private float moveSpeed = 2.5f;

    [Header("원거리 공격")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float fireCooldown = 1.2f;
    [SerializeField] private float bulletSpeed = 8f;
    [SerializeField] private float muzzleOffset = 0.6f; // 자기 콜라이더에 바로 맞지 않도록 띄우는 거리

    [Header("BT")]
    [Tooltip("트리를 평가하는 주기(초). 매 프레임 돌릴 필요가 없다.")]
    [SerializeField] private float tickInterval = 0.1f;

    [Header("디버그")]
    [SerializeField] private bool drawGizmos = true;

    public float DetectRange => detectRange;
    public float LoseRange => loseRange;

    /// <summary>BT 노드가 설정하는 이동 방향. 실제 이동은 FixedUpdate에서 처리한다.</summary>
    public Vector2 MoveInput { get; set; }

    private Rigidbody2D rb;
    private EnemyContext ctx;
    private Node root;
    private float nextTickTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;                                  // 탑다운이므로 중력 제거
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 충돌로 빙글빙글 도는 것 방지

        if (target == null)
        {
            PlayerMovement player = FindFirstObjectByType<PlayerMovement>();
            if (player != null) target = player.transform;
        }

        ctx = new EnemyContext { Ai = this, Self = transform, Target = target };
        root = BuildTree();
    }

    private Node BuildTree()
    {
        return new Selector(
            // 1) 플레이어를 인지한 동안: 사거리면 쏘고, 아니면 접근
            new Sequence(
                new DetectTargetNode(ctx),
                new Selector(
                    new Sequence(
                        new Condition(() => ctx.Distance <= attackRange),
                        new Cooldown(fireCooldown, new RangedAttackNode(ctx))
                    ),
                    new ChaseNode(ctx)
                )
            ),
            // 2) 그 외에는 대기
            new IdleNode(ctx)
        );
    }

    private void Update()
    {
        if (Time.time < nextTickTime) return;
        nextTickTime = Time.time + tickInterval;

        UpdateContext();
        root.Tick();
    }

    /// <summary>틱마다 블랙보드 값을 한 번만 갱신한다.</summary>
    private void UpdateContext()
    {
        ctx.Target = target;

        if (target == null)
        {
            ctx.Distance = float.MaxValue;
            ctx.Direction = Vector2.zero;
            return;
        }

        Vector2 toTarget = (Vector2)target.position - (Vector2)transform.position;
        ctx.Distance = toTarget.magnitude;
        ctx.Direction = ctx.Distance > 0.001f ? toTarget / ctx.Distance : Vector2.zero;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + MoveInput * moveSpeed * Time.fixedDeltaTime);
    }

    /// <summary>RangedAttackNode가 호출. 지정 방향으로 투사체를 1발 생성한다.</summary>
    public void FireBullet(Vector2 direction)
    {
        if (bulletPrefab == null) return;

        Vector3 spawnPos = transform.position + (Vector3)(direction * muzzleOffset);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.Euler(0f, 0f, angle));
        if (bullet.TryGetComponent(out Rigidbody2D bulletRb))
        {
            bulletRb.linearVelocity = direction * bulletSpeed;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!drawGizmos) return;

        Gizmos.color = Color.yellow;                       // 감지
        Gizmos.DrawWireSphere(transform.position, detectRange);
        Gizmos.color = new Color(1f, 0.6f, 0f);            // 인지 해제
        Gizmos.DrawWireSphere(transform.position, loseRange);
        Gizmos.color = Color.red;                          // 사거리
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
