using System;

namespace BT
{
    /// <summary>
    /// 람다로 판정하는 조건 노드. 참이면 Success, 거짓이면 Failure.
    /// 예) new Condition(() => ctx.Distance &lt;= 3f)
    /// </summary>
    public class Condition : Node
    {
        private readonly Func<bool> predicate;

        public Condition(Func<bool> predicate)
        {
            this.predicate = predicate;
        }

        protected override NodeState OnUpdate()
        {
            return predicate() ? NodeState.Success : NodeState.Failure;
        }
    }
}
