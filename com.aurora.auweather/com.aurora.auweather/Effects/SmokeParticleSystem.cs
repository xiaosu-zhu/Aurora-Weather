using Com.Aurora.Shared.Helpers;
using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Shapes;

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

        private string[] smokeSurfaceName;

        public SmokeParticleSystem()
        {
            this.InitializeConstants();
        }

        protected override void InitializeConstants()
        {
            smokeSurfaceName = new string[] {"Assets/smoke1.png", "Assets/smoke2.png",
            "Assets/smoke3.png","Assets/smoke4.png","Assets/smoke5.png","Assets/smoke6.png"};
            minLifetime = 10;
            maxLifetime = 20;
            minRotationAngle = -1.5708f;
            maxRotationAngle = 1.5708f;
            minRotationSpeed = 0;
            maxRotationSpeed = 0.1f;
            minScaleX = 1;
            maxScaleX = 4;
            minScaleY = 1;
            maxScaleY = 4;
            minNumParticles = 7;
            maxNumParticles = 10;
            blendState = CanvasBlend.Add;
            minInitialSpeed = 100;
            maxInitialSpeed = 150;
            minAcceleration = 0;
            maxAcceleration = 0;
        }

        public override async Task CreateResourcesAsync(ICanvasResourceCreator resourceCreator)
        {
            List<CanvasBitmap> tempSurfaceList = new List<CanvasBitmap>();
            List<Vector2> tempCenterList = new List<Vector2>();
            List<Rect> tempBoundsList = new List<Rect>();
            foreach (var surfacename in smokeSurfaceName)
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
        }

        public override void AddParticles(Vector2 size)
        {
            // 添加的数量
            int numParticles = Tools.Random.Next(minNumParticles, maxNumParticles);

            // 激活这些粒子
            for (int i = 0; i < numParticles; i++)
            {
                // 从空闲粒子堆里取粒子，如果没有，那么就 new 一个
                Particle particle = (FreeParticles.Count > 0) ? FreeParticles.Pop() : new Particle();
                var where = new Vector2((float)Tools.RandomBetween(-size.X * 0.5f, size.X), (float)Tools.RandomBetween(0, size.Y));
                // 初始化粒子参数
                InitializeParticle(particle, where);
                particle.Key = Tools.Random.Next(0, 6);
                // 将此粒子加入激活粒子队列
                ActiveParticles.Add(particle);
            }
        }

        /// <summary>
        /// 为配合雨滴，这里方向全部取向右，即 angle 在 -0.262f ~ 0.262f
        /// </summary>
        /// <returns></returns>
        protected override Vector2 PickDirection()
        {
            float angle = Tools.RandomBetween(-0.05f, 0.131f);
            return new Vector2((float)Math.Cos(angle),
                               (float)Math.Sin(angle));
        }

        public override void Draw(CanvasDrawingSession drawingSession, bool useSpriteBatch)
        {
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
                float scaleX = particle.ScaleX * (.75f + .25f * normalizedLifetime);
                float scaleY = particle.ScaleY * (.75f + .25f * normalizedLifetime);

#if WINDOWS_UWP
                if (spriteBatch != null)
                {
                    spriteBatch.Draw(smokeSurfaces[particle.Key], particle.Position, new Vector4(1, 1, 1, alpha), bitmapCenter,
                        particle.Rotation, new Vector2(scaleX, scaleY), CanvasSpriteFlip.None);
                }
                else
#endif
                {
                    // Compute a transform matrix for this particle.
                    var transform = Matrix3x2.CreateRotation(particle.Rotation, bitmapCenter) *
                                    Matrix3x2.CreateScale(scaleX, scaleY, bitmapCenter) *
                                    Matrix3x2.CreateTranslation(particle.Position - bitmapCenter);

                    // Draw the particle.
                    drawingSession.DrawImage(smokeSurfaces[particle.Key], 0, 0, bitmapBounds, alpha, CanvasImageInterpolation.Linear, new Matrix4x4(transform));
                }
            }
        }
    }
}
