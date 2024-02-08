using System;
using UnityEngine;

namespace Nutlet.Utility
{
    /// <summary> 继承MonoBehaviour的对象池对象基类，IsActive和gameObject.activeSelf绑定 </summary>
    public abstract class PooledObjectMono : MonoBehaviour, IPooledObject
    {
        public bool IsActive => gameObject != null && gameObject.activeSelf;
        
        private bool _isDisposed;
        private bool _isActive;

        public void Active()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(PooledObjectMono));
            
            OnActive();
            gameObject.SetActive(true);
        }

        public void Recover()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(PooledObjectMono));
            if (!IsActive) 
                return;
            
            OnRecover();
            if (gameObject != null)
                gameObject.SetActive(false);
        }

        public void Dispose()
        {            
            if (_isDisposed)
                return;
            
            Recover();
            OnDispose();
            Destroy(gameObject);

            _isDisposed = true;
        }

        /// <summary> 当对象激活时的处理，无需继承base.OnActive() </summary>
        protected virtual void OnActive() { }
        /// <summary> 当对象回收时的处理，无需继承base.OnRecover() </summary>
        protected virtual void OnRecover() { }
        /// <summary> 当对象释放时的处理，无需继承base.OnDispose() </summary>
        protected virtual void OnDispose() { }
    }
}