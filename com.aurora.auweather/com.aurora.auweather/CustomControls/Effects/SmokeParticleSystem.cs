// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.Shared.Helpers;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;

namespace Com.Aurora.AuWeather.Effects
{
    /// <summary>
    /// 烟雾系统使用了多种贴图，同时要确保烟雾粒子都向一个大致方向运动
    /// </summary>
    public class SmokeParticleSystem : ParticleSystem
    {
        private CanvasBitmap[] smokeSurfaces;
        private Vector2[] surfacesCenter;
        private Rect[] surfacesBounds;

        private const int inFrames = 120;
        private byte flag = 0;
        private bool isImmersive = false;

        private static readonly string[][] smokeSurfaceName = {new string[] {"Assets/Particle/cloud1.png", "Assets/Particle/cloud2.png",
            "Assets/Particle/cloud3.png","Assets/Particle/cloud4.png","Assets/Particle/cloud5.png","Assets/Particle/cloud6.png"} ,new string[] { "Assets/Particle/smoke1.png", "Assets/Particle/smoke2.png",
            "Assets/Particle/smoke3.png","Assets/Particle/smoke4.png","Assets/Particle/smoke5.png","Assets/Particle/smoke6.png"},new string[] { "Assets/Particle/smoke1.png", "Assets/Particle/smoke2.png",
            "Assets/Particle/smoke3.png","Assets/Particle/smoke4.png","Assets/Particle/smoke5.png","Assets/Particle/smoke6.png"} };
        private bool inited;
        private float minDensity = 0f;
        private float maxDensity = 0f;

        public SmokeParticleSystem()
        {
            InitializeConstants();
        }

        protected override void InitializeConstants()
        {
            minRotationAngle = -0.025708f;
            maxRotationAngle = 0.025708f;
            minRotationSpeed = 0;
            maxRotationSpeed = 0.001f;
            minScaleX = 1;
            maxScaleX = 1;
            minScaleY = 1;
            maxScaleY = 1;
            blendState = CanvasBlend.SourceOver;
            minInitialSpeed = 1;
            maxInitialSpeed = 3;
            minAcceleration = 0;
            maxAcceleration = 0;
            minDensity = 0.02f;
            maxDensity = 0.02625f;
        }

        public void ChangeCondition()
        {
            if (flag == 0)
            {
                ChangetoCloudy();
            }
            if (flag == 1)
            {
                ChangetoOvercast();
            }
            if (flag == 2)
            {
                ChangetoFog();
            }
        }

        private void ChangetoFog()
        {
            minRotationAngle = -0.023708f;
            maxRotationAngle = 0.023708f;
            minRotationSpeed = 0;
            maxRotationSpeed = 0.0015f;
            minScaleX = 1;
            maxScaleX = 1;
            minScaleY = 1;
            maxScaleY = 1;
            blendState = CanvasBlend.SourceOver;
            minInitialSpeed = 1;
            maxInitialSpeed = 3.5f;
            minAcceleration = 0;
            maxAcceleration = 0;
            minDensity = 0.035f;
            maxDensity = 0.04625f;
        }

        private void ChangetoOvercast()
        {
            minRotationAngle = -0.023708f;
            maxRotationAngle = 0.023708f;
            minRotationSpeed = 0;
            maxRotationSpeed = 0.0015f;
            minScaleX = 1;
            maxScaleX = 1;
            minScaleY = 1;
            maxScaleY = 1;
            blendState = CanvasBlend.SourceOver;
            minInitialSpeed = 1;
            maxInitialSpeed = 3.5f;
            minAcceleration = 0;
            maxAcceleration = 0;
            minDensity = 0.035f;
            maxDensity = 0.04625f;
        }

        private void ChangetoCloudy()
        {
            minRotationAngle = -0.025708f;
            maxRotationAngle = 0.025708f;
            minRotationSpeed = 0;
            maxRotationSpeed = 0.001f;
            minScaleX = 1;
            maxScaleX = 1;
            minScaleY = 1;
            maxScaleY = 1;
            blendState = CanvasBlend.SourceOver;
            minInitialSpeed = 1;
            maxInitialSpeed = 3;
            minAcceleration = 0;
            maxAcceleration = 0;
            minDensity = 0.02f;
            maxDensity = 0.02625f;
        }

