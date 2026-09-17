using UnityEngine;

/// <summary>
/// BT 노드들이 공유하는 블랙보드.
/// 거리/방향 같은 값은 매 틱 EnemyAI가 한 번만 계산해서 여기에 채워 넣고,
/// 각 노드는 계산 없이 읽기만 한다.
/// </summary>
public class EnemyContext
{
    public EnemyAI Ai;
    public Transform Self;
    public Transform Target;

    /// <summary>타겟까지의 거리</summary>
    public float Distance;

    /// <summary>타겟 방향(정규화)</summary>
    public Vector2 Direction;

    /// <summary>플레이어를 인지한 상태인지(히스테리시스 적용됨)</summary>
    public bool IsAware;

    public bool HasTarget => Target != null;
}
