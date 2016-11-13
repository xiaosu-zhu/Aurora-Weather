// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.Shared.Helpers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI;

namespace Com.Aurora.AuWeather.Effects
{
    /// <summary>
    /// 通过直线生成闪电，并添加发光效果，初始化间隔和每个闪电持续时间
    /// 在画布上半部生成
    /// </summary>
    public class ThunderGenerator
    {
        private float maxDuration;
        private float minDuration;
        private CanvasBlend blendState;
        private Thunder currentThunder;
        GaussianBlurEffect blur = new GaussianBlurEffect();

        public ThunderGenerator()
        {
            InitializeConstants();
        }

        private void InitializeConstants()
        {
            minDuration = 1f;
            maxDuration = 1.8f;
            blendState = CanvasBlend.Add;
        }


        /// <summary>
        /// 控制亮度
        /// </summary>
        public void Update(float elapsedTime, Vector2 size)
        {
            if (currentThunder != null)
                currentThunder.Update(elapsedTime);
        }

        /// <summary>
        /// 在任意位置，生成一道闪电
        /// </summary>
        /// <param name="size"></param>
        public void Generate(Vector2 size)
        {
            currentThunder = //thunders[Tools.Random.Next(16)];
                new Thunder(Tools.RandomBetween(minDuration, maxDuration), size);
            currentThunder.Generate(size);
        }

        public void Draw(ICanvasAnimatedControl sender, CanvasDrawingSession drawingSession)
        {
            if (currentThunder == null)
            {
                return;
            }
            // 保护原先画布的混合模式
            var previousBlend = drawingSession.Blend;
            drawingSession.Blend = blendState;
            var builder = new CanvasPathBuilder(sender);
            builder.BeginFigure(0, 0);
            for (int i = 0; i < currentThunder.LifeLong; i++)
            {
                builder.AddLine(currentThunder.Path[i].X, currentThunder.Path[i].Y);
            }
            builder.EndFigure(CanvasFigureLoop.Open);
            builder.SetSegmentOptions(CanvasFigureSegmentOptions.ForceRoundLineJoin);

            // Draw the particle.
            var path = CanvasGeometry.CreatePath(builder);
            var NormalizeLifeTime = currentThunder.TimeSinceStart / currentThunder.Duration;
            byte opacity = (byte)((NormalizeLifeTime - 1) * (NormalizeLifeTime - 1) * 255);
            CanvasCommandList cl = new CanvasCommandList(sender);
            using (CanvasDrawingSession clds = cl.CreateDrawingSession())
            {
                clds.DrawGeometry(path, currentThunder.Position, Color.FromArgb((byte)(0.75f * opacity), 255, 255, 255), 6 * currentThunder.Luminace);
            }
            var lightAmount = 20.6f * currentThunder.Luminace * (NormalizeLifeTime - 1) * (NormalizeLifeTime - 1);
            blur.Source = cl;
            blur.BlurAmount = lightAmount;
            drawingSession.DrawImage(blur);
            drawingSession.DrawGeometry(path, currentThunder.Position, Color.FromArgb(opacity, 255, 240, 180), 2 * currentThunder.Luminace);
            drawingSession.Blend = previousBlend;
            if (NormalizeLifeTime > 1)
            {
                currentThunder = null;
            }
        }
    }
}
