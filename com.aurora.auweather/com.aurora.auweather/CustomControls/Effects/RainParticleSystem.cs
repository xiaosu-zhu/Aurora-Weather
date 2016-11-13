// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Models;
using System;
using System.Threading.Tasks;
using System.Numerics;
using Com.Aurora.Shared.Helpers;
using Microsoft.Graphics.Canvas;
using Windows.Foundation;

namespace Com.Aurora.AuWeather.Effects
{
    // 简单雨滴是一个矩形，从画布顶部出发，以一个固定角度和初速度，以 g(?) 的加速度下落，同时，雨滴纹理的旋转角度应与运动方向相同
    // 下落到画布底部后，粒子被回收。同时，上方的出发位置是随机的，雨的规模(计算加速度、速度、缩放和角度)可以被用户设置
    public class RainParticleSystem : ParticleSystem
    {
        private RainLevel rainLevel;
        private CanvasBitmap snowbitmap;
        private CanvasBitmap rainbitmap;
        private Vector2 snowCenter;
        private Rect snowBounds;
        private Rect rainBounds;
        private Vector2 rainCenter;


        /// <summary>
        /// 替换 minNum 和 maxNum，以适应不同窗口尺寸（每像素生成的粒子数量）
        /// </summary>
        private float minDensity, maxDensity;
        private float numParticles = 0;

        /// <summary>
        /// 根据雨的规模设置初始化参数(已弃用，使用 <see cref="ChangeConstants(RainLevel)"/>)
        /// </summary>
        /// <param name="rainLevel"></param>
        protected void InitializeConstants(RainLevel rainLevel)
        {

        }

        private void InitializelSnow()
        {
            bitmap = snowbitmap;
            bitmapCenter = snowCenter;
            bitmapBounds = snowBounds;
            minRotationAngle = 1;
            maxRotationAngle = 2;

            minInitialSpeed = 50;
            maxInitialSpeed = 80;

            minAcceleration = 0;
            maxAcceleration = 20;

            minScaleX = 0.3f;
            maxScaleX = 0.6f;

            minScaleY = 0.3f;
            maxScaleY = 0.6f;

            minDensity = 0.0002f;
            maxDensity = 0.0005f;
        }

        private void InitializesSnow()
        {
            bitmap = snowbitmap;
            bitmapCenter = snowCenter;
            bitmapBounds = snowBounds;
            minRotationAngle = 1;
            maxRotationAngle = 2;

            minInitialSpeed = 60;
            maxInitialSpeed = 90;

            minAcceleration = 0;
            maxAcceleration = 10;

            minScaleX = 0.2f;
            maxScaleX = 0.4f;

            minScaleY = 0.2f;
            maxScaleY = 0.4f;

            minDensity = 0;
            maxDensity = 0.0002f;
        }

        private void Initializeextreme()
        {
            minRotationAngle = 1.5708f;
            maxRotationAngle = 1.5708f;

            minInitialSpeed = 1800;
            maxInitialSpeed = 2000;

            minAcceleration = 1200;
            maxAcceleration = 1500;

            minScaleX = 0.4f;
            maxScaleX = 0.7f;

            minScaleY = 0.9f;
            maxScaleY = 4;

            minDensity = 0.015f;
            maxDensity = 0.02f;
        }

        private void Initializeheavy()
        {
            minRotationAngle = 1.54f;
            maxRotationAngle = 1.57f;

            minInitialSpeed = 1400;
            maxInitialSpeed = 1600;

            minAcceleration = 900;
            maxAcceleration = 1000;

            minScaleX = 0.3f;
            maxScaleX = 0.6f;

            minScaleY = 0.9f;
            maxScaleY = 2;

            minDensity = 0.01f;
            maxDensity = 0.015f;
        }

        private void Initializemoderate()
        {
            minRotationAngle = 1.45f;
            maxRotationAngle = 1.52f;

            minInitialSpeed = 900;
            maxInitialSpeed = 1200;

            minAcceleration = 600;
            maxAcceleration = 800;

            minScaleX = 0.2f;
            maxScaleX = 0.5f;

            minScaleY = 0.9f;
            maxScaleY = 1.3f;

            minDensity = 0.005f;
            maxDensity = 0.01f;
        }

        private void InitializeLight()
        {
            minRotationAngle = 1.404f;
            maxRotationAngle = 1.484f;

            minInitialSpeed = 600;
            maxInitialSpeed = 950;

            minAcceleration = 400;
            maxAcceleration = 600;

            minScaleX = 0.1f;
            maxScaleX = 0.4f;

            minScaleY = 0.7f;
            maxScaleY = 1;

            minDensity = 0f;
            maxDensity = 0.005f;
        }

        private void InitializeShower()
        {
            minRotationAngle = 1.54f;
            maxRotationAngle = 1.57f;

            minInitialSpeed = 1400;
            maxInitialSpeed = 1600;

            minAcceleration = 900;
            maxAcceleration = 1000;

            minScaleX = 0.3f;
            maxScaleX = 0.6f;

            minScaleY = 0.9f;
            maxScaleY = 2;

            minDensity = 0.01f;
            maxDensity = 0.015f;
        }

