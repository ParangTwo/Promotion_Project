namespace BT
{
    public enum NodeState
    {
        Success,   // 성공: 이 노드의 일이 끝남
        Failure,   // 실패: 조건 불충족 / 실행 불가
        Running    // 진행 중: 다음 틱에도 이어서 실행
    }

    /// <summary>
    /// 모든 BT 노드의 기반 클래스.
    /// Tick()이 OnStart -> OnUpdate -> OnStop 수명주기를 관리하므로
    /// 파생 클래스는 OnUpdate만 구현하면 된다.
    /// </summary>
    public abstract class Node
    {
        /// <summary>직전 Tick에서 Running을 반환해 아직 진행 중인 상태인지</summary>
        public bool IsRunning { get; private set; }

        public NodeState Tick()
        {
            if (!IsRunning) OnStart();

            NodeState state = OnUpdate();

            IsRunning = state == NodeState.Running;
            if (!IsRunning) OnStop();

            return state;
        }

        /// <summary>
        /// 우선순위가 더 높은 가지에 실행권을 빼앗겼을 때 호출.
        /// 진행 중이던 이동/연출을 정리한다.
        /// </summary>
        public void Abort()
        {
            if (!IsRunning) return;

            IsRunning = false;
            OnStop();
            OnAbort();
        }

        protected virtual void OnStart() { }
        protected virtual void OnStop() { }

        /// <summary>자식에게 중단을 전파해야 하는 컴포지트/데코레이터가 재정의한다.</summary>
        protected virtual void OnAbort() { }

        protected abstract NodeState OnUpdate();
    }
}
