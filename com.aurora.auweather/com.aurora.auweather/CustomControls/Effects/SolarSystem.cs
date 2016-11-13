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
    public class SolarSystem : IDisposable
    {
        private CanvasBitmap[] sunSurfaces;
        private Rect bound;
        private Vector2 center;
        private string surfaceNameHeader = "Assets/sun/";
        private uint surfaceCount = 60;

        private const uint inFrames = 120;
        private uint nowFrame = 0;
        private float slowFactor = 0.1f;
        private double yOffset;
        private double xOffset;
        private Vector2 position = new Vector2(0, 0);
        private float opcity = 0;
        private float rotation = 0;
        private bool surfaceLoaded = false;

        public async Task LoadSurfaceAsync(ICanvasResourceCreator resourceCreator)
        {
            if (surfaceLoaded)
                return;
            List<CanvasBitmap> tempSurfaceList = new List<CanvasBitmap>();
            for (int i = 0; i < surfaceCount; i++)
            {
                var surf = await CanvasBitmap.LoadAsync(resourceCreator, surfaceNameHeader + i.ToString("00000") + ".png");
                var center = surf.Size.ToVector2() / 2;
                var bounds = surf.Bounds;
                tempSurfaceList.Add(surf);

            }
            this.center = tempSurfaceList[0].Size.ToVector2() / 2;
            bound = tempSurfaceList[0].Bounds;
            sunSurfaces = tempSurfaceList.ToArray();
            xOffset = bound.Width / 2;
            yOffset = bound.Height / 2;
            surfaceLoaded = true;
        }

        public void Update()
        {
            if (surfaceLoaded)
            {
                if (nowFrame < inFrames)
                {
                    var progress = nowFrame / (float)inFrames;
                    progress = (float)EasingHelper.QuinticEase(Windows.UI.Xaml.Media.Animation.EasingMode.EaseOut, progress);
                    position.X = (float)(xOffset * (progress - 0.5)) / 2;
                    position.Y = (float)(yOffset * (progress - 0.5)) / 2;
                    opcity = 0.8f * progress;
                }
                nowFrame++;
                rotation = 0.000174532922222222f * nowFrame;
            }
        }

        public void Draw(CanvasDrawingSession drawingSession, bool useSpriteBatch)
        {
            if (surfaceLoaded)
            {
                // 保护原先画布的混合模式
                var previousBlend = drawingSession.Blend;

                drawingSession.Blend = CanvasBlend.SourceOver;

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
        }


        protected void Draw(CanvasDrawingSession drawingSession
#if WINDOWS_UWP
            , CanvasSpriteBatch spriteBatch
#endif
            )
        {

#if WINDOWS_UWP
            if (spriteBatch != null)
            {
                spriteBatch.Draw(sunSurfaces[(uint)(nowFrame * slowFactor) % surfaceCount], position, new Vector4(1, 1, 1, opcity), center, rotation, new Vector2(1, 1), CanvasSpriteFlip.None);
            }
            else
#endif
            {
                // Compute a transform matrix for this particle.
                var transform = Matrix3x2.CreateRotation(rotation, center) *
                                Matrix3x2.CreateScale(1, 1, center) *
                                Matrix3x2.CreateTranslation(position - center);

                // Draw the particle.
                drawingSession.DrawImage(sunSurfaces[(uint)(nowFrame * slowFactor) % surfaceCount], 0, 0, bound, opcity, CanvasImageInterpolation.Linear, new Matrix4x4(transform));
            }
        }

        public void Dispose()
        {
            if (surfaceLoaded)
            {
                nowFrame = 0;
                foreach (var surface in sunSurfaces)
                {
                    surface.Dispose();
                }
                surfaceLoaded = false;
            }
        }
    }
}