        public override async Task LoadSurfaceAsync(ICanvasResourceCreator resourceCreator)
        {
            if (surfaceLoaded)
                return;
            snowbitmap = await CanvasBitmap.LoadAsync(resourceCreator, "Assets/Particle/snow.png");
            rainbitmap = await CanvasBitmap.LoadAsync(resourceCreator, "Assets/Particle/rain.png");
            snowCenter = snowbitmap.Size.ToVector2() / 2;
            snowBounds = snowbitmap.Bounds;
            rainCenter = rainbitmap.Size.ToVector2() / 2;
            rainBounds = rainbitmap.Bounds;
            bitmap = rainbitmap;
            bitmapCenter = rainCenter;
            bitmapBounds = rainBounds;
            surfaceLoaded = true;
        }

        /// <summary>
        /// 根据旋转角度确定下落角度
        /// </summary>
        /// <returns></returns>
        private Vector2 PickDirection(float angle)
        {
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        /// <summary>
        /// 更新粒子物理属性，如果粒子超过边界，将其回收
        /// </summary>
        /// <param name="elapsedTime"></param>
        /// <param name="size"></param>
        public void Update(float elapsedTime, Vector2 size)
        {
            {
                if (surfaceLoaded)
                {
                    for (int i = ActiveParticles.Count - 1; i >= 0; i--)
                    {
                        var p = ActiveParticles[i];
                        if (p == null)
                        {
                            ActiveParticles.RemoveAt(i);
                            return;
                        }
                        if (p.Position.X > 0 - size.Y * (float)Math.Tan(1.5708 - (minRotationAngle + maxRotationAngle) / 2) && p.Position.X <= size.X && p.Position.Y <= size.Y)
                        {
                            p.Update(elapsedTime);
                        }
                        else
                        {
                            ActiveParticles.RemoveAt(i);
                            FreeParticles.Push(p);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 获得画布尺寸，在画布顶部生成粒子
        /// </summary>
        /// <param name="size"></param>
        public void AddRainDrop(Vector2 size)
        {
            {
                if (surfaceLoaded)
                {
                    numParticles += (Tools.RandomBetween(minDensity, maxDensity) * size.X);
                    var actualAdd = (int)numParticles;
                    numParticles %= 1;
                    for (int i = 0; i < actualAdd; i++)
                    {
                        Particle particle = (FreeParticles.Count > 0) ? FreeParticles.Pop() : new Particle();
                        float x = Tools.RandomBetween(0 - size.Y * (float)Math.Tan(1.5708 - (minRotationAngle + maxRotationAngle) / 2), size.X);
                        InitializeParticle(particle, new Vector2(x, -5));
                        if (ActiveParticles.Capacity <= ActiveParticles.Count)
                        {
                            ActiveParticles.Capacity = ActiveParticles.Count * 2;
                        }
                        ActiveParticles.Add(particle);
                    }
                }
            }
        }

        protected override void InitializeParticle(Particle particle, Vector2 where)
        {
            float velocity = Tools.RandomBetween(minInitialSpeed, maxInitialSpeed);
            float acceleration = Tools.RandomBetween(minAcceleration, maxAcceleration);
            float lifetime = Tools.RandomBetween(minLifetime, maxLifetime);
            float scaleX = Tools.RandomBetween(minScaleX, maxScaleX);
            float scaleY = Tools.RandomBetween(minScaleY, maxScaleY);
            float rotationSpeed = Tools.RandomBetween(minRotationSpeed, maxRotationSpeed);
            float rotation = Tools.RandomBetween(minRotationAngle, maxRotationAngle);
            Vector2 direction = PickDirection(rotation);
            particle.Initialize(where, velocity * direction, acceleration * direction, lifetime, scaleX, scaleY, rotation, rotationSpeed);
        }

        internal void ChangeConstants(RainLevel rainLevel)
        {
            this.rainLevel = rainLevel;
            bitmap = rainbitmap;
            bitmapCenter = rainCenter;
            bitmapBounds = rainBounds;
            minLifetime = 0;
            maxLifetime = 0;
            minRotationSpeed = 0;
            maxRotationSpeed = 0;
            switch (rainLevel)
            {
                case RainLevel.light:
                    InitializeLight();
                    break;
                case RainLevel.moderate:
                    Initializemoderate();
                    break;
                case RainLevel.heavy:
                    Initializeheavy();
                    break;
                case RainLevel.extreme:
                    Initializeextreme();
                    break;
                case RainLevel.sSnow:
                    InitializesSnow();
                    break;
                case RainLevel.lSnow:
                    InitializelSnow();
                    break;
                case RainLevel.shower:
                    InitializeShower();
                    break;
                default:
                    InitializeLight();
                    break;
            }
            blendState = CanvasBlend.Add;
        }
    }
}
