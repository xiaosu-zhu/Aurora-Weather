// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading.Tasks;
using Windows.Foundation;
using Com.Aurora.Shared.Helpers;

namespace Com.Aurora.AuWeather.Effects
{
    // 此抽象类提供了粒子系统的基础功能，子类可以继承派生出不同的效果
    public abstract class ParticleSystem : IDisposable
    {
        // 粒子的纹理
        protected CanvasBitmap bitmap;

        // 纹理中心及边界
        protected Vector2 bitmapCenter;
        protected Rect bitmapBounds;

        // 当前激活的粒子，它们将会被循环利用
        List<Particle> activeParticles = new List<Particle>();

        // 当前释放的粒子，画布上需要新粒子时，首先从这里调用，粒子到达生命周期/不在画布内，就会被回收到这里
        static Stack<Particle> freeParticles = new Stack<Particle>();

        public List<Particle> ActiveParticles
        {
            get { return activeParticles; }
        }

        protected static Stack<Particle> FreeParticles
        {
            get { return freeParticles; }
        }


        // 下面的这些字段控制了粒子系统的“外观”，在 InitializeConstants 方法中会被粒子系统设置，
        // 紧接着会在 InitializeParticle 方法中用来初始化粒子，这个方法可以被子类重写以自定义

        #region Constants to be set by subclasses

        // 每次调用 AddParticles 方法时要添加的粒子数量，将会从 minNumParticles 和 maxNumParticles 之间取一个随机数
        protected int minNumParticles;
        protected int maxNumParticles;

        // 粒子纹理的文件名
        protected string bitmapFilename;

        // 粒子初始化时设置的速度，在 minInitialSpeed 和 maxInitialSpeed 中取一个随机数
        // 粒子运动方向将由 PickDirection 方法设置
        protected float minInitialSpeed;
        protected float maxInitialSpeed;

        // 粒子初始化时设置的加速度，在 minAcceleration 和 maxAcceleration 中取一个随机数
        // 粒子加速度方向默认与速度方向一致，可以由 PickAccelerationDirection 方法设置
        protected float minAcceleration;
        protected float maxAcceleration;

        // 粒子初始化时设置的旋转速度，在 minRotationSpeed 和 maxRotationSpeed 中取一个随机数
        // 较低的旋转速度会让粒子安静，较高的旋转速度会让粒子爆炸
        protected float minRotationSpeed;
        protected float maxRotationSpeed;

        protected float minRotationAngle;
        protected float maxRotationAngle;

        // 粒子的生命周期，同时它们也用来计算透明度和缩放以使粒子的出现不那么突兀
        protected float minLifetime;
        protected float maxLifetime;

        // 粒子的缩放可以让粒子的样式更加多变(大小不一)，缩放倍率将会随机在 minScale 和 maxScale 之间取值
        protected float minScaleX;
        protected float maxScaleX;
        protected float minScaleY;
        protected float maxScaleY;

        // 绘制的混合模式。火焰、烟雾在叠加模式下效果较好
        protected CanvasBlend blendState;
        protected bool surfaceLoaded;


        #endregion


        // Constructs a new ParticleSystem.
        protected ParticleSystem()
        {
            InitializeConstants();
        }


        // 此方法必须在子类中实现，并且要设置所有参数的值
        protected virtual void InitializeConstants()
        {

        }


        // 载入纹理
        public virtual async Task LoadSurfaceAsync(ICanvasResourceCreator resourceCreator)
        {
            if (surfaceLoaded)
                return;
            bitmap = await CanvasBitmap.LoadAsync(resourceCreator, bitmapFilename);
            surfaceLoaded = true;
        }


        // 在画布的某一位置添加粒子
        public virtual void AddParticles(Vector2 where)
        {
            if (surfaceLoaded)
            {
                // 添加的数量
                int numParticles = Tools.Random.Next(minNumParticles, maxNumParticles);

                // 激活这些粒子
                for (int i = 0; i < numParticles; i++)
                {
                    // 从空闲粒子堆里取粒子，如果没有，那么就 new 一个
                    Particle particle = (freeParticles.Count > 0) ? freeParticles.Pop() : new Particle();
                    // 初始化粒子参数
                    InitializeParticle(particle, where);
                    // 将此粒子加入激活粒子队列
                    if (ActiveParticles.Capacity <= ActiveParticles.Count)
                    {
                        ActiveParticles.Capacity = ActiveParticles.Count * 2;
                    }
                    activeParticles.Add(particle);
                }
            }
        }


