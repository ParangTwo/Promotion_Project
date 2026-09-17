namespace BT
{
    public abstract class Composite : Node
    {
        protected readonly Node[] children;
        protected int runningIndex = -1;

        protected Composite(params Node[] children)
        {
            this.children = children;
        }

        /// <summary>exceptIndex를 제외한 진행 중 자식을 중단시킨다.</summary>
        protected void AbortRunningChild(int exceptIndex = -1)
        {
            if (runningIndex >= 0 && runningIndex != exceptIndex)
            {
                children[runningIndex].Abort();
            }
            runningIndex = -1;
        }

        protected override void OnAbort() => AbortRunningChild();
    }

    /// <summary>
    /// 우선순위 선택자. 자식 중 하나라도 Failure가 아닌 결과를 낼 때까지 위에서부터 실행한다.
    /// 매 틱 항상 첫 번째 자식부터 다시 평가하므로, 조건이 바뀌면
    /// 위쪽(= 우선순위 높은) 가지가 아래쪽 가지의 실행권을 빼앗는다.
    /// </summary>
    public class Selector : Composite
    {
        public Selector(params Node[] children) : base(children) { }

        protected override NodeState OnUpdate()
        {
            for (int i = 0; i < children.Length; i++)
            {
                NodeState state = children[i].Tick();
                if (state == NodeState.Failure) continue;

                // 다른 가지가 진행 중이었다면 여기서 중단시킨다.
                if (runningIndex >= 0 && runningIndex != i) children[runningIndex].Abort();

                runningIndex = state == NodeState.Running ? i : -1;
                return state;
            }

            AbortRunningChild();
            return NodeState.Failure;
        }
    }

    /// <summary>
    /// 시퀀스. 자식을 앞에서부터 실행하다가 하나라도 Failure면 즉시 중단한다.
    /// 매 틱 처음부터 재평가하므로 앞쪽 조건이 깨지면 뒤쪽 액션도 함께 멈춘다.
    /// </summary>
    public class Sequence : Composite
    {
        public Sequence(params Node[] children) : base(children) { }

        protected override NodeState OnUpdate()
        {
            for (int i = 0; i < children.Length; i++)
            {
                NodeState state = children[i].Tick();
                if (state == NodeState.Success) continue;

                if (runningIndex >= 0 && runningIndex != i) children[runningIndex].Abort();

                runningIndex = state == NodeState.Running ? i : -1;
                return state;
            }

            AbortRunningChild();
            return NodeState.Success;
        }
    }
}
