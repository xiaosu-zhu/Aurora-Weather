using Com.Aurora.AuWeather.ViewModels;
using Com.Aurora.AuWeather.ViewModels.Events;
using Com.Aurora.Shared.Converters;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System;
using System.Threading.Tasks;
using Windows.Foundation;
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
        /// <summary>
        /// use binary: 1111 1111 to implement every DetailGrid Animation status
        /// </summary>
        private int detailGridAnimation_FLAG = 0;

        private Point[] DetailGridPoint = new Point[9];

        public double SunRiseStrokeLength
        {
            get { return (double)GetValue(SunRiseStrokeLengthProperty); }
            set { SetValue(SunRiseStrokeLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SunRiseStrokeLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SunRiseStrokeLengthProperty =
            DependencyProperty.Register("SunRiseStrokeLength", typeof(double), typeof(NowWeatherPage), new PropertyMetadata(0d));


        public bool DetailsPanelIsNormalState = true;

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
            CalcDetailGridPosition();
            isAnimating = true;
            WeatherCanvas.ChangeCondition(DataContext.Condition, DataContext.IsNight, DataContext.IsSummer);
            ScrollableRoot.ViewChanged += ScrollableRoot_ViewChanged;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(async () =>
            {
                TempraturePathAnimation.Begin();
                Forecast0.SetCondition(DataContext.Forecast0, DataContext.IsNight);
                Forecast1.SetCondition(DataContext.Forecast1, DataContext.IsNight);
                Forecast2.SetCondition(DataContext.Forecast2, DataContext.IsNight);
                Forecast3.SetCondition(DataContext.Forecast3, DataContext.IsNight);
                Forecast4.SetCondition(DataContext.Forecast4, DataContext.IsNight);
                await Task.Delay(3000);
                isAnimating = false;
            }));
        }

        #region DetailGrid Animation
        private void DetailGrid0Play()
        {
            if (detailGridAnimation_FLAG % 2 == 0)
            {
                DetailTempratureIn.Begin();
                detailGridAnimation_FLAG++;
            }
        }
        private void DetailGrid1Play()
        {
            if ((detailGridAnimation_FLAG >> 1) % 2 == 0)
            {
                ThreadPoolTimer.CreatePeriodicTimer((work) =>
                                {
                                    this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                                     {
                                         fengchezhuan.Angle += 0.1396263377777778 * DataContext.Wind.Speed.MPS;
                                     }));
                                }, TimeSpan.FromMilliseconds(16));
                detailGridAnimation_FLAG += 2;
            }
        }
        private void DetailGrid2Play()
        {
            if ((detailGridAnimation_FLAG >> 2) % 2 == 0)
            {
                WaterDropTransAni.Begin();
                detailGridAnimation_FLAG += 4;
            }
        }
        private void DetailGrid3Play()
        {
            if ((detailGridAnimation_FLAG >> 3) % 2 == 0)
            {
                PcpnTransAni.Begin();
                detailGridAnimation_FLAG += 8;
            }
        }
        private void DetailGrid4Play()
        {
            if ((detailGridAnimation_FLAG >> 4) % 2 == 0)
            {
                SunRiseAni.Begin();
                detailGridAnimation_FLAG += 16;
            }
        }
        private void DetailGrid6Play()
        {
            if ((detailGridAnimation_FLAG >> 5) % 2 == 0)
            {
                VisTransAni.Begin();
                detailGridAnimation_FLAG += 32;
            }
        }
        private void DetailGrid7Play()
        {
            if ((detailGridAnimation_FLAG >> 6) % 2 == 0)
            {
                PressureTransAni.Begin();
                detailGridAnimation_FLAG += 64;
            }
        }
        private void DetailGrid8Play()
        {
            if ((detailGridAnimation_FLAG >> 6) % 2 == 0)
            {
                detailGridAnimation_FLAG += 128;
            }
        }
        #endregion

        #region Hold Bezier
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
                ScrollPathPoint(verticalOffset);
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
        #endregion

        #region Scroll Bezier
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
        #endregion

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            DataContext.FetchDataComplete -= MModel_FetchDataComplete;
            DataContext.ParameterChanged -= MModel_ParameterChanged;
        }

        #region Change Layout
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DetailsPanel.ActualWidth < 720 && !DetailsPanelIsNormalState)
            {
                DetailsPanelGotoNormalState();
            }
            else if (DetailsPanel.ActualWidth >= 720 && DetailsPanelIsNormalState)
            {
                DetailsPanelGotoWideState();
            }
        }

        private void DetailsPanelGotoNormalState()
        {
            DetailsPanelIsNormalState = true;
            var length = GridLength.Auto;
            Column2.Width = length;
            UIHelper.Change_Row_Column(DetailGrid2, 1, 0);
            UIHelper.Change_Row_Column(DetailGrid3, 1, 1);
            UIHelper.Change_Row_Column(DetailGrid4, 2, 0);
            UIHelper.Change_Row_Column(DetailGrid5, 2, 1);
            UIHelper.Change_Row_Column(DetailGrid6, 3, 0);
            UIHelper.Change_Row_Column(DetailGrid7, 3, 1);
            UIHelper.Change_Row_Column(DetailGrid8, 3, 2);
            UIHelper.ReverseVisibility(DetailGrid8);
        }

        private void DetailsPanelGotoWideState()
        {
            DetailsPanelIsNormalState = false;
            var length = new GridLength(1, GridUnitType.Star);
            Column2.Width = length;
            UIHelper.Change_Row_Column(DetailGrid2, 0, 2);
            UIHelper.Change_Row_Column(DetailGrid3, 1, 0);
            UIHelper.Change_Row_Column(DetailGrid4, 1, 1);
            UIHelper.Change_Row_Column(DetailGrid5, 1, 2);
            UIHelper.Change_Row_Column(DetailGrid6, 2, 0);
            UIHelper.Change_Row_Column(DetailGrid7, 2, 1);
            UIHelper.Change_Row_Column(DetailGrid8, 2, 2);
            UIHelper.ReverseVisibility(DetailGrid8);
        }
        #endregion

        private void CalcDetailGridPosition()
        {
            DetailGridPoint[0] = DetailGrid0.GetPositioninRoot(ScrollableRoot);
            DetailGridPoint[1] = DetailGrid1.GetPositioninRoot(ScrollableRoot);
            DetailGridPoint[2] = DetailGrid2.GetPositioninRoot(ScrollableRoot);
            DetailGridPoint[3] = DetailGrid3.GetPositioninRoot(ScrollableRoot);
            DetailGridPoint[4] = DetailGrid4.GetPositioninRoot(ScrollableRoot);
            DetailGridPoint[5] = DetailGrid5.GetPositioninRoot(ScrollableRoot);
            DetailGridPoint[6] = DetailGrid6.GetPositioninRoot(ScrollableRoot);
            DetailGridPoint[7] = DetailGrid7.GetPositioninRoot(ScrollableRoot);
            DetailGridPoint[8] = DetailGrid8.GetPositioninRoot(ScrollableRoot);
            for (int i = 0; i < DetailGridPoint.Length; i++)
            {
                DetailGridPoint[i].Y += ScrollableRoot.VerticalOffset;
            }
        }

        private void ScrollableRoot_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            CalcDetailGridPosition();
            DetailGridPlay(ScrollableRoot.ActualHeight + ScrollableRoot.VerticalOffset - 240);
        }

        private void DetailGridPlay(double offsetProgress)
        {
            if (offsetProgress > DetailGridPoint[0].Y)
            {
                DetailGrid0Play();
            }
            if (offsetProgress > DetailGridPoint[1].Y)
            {
                DetailGrid1Play();
            }
            if (offsetProgress > DetailGridPoint[2].Y)
            {
                DetailGrid2Play();
            }
            if (offsetProgress > DetailGridPoint[3].Y)
            {
                DetailGrid3Play();
            }
            if (offsetProgress > DetailGridPoint[4].Y)
            {
                DetailGrid4Play();
            }
            if (offsetProgress > DetailGridPoint[6].Y)
            {
                DetailGrid6Play();
            }
            if (offsetProgress > DetailGridPoint[7].Y)
            {
                DetailGrid7Play();
            }
            if (offsetProgress > DetailGridPoint[8].Y)
            {
                DetailGrid8Play();
            }
        }


    }
}
