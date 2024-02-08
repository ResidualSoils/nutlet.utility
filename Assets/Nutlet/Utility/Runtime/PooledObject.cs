using System;

namespace Nutlet.Utility
{
    /// <summary> 普通对象池对象积累，继承此类用以使用在NObjectPool对象池中 </summary>
    public abstract class PooledObject : IPooledObject
    {
        public bool IsActive { get; private set; }
        private bool _isDisposed;
        
        public void Active()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(PooledObject));
            
            IsActive = true;
            OnActive();
        }
        
        public void Recover()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(PooledObject));
            if (!IsActive) 
                return;
            
            OnRecover();
            IsActive = false;
        }

        public void Dispose()
        {
            if (_isDisposed)
                return;
            
            Recover();
            OnDispose();
            
            _isDisposed = true;
        }
        
        /// <summary> 当对象激活时的处理，无需继承base.OnActive() </summary>
        protected virtual void OnActive() { }
        /// <summary> 当对象激活时的处理，无需继承base.OnRecover() </summary>
        protected virtual void OnRecover() { }
        /// <summary> 当对象激活时的处理，无需继承base.OnDispose() </summary>
        protected virtual void OnDispose() { }
    }
}