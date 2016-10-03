using Windows.UI.Xaml.Controls;

namespace Com.Aurora.Shared.Extensions
{
    public static class UIElementExtensions
    {
        public static void ReplaceElements(this Canvas canvas, double horizontalScale, double verticalScale)
        {
            foreach (var item in canvas.Children)
            {
                var left = (double)item.GetValue(Canvas.LeftProperty);
                item.SetValue(Canvas.LeftProperty, left * horizontalScale);
                var top = (double)item.GetValue(Canvas.TopProperty);
                item.SetValue(Canvas.TopProperty, top * verticalScale);
            }
        }
    }
}
