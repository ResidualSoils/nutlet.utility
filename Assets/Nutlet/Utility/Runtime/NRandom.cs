using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Nutlet.Utility
{
    public static class NRandom
    {
        [ThreadStatic]
        private static Random _rdm;

        private static Random Rdm => _rdm ??= new Random();

        #region --- Basic ---

        public static int Next() => Rdm.Next();

        public static int Next(int max) => Rdm.Next(max);

        public static int Next(int min, int max) => Rdm.Next(min, max);

        public static double NextDouble() => Rdm.NextDouble();

        public static double NextDouble(double max)
        {
            if (max < 0)
                throw new ArgumentOutOfRangeException(nameof(max));
            return Rdm.NextDouble() * max;
        }

        public static double NextDouble(double min, double max)
        {
            if (min > max)
                throw new ArgumentException("min can't larger than max");
            return min + Rdm.NextDouble() * (max - min);
        }

        public static float NextFloat() => (float)NextDouble();

        public static float NextFloat(float max) => NextFloat() * max;

        public static float NextFloat(float min, float max) => min + NextFloat() * (max - min);

        #endregion

        #region --- Public ---

        /// <summary> 返回alpha为1的随机颜色 </summary>
        public static Color RandomColorRGB(float alpha = 1)
        {
            return new Color(NextFloat(), NextFloat(), NextFloat(), alpha);
        }

        /// <summary> 返回 +1 或 -1 </summary>
        public static int RandomDirectedNumber() => Rdm.Next(-1, 1) == 0 ? 1 : -1;

        public static Vector3 RandomNormalizedVector3()
        {
            return new Vector3(NextFloat(-1, 1), NextFloat(-1, 1), NextFloat(-1, 1)).normalized;
        }

        public static Vector2 RandomNormalizedVector2()
        {
            return Vector2.up.RotateClockwise(NextFloat(360));
        }

        /// <summary>
        /// 生成一个指定长度的随机值数组
        /// </summary>
        /// <param name="min"> 最小值（包含） </param>
        /// <param name="max"> 最大值（不包含） </param>
        /// <param name="length"> 生成的数组长度 </param>
        public static int[] GenerateRandomIntArray(int min, int max, int length)
        {
            var result = new int[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = Rdm.Next(min, max);
            }

            return result;
        }

        /// <summary>
        /// 生成一个指定长度的随机值数组
        /// </summary>
        /// <param name="min"> 最小值 </param>
        /// <param name="max"> 最大值 </param>
        /// <param name="length"> 生成的数组长度 </param>
        public static float[] GenerateRandomFloatArray(float min, float max, int length)
        {
            var result = new float[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = NextFloat(min, max);
            }

            return result;
        }

        #endregion
        
        #region --- Extensions ---
        
        /// <summary> 随机返回集合中的一个值 </summary>
        public static T RandomOne<T>(this IEnumerable<T> source)
        {
            var count = source switch
            {
                T[] array => array.Length,
                ICollection<T> collection => collection.Count,
                _ => source.Count()
            };
            
            var rdm = Next(count);
            return source.ElementAt(rdm);
        }

        /// <summary>
        /// 根据权重随机返回集合中的一个值
        /// </summary>
        /// <param name="selectWeight"> 获取权重的回调 </param>
        /// <exception cref="ArgumentNullException"> 权重回调为空 </exception>
        /// <exception cref="InvalidOperationException"> 集合发生变化 </exception>
        public static T RandomOneByWeight<T>(this IEnumerable<T> source, Func<T, float> selectWeight)
        {
            if (selectWeight == null)
                throw new ArgumentNullException(nameof(selectWeight));

            var sum = 0f;
            foreach (var e in source)
            {
                var weight = selectWeight(e);
                if (weight < 0)
                    throw new InvalidOperationException("Weight can't less than 0.");
                sum += selectWeight(e);
            }
            
            var rdm = NextFloat(sum);
            foreach (var e in source)
            {
                rdm -= selectWeight(e);
                if (rdm < float.Epsilon)
                    return e;
            }

            throw new InvalidOperationException("Enumeration may be multiple");
        }
        
        /// <summary> 打乱此集合 </summary>
        public static IList<T> ToRandom<T>(this IList<T> list)
        {
            for (var i = 0; i < list.Count; i++)
            {
                var temp = list[i];
                var index = Rdm.Next(0, list.Count);
                list[i] = list[index];
                list[index] = temp;
            }

            return list;
        }

        /// <summary> 打乱此集合 </summary>
        public static T[] ToRandom<T>(this T[] array)
        {
            for (var i = 0; i < array.Length; i++)
            {
                var temp = array[i];
                var index = Rdm.Next(0, array.Length);
                array[i] = array[index];
                array[index] = temp;
            }

            return array;
        }
        
        /// <summary> 打乱此集合 </summary>
        public static IEnumerable<T> ToRandom<T>(this IEnumerable<T> enumerable)
        {
            var array = enumerable.ToArray();
            return array.ToRandom();
        }

        #endregion
    }
}