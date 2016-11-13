// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.Graphics.Canvas;
using System.Numerics;

namespace Com.Aurora.AuWeather.Effects
{
    /// <summary>
    /// 模拟星空使用固定的圆形发光点，并且随机进行亮度变化
    /// </summary>
    public class StarParticleSystem : ParticleSystem
    {
        public StarParticleSystem()
        {
            InitializeConstants();
        }

        protected override void InitializeConstants()
        {
            bitmapFilename = "Assets/Particle/star.png";
            minLifetime = 0.2f;
            maxLifetime = 20;
            minRotationAngle = 0;
            maxRotationAngle = 0;
            minRotationSpeed = 0;
            maxRotationSpeed = 0;
            minInitialSpeed = 0;
            maxInitialSpeed = 0;
            minAcceleration = 0;
            maxAcceleration = 0;
            minScaleX = 0.2f;
            maxScaleX = 0.6f;
            minScaleY = 0.5f;
            maxScaleY = 1;
            minNumParticles = 1;
            maxNumParticles = 4;
            blendState = CanvasBlend.Add;
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
                float normalizedLifetime = particle.TimeSinceStart / particle.Lifetime;

                // We want particles to fade in and fade out, so we'll calculate alpha to be
                // (normalizedLifetime) * (1 - normalizedLifetime). This way, when normalizedLifetime
                // is 0 or 1, alpha is 0. The maximum value is at normalizedLifetime = .5, and is:
                //
                //      (normalizedLifetime) * (1-normalizedLifetime)
                //      (.5)                 * (1-.5)
                //      .25
                //
                // Since we want the maximum alpha to be 1, not .25, we'll scale the entire equation by 4.
                float alpha = 4 * normalizedLifetime * (1 - normalizedLifetime);

                // Make particles grow as they age.
                // They'll start at 75% of their size, and increase to 100% once they're finished.
                float scale = particle.ScaleX * (.75f + .25f * normalizedLifetime);

#if WINDOWS_UWP
                if (spriteBatch != null)
                {
                    spriteBatch.Draw(bitmap, particle.Position, new Vector4(1, 1, 1, alpha), bitmapCenter,
                        0, new Vector2(scale), CanvasSpriteFlip.None);
                }
                else
#endif
                {
                    // Compute a transform matrix for this particle.
                    var transform = Matrix3x2.CreateRotation(particle.Rotation, bitmapCenter) *
                                    Matrix3x2.CreateScale(scale) *
                                    Matrix3x2.CreateTranslation(particle.Position - bitmapCenter);

                    // Draw the particle.
                    drawingSession.DrawImage(bitmap, 0, 0, bitmapBounds, alpha, CanvasImageInterpolation.Linear, new Matrix4x4(transform));
                }
            }
        }
    }
}
