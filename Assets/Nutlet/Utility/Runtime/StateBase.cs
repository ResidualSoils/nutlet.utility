using System;

namespace Nutlet.Utility
{
    /// <summary>
    /// 继承此类实现状态机中的状态
    /// </summary>
    /// <typeparam name="THost"> 状态机宿主类型 </typeparam>
    public abstract class StateBase<THost> : IDisposable
    {
        protected StateMachine<THost> StateMachine { get; private set; }
        protected THost Host => StateMachine.Host;

        internal void SetMachine(StateMachine<THost> machine)
        {
            StateMachine = machine;
        }

        internal void StateStart() => Start();

        internal void StateUpdate() => Update();

        internal void StateFinish() => Finish();

        /// <summary> 状态进入时会调用一次, 无需实现 base.Start() </summary>
        protected virtual void Start() { }

        /// <summary> 状态更新时调用, 无需实现 base.Update() </summary>
        protected virtual void Update() { }

        /// <summary> 状态结束时调用，如果切换状态时 isForce 为 true 则不会调用, 无需实现 base.Finish() </summary>
        protected virtual void Finish() { }

        /// <summary> 可实现 Dispose 方法，无需实现 base.Dispose() </summary>
        public virtual void Dispose() { }
    }
}
