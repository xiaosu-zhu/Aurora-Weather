using Com.Aurora.AuWeather.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
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
            WeatherCanvas.ChangeCondition(Models.WeatherCondition.moderate_rain, false, false);
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
            var actualWidth = Root.ActualWidth;
            SetPathPoint1(BezierControl1, actualWidth, 1f / 18f);
            SetPathPoint2(BezierControl1, actualWidth, 2f / 18f);
            SetPathPoint3(BezierControl1, actualWidth, 3f / 18f);
            SetPathPoint1(BezierControl2, actualWidth, 4f / 18f);
            SetPathPoint2(BezierControl2, actualWidth, 5f / 18f);
            SetPathPoint3(BezierControl2, actualWidth, 6f / 18f);
            SetPathPoint1(BezierControl3, actualWidth, 7f / 18f);
            SetPathPoint2(BezierControl3, actualWidth, 8f / 18f);
            SetPathPoint3(BezierControl3, actualWidth, 9f / 18f);
            SetPathPoint1(BezierControl4, actualWidth, 10f / 18f);
            SetPathPoint2(BezierControl4, actualWidth, 11f / 18f);
            SetPathPoint3(BezierControl4, actualWidth, 12f / 18f);
            SetPathPoint1(BezierControl5, actualWidth, 13f / 18f);
            SetPathPoint2(BezierControl5, actualWidth, 14f / 18f);
            SetPathPoint3(BezierControl5, actualWidth, 15f / 18f);
            SetPathPoint1(BezierControl6, actualWidth, 16f / 18f);
            SetPathPoint2(BezierControl6, actualWidth, 17f / 18f);
            SetPathPoint3(BezierControl6, actualWidth, 1);
            SetEndPoint(endPoint1, actualWidth);
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