        public async Task LoadSurfaceAsync(ICanvasResourceCreator resourceCreator, byte v)
        {
            if (surfaceLoaded && v == flag)
                return;
            flag = v;
            List<CanvasBitmap> tempSurfaceList = new List<CanvasBitmap>();
            List<Vector2> tempCenterList = new List<Vector2>();
            List<Rect> tempBoundsList = new List<Rect>();
            foreach (var surfacename in smokeSurfaceName[flag])
            {
                var surf = await CanvasBitmap.LoadAsync(resourceCreator, surfacename);
                var center = surf.Size.ToVector2() / 2;
                var bounds = surf.Bounds;
                tempSurfaceList.Add(surf);
                tempCenterList.Add(center);
                tempBoundsList.Add(bounds);
            }
            smokeSurfaces = tempSurfaceList.ToArray();
            surfacesCenter = tempCenterList.ToArray();
            surfacesBounds = tempBoundsList.ToArray();
            surfaceLoaded = true;
            ChangeCondition();
        }

        public override void AddParticles(Vector2 size)
        {

            if (surfaceLoaded)
            {
                if (!inited)
                {
                    InitSmoke(size);
                }
                // 添加的数量
                var m = (int)(size.X * minDensity);
                var n = (int)(size.X * maxDensity);
                var actualAdd = Tools.Random.Next(m, n) - ActiveParticles.Count;
                if (actualAdd < 0)
                {
                    actualAdd = 0;
                }
                // 激活这些粒子
                for (int i = 0; i < actualAdd; i++)
                {
                    // 从空闲粒子堆里取粒子，如果没有，那么就 new 一个
                    Particle particle = (FreeParticles.Count > 0) ? FreeParticles.Pop() : new Particle();
                    particle.Key = Tools.Random.Next(0, smokeSurfaceName[flag].Length);
                    var where = new Vector2(0 - (float)(surfacesBounds[particle.Key].Width / 2), Tools.RandomBetween(0 - (float)(surfacesBounds[particle.Key].Height / 2), size.Y / 5 - (float)(surfacesBounds[particle.Key].Height / 2)));
                    // 初始化粒子参数
                    InitializeParticle(particle, where);

                    // 将此粒子加入激活粒子队列
                    if (ActiveParticles.Capacity <= ActiveParticles.Count)
                    {
                        ActiveParticles.Capacity = ActiveParticles.Count * 2;
                    }
                    ActiveParticles.Add(particle);
                }
            }
        }

        private void InitSmoke(Vector2 size)
        {
            var m = (int)(size.X * minDensity);
            var n = (int)(size.X * maxDensity);
            var add = Tools.Random.Next(m, n);
            for (int i = 0; i < add; i++)
            {
                // 从空闲粒子堆里取粒子，如果没有，那么就 new 一个
                Particle particle = (FreeParticles.Count > 0) ? FreeParticles.Pop() : new Particle();
                particle.Key = Tools.Random.Next(0, smokeSurfaceName[flag].Length);
                var where = new Vector2(Tools.RandomBetween(0 - (float)(surfacesBounds[particle.Key].Width / 2), size.X + (float)(surfacesBounds[particle.Key].Width / 2)), Tools.RandomBetween((0 - (float)(surfacesBounds[particle.Key].Height / 2)), size.Y / 5 - (float)(surfacesBounds[particle.Key].Height / 2)));
                // 初始化粒子参数
                InitializeParticle(particle, where);

                // 将此粒子加入激活粒子队列
                if (ActiveParticles.Capacity <= ActiveParticles.Count)
                {
                    ActiveParticles.Capacity = ActiveParticles.Count * 2;
                }
                ActiveParticles.Add(particle);
            }
            inited = true;
        }

        /// <summary>
        /// 更新粒子物理属性，如果粒子超过边界，将其回收
        /// </summary>
        /// <param name="elapsedTime"></param>
        /// <param name="size"></param>
        public void Update(float elapsedTime, Vector2 size)
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
                    if (p.Position.X < (size.X + (surfacesBounds[p.Key].Width) / 2))
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

        /// <summary>
        /// 为配合雨滴，这里方向全部取向右，即 angle 在 -0.262f ~ 0.262f
        /// </summary>
        /// <returns></returns>
        protected override Vector2 PickDirection()
        {
            float angle = Tools.RandomBetween(-0.075f, 0.08f);
            return new Vector2((float)Math.Cos(angle),
                               (float)Math.Sin(angle));
        }

