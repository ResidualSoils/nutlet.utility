using System;
using System.Collections.Generic;
using UnityEngine;

namespace Nutlet.Utility
{
    public class NTimer : MonoSingleton<NTimer>
    {
        private readonly List<TimerBase> _timers = new List<TimerBase>();

        /// <summary> 延迟delay秒后执行 </summary>
        public ITimer Delay(float delay, Action action)
        {
            var info = new DelayInfo(action, delay);
            _timers.Add(info);
            return info;
        }

        /// <summary> 延迟frame帧后执行 </summary>
        public ITimer DelayFrame(int frame, Action action)
        {
            var info = new FrameInfo(action, frame);
            _timers.Add(info);
            return info;

        }

        /// <summary> 间隔interval秒执行，isStop返回true时停止 </summary>
        public ITimer Interval(float interval, Func<bool> isStop, Action action)
        {
            var info = new IntervalInfo(action, isStop, () => interval);
            _timers.Add(info);
            return info;
        }

        /// <summary> 间隔interval秒执行 </summary>
        public ITimer Interval(float interval, Action action)
        {
            var info = new IntervalInfo(action, () => false, () => interval);
            _timers.Add(info);
            return info;
        }

        /// <summary> 间隔interval返回的秒数执行，isStop返回true时停止 </summary>
        public ITimer Interval(Func<float> interval, Func<bool> isStop, Action action)
        {
            var info = new IntervalInfo(action, isStop, interval);
            _timers.Add(info);
            return info;
        }

        /// <summary> 间隔interval返回的秒数执行 </summary>
        public ITimer Interval(Func<float> interval, Action action)
        {
            var info = new IntervalInfo(action, () => false, interval);
            _timers.Add(info);
            return info;
        }

        private void Update()
        {
            for (var i = _timers.Count - 1; i >= 0; i--)
            {
                if (_timers[i].IsDisposed)
                {
                    _timers.RemoveAt(i);
                    continue;
                }

                if (_timers[i].IsPause)
                    continue;

                _timers[i].Update();
            }
        }

        private abstract class TimerBase : ITimer
        {
            public bool IsDisposed;
            public bool IsUnscale;
            public bool IsPause;

            public void SetUnscale() => IsUnscale = true;

            public abstract void Reset();

            public void Pause() => IsPause = true;

            public void UnPause() => IsPause = false;

            public void Dispose() => IsDisposed = true;

            public abstract void Update();
        }

        private class DelayInfo : TimerBase
        {
            private readonly Action _action;
            private readonly float _delayDuration;
            private float _timer;

            public DelayInfo(Action action, float delay)
            {
                _action = action;
                _delayDuration = delay;
            }

            public override void Reset() => _timer = 0;

            public override void Update()
            {
                if (_timer > _delayDuration)
                {
                    _action?.Invoke();
                    IsDisposed = true;
                }
                else
                {
                    _timer += IsUnscale ? Time.unscaledDeltaTime : Time.deltaTime;
                }
            }
        }

        private class IntervalInfo : TimerBase
        {
            private readonly Action _action;
            private readonly Func<float> _interval;
            private readonly Func<bool> _isStop;
            private float _timer;

            public IntervalInfo(Action action, Func<bool> isStop, Func<float> interval)
            {
                _action = action;
                _isStop = isStop;
                _interval = interval;
            }

            public override void Reset() => _timer = 0;

            public override void Update()
            {
                if (_isStop())
                {
                    IsDisposed = true;
                }
                else if (_timer > _interval?.Invoke())
                {
                    _action?.Invoke();
                    _timer = 0;
                }
                else
                {
                    _timer += IsUnscale ? Time.unscaledDeltaTime : Time.deltaTime;
                }
            }
        }

        private class FrameInfo : TimerBase
        {
            private readonly Action _action;
            private readonly int _delayFrame;
            private int _timer;

            public FrameInfo(Action action, int frame)
            {
                _action = action;
                _delayFrame = frame;
            }

            public override void Reset() => _timer = 0;

            public override void Update()
            {
                if (_timer > _delayFrame)
                {
                    _action?.Invoke();
                    IsDisposed = true;
                }
                else
                {
                    _timer += 1;
                }
            }
        }
    }

    public interface ITimer : IDisposable
    {
        /// <summary> this timer will loop in FixedUpdate </summary>
        void SetUnscale();

        /// <summary> reset timer </summary>
        void Reset();

        void Pause();
        void UnPause();
    }
}