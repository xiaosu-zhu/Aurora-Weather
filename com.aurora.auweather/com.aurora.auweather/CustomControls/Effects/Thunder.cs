// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Com.Aurora.AuWeather.Effects
{
    /// <summary>
    /// 闪电类，包含闪电的亮度，持续时间以及规模，同时包含了用于绘制的 Path
    /// http://drilian.com/2009/02/25/lightning-bolts/
    /// </summary>
    public class Thunder
    {
        // 0.5-1
        public float Luminace;
        // from generator
        public float Duration;
        public float scale;

        public Vector2[] Path { get; private set; }
        public Vector2 Position { get; private set; }
        public int LifeLong { get; private set; }
        public float TimeSinceStart { get; private set; }

        public Thunder(float duration, Vector2 size)
        {
            this.Duration = duration;
            this.Path = null;
            Luminace = Tools.RandomBetween(0.5f, 1);
            scale = Tools.RandomBetween(80, 160);
            GeneratePath(size);
        }

        private void GeneratePath(Vector2 size)
        {
            List<Vector2> points = new List<Vector2>();
            var start = new Vector2(0, 0);
            points.Add(start);
            var end = new Vector2(Tools.RandomBetween(size.X * -0.5f, size.X * 1.5f), Tools.RandomBetween(size.Y * 0.5f, size.Y));
            points.Add(end);
            List<Vector2> midPoints = new List<Vector2>();
            var mscale = scale;
            for (int j = 0; j < 6; j++)
            {
                for (int i = 0; i < points.Count - 1; i++)
                {
                    var middlePoint = (points[i] + points[i + 1]) / 2;
                    // set offset
                    var nVector = (points[i + 1] - points[i]);
                    nVector /= (float)Math.Sqrt(nVector.X * nVector.X + nVector.Y * nVector.Y);
                    var mem = nVector.X;
                    nVector.X = nVector.Y;
                    nVector.Y = -mem;
                    nVector *= ((float)Tools.Random.Next(4)).CompareTo(1.5f) * Tools.RandomBetween(mscale * 0.85f, mscale * 1.15f);
                    middlePoint += nVector;
                    midPoints.Add(middlePoint);
                }
                for (int i = 0; i < midPoints.Count; i++)
                {
                    points.Insert(2 * i + 1, midPoints[i]);
                }
                mscale /= 2;
                midPoints.Clear();
            }
            this.Path = points.ToArray();
            points.Clear();
            points = null;
        }

        internal void Update(float elapsedTime)
        {
            if (LifeLong < Path.Length)
            {
                LifeLong += 22;
                if (LifeLong > Path.Length)
                {
                    LifeLong = Path.Length;
                }
            }
            TimeSinceStart += elapsedTime;
        }

        /// <summary>
        /// 在画布的某一位置布置元素
        /// </summary>
        /// <param name="size"></param>
        internal void Generate(Vector2 size)
        {
            this.Position = new Vector2(Tools.RandomBetween(size.X * 0.2f, size.X * 0.8f), 0);
            this.LifeLong = 0;
            TimeSinceStart = 0;
        }
    }
}
