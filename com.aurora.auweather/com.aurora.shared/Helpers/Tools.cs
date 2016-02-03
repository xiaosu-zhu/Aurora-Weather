using System;
using System.Collections.Generic;
using System.Linq;

namespace Com.Aurora.Shared.Helpers
{
    /// <summary>
    /// 包含各种常用静态方法的封装
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// 把枚举值放入一个列表中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<T> GetEnumAsList<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>().ToList();
        }

        /// <summary>
        /// 角度转弧度
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float DegreesToRadians(float angle)
        {
            return angle * (float)Math.PI / 180;
        }

        /// <summary>
        /// 弧度转角度
        /// </summary>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static float RadiansToDegrees(float angle)
        {
            return angle * 180 / (float)Math.PI;
        }

        static Random random = new Random(Guid.NewGuid().GetHashCode());

        public static Random Random
        {
            get { return random; }
        }

        /// <summary>
        /// 得到一个区间内的随机数
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float RandomBetween(float min, float max)
        {
            return min == max ? min : min + (float)random.NextDouble() * (max - min);
        }
    }
}
