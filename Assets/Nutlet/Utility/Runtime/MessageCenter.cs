using System;
using System.Collections.Generic;

namespace Nutlet.Utility
{
    /// <summary> 实现发布订阅模式 </summary>
    public class MessageCenter : IDisposable
    {
        private readonly Dictionary<Type, IDisposable> _publishersDic = new Dictionary<Type, IDisposable>();
        private bool _isDisposed;
        
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="msg"> 消息 </param>
        /// <param name="isCompleted"> 基于IObservable的Completed选项，默认为false </param>
        /// <typeparam name="T"> 消息类型 </typeparam>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"> 实例已经被Dispose </exception>
        public void Post<T>(T msg, bool isCompleted = false)
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(MessageCenter));
            
            var t = typeof(T);
            if (_publishersDic.ContainsKey(t) && _publishersDic[t] is NSubject<T> sub)
            {
                sub.OnNext(msg);
                if (isCompleted)
                    sub.OnCompleted();
            }
        }

        /// <summary>
        /// 注册消息
        /// </summary>
        /// <typeparam name="T"> 消息类型 </typeparam>
        /// <returns></returns>
        /// <exception cref="ObjectDisposedException"> 实例已经被Dispose </exception>
        public IObservable<T> Register<T>()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(nameof(MessageCenter));
            
            var t = typeof(T);
            if (_publishersDic.ContainsKey(t) && _publishersDic[t] is NSubject<T> sub)
                return sub;

            var newSub = new NSubject<T>();
            _publishersDic.Add(t, newSub);
            return newSub;
        }

        public void Dispose()
        {
            _publishersDic.Foreach(kv => kv.Value.Dispose());
            _publishersDic.Clear();
        }
    }
}