        public override void Draw(CanvasDrawingSession drawingSession, bool useSpriteBatch)
        {
            if (!surfaceLoaded)
                return;
            // 保护原先画布的混合模式
            var previousBlend = drawingSession.Blend;

            drawingSession.Blend = blendState;

#if WINDOWS_UWP
            if (useSpriteBatch)
            {
                // 使用 SpriteBatch 可以提高性能
                using (var spriteBatch = drawingSession.CreateSpriteBatch())
                {
                    Draw(drawingSession, spriteBatch);
                }
            }
            else
            {
                Draw(drawingSession, null);
            }
#else
            Draw(drawingSession);
#endif

            drawingSession.Blend = previousBlend;
        }


        protected override void Draw(CanvasDrawingSession drawingSession
#if WINDOWS_UWP
            , CanvasSpriteBatch spriteBatch
#endif
            )
        {
            // 逆向遍历队列，可以让新粒子绘制在旧粒子下方，这样当很多粒子在同一个位置生成时，效果较好
            for (int i = ActiveParticles.Count - 1; i >= 0; i--)
            {
                Particle particle = ActiveParticles[i];

                // NormalizedLifeTime 是一个0到1之间的值，用来表示粒子在生命周期中的进度，这个值接近0或接近1时，
                // 粒子将会渐隐/渐显，使用它来计算粒子的透明度和缩放
                float normalizedLifetime = particle.TimeSinceStart / 4;
                if (normalizedLifetime > 1)
                {
                    normalizedLifetime = 1;
                }

                // We want particles to fade in and fade out, so we'll calculate alpha to be
                // (normalizedLifetime) * (1 - normalizedLifetime). This way, when normalizedLifetime
                // is 0 or 1, alpha is 0. The maximum value is at normalizedLifetime = .5, and is:
                //
                //      (normalizedLifetime) * (1-normalizedLifetime)
                //      (.5)                 * (1-.5)
                //      .25
                //
                // Since we want the maximum alpha to be 1, not .25, we'll scale the entire equation by 4.
                float alpha = (float)EasingHelper.QuinticEase(Windows.UI.Xaml.Media.Animation.EasingMode.EaseOut, normalizedLifetime);
                var x = particle.ScaleX;
                var y = particle.ScaleY;
                // Make particles grow as they age.
                // They'll start at 75% of their size, and increase to 100% once they're finished.
                if (isImmersive)
                {
                    alpha *= 0.8f;
                    x *= 1.2f;
                    y *= 1.2f;
                }
#if WINDOWS_UWP
                if (spriteBatch != null)
                {
                    spriteBatch.Draw(smokeSurfaces[particle.Key], particle.Position, new Vector4(1, 1, 1, alpha), bitmapCenter,
                        particle.Rotation, new Vector2(x, y), CanvasSpriteFlip.None);
                }
                else
#endif
                {
                    // Compute a transform matrix for this particle.
                    var transform = Matrix3x2.CreateRotation(particle.Rotation, bitmapCenter) *
                                    Matrix3x2.CreateScale(x, y, bitmapCenter) *
                                    Matrix3x2.CreateTranslation(particle.Position - bitmapCenter);

                    // Draw the particle.
                    drawingSession.DrawImage(smokeSurfaces[particle.Key], 0, 0, bitmapBounds, alpha, CanvasImageInterpolation.Linear, new Matrix4x4(transform));
                }
            }
        }
        public void smokeDispose()
        {
            Dispose();
            if (surfaceLoaded)
            {
                foreach (var item in smokeSurfaces)
                {
                    item.Dispose();

                }
            }
            surfaceLoaded = false;
            inited = false;
        }

        internal void ImmersiveIn()
        {
            inited = false;
            for (int i = ActiveParticles.Count - 1; i >= 0; i--)
            {
                var p = ActiveParticles[i];
                ActiveParticles.RemoveAt(i);
                FreeParticles.Push(p);

            }
            isImmersive = true;
        }
        internal void ImmersiveOut()
        {
            inited = false;
            for (int i = ActiveParticles.Count - 1; i >= 0; i--)
            {
                var p = ActiveParticles[i];
                ActiveParticles.RemoveAt(i);
                FreeParticles.Push(p);

            }
            isImmersive = false;
        }
    }
}
