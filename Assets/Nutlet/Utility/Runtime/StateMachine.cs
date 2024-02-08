using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nutlet.Utility
{
    /// <summary>
    /// 有限状态机
    /// </summary>
    /// <typeparam name="THost"> 状态机宿主类型 </typeparam>
    public class StateMachine<THost>
    {
        private readonly Dictionary<Type, StateBase<THost>> _states = new Dictionary<Type, StateBase<THost>>();
        private readonly THost _host;
        private StateBase<THost> _entry;
        private StateBase<THost> _now;
        private StateBase<THost> _prev;

        private bool _isDisposed;
        private bool _isStarted;
        
        /// <summary> 状态机宿主 </summary>
        public THost Host => _host;
        /// <summary> 任一状态开始事件（在其他所有状态之前执行） </summary>
        public event Action AnyStateStart;
        /// <summary> 任一状态更新事件（在其他所有状态之前执行） </summary>
        public event Action AnyStateUpdate;
        /// <summary> 任一状态结束事件（在其他所有状态之前执行） </summary>
        public event Action AnyStateFinish;

        public StateMachine(THost host)
        {
            _host = host;
        }

        /// <summary> 向状态机中增加状态 </summary>
        public StateMachine<THost> AddStates(params StateBase<THost>[] states)
        {
            CheckDispose();
            
            if (states.Length <= 0) 
                return this;

            if (_entry == null)
            {
                _entry = states[0];
                _now = _entry;
                _prev = _entry;
            }

            foreach (var state in states)
            {
                if (_states.ContainsValue(state))
                {
                    NAssert.TraceWarning($"multiple state type -- {state.GetType().Name}");
                }
                else
                {
                    state.SetMachine(this);
                    _states.Add(state.GetType(), state);
                }
            }

            return this;
        }
        
        /// <summary> 开始运行状态机 </summary>
        public void StartMachine()
        {
            CheckDispose();
            
            AnyStateStart?.Invoke();
            _entry.StateStart();
            _isStarted = true;
        }

        /// <summary> 状态机更新方法 </summary>
        public void UpdateMachine()
        {
            CheckDispose();
            
            if (!_isStarted) 
                return;

            AnyStateUpdate?.Invoke();
            _now.StateUpdate();
        }

        /// <summary> 切换状态 </summary>
        /// <param name="isForce"> 是否强制切换，可强制切换到当前状态 </param>
        /// <typeparam name="TState"> 切换的类型 </typeparam>
        /// <exception cref="StateNotFoundException"> 当前状态机中没有指定状态 </exception>
        public void ChangeState<TState>(bool isForce = false) where TState : StateBase<THost>
        {
            CheckDispose();
            
            if (!_states.TryGetValue(typeof(TState), out var state))
            {
                throw new StateNotFoundException($"No state {typeof(TState).Name} in StateMachine<{typeof(THost).Name}>");
            }

            if (_now is TState && !isForce)
            {
                NAssert.TraceWarning($"Can't change state {typeof(TState).Name} to it self on StateMachine<{typeof(THost).Name}>, or use force change");
                return;
            }

            if (!isForce)
            {
                AnyStateFinish?.Invoke();
                _now.StateFinish();
            }

            _prev = _now;
            _now = state;

            AnyStateStart?.Invoke();
            _now.StateStart();
        }

        public void Dispose()
        {
            if (_isDisposed) 
                return;
            
            AnyStateStart = null;
            AnyStateUpdate = null;
            AnyStateFinish = null;
            
            try
            {
                _states.Values.Foreach(s => s.Dispose());
                _states.Clear();
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                throw;
            }
            finally
            {
                _isDisposed = true;
            }
        }

        private void CheckDispose()
        {
            if (_isDisposed)
                throw new ObjectDisposedException($"StateMachine<{typeof(THost).Name}>");
        }
    }
    
    public class StateMachine
    {
        /// <summary>
        /// 创建一个状态机
        /// </summary>
        /// <param name="host"> 状态机宿主 </param>
        /// <typeparam name="THost"> 状态机宿主类型 </typeparam>
        public static StateMachine<THost> Create<THost>(THost host) => new StateMachine<THost>(host);
    }

    public class StateNotFoundException : Exception
    {
        public StateNotFoundException()
        {
        }

        public StateNotFoundException(string message) : base(message)
        {
        }

        public StateNotFoundException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}