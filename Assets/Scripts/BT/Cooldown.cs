using UnityEngine;

namespace BT
{
    /// <summary>
    /// 쿨다운 데코레이터. 자식이 Success를 반환한 뒤 duration초 동안은 Failure를 반환한다.
    /// 공격 간격 제어에 사용.
    /// </summary>
    public class Cooldown : Node
    {
        private readonly Node child;
        private readonly float duration;
        private float nextAllowedTime;

        public Cooldown(float duration, Node child)
        {
            this.duration = duration;
            this.child = child;
        }

        protected override NodeState OnUpdate()
        {
            // 이미 진행 중인 연출은 쿨다운과 무관하게 끝까지 실행시킨다.
            if (!child.IsRunning && Time.time < nextAllowedTime) return NodeState.Failure;

            NodeState state = child.Tick();
            if (state == NodeState.Success) nextAllowedTime = Time.time + duration;

            return state;
        }

        protected override void OnAbort() => child.Abort();
    }
}
