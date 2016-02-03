using Com.Aurora.AuWeather.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Com.Aurora.AuWeather
{
    public sealed partial class NowWeatherPage : Page
    {
        NowWeatherPageViewModel mModel;
        public NowWeatherPage()
        {
            this.InitializeComponent();
            mModel = new NowWeatherPageViewModel();
            this.DataContext = mModel;
            mModel.FetchDataComplete += MModel_FetchDataComplete;
        }

        private void MModel_FetchDataComplete(object sender, FetchDataCompleteEventArgs e)
        {
            this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
            {
                TempraturePathAnimation.Begin();
            }));
        }

        private void RelativePanel_LayoutUpdated(object sender, object e)
        {
            SetPathPoint2(BezierControl1, Root.ActualWidth, 1f / 8f);
            SetPathPoint3(BezierControl1, Root.ActualWidth, 2f / 8f);
            SetPathPoint1(BezierControl2, Root.ActualWidth, 2f / 8f);
            SetPathPoint2(BezierControl2, Root.ActualWidth, 3f / 8f);
            SetPathPoint3(BezierControl2, Root.ActualWidth, 4f / 8f);
            SetPathPoint1(BezierControl3, Root.ActualWidth, 4f / 8f);
            SetPathPoint2(BezierControl3, Root.ActualWidth, 5f / 8f);
            SetPathPoint3(BezierControl3, Root.ActualWidth, 6f / 8f);
            SetPathPoint1(BezierControl4, Root.ActualWidth, 6f / 8f);
            SetPathPoint2(BezierControl4, Root.ActualWidth, 7f / 8f);
            SetPathPoint3(BezierControl4, Root.ActualWidth, 1);
            SetEndPoint(endPoint1, Root.ActualWidth);
        }

        private void SetEndPoint(LineSegment endPoint1, double actualWidth)
        {
            var p = endPoint1.Point;
            p.X = actualWidth;
            endPoint1.Point = p;
        }

        private void SetPathPoint1(BezierSegment control, double actualWidth, float v)
        {
            var p = control.Point1;
            p.X = actualWidth * v;
            control.Point1 = p;
        }
        private void SetPathPoint2(BezierSegment control, double actualWidth, float v)
        {
            var p = control.Point2;
            p.X = actualWidth * v;
            control.Point2 = p;
        }
        private void SetPathPoint3(BezierSegment control, double actualWidth, float v)
        {
            var p = control.Point3;
            p.X = actualWidth * v;
            control.Point3 = p;
        }
    }
}
