using Windows.Foundation;
using Windows.UI.Xaml;

namespace Com.Aurora.Shared.Extensions
{
    public static class FrameworkElementExtensions
    {
        public static Point GetPositioninParent(this FrameworkElement f, FrameworkElement root)
        {
            if (f != null)
            {
                var transform = f.TransformToVisual(root);
                return transform.TransformPoint(new Point(0, 0));
            }
            return default(Point);
        }
    }
}
