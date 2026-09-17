using UnityEngine;
using BT;

/// <summary>
/// 플레이어 감지. detectRange 안으로 들어오면 인지하고,
/// loseRange 밖으로 나가야 인지를 푼다(히스테리시스).
/// 경계선에서 추격/대기가 매 틱 번갈아 바뀌는 현상을 막기 위한 장치다.
/// </summary>
public class DetectTargetNode : Node
{
    private readonly EnemyContext ctx;

    public DetectTargetNode(EnemyContext ctx) => this.ctx = ctx;

    protected override NodeState OnUpdate()
    {
        if (!ctx.HasTarget)
        {
            ctx.IsAware = false;
            return NodeState.Failure;
        }

        float threshold = ctx.IsAware ? ctx.Ai.LoseRange : ctx.Ai.DetectRange;
        ctx.IsAware = ctx.Distance <= threshold;

        return ctx.IsAware ? NodeState.Success : NodeState.Failure;
    }
}

/// <summary>
/// 타겟 방향으로 이동. 도착 개념이 없으므로 항상 Running을 반환하고,
/// 상위에서 중단(Abort)될 때 이동 입력을 0으로 되돌린다.
/// </summary>
public class ChaseNode : Node
{
    private readonly EnemyContext ctx;

    public ChaseNode(EnemyContext ctx) => this.ctx = ctx;

    protected override NodeState OnUpdate()
    {
        ctx.Ai.MoveInput = ctx.Direction;
        return NodeState.Running;
    }

    protected override void OnStop() => ctx.Ai.MoveInput = Vector2.zero;
}

/// <summary>
/// 제자리에 멈춰 타겟 방향으로 투사체를 1발 발사하고 Success.
/// 발사 간격은 상위의 Cooldown 데코레이터가 관리한다.
/// </summary>
public class RangedAttackNode : Node
{
    private readonly EnemyContext ctx;

    public RangedAttackNode(EnemyContext ctx) => this.ctx = ctx;

    protected override NodeState OnUpdate()
    {
        ctx.Ai.MoveInput = Vector2.zero;
        ctx.Ai.FireBullet(ctx.Direction);
        return NodeState.Success;
    }
}

/// <summary>기본 상태. 아무것도 하지 않고 항상 Running(트리의 마지막 가지).</summary>
public class IdleNode : Node
{
    private readonly EnemyContext ctx;

    public IdleNode(EnemyContext ctx) => this.ctx = ctx;

    protected override NodeState OnUpdate()
    {
        ctx.Ai.MoveInput = Vector2.zero;
        return NodeState.Running;
    }
}
