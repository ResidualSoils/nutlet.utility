using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Nutlet.Utility
{
    public static class Extensions
    {
        /// <summary> 递归遍历目录下所有文件 </summary>
        public static FileInfo[] GetFilesRecursively(this DirectoryInfo dir)
        {
            var files = new List<FileInfo>();
            files.AddRange(dir.GetFiles());
            var dirs = dir.GetDirectories();
            foreach (var d in dirs)
            {
                files.AddRange(d.GetFilesRecursively());
            }

            return files.ToArray();
        }

        /// <summary>
        /// 改变HDR颜色的强度
        /// </summary>
        /// <param name="color"></param>
        /// <param name="intensity"></param>
        /// <returns></returns>
        public static Color ChangeIntensity(this Color color, float intensity)
        {
            var hdr = color * Mathf.Pow(Mathf.Clamp(intensity, 0, 10), 2);
            hdr.a = color.a;
            return hdr;
        }
        
        /// <summary>
        /// 获取指定起始字符串后的所有字符
        /// </summary>
        /// <param name="start"> 指定起始字符串 </param>
        /// <param name="containsStart"> 是否包含指定起始字符串 </param>
        /// <returns></returns>
        public static string TakeFrom(this string str, string start, bool containsStart = true)
        {
            var index = str.IndexOf(start, StringComparison.Ordinal);
            if (index < 0)
            {
                return string.Empty;
            }
            else
            {
                return containsStart
                    ? new string(str.TakeFrom(index).ToArray())
                    : new string(str.TakeFrom(index + start.Length).ToArray());
            }
        }
        
        /// <summary> 将字典转换为Java中的HashMap </summary>
        public static AndroidJavaObject ToHashMap<TKey, TValue>(this Dictionary<TKey, TValue> dic)
        {
            var hashMap = new AndroidJavaObject("java.util.HashMap");
            foreach (var kv in dic)
            {
                hashMap.Call<string>("put", kv.Key, kv.Value);
            }

            return hashMap;
        }

        #region --- Collections ---

        /// <summary> 增加一个键值对，如果已有此键，则更新其值 </summary>
        public static void AddOrUpdate<TKey, TValue>(this Dictionary<TKey, TValue> dic, TKey key, TValue value)
        {
            if (dic.ContainsKey(key))
                dic[key] = value;
            else
                dic.Add(key, value);
        }

        /// <summary>
        /// 添加一个序列到链表中
        /// </summary>
        /// <param name="range"> 待添加的序列 </param>
        public static void AddRange<T>(this LinkedList<T> source, IEnumerable<T> range)
        {
            foreach (var t in range)
            {
                source.AddLast(t);
            }
        }
        
        public static void Foreach<T>(this IEnumerable<T> source, Action<T> iterator)
        {
            foreach (var e in source)
            {
                iterator?.Invoke(e);
            }
        }
        
        /// <summary>
        /// 遍历当前以及下一个元素
        /// [a, b, c, d] -> (a, b), (b, c), (c, d)
        /// </summary>
        /// <param name="iterator"> 迭代方法 </param>
        public static void ForeachPair<T>(this IEnumerable<T> source, Action<(T current, T next)> iterator)
        {
            source.ForeachPair((current, next) => iterator?.Invoke((current, next)));
        }
        
        /// <summary>
        /// 遍历当前以及下一个元素
        /// [a, b, c, d] -> (a, b), (b, c), (c, d)
        /// </summary>
        /// <param name="iterator"> 迭代方法 </param>
        public static void ForeachPair<T>(this IEnumerable<T> source, Action<T, T> iterator)
        {
            T current = default;
            var isFirst = true;
            foreach (var next in source)
            {
                if (!isFirst)
                {
                    iterator?.Invoke(current, next);
                }
                current = next;
                isFirst = false;
            }
        }

        /// <summary>
        /// 返回从指定索引开始的序列
        /// </summary>
        /// <param name="startIndex"> 开始的索引 </param>
        public static IEnumerable<T> TakeFrom<T>(this IEnumerable<T> source, int startIndex)
        {
            foreach (var t in source)
            {
                if (startIndex > 0)
                    startIndex--;
                else
                    yield return t;
            }
        }
        
        #endregion

        #region --- Transform ---

        /// <summary> 在x轴移动Transform </summary>
        public static Transform AddX(this Transform t, float x)
        {
            var p = t.position;
            p.x += x;
            t.position = p;
            return t;
        }
        // <summary> 在y轴移动Transform </summary>
        public static Transform AddY(this Transform t, float y)
        {
            var p = t.position;
            p.y += y;
            t.position = p;
            return t;
        }
        // <summary> 在z轴移动Transform </summary>
        public static Transform AddZ(this Transform t, float z)
        {
            var p = t.position;
            p.z += z;
            t.position = p;
            return t;
        }

        /// <summary> 直接改变Transform的x轴位置 </summary>
        public static Transform ChangeX(this Transform t, float x)
        {
            var p = t.position;
            p.x = x;
            t.position = p;
            return t;
        }
        /// <summary> 直接改变Transform的y轴位置 </summary>
        public static Transform ChangeY(this Transform t, float y)
        {
            var p = t.position; p.y = y; t.position = p;
            return t;
        }
        /// <summary> 直接改变Transform的z轴位置 </summary>
        public static Transform ChangeZ(this Transform t, float z)
        {
            var p = t.position;
            p.z = z;
            t.position = p;
            return t;
        }
        
        /// <summary>
        /// 递归寻找所有对应名字的子对象
        /// </summary>
        /// <param name="name"> 子对象名字 <param>
        public static IEnumerable<Transform> DeepFindAll(this Transform root, string name)
        {
            var list = new LinkedList<Transform>();
            for (var i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (child.name == name)
                    list.AddLast(child);

                if (child.childCount > 0)
                    list.AddRange(child.DeepFindAll(name));
            }

            return list.ToArray();
        }

        /// <summary>
        /// 递归寻找所有子对象中第一个对应名字的对象
        /// </summary>
        /// <param name="name"> 子对象名字 <param>
        public static Transform FindRecursively(this Transform root, string name)
        {
            for (var i = 0; i < root.childCount; i++)
            {
                var child = root.GetChild(i);
                if (child.name == name)
                    return child;
                var other = child.FindRecursively(name);
                if (other != null)
                    return other;
            }

            return null;
        }

        #endregion
        
        #region --- Vector ---

        /// <summary> 显示位置 </summary>
        public static void DebugPos(this Vector3 pos)
        {
            Debug.DrawLine(pos, pos + Vector3.up, Color.green, 1, false);
            Debug.DrawLine(pos, pos + Vector3.right, Color.red, 1, false);
            Debug.DrawLine(pos, pos + Vector3.forward, Color.blue, 1, false);
        }

        /// <summary> 顺时针转动一个2维向量 </summary>
        public static Vector2 RotateClockwise(this Vector2 vec, float angle)
        {
            var x = vec.x * Mathf.Cos(angle * Mathf.Deg2Rad) + vec.y * Mathf.Sin(angle * Mathf.Deg2Rad);
            var y = -vec.x * Mathf.Sin(angle * Mathf.Deg2Rad) + vec.y * Mathf.Cos(angle * Mathf.Deg2Rad);
            return new Vector2(x, y);
        }

        #endregion
    }
}