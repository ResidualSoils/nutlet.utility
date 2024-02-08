using System;
using System.Collections.Generic;
using System.Linq;

namespace Nutlet.Utility
{
    public partial class NObjectPool
    {
        private class ObjectPool<TObject> : INutletObjectPool<TObject>, IObjectPoolBuilder<TObject> where TObject : IPooledObject
        {
            private readonly Func<TObject> _factory;
            private int _capacity;
            private bool _filled;
            private bool _autoDispose;
            private bool _isBuild;
            private List<TObject> _objects;

            public ObjectPool(Func<TObject> factory)
            {
                _factory = factory;
            }

            public TObject Get()
            {
                foreach (var obj in ObjectsIterator())
                {
                    if (obj.IsActive) 
                        continue;
                    
                    obj.Active();
                    return obj;
                }

                return Gen(true);
            }

            public IEnumerable<TObject> AllActive => ObjectsIterator().Where(e => e.IsActive);

            public void Recover(TObject obj)
            {
                foreach (var o in ObjectsIterator())
                {
                    if (o.IsActive && Equals(o, obj))
                        o.Recover();
                }
            }

            public void RecoverAll()
            {
                foreach (var obj in ObjectsIterator())
                {
                    if (obj.IsActive)
                        obj.Recover();
                }
            }

            private TObject Gen(bool isActive)
            {
                var obj = _factory();
                if (obj == null)
                    throw new NullReferenceException("Object is null, which created by pool");
                
                if (isActive) 
                    obj.Active();
                else
                    obj.Recover();
                _objects.Add(obj);
                return obj;
            }

            public IObjectPoolBuilder<TObject> Capacity(int capacity)
            {
                SetCapacity(capacity);
                return this;
            }
            
            public IObjectPoolBuilder<TObject> AutoDispose()
            {
                _autoDispose = true;
                return this;
            }

            public IObjectPoolBuilder<TObject> Filled()
            {
                _filled = true;
                return this;
            }

            public INutletObjectPool<TObject> Build()
            {
                SelfBuild();
                return this;
            }

            public void Dispose()
            {
                foreach (var obj in ObjectsIterator())
                {
                    obj.Dispose();
                }
                _objects.Clear();
            }

            // 使用此方法遍历_objects, 在遍历的过程中处理一些逻辑
            // Do some logic when iterate
            private IEnumerable<TObject> ObjectsIterator()
            {
                for (var i = _objects.Count - 1; i >= 0; i--)
                {
                    var obj = _objects[i];

                    if (_autoDispose && !obj.IsActive && _objects.Count > _capacity)
                    {
                        // Handel the situation that AutoDispose is on
                        // AutoDispose开启状态下额外处理
                        _objects.RemoveAt(i);
                    }
                    else if (obj == null)
                    {
                        // Handle the situation that game object is destroyed
                        // 主要处理MonoBehaviour被摧毁的情况
                        _objects.RemoveAt(i);
                    }
                    else
                    {
                        yield return obj;
                    }
                }
            }

            private void SetCapacity(int capacity)
            {
                _capacity = capacity;
                if (_objects == null)
                    _objects = new List<TObject>(capacity);
                else
                    _objects.Capacity = capacity;
            }

            private void SelfBuild()
            {
                if (_isBuild)
                    throw new Exception("This pool has be build, Should not do it again");
                _isBuild = true;
                
                _objects = new List<TObject>(_capacity);
                if (!_filled)
                    return;
                
                for (var i = 0; i < _capacity; i++)
                {
                    Gen(false);
                }
            }
        }
    }
}