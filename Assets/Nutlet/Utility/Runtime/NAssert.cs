using System.Diagnostics;

namespace Nutlet.Utility
{
    public static class NAssert
    {
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Check(bool condition, string message)
        {
            if (!condition)
                UnityEngine.Debug.LogError($"[NAssert] CHECK {message} {DebugLine}");
        }

        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void CheckNull(object obj)
        {
            if (obj == null)
                UnityEngine.Debug.LogError($"[NAssert] CHECK NULL {DebugLine}");
        }
        
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void Trace(string message)
        {
            UnityEngine.Debug.Log("[NAssert] " + message);
        }
        
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void TraceWarning(string message)
        {
            UnityEngine.Debug.LogWarning("[NAssert] " + message);
        }
        
        [Conditional("UNITY_EDITOR"), Conditional("DEVELOPMENT_BUILD")]
        public static void TraceError(string message)
        {
            UnityEngine.Debug.LogError("[NAssert] " + message);
        }

        private static string DebugLine
        {
            get
            {
                var st = new StackTrace(true);
                if (st.FrameCount > 1 && st.GetFrame(1).HasMethod())
                {
                    var frame = st.GetFrame(1);
                    var method = frame.GetMethod();
                    var line = frame.GetFileLineNumber();
                    return $"at line {line} {method.Name}";
                }

                return string.Empty;
            }
        }
    }
}
