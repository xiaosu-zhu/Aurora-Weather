using Com.Aurora.AuWeather.ViewModels;
using Com.Aurora.Shared.Converters;
using Com.Aurora.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Graphics.Display;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Com.Aurora.AuWeather
{
    public sealed partial class NowWeatherPage : Page
    {
        private double verticalOffset;
        private double actualWidth;
        private bool isAnimating = false;
        private bool isFadeOut = false;
        public double SunRiseStrokeLength
        {
            get { return (double)GetValue(SunRiseStrokeLengthProperty); }
            set { SetValue(SunRiseStrokeLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SunRiseStrokeLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SunRiseStrokeLengthProperty =
            DependencyProperty.Register("SunRiseStrokeLength", typeof(double), typeof(NowWeatherPage), new PropertyMetadata(0d));


        public bool DetailsPanelIsNormalState { get; private set; }

        public NowWeatherPage()
        {
            this.InitializeComponent();
            //DataContext = new NowWeatherPageViewModel();
            
            DataContext.FetchDataComplete += MModel_FetchDataComplete;
            DataContext.ParameterChanged += MModel_ParameterChanged;
        }

        private void MModel_ParameterChanged(object sender, ParameterChangedEventArgs e)
        {
            if (e.Parameter is int)
            {
                TempratureConverter.ChangeParameter((int)e.Parameter);
            }
            if (e.Parameter is string)
            {
                DateTimeConverter.ChangeParameter((string)e.Parameter);
            }
        }

        private async void MModel_FetchDataComplete(object sender, FetchDataCompleteEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(async () =>
            {
                isAnimating = true;
                WeatherCanvas.ChangeCondition(DataContext.Condition, DataContext.IsNight, DataContext.IsSummer);
                TempraturePathAnimation.Begin();
                DetailGrid0Play();
                DetailGrid1Play();
                DetailGrid2Play();
                Forecast0.SetCondition(DataContext.Forecast0, DataContext.IsNight);
                Forecast1.SetCondition(DataContext.Forecast1, DataContext.IsNight);
                Forecast2.SetCondition(DataContext.Forecast2, DataContext.IsNight);
                Forecast3.SetCondition(DataContext.Forecast3, DataContext.IsNight);
                Forecast4.SetCondition(DataContext.Forecast4, DataContext.IsNight);
                await Task.Delay(3000);
                isAnimating = false;
            }));
        }

        private void DetailGrid2Play()
        {
            SunRiseAni.Begin();
        }

        private void DetailGrid0Play()
        {
            DetailTempratureIn.Begin();
            ThreadPoolTimer.CreatePeriodicTimer((work) =>
            {
                this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                 {
                     fengchezhuan.Angle += 0.1396263377777778 * DataContext.Wind.Speed.MPS;
                 }));
            }, TimeSpan.FromMilliseconds(16));
        }

        private void DetailGrid1Play()
        {
            WaterDropTransAni.Begin();
            PcpnTransAni.Begin();
        }

        private void RelativePanel_LayoutUpdated(object sender, object e)
        {
            if (actualWidth != Root.ActualWidth || isAnimating)
            {
                actualWidth = Root.ActualWidth;
                SetPathPoint1(BezierControl1, actualWidth, 1f / 21f);
                SetPathPoint2(BezierControl1, actualWidth, 2f / 21f);
                SetPathPoint3(BezierControl1, actualWidth, 3f / 21f);
                SetPathPoint1(BezierControl2, actualWidth, 4f / 21f);
                SetPathPoint2(BezierControl2, actualWidth, 5f / 21f);
                SetPathPoint3(BezierControl2, actualWidth, 6f / 21f);
                SetPathPoint1(BezierControl3, actualWidth, 7f / 21f);
                SetPathPoint2(BezierControl3, actualWidth, 8f / 21f);
                SetPathPoint3(BezierControl3, actualWidth, 9f / 21f);
                SetPathPoint1(BezierControl4, actualWidth, 10f / 21f);
                SetPathPoint2(BezierControl4, actualWidth, 11f / 21f);
                SetPathPoint3(BezierControl4, actualWidth, 12f / 21f);
                SetPathPoint1(BezierControl5, actualWidth, 13f / 21f);
                SetPathPoint2(BezierControl5, actualWidth, 14f / 21f);
                SetPathPoint3(BezierControl5, actualWidth, 15f / 21f);
                SetPathPoint1(BezierControl6, actualWidth, 16f / 21f);
                SetPathPoint2(BezierControl6, actualWidth, 17f / 21f);
                SetPathPoint3(BezierControl6, actualWidth, 18f / 21f);
                SetPathPoint1(BezierControl7, actualWidth, 19f / 21f);
                SetPathPoint2(BezierControl7, actualWidth, 20f / 21f);
                SetPathPoint3(BezierControl7, actualWidth, 1);
                SetEndPoint(endPoint1, actualWidth);
            }

            if (verticalOffset != ScrollableRoot.VerticalOffset && ScrollableRoot.VerticalOffset < 376)
            {
                verticalOffset = ScrollableRoot.VerticalOffset;
                var offset = verticalOffset > 368 ? 368 : verticalOffset;
                offset /= 368;
                offset = EasingHelper.CircleEase(Windows.UI.Xaml.Media.Animation.EasingMode.EaseOut, offset);
                NowTemp.FontSize = 96 - 48 * offset;
                TempAniTrans.X = -(actualWidth - NowTemp.ActualWidth - ButtonOffset.ActualWidth - 32) * offset / 2;
                if (verticalOffset > 2 && !isFadeOut)
                {
                    isFadeOut = true;
                    TempratureOut.Begin();
                }
                else if (verticalOffset < 2 && isFadeOut)
                {
                    isFadeOut = false;
                    TempratureIn.Begin();
                }

            }
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

        private void ScrollableRoot_LayoutUpdated(object sender, object e)
        {
            if (verticalOffset == ScrollableRoot.VerticalOffset || ScrollableRoot.VerticalOffset > 201)
                return;
            verticalOffset = ScrollableRoot.VerticalOffset;
            ScrollPathPoint(verticalOffset);
        }

        private void ScrollPathPoint(double verticalOffset)
        {
            var offset = verticalOffset > 200 ? 200 : verticalOffset;
            offset = -64 * (1 - offset / 200);
            double[] results = new double[] { DataContext.TempraturePath0 * offset, DataContext.TempraturePath1 * offset, DataContext.TempraturePath2 * offset,
                DataContext.TempraturePath3 * offset, DataContext.TempraturePath4 * offset, DataContext.TempraturePath5 * offset };
            CalculateY0(offset, PathFigure, results[0]);
            CalculateY1(offset, BezierControl1, results[0]);
            CalculateY2(offset, BezierControl2, results[0], results[1]);
            CalculateY2(offset, BezierControl3, results[1], results[2]);
            CalculateY2(offset, BezierControl4, results[2], results[3]);
            CalculateY2(offset, BezierControl5, results[3], results[4]);
            CalculateY2(offset, BezierControl6, results[4], results[5]);
            CalculateY1(offset, BezierControl7, results[5]);
        }

        private void CalculateY0(double offset, PathFigure pathFigure, double result)
        {
            var p = pathFigure.StartPoint;
            p.Y = result;
            pathFigure.StartPoint = p;
        }

        private void CalculateY1(double offset, BezierSegment control, double result)
        {
            var p = control.Point1;
            p.Y = result;
            control.Point1 = p;
            p = control.Point2;
            p.Y = result;
            control.Point2 = p;
            p = control.Point3;
            p.Y = result;
            control.Point3 = p;
        }
        private void CalculateY2(double offset, BezierSegment control, double result1, double result2)
        {
            var p = control.Point1;
            p.Y = result1;
            control.Point1 = p;
            p = control.Point2;
            p.Y = result2;
            control.Point2 = p;
            p = control.Point3;
            p.Y = result2;
            control.Point3 = p;
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            DataContext.FetchDataComplete -= MModel_FetchDataComplete;
            DataContext.ParameterChanged -= MModel_ParameterChanged;
        }

        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DetailsPanel.ActualWidth < 720)
            {
                this.DetailsPanelGotoNormalState();
            }
            else
            {
                this.DetailsPanelGotoWideState();
            }
        }

        private void DetailsPanelGotoNormalState()
        {
            if (!DetailsPanelIsNormalState)
            {
                return;
            }
            else
            {
                DetailsPanelIsNormalState = false;
                var length = GridLength.Auto;
                Column2.Width = length;
                UIHelper.SetControlPositioninGrid(DetailGrid2, 1, 0);
                UIHelper.SetControlPositioninGrid(DetailGrid3, 1, 1);
                UIHelper.SetControlPositioninGrid(DetailGrid4, 2, 0);
                UIHelper.SetControlPositioninGrid(DetailGrid5, 2, 1);
                UIHelper.SetControlPositioninGrid(DetailGrid6, 3, 0);
                UIHelper.SetControlPositioninGrid(DetailGrid7, 3, 1);
                UIHelper.SetControlPositioninGrid(DetailGrid8, 3, 2);
                UIHelper.ReverseVisibility(DetailGrid8);
            }
        }

        private void DetailsPanelGotoWideState()
        {
            if (DetailsPanelIsNormalState)
            {
                return;
            }
            else
            {
                DetailsPanelIsNormalState = true;
                var length = new GridLength(1, GridUnitType.Star);
                Column2.Width = length;
                UIHelper.SetControlPositioninGrid(DetailGrid2, 0, 2);
                UIHelper.SetControlPositioninGrid(DetailGrid3, 1, 0);
                UIHelper.SetControlPositioninGrid(DetailGrid4, 1, 1);
                UIHelper.SetControlPositioninGrid(DetailGrid5, 1, 2);
                UIHelper.SetControlPositioninGrid(DetailGrid6, 2, 0);
                UIHelper.SetControlPositioninGrid(DetailGrid7, 2, 1);
                UIHelper.SetControlPositioninGrid(DetailGrid8, 2, 2);
                UIHelper.ReverseVisibility(DetailGrid8);
            }
        }


    }
}
