using UnityEngine;

namespace Nutlet.Utility
{
    /// <summary>
    /// 继承此类以实现MonoBehaviour的单例模式
    /// </summary>
    /// <typeparam name="T"> 子类类型 </typeparam>
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        private static readonly object _creationLock = new object();
        private static bool _isInit;

        /// <summary> 获取单例实例（Lazy模式） </summary>
        public static T Instance
        {
            get
            {
                if (!_isInit)
                {
                    CreateInstance();
                }
                return _instance;
            }
        }

        private static void CreateInstance()
        {
            lock (_creationLock)
            {
                if (_isInit)
                    return;
                _isInit = true;
            
                var instances = FindObjectsOfType<T>(true);
                if (instances.Length > 1)
                    Debug.LogWarning($"There are multiple instance ({typeof(T).Name}) in scene");

                if (instances.Length == 0)
                {
                    var go = new GameObject($"{typeof(T).Name}");
                    _instance = go.AddComponent<T>();
                }
                else
                {
                    _instance = instances[0];
                }
            
                DontDestroyOnLoad(_instance);
            }
        }
    }
}