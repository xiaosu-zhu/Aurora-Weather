using Microsoft.Graphics.Canvas;
using System;
using System.Numerics;
using Windows.Storage.Streams;
using Windows.Foundation;
using Microsoft.Graphics.Canvas.Effects;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Effects
{
    public class BGBlur : IDisposable
    {
        private Rect bound;
        private Vector2 center = new Vector2(0f, 0f);
        private Vector2 position = new Vector2(0f, 0f);
        private bool enableBlur = true;
        private Vector2 scale = new Vector2(1, 1);
        private CanvasBitmap tempSurface;
        private GaussianBlurEffect blur = new GaussianBlurEffect { BorderMode = EffectBorderMode.Soft };
        private int nowFrame = 0;

        public async Task LoadSurfaceAsync(ICanvasResourceCreator creator, IRandomAccessStream stream)
        {
            tempSurface = await CanvasBitmap.LoadAsync(creator, stream);
            bound = tempSurface.Bounds;
            blur.Source = tempSurface;
        }

        public void update(Vector2 size)
        {
            if (bound != default(Rect) && tempSurface != null)
            {
                scale.X = (float)(size.X / bound.Width > size.Y / bound.Height ? size.X / bound.Width : size.Y / bound.Height);
                scale.Y = scale.X;
                nowFrame++;
                if (enableBlur)
                {
                    blur.BlurAmount = 8f * (float)Math.Abs(Math.Sin(nowFrame));
                }
            }
        }

        public void Draw(CanvasDrawingSession drawingSession)
        {
            // 保护原先画布的混合模式
            var previousBlend = drawingSession.Blend;

            drawingSession.Blend = CanvasBlend.SourceOver;
            // Compute a transform matrix for this particle.
            if (tempSurface != null)
            {
                var transform = Matrix3x2.CreateRotation(0f, center) *
                    Matrix3x2.CreateScale(scale.X, scale.Y, center) *
                    Matrix3x2.CreateTranslation(position - center);
                drawingSession.DrawImage(blur, new Rect(0, 0, bound.Width * scale.X, bound.Height * scale.Y), bound, 1);
                //drawingSession.DrawImage(blur, 0f, 0f, bound, 1f, CanvasImageInterpolation.Linear, new Matrix4x4(transform));
            }
            drawingSession.Blend = previousBlend;
        }

        public void Dispose()
        {
            if (tempSurface != null)
            {
                bound = default(Rect);
                tempSurface.Dispose();
                tempSurface = null;
                blur.Dispose();
            }
        }
    }
}
