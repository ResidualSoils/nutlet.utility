using System;
using System.Collections.Generic;

namespace Nutlet.Utility
{
    /// <summary> 继承了此接口的类才可以使用NObjectPool </summary>
    public interface IPooledObject : IDisposable
    {
        /// <summary> 对象是否为激活状态 </summary>
        bool IsActive { get; }
        /// <summary> 激活对象 </summary>
        void Active();
        /// <summary> 回收对象 </summary>
        void Recover();
    }

    /// <summary> 对象池接口 </summary>
    public interface INutletObjectPool<TObject> : IDisposable where TObject : IPooledObject
    {
        /// <summary> 从对象池中获取一个对象 </summary>
        TObject Get();
        /// <summary> 所有处于激活（IsActive = true）状态的对象 </summary>
        IEnumerable<TObject> AllActive { get; }
        /// <summary> 回收指定对象 </summary>
        void Recover(TObject obj);
        /// <summary> 回收所有对象 </summary>
        void RecoverAll();
    }
    
    /// <summary> 对象池构建选项接口 </summary>
    public interface IObjectPoolBuilder<TObject> where TObject : IPooledObject
    {
        /// <summary> 设置对象池容量 </summary>
        /// <param name="capacity"> 对象池容量 </param>
        IObjectPoolBuilder<TObject> Capacity(int capacity);
        /// <summary> 自动释放模式，开启后超过容量的对象在回收后会被释放 </summary>
        IObjectPoolBuilder<TObject> AutoDispose();
        /// <summary> 对象池会初始生成对象，达到容量的数量 </summary>
        IObjectPoolBuilder<TObject> Filled();
        /// <summary> 构建对象池，只能调用一次 </summary>
        INutletObjectPool<TObject> Build();
    }

    /// <summary> 用于创建对象池的工具 </summary>
    public partial class NObjectPool
    {
        /// <summary>
        /// 从一个工厂方法开始创建对象池
        /// </summary>
        /// <param name="factory"> 无参工厂方法 </param>
        /// <typeparam name="TObject"> 对象池内对象类型，应继承IPooledObject </typeparam>
        /// <exception cref="ArgumentNullException"> 工厂方法不能为null </exception>
        public static IObjectPoolBuilder<TObject> From<TObject>(Func<TObject> factory) where TObject : IPooledObject
        {
            if (factory == null)
                throw new ArgumentNullException(nameof(factory));
            
            return new ObjectPool<TObject>(factory);
        }
    }
}