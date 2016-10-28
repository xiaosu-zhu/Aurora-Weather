// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.CustomControls;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.AuWeather.ViewModels.Events;
using Com.Aurora.Shared.Converters;
using Com.Aurora.Shared.Helpers;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Foundation;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace Com.Aurora.AuWeather
{
    public sealed partial class NowWeatherPage : Page, IThemeble
    {
        private const double FENGCHE_ZHUANSU = 0.1396263377777778;
        private const int NORMAL_SIZE_WIDTH = 720;
        private const int WIDE_SIZE_WIDTH = 1024;
        private bool loaded = false;
        private bool isFadeOut = false;
        /// <summary>
        /// use binary: 1111 1111 to implement every DetailGrid Animation status
        /// </summary>
        private bool[] detailGridAnimation_FLAG = new bool[7] { false, false, false, false, false, false, false };

        private double[] DetailGridPoint = new double[8];

        public double SunRiseStrokeLength
        {
            get { return (double)GetValue(SunRiseStrokeLengthProperty); }
            set { SetValue(SunRiseStrokeLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SunRiseStrokeLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SunRiseStrokeLengthProperty =
            DependencyProperty.Register("SunRiseStrokeLength", typeof(double), typeof(NowWeatherPage), new PropertyMetadata(0d));


        public double AqiCircleStorkeLength
        {
            get { return (double)GetValue(AqiCircleStorkeLengthProperty); }
            set { SetValue(AqiCircleStorkeLengthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AqiCircleStorkeLength.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AqiCircleStorkeLengthProperty =
            DependencyProperty.Register("AqiCircleStorkeLength", typeof(double), typeof(NowWeatherPage), new PropertyMetadata(0d));

        public bool DetailsPanelIsNormalState = true;
        private bool rootIsWideState = false;
        private ThreadPoolTimer fengcheTimer;
        private ThreadPoolTimer immersiveTimer;
        private bool isImmersiveMode = false;
        private bool isImmersiveAllIn = false;
        private MainPage baba = MainPage.Current;

        public NowWeatherPage()
        {
            this.InitializeComponent();
            Context.FetchDataComplete += MModel_FetchDataComplete;
            Context.ParameterChanged += MModel_ParameterChanged;
            Context.FetchDataFailed += Context_FetchDataFailed;
            Context.TimeUpdated += Context_TimeUpdated;
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
            //DataContext = new NowWeatherPageViewModel();
            var loader = new ResourceLoader();
            UpdateIndText.Text = loader.GetString("RefreshStart");
        }

        private void Context_TimeUpdated(object sender, TimeUpdatedEventArgs e)
        {
            detailGridAnimation_FLAG[4] = false;
            DetailGrid4Play();
            baba.ReloadTheme();
            if (e.IsDayNightChanged)
            {
                if ((Context.Condition == WeatherCondition.sunny) || (Context.Condition == WeatherCondition.windy)
                || (Context.Condition == WeatherCondition.calm) ||
                (Context.Condition == WeatherCondition.light_breeze) ||
                (Context.Condition == WeatherCondition.moderate) ||
                (Context.Condition == WeatherCondition.fresh_breeze) ||
                (Context.Condition == WeatherCondition.strong_breeze) ||
                (Context.Condition == WeatherCondition.high_wind) ||
                (Context.Condition == WeatherCondition.gale))
                {
                    WeatherCanvas.ChangeCondition(Context.Condition, Context.IsNight, Context.IsSummer);
                    if (!Context.IsNight)
                    {
                        var s = new SolidColorBrush(Colors.Black);
                        baba.ChangeColor(s);
                        RefreshButton.Foreground = s;
                    }
                    else
                    {
                        baba.ChangeColor(new SolidColorBrush(Colors.White));
                        RefreshButton.Foreground = new SolidColorBrush(Colors.White);
                    }
                }
            }
        }

        private async void Context_FetchDataFailed(object sender, FetchDataFailedEventArgs e)
        {
            this.Context.FetchDataFailed -= Context_FetchDataFailed;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(async () =>
             {
                 var loader = new ResourceLoader();
                 var d = new MessageDialog(e.Message);
                 d.Title = loader.GetString("Error");
                 d.Commands.Add(new UICommand(loader.GetString("Setting"), new UICommandInvokedHandler(NavigateToSettings)));
                 d.Commands.Add(new UICommand(loader.GetString("Quit"), new UICommandInvokedHandler(QuitAll)));
                 d.CancelCommandIndex = 1;
                 d.DefaultCommandIndex = 0;
                 await d.ShowAsync();
             }));
        }

        private void DataFailed_Refresh(IUICommand command)
        {
            Context.FetchDataFailed += Context_FetchDataFailed;
            Context.RefreshAsync();
        }

        private void QuitAll(IUICommand command)
        {
            App.Current.Exit();
        }

        private void NavigateToSettings(IUICommand command)
        {
            Context.FetchDataFailed += Context_FetchDataFailed;
            baba.Navigate(typeof(SettingsPage));
        }

        private async void Current_Resuming(object sender, object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
              {
                  Context.RefreshCurrentTime();
                  Context.CurrentTimeRefreshTask();
              }));
        }

        private void MModel_ParameterChanged(object sender, ParameterChangedEventArgs e)
        {

        }

        internal void Refresh()
        {
            Context.RefreshAsync();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            baba.ChangeColor(Colors.Transparent, Colors.White, new SolidColorBrush(Colors.White));
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            this.Page_Unloaded(null, null);
        }

        private async void MModel_FetchDataComplete(object sender, FetchDataCompleteEventArgs e)
        {
            var loader = new ResourceLoader();
            UpdateIndText.Text = loader.GetString("RefreshComplete");
            LoadingDot.IsActive = false;
            Forecast0.SetCondition(Context.Forecast0, Context.IsNight);
            Forecast1.SetCondition(Context.Forecast1, Context.IsNight);
            Forecast2.SetCondition(Context.Forecast2, Context.IsNight);
            Forecast3.SetCondition(Context.Forecast3, Context.IsNight);
            Forecast4.SetCondition(Context.Forecast4, Context.IsNight);
            if ((Context.Condition == WeatherCondition.sunny) || (Context.Condition == WeatherCondition.windy)
                || (Context.Condition == WeatherCondition.calm) ||
                (Context.Condition == WeatherCondition.light_breeze) ||
                (Context.Condition == WeatherCondition.moderate) ||
                (Context.Condition == WeatherCondition.fresh_breeze) ||
                (Context.Condition == WeatherCondition.strong_breeze) ||
                (Context.Condition == WeatherCondition.high_wind) ||
                (Context.Condition == WeatherCondition.gale))
            {
                if (!Context.IsNight)
                {
                    var s = new SolidColorBrush(Colors.Black);
                    baba.ChangeColor(s);
                    RefreshButton.Foreground = s;
                }
                else
                {
                    baba.ChangeColor(new SolidColorBrush(Colors.White));
                    RefreshButton.Foreground = new SolidColorBrush(Colors.White);
                }
            }
            else
            {
                baba.ChangeColor(new SolidColorBrush(Colors.White));
                RefreshButton.Foreground = new SolidColorBrush(Colors.White);
            }
            WeatherCanvas.ChangeCondition(Context.Condition, Context.IsNight, Context.IsSummer);
            if (Context.Aqi == null)
            {
                AQIPanel.Visibility = Visibility.Collapsed;
            }
            if (Context.Comf == null && Context.Cw == null && Context.Drsg == null)
            {
                SuggestionPanel.Visibility = Visibility.Collapsed;
            }
            switch (Context.source)
            {
                case DataSource.HeWeather:
                    DataSourceImage.Source = new BitmapImage(new Uri("http://heweather.com/weather/images/logo.jpg"));
                    DataSourceContent.Text = loader.GetString("HeWeather");
                    break;
                case DataSource.Caiyun:
                    DataSourceImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Logos/Caiyun.png"));
                    DataSourceContent.Text = loader.GetString("CaiyunWeather");
                    break;
                case DataSource.Wunderground:
                    DataSourceImage.Source = new BitmapImage(new Uri("ms-appx:///Assets/Logos/Wunder.png"));
                    DataSourceContent.Text = string.Empty;
                    break;
                default:
                    break;
            }
            baba.ChangeCondition(Context.Condition, Context.IsNight, Context.City, Context.NowL, Context.NowH);
            ScrollableRoot.RefreshComplete();
            if(LoadingDot.Visibility == Visibility.Collapsed)
            {
                TempraturePathAnimation.Completed += (s, v) =>
                {
                    RefreshCompleteAni.Begin();
                };
                TempraturePathAnimation.Begin();
                AQIAni.Begin();
                DetailGrid0Play();
                DetailGrid1Play();
                DetailGrid2Play();
                DetailGrid3Play();
                DetailGrid4Play();
                DetailGrid6Play();
                DetailGrid7Play();
            }

            try
            {
                if (Windows.System.UserProfile.UserProfilePersonalizationSettings.IsSupported() && Context.SetWallPaper)
                {
                    var file = await FileIOHelper.GetFilebyUriAsync(await Context.GetCurrentBackgroundAsync());
                    var lFile = await FileIOHelper.CreateWallPaperFileAsync(Guid.NewGuid().ToString() + ".png");
                    var d = Windows.Devices.Input.PointerDevice.GetPointerDevices();
                    var m = d.ToArray();
                    var scaleFactor = Windows.Graphics.Display.DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                    var size = new Size(m[0].PhysicalDeviceRect.Width, m[0].PhysicalDeviceRect.Height);
                    var ratio = size.Height / size.Width;
                    size.Height *= scaleFactor;
                    size.Width *= scaleFactor;
                    var cropSize = new Size();
                    double scale;
                    var startPoint = new Point();
                    using (var stream = await file.OpenReadAsync())
                    {
                        var bitmap = new BitmapImage();
                        await bitmap.SetSourceAsync(stream);
                        var width = bitmap.PixelWidth;
                        var height = bitmap.PixelHeight;
                        var center = new Point(width / 2, height / 2);
                        if (width * ratio >= height)
                        {
                            cropSize.Width = height / ratio;
                            cropSize.Height = height;
                            scale = size.Height / height;
                        }
                        else
                        {
                            cropSize.Width = width;
                            cropSize.Height = width * ratio;
                            scale = size.Width / width;
                        }

                        startPoint.X = center.X - cropSize.Width / 2;
                        startPoint.Y = center.Y - cropSize.Height / 2;
                    }
                    await ImagingHelper.CropandScaleAsync(file, lFile, startPoint, cropSize, scale);
                    var uc = await ImagingHelper.SetWallpaperAsync(lFile);
                }
            }
            catch (Exception)
            {
            }

        }


        private async void LoadingDot_DotFinish(object sender, EventArgs e)
        {
            TempraturePathAnimation.Completed += (s, v) =>
            {
                RefreshCompleteAni.Begin();
            };
            TempraturePathAnimation.Begin();
            AQIAni.Begin();
            if (rootIsWideState)
            {
                DetailGrid0Play();
                DetailGrid1Play();
                DetailGrid2Play();
                DetailGrid3Play();
                DetailGrid4Play();
                DetailGrid6Play();
                DetailGrid7Play();
            }
            for (int i = 0; i < detailGridAnimation_FLAG.Length; i++)
            {
                detailGridAnimation_FLAG[i] = false;
            }
            CalcDetailGridPosition();
            loaded = true;
            ScrollableRoot.ViewChanged += ScrollableRoot_ViewChanged;
            if (Context.AlwaysShowBackground)
            {
                WeatherCanvas.ImmersiveIn(await Context.GetCurrentBackgroundAsync());
            }
            LoadingDot.Visibility = Visibility.Collapsed;
        }

        #region DetailGrid Animation
        private void DetailGrid0Play()
        {
            if (!detailGridAnimation_FLAG[0])
            {
                DetailTempratureIn.Begin();
                detailGridAnimation_FLAG[0] = true;
            }
        }
        private void DetailGrid1Play()
        {
            if (isImmersiveMode)
            {
                return;
            }
            if (!detailGridAnimation_FLAG[1])
            {
                if (fengcheTimer != null)
                {
                    fengcheTimer.Cancel();
                }
                fengcheTimer = ThreadPoolTimer.CreatePeriodicTimer((work) =>
                                  {
                                      if (!Context.EnableDynamic)
                                      {
                                          return;
                                      }
                                      var task = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, new Windows.UI.Core.DispatchedHandler(() =>
                                                                         {
                                                                             fengchezhuan.Angle += FENGCHE_ZHUANSU * Context.Wind.Speed.MPS;
                                                                         }));

                                  }, TimeSpan.FromMilliseconds(16));
                detailGridAnimation_FLAG[1] = true;
            }
        }
        private void DetailGrid2Play()
        {
            if (!detailGridAnimation_FLAG[2])
            {
                WaterDropTransAni.Begin();
                detailGridAnimation_FLAG[2] = true;
            }
        }
        private void DetailGrid3Play()
        {
            if (!detailGridAnimation_FLAG[3])
            {
                PcpnTransAni.Begin();
                detailGridAnimation_FLAG[3] = true;
            }
        }
        private void DetailGrid4Play()
        {
            if (!detailGridAnimation_FLAG[4])
            {
                SunRiseAni.Begin();
                detailGridAnimation_FLAG[4] = true;
            }
        }
        private void DetailGrid6Play()
        {
            if (!detailGridAnimation_FLAG[5])
            {
                VisTransAni.Begin();
                detailGridAnimation_FLAG[5] = true;
            }
        }
        private void DetailGrid7Play()
        {
            if (!detailGridAnimation_FLAG[6])
            {
                PressureTransAni.Begin();
                detailGridAnimation_FLAG[6] = true;
            }
        }
        #endregion
        private void RelativePanel_LayoutUpdated(object sender, object e)
        {

            var v = ScrollableRoot.VerticalOffset;
            if (v < 536)
            {
                if (v > 2 && !isFadeOut)
                {
                    isFadeOut = true;
                    TempratureOut.Begin();
                }
                else if (v < 2 && isFadeOut)
                {
                    isFadeOut = false;
                    TempratureIn.Begin();
                }
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Context.FetchDataComplete -= MModel_FetchDataComplete;
            Context.ParameterChanged -= MModel_ParameterChanged;
            Context.FetchDataFailed -= Context_FetchDataFailed;
            Context.TimeUpdated -= Context_TimeUpdated;
            Context.Unload();
            if (immersiveTimer != null)
            {
                immersiveTimer.Cancel();
                immersiveTimer = null;
            }
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
            if (fengcheTimer != null)
            {
                fengcheTimer.Cancel();
                fengcheTimer = null;
            }
            LoadingDot.IsActive = false;
        }

        #region DetailsPanel Change Layout
        private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (DetailsPanel.ActualWidth < NORMAL_SIZE_WIDTH && !DetailsPanelIsNormalState)
            {
                DetailsPanelGotoNormalState();
            }
            else if (DetailsPanel.ActualWidth >= NORMAL_SIZE_WIDTH && DetailsPanelIsNormalState)
            {
                DetailsPanelGotoWideState();
            }
            CalcDetailGridPosition();
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
            var headerHeight = WeatherCanvas.ActualHeight + Forecast0.ActualHeight + AQIPanel.ActualHeight;
            if (DetailsPanelIsNormalState)
            {
                DetailGridPoint[0] = headerHeight;
                DetailGridPoint[1] = DetailGridPoint[0];
                DetailGridPoint[2] = DetailGridPoint[0] + DetailGrid0.ActualHeight;
                DetailGridPoint[3] = DetailGridPoint[2];
                DetailGridPoint[4] = DetailGridPoint[2] + DetailGrid2.ActualHeight;
                DetailGridPoint[5] = DetailGridPoint[4];
                DetailGridPoint[6] = DetailGridPoint[4] + DetailGrid4.ActualHeight;
                DetailGridPoint[7] = DetailGridPoint[6];
            }
            else
            {
                DetailGridPoint[0] = headerHeight;
                DetailGridPoint[1] = DetailGridPoint[0];
                DetailGridPoint[2] = DetailGridPoint[0];
                DetailGridPoint[3] = DetailGridPoint[0] + DetailGrid0.ActualHeight;
                DetailGridPoint[4] = DetailGridPoint[3];
                DetailGridPoint[5] = DetailGridPoint[3];
                DetailGridPoint[6] = DetailGridPoint[3] + DetailGrid3.ActualHeight;
                DetailGridPoint[7] = DetailGridPoint[6];
            }
        }

        private void ScrollableRoot_ViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            DetailGridPlay(ScrollableRoot.VerticalOffset + 480);
        }

        private void DetailGridPlay(double offsetProgress)
        {
            if (offsetProgress > DetailGridPoint[0])
            {
                DetailGrid0Play();
            }
            if (offsetProgress > DetailGridPoint[1])
            {
                DetailGrid1Play();
            }
            if (offsetProgress > DetailGridPoint[2])
            {
                DetailGrid2Play();
            }
            if (offsetProgress > DetailGridPoint[3])
            {
                DetailGrid3Play();
            }
            if (offsetProgress > DetailGridPoint[4])
            {
                DetailGrid4Play();
            }
            if (offsetProgress > DetailGridPoint[6])
            {
                DetailGrid6Play();
            }
            if (offsetProgress > DetailGridPoint[7])
            {
                DetailGrid7Play();
            }
        }

        #region Root Mode Changing
        private void Root_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if ((Window.Current.Content as Frame).ActualWidth < NORMAL_SIZE_WIDTH)
            {
                RootGotoNarrowState();
            }
            else if ((Window.Current.Content as Frame).ActualWidth < WIDE_SIZE_WIDTH)
            {
                RootGotoNormalState();
            }
            else if ((Window.Current.Content as Frame).ActualWidth >= WIDE_SIZE_WIDTH && !rootIsWideState)
            {
                RootGotoWideState();
            }
        }

        private void RootGotoNarrowState()
        {
            if (isImmersiveMode)
            {
                isImmersiveMode = false;
                ImmersiveBackButton_Click(null, null);
            }
            if (rootIsWideState)
            {
                rootIsWideState = false;
                LargeModeSubPanel.Content = null;
                WeatherPanel.Children.Add(DetailsPanel);
            }
            UIHelper.ChangeTitlebarButtonColor(Colors.Transparent, Colors.White);
        }

        private void RootGotoWideState()
        {
            Color c;
            if (Context.Theme == ElementTheme.Dark)
            {
                var d = this.Resources.ThemeDictionaries["Dark"] as ResourceDictionary;
                c = (Color)d["SystemBaseHighColor"];
            }
            else if (Context.Theme == ElementTheme.Light)
            {
                var d = this.Resources.ThemeDictionaries["Light"] as ResourceDictionary;
                c = (Color)d["SystemBaseHighColor"];
            }
            else
            {
                c = (Color)Resources["SystemBaseHighColor"];
            }
            UIHelper.ChangeTitlebarButtonColor(Colors.Transparent, c);
            rootIsWideState = true;
            WeatherPanel.Children.Remove(DetailsPanel);
            LargeModeSubPanel.Content = DetailsPanel;
            if (loaded)
            {
                if (fengcheTimer != null)
                {
                    fengcheTimer.Cancel();
                    detailGridAnimation_FLAG[1] = false;
                }
                DetailGrid0Play();
                DetailGrid1Play();
                DetailGrid2Play();
                DetailGrid3Play();
                DetailGrid4Play();
                DetailGrid6Play();
                DetailGrid7Play();
            }
        }

        private void RootGotoNormalState()
        {
            if (rootIsWideState)
            {
                rootIsWideState = false;
                LargeModeSubPanel.Content = null;
                WeatherPanel.Children.Add(DetailsPanel);
                UIHelper.ChangeTitlebarButtonColor(Colors.Transparent, Colors.White);
            }
        }
        #endregion

        private async void ImmersiveButton_Click(object sender, RoutedEventArgs e)
        {
            if (Context.EnableFullScreen)
            {
                ApplicationView.GetForCurrentView().TryEnterFullScreenMode();
                await Task.Delay(160);
            }
            isImmersiveMode = true;
            MainCanvas.PointerMoved += MainCanvas_PointerMoved;
            ImmersiveWidthIn.From = MainCanvas.ActualWidth;
            ImmersiveWidthIn.To = Root.ActualWidth;
            ImmersiveHeightIn.From = MainCanvas.ActualHeight;
            ImmersiveHeightIn.To = Root.ActualHeight;
            UIHelper.ChangeTitlebarButtonColor(Colors.Transparent, Colors.White);
            Application.Current.Resuming += Current_Resuming;
            ImmersiveTransAni.Completed += (s, args) =>
            {
                MainCanvas.Width = double.NaN;
                MainCanvas.Height = double.NaN;
                if (Context.ImmersiveTimeout == 0)
                {
                    if (immersiveTimer != null)
                    {
                        immersiveTimer.Cancel();
                    }
                    return;
                }
                if (immersiveTimer != null)
                {
                    immersiveTimer.Cancel();

                }
                immersiveTimer = ThreadPoolTimer.CreateTimer(async (task) =>
                    {
                        await GotoImmersiveAllin();
                    }, TimeSpan.FromSeconds(Context.ImmersiveTimeout));
            };
            ImmersiveTransAni.Begin();
            if (fengcheTimer != null)
            {
                fengcheTimer.Cancel();
                detailGridAnimation_FLAG[1] = false;
            }
            await Task.Delay(1000);
            WeatherCanvas.ImmersiveIn(await Context.GetCurrentBackgroundAsync());
            //UserProfilePersonalizationSettings profileSettings = UserProfilePersonalizationSettings.Current;
            // Only support files stored in local folder


        }

        private void MainCanvas_PointerMoved(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (isImmersiveMode)
            {
                if (isImmersiveAllIn)
                {
                    Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
                    ImmersiveAllBack.Begin();
                    isImmersiveAllIn = false;
                }
                if (Context.ImmersiveTimeout == 0)
                {
                    if (immersiveTimer != null)
                    {
                        immersiveTimer.Cancel();
                    }
                    return;
                }
                if (immersiveTimer != null)
                {
                    immersiveTimer.Cancel();

                }
                immersiveTimer = ThreadPoolTimer.CreateTimer(async (task) =>
                {
                    await GotoImmersiveAllin();

                }, TimeSpan.FromSeconds(Context.ImmersiveTimeout));
            }
        }

        private async Task GotoImmersiveAllin()
        {
            if (isImmersiveMode)
            {
                var t = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
                {
                    ImmersiveAllIn.Begin();
                    Window.Current.CoreWindow.PointerCursor = null;
                }));
                await Task.Delay(160);
                isImmersiveAllIn = true;
            }
        }

        private void ImmersiveBackButton_Click(object sender, RoutedEventArgs e)
        {
            MainCanvas.PointerMoved -= MainCanvas_PointerMoved;
            if (immersiveTimer != null)
            {
                immersiveTimer.Cancel();
                immersiveTimer = null;
            }
            Window.Current.CoreWindow.PointerCursor = new Windows.UI.Core.CoreCursor(Windows.UI.Core.CoreCursorType.Arrow, 1);
            ImmersiveHeightBack.From = MainCanvas.ActualHeight;
            ImmersiveWidthBack.From = MainCanvas.ActualWidth;
            ImmersiveHeightBack.To = ScrollViewerConverter.WeatherCanvasHeight - ScrollableRoot.VerticalOffset < 160 ? 160 : ScrollViewerConverter.WeatherCanvasHeight - ScrollableRoot.VerticalOffset;
            ImmersiveWidthBack.To = rootIsWideState ? Root.ActualWidth / 2 : Root.ActualWidth;
            App.Current.Resuming -= Current_Resuming;
            ImmersiveBackAni.Completed += (s, args) =>
            {
                if (isFadeOut)
                {
                    TempratureOut.Begin();
                }
                Binding HeightBinding = new Binding();
                HeightBinding.Source = ScrollableRoot;
                HeightBinding.Path = new PropertyPath("VerticalOffset");
                HeightBinding.Converter = new ScrollViewerConverter();
                BindingOperations.SetBinding(MainCanvas, HeightProperty, HeightBinding);
                MainCanvas.Width = double.NaN;
                DetailGrid1Play();
            };
            ImmersiveBackAni.Completed += (s, v) =>
            {
                ApplicationView.GetForCurrentView().ExitFullScreenMode();

            };
            ImmersiveBackAni.Begin();
            isImmersiveMode = false;
            WeatherCanvas.ImmersiveOut(!Context.AlwaysShowBackground);
        }

        private void ScrollableRoot_RefreshStart(object sender, Aurora.Shared.Controls.RefreshStartEventArgs e)
        {
            var loader = new ResourceLoader();
            UpdateIndText.Text = loader.GetString("RefreshStart");
            Context.RefreshAsync();
        }

        private void Flyout_Opened(object sender, object e)
        {
            if (immersiveTimer != null)
            {
                immersiveTimer.Cancel();
            }
        }

        private void Flyout_Closed(object sender, object e)
        {
            if(Context.ImmersiveTimeout == 0)
            {
                if (immersiveTimer != null)
                {
                    immersiveTimer.Cancel();
                }
                return;
            }
            if (immersiveTimer != null)
            {
                immersiveTimer.Cancel();
            }
            immersiveTimer = ThreadPoolTimer.CreateTimer(async (task) =>
            {
                await GotoImmersiveAllin();
            }, TimeSpan.FromSeconds(Context.ImmersiveTimeout));
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            baba.NavigatetoSettings(typeof(Cities));
            Color c;
            SolidColorBrush s;
            if (Context.Theme == ElementTheme.Dark)
            {
                var d = this.Resources.ThemeDictionaries["Dark"] as ResourceDictionary;
                c = (Color)d["SystemBaseHighColor"];
                s = (SolidColorBrush)d["SystemControlForegroundBaseHighBrush"];
            }
            else if (Context.Theme == ElementTheme.Light)
            {
                var d = this.Resources.ThemeDictionaries["Light"] as ResourceDictionary;
                c = (Color)d["SystemBaseHighColor"];
                s = (SolidColorBrush)d["SystemControlForegroundBaseHighBrush"];
            }
            else
            {
                c = (Color)Resources["SystemBaseHighColor"];
                s = (SolidColorBrush)Resources["SystemControlForegroundBaseHighBrush"];
            }
            baba.ChangeColor(Colors.Transparent, c, s);
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            var loader = new ResourceLoader();
            UpdateIndText.Text = loader.GetString("RefreshStart");
        }

        private void MainCanvas_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (isImmersiveAllIn || isImmersiveMode)
            {

            }
            else
            {
                ScrollableRoot.ChangeView(0f, 0f, 1f);
            }
        }

        public void ChangeThemeColor(Color color)
        {
            var color1 = Color.FromArgb(Convert.ToByte(color.A * 0.9), color.R, color.G, color.B);
            var color2 = Color.FromArgb(Convert.ToByte(color.A * 0.6), color.R, color.G, color.B);
            var color3 = Color.FromArgb(Convert.ToByte(color.A * 0.8), color.R, color.G, color.B);
            (Resources["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            (Resources["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            (Resources["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            (Resources["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            (Resources["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            (Resources["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            (Resources["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
        }

        private void AQIDetailButton_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (O3Grid.Visibility == Visibility.Collapsed)
            {
                O3Grid.Visibility = Visibility.Visible;
                NO2Grid.Visibility = Visibility.Visible;
                COGrid.Visibility = Visibility.Visible;
            }
            else
            {
                O3Grid.Visibility = Visibility.Collapsed;
                NO2Grid.Visibility = Visibility.Collapsed;
                COGrid.Visibility = Visibility.Collapsed;
            }
            AQIDetailButtonIn.Begin();
        }

        private void AQIDetailButton_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AQIDetailButtonIn.Begin();
        }

        private void AQIDetailButton_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AQIDetailButtonNormal.Begin();
        }

        private void AQIDetailButton_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            AQIDetailButtonPress.Begin();
        }
    }
}
