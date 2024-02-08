using System;
using System.Collections.Generic;

namespace Nutlet.Utility
{
    /// <summary> 无状态的可观察对象 </summary>
    public class NSubject<T> : IObservable<T>, IDisposable
    {
        private readonly LinkedList<IObserver<T>> _observers = new LinkedList<IObserver<T>>();
        private readonly object _observerLock = new object();
        private bool _isDisposed;

        /// <summary> 向所有观察者提供新数据 </summary>
        public void OnNext(T value)
        {
            if (_isDisposed) 
                throw new ObjectDisposedException("Subject is disposed");

            foreach (var observer in _observers)
            {
                observer.OnNext(value);
            }
        }

        /// <summary> 通知所有观察者错误 </summary>
        public void OnError(Exception e)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Subject is disposed");

            foreach (var observer in _observers)
            {
                observer.OnError(e);
            }
        }

        /// <summary> 通知所有观察者已完成 </summary>
        public void OnCompleted()
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Subject is disposed");

            foreach (var observer in _observers)
            {
                observer.OnCompleted();
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("Subject is disposed");
            if (observer == null)
                throw new ArgumentNullException(nameof(observer));

            lock (_observerLock)
            {
                return new Disposable(observer, this);
            }
        }

        public void Dispose()
        {
            lock (_observerLock)
            {
                _isDisposed = true;
                _observers.Clear();
            }
        }

        private class Disposable : IDisposable
        {
            private IObserver<T> _observer;
            private readonly NSubject<T> _subject;

            public Disposable(IObserver<T> observer, NSubject<T> subject)
            {
                _observer = observer;
                _subject = subject;

                _subject._observers.AddLast(observer);
            }

            public void Dispose()
            {
                lock (_subject._observerLock)
                {
                    _subject._observers.Remove(_observer);
                }
            }
        }
    }
}