using Com.Aurora.Shared.Helpers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using Microsoft.Graphics.Canvas.Geometry;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
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
        private float maxSpan;
        private float minDuration;
        private float minSpan;
        private Thunder[] thunders;
        private CanvasBlend blendState;
        private Thunder currentThunder;
        private float timeToCreate = 0;
        GaussianBlurEffect blur = new GaussianBlurEffect();

        public ThunderGenerator()
        {
            InitializeConstants();
        }

        private void InitializeConstants()
        {
            minDuration = 0.2f;
            maxDuration = 1.0f;
            minSpan = 1.5f;
            maxSpan = 8f;
            blendState = CanvasBlend.Add;
        }

        public void GenerateThunder(Vector2 size)
        {
            // store 16 thunders
            List<Thunder> t = new List<Thunder>();
            for (int i = 0; i < 16; i++)
            {
                t.Add(new Thunder(Tools.RandomBetween(minDuration, maxDuration), size));
            }
            this.thunders = t.ToArray();
        }


        /// <summary>
        /// 控制亮度
        /// </summary>
        public void Update(float elapsedTime, Vector2 size)
        {
            timeToCreate -= elapsedTime;
            if (timeToCreate < 0)
            {
                timeToCreate = Tools.RandomBetween(minSpan, maxSpan);
                timeToCreate += elapsedTime;
                SelectThunder(size);
            }
        }

        public void SelectThunder(Vector2 size)
        {
            currentThunder = thunders[Tools.Random.Next(16)];
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
            foreach (var point in currentThunder.Path)
            {
                builder.AddLine(point.X, point.Y);
            }
            builder.EndFigure(CanvasFigureLoop.Open);
            //var transform = Matrix3x2.CreateRotation(particle.Rotation - 1.5708f, bitmapCenter) *
            //                       Matrix3x2.CreateScale(/*scale*/particle.ScaleX, particle.ScaleY, bitmapCenter) *
            //                       Matrix3x2.CreateTranslation(particle.Position - bitmapCenter);

            // Draw the particle.
            CanvasCommandList cl = new CanvasCommandList(sender);
            using (CanvasDrawingSession clds = cl.CreateDrawingSession())
            {
                var path = CanvasGeometry.CreatePath(builder);
                clds.DrawGeometry(path, currentThunder.Position, Color.FromArgb(150, 255, 240, 180), 2);
            }
            blur.Source = cl;
            blur.BlurAmount = 4;
            drawingSession.DrawImage(blur);
            drawingSession.DrawImage(cl);
            drawingSession.Blend = previousBlend;
        }
    }
}
