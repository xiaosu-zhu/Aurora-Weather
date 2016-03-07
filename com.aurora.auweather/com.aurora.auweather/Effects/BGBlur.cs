﻿using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using Windows.Storage.Streams;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.Effects;
using System.Threading.Tasks;
using Com.Aurora.Shared.Helpers;

namespace Com.Aurora.AuWeather.Effects
{
    public class BackBlur : IDisposable
    {
        private Rect bound;
        private Vector2 center = new Vector2(0f, 0f);
        private Vector2 position = new Vector2(0f, 0f);
        private bool enableBlur;
        private Vector2 scale = new Vector2(1, 1);
        private CanvasBitmap tempSurface;
        private GaussianBlurEffect blur = new GaussianBlurEffect { BorderMode = EffectBorderMode.Hard, Optimization = EffectOptimization.Balanced };
        private uint nowFrame = 0;
        private uint blurFrame = 0;
        private float opacity = 0f;
        private const uint inFrames = 120;
        private bool isImmersive;
        private bool canDraw = false;

        public async Task LoadSurfaceAsync(ICanvasResourceCreator creator, IRandomAccessStream stream)
        {
            tempSurface = await CanvasBitmap.LoadAsync(creator, stream);
            bound = tempSurface.Bounds;
            blur.Source = tempSurface;
        }

        public void update(Vector2 size)
        {
            if (tempSurface != null && canDraw)
            {
                scale.X = (float)(size.X / bound.Width > size.Y / bound.Height ? size.X / bound.Width : size.Y / bound.Height);
                scale.Y = scale.X;
                if (isImmersive)
                {
                    nowFrame++;
                    if (nowFrame <= inFrames)
                        opacity = (float)EasingHelper.CircleEase(Windows.UI.Xaml.Media.Animation.EasingMode.EaseOut, (double)nowFrame / inFrames);
                }
                else if (nowFrame != 0)
                {
                    nowFrame -= 1;
                    opacity = (float)EasingHelper.CircleEase(Windows.UI.Xaml.Media.Animation.EasingMode.EaseIn, (double)nowFrame / inFrames);
                    if (nowFrame == 0)
                    {
                        canDraw = false;
                    }
                }

                if (enableBlur)
                {
                    blurFrame++;
                    if (blurFrame <= inFrames)
                        blur.BlurAmount = 16f * (float)EasingHelper.CircleEase(Windows.UI.Xaml.Media.Animation.EasingMode.EaseOut, (double)blurFrame / inFrames);
                }
                else if (blurFrame != 0)
                {
                    blurFrame -= 1;
                    blur.BlurAmount = 16f * (float)EasingHelper.CircleEase(Windows.UI.Xaml.Media.Animation.EasingMode.EaseIn, (double)blurFrame / inFrames);
                }
            }
        }

        internal void BlurOut()
        {
            enableBlur = false;
            blurFrame = inFrames;
        }

        public void ImmersiveIn()
        {
            nowFrame = 0;
            blurFrame = 0;
            opacity = 0f;
            canDraw = true;
            isImmersive = true;
        }

        internal void BlurIn()
        {
            enableBlur = true;
            blurFrame = 0;
        }

        public void ImmersiveOut()
        {
            nowFrame = inFrames;
            opacity = 1f;
            isImmersive = false;
        }

        public void Draw(CanvasDrawingSession drawingSession)
        {
            // 保护原先画布的混合模式
            var previousBlend = drawingSession.Blend;

            drawingSession.Blend = CanvasBlend.SourceOver;
            // Compute a transform matrix for this particle.
            if (canDraw)
            {
                var transform = Matrix3x2.CreateRotation(0f, center) *
                    Matrix3x2.CreateScale(scale.X, scale.Y, center) *
                    Matrix3x2.CreateTranslation(position - center);
                if (enableBlur)
                    drawingSession.DrawImage(blur, new Rect(0, 0, bound.Width * scale.X, bound.Height * scale.Y), bound);
                else
                    drawingSession.DrawImage(tempSurface, 0f, 0f, bound, opacity, CanvasImageInterpolation.Linear, new Matrix4x4(transform));
            }
            drawingSession.Blend = previousBlend;
        }

        public void Dispose()
        {
            if (tempSurface != null)
            {
                bound = default(Rect);
                tempSurface.Dispose();
                blur.Dispose();
                canDraw = false;
            }
        }
    }
}