        // 使用设置好的值计算粒子的随机参数，然后初始化粒子
        protected virtual void InitializeParticle(Particle particle, Vector2 where)
        {
            // 首先确定运动方向
            Vector2 direction = PickDirection();

            // 确定物理参数
            float velocity = Tools.RandomBetween(minInitialSpeed, maxInitialSpeed);
            float acceleration = Tools.RandomBetween(minAcceleration, maxAcceleration);
            float lifetime = Tools.RandomBetween(minLifetime, maxLifetime);
            float scaleX = Tools.RandomBetween(minScaleX, maxScaleX);
            float scaleY = Tools.RandomBetween(minScaleY, maxScaleY);
            float rotationSpeed = Tools.RandomBetween(minRotationSpeed, maxRotationSpeed);
            float rotation = Tools.RandomBetween(minRotationAngle, maxRotationAngle);

            // 调用粒子的初始化方法
            particle.Initialize(where, velocity * direction, acceleration * direction, lifetime, scaleX, scaleY, rotation, rotationSpeed);
        }


        // 默认的运动方向是随机取值
        //           |
        //          270°
        //  <-180°        0°->
        //          90°
        //           |
        protected virtual Vector2 PickDirection()
        {
            float angle = Tools.RandomBetween(0, (float)Math.PI * 2);

            return new Vector2((float)Math.Cos(angle),
                               (float)Math.Sin(angle));
        }


        // 刷新所有的激活粒子
        public virtual void Update(float elapsedTime)
        {
            if (surfaceLoaded)
            {
                // 从队列的末尾向前遍历，这样执行 Remove 的时候不会出错
                for (int i = activeParticles.Count - 1; i >= 0; i--)
                {
                    Particle particle = activeParticles[i];
                    if (particle == null)
                    {
                        ActiveParticles.RemoveAt(i);
                        return;
                    }
                    if (!particle.Update(elapsedTime))
                    {
                        // 如果粒子不再存活，将它去掉
                        activeParticles.RemoveAt(i);
                        freeParticles.Push(particle);
                    }
                }
            }
        }


        // 绘制所有粒子
        public virtual void Draw(CanvasDrawingSession drawingSession, bool useSpriteBatch)
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


        protected virtual void Draw(CanvasDrawingSession drawingSession
#if WINDOWS_UWP
            , CanvasSpriteBatch spriteBatch
#endif
            )
        {
            // 逆向遍历队列，可以让新粒子绘制在旧粒子下方，这样当很多粒子在同一个位置生成时，效果较好
            for (int i = activeParticles.Count - 1; i >= 0; i--)
            {
                Particle particle = activeParticles[i];

                // NormalizedLifeTime 是一个0到1之间的值，用来表示粒子在生命周期中的进度，这个值接近0或接近1时，
                // 粒子将会渐隐/渐显，使用它来计算粒子的透明度和缩放
                //float normalizedLifetime = particle.TimeSinceStart / particle.Lifetime;

                // We want particles to fade in and fade out, so we'll calculate alpha to be
                // (normalizedLifetime) * (1 - normalizedLifetime). This way, when normalizedLifetime
                // is 0 or 1, alpha is 0. The maximum value is at normalizedLifetime = .5, and is:
                //
                //      (normalizedLifetime) * (1-normalizedLifetime)
                //      (.5)                 * (1-.5)
                //      .25
                //
                // Since we want the maximum alpha to be 1, not .25, we'll scale the entire equation by 4.
                //float alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);

                // Make particles grow as they age.
                // They'll start at 75% of their size, and increase to 100% once they're finished.
                //float scale = particle.Scale * (.75f + .25f * normalizedLifetime);

#if WINDOWS_UWP
                if (spriteBatch != null)
                {
                    spriteBatch.Draw(bitmap, particle.Position, new Vector4(1, 1, 1, 1/*alpha*/), bitmapCenter,
                        particle.Rotation - 1.5708f, new Vector2(particle.ScaleX, particle.ScaleY/*scale*/), CanvasSpriteFlip.None);
                }
                else
#endif
                {
                    // Compute a transform matrix for this particle.
                    var transform = Matrix3x2.CreateRotation(particle.Rotation - 1.5708f, bitmapCenter) *
                                    Matrix3x2.CreateScale(/*scale*/particle.ScaleX, particle.ScaleY, bitmapCenter) *
                                    Matrix3x2.CreateTranslation(particle.Position - bitmapCenter);

                    // Draw the particle.
                    drawingSession.DrawImage(bitmap, 0, 0, bitmapBounds, 1/*alpha*/, CanvasImageInterpolation.Linear, new Matrix4x4(transform));
                }
            }
        }

        public void Dispose()
        {
            if (surfaceLoaded && bitmap != null)
            {
                bitmap.Dispose();

                surfaceLoaded = false;
            }
            freeParticles.Clear();
            ActiveParticles.Clear();
        }
    }
}
