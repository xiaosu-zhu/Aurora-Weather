using com.aurora.auweather.Models;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using System.Windows;
using Windows.UI.Xaml;
using System;
using com.aurora.auweather.ViewModels;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;

namespace com.aurora.auweather
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var view = ApplicationView.GetForCurrentView();
            view.TitleBar.ForegroundColor = Colors.White;
            view.TitleBar.InactiveForegroundColor = Colors.White;

            // button
            view.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
            view.TitleBar.ButtonForegroundColor = Colors.White;
            view.TitleBar.ButtonHoverForegroundColor = Colors.White;
            view.TitleBar.ButtonPressedForegroundColor = Colors.White;
            view.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
        }

        Random rnd = new Random();
        private Vector2 RndPosition()
        {
            double x = rnd.NextDouble() * 500f;
            double y = rnd.NextDouble() * 500f;
            return new Vector2((float)x, (float)y);
        }

        private float RndRadius()
        {
            return (float)rnd.NextDouble() * 150f;
        }

        private byte RndByte()
        {
            return (byte)rnd.Next(256);
        }

        private void TitleBar_Loaded(object sender, RoutedEventArgs e)
        {
            //SettingsModel origin = new SettingsModel();
            //origin.AllowLocation = false;
            //origin.RefreshState = RefreshState.none;
            //origin.SavedCities = new CitySettingsModel[] {new CitySettingsModel(new Models.HeWeather.CityInfo
            //{
            //    City = "北京", Id = "CN101010100"
            //}),new CitySettingsModel(new Models.HeWeather.CityInfo
            //{
            //    City = "郑州", Id = "CN101180101"
            //}) };
            //origin.SaveSettings();
            //var actual = SettingsModel.ReadSettings();
        }

        private void MainCanvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        {
            float radius = (float)(1 + Math.Sin(args.Timing.TotalTime.TotalSeconds)) * 10f;
            blur.BlurAmount = radius;
            args.DrawingSession.DrawImage(blur);
        }
        GaussianBlurEffect blur;
        private void MainCanvas_CreateResources(
            Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender,
            Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            CanvasCommandList cl = new CanvasCommandList(sender);
            using (CanvasDrawingSession clds = cl.CreateDrawingSession())
            {
                for (int i = 0; i < 100; i++)
                {
                    clds.DrawText("Hello, World!", RndPosition(), Color.FromArgb(255, RndByte(), RndByte(), RndByte()));
                    clds.DrawCircle(RndPosition(), RndRadius(), Color.FromArgb(255, RndByte(), RndByte(), RndByte()));
                    clds.DrawLine(RndPosition(), RndPosition(), Color.FromArgb(255, RndByte(), RndByte(), RndByte()));
                }
            }

            blur = new GaussianBlurEffect()
            {
                Source = cl,
                BlurAmount = 10.0f
            };
        }

        private void RelativePanel_LayoutUpdated(object sender, object e)
        {
            var width = Root.ActualWidth;
            BezierControl1.Point2 = new Windows.Foundation.Point(width / 8, BezierControl1.Point2.Y);
            BezierControl1.Point3 = new Windows.Foundation.Point(width * 2 / 8, BezierControl1.Point3.Y);
            BezierControl2.Point1 = new Windows.Foundation.Point(width * 2 / 8, BezierControl2.Point1.Y);
            BezierControl2.Point2 = new Windows.Foundation.Point(width * 3 / 8, BezierControl2.Point2.Y);
            BezierControl2.Point3 = new Windows.Foundation.Point(width * 4 / 8, BezierControl2.Point3.Y);
            BezierControl3.Point1 = new Windows.Foundation.Point(width * 4 / 8, BezierControl3.Point1.Y);
            BezierControl3.Point2 = new Windows.Foundation.Point(width * 5 / 8, BezierControl3.Point2.Y);
            BezierControl3.Point3 = new Windows.Foundation.Point(width * 6 / 8, BezierControl3.Point3.Y);
            BezierControl4.Point1 = new Windows.Foundation.Point(width * 6 / 8, BezierControl3.Point1.Y);
            BezierControl4.Point2 = new Windows.Foundation.Point(width * 7 / 8, BezierControl3.Point2.Y);
            BezierControl4.Point3 = new Windows.Foundation.Point(width, BezierControl3.Point3.Y);
            endPoint1.Point = new Windows.Foundation.Point(width, endPoint1.Point.Y);
        }

        //private async void Grid_Loaded(object sender, RoutedEventArgs e)
        //{
        //    #region
        //    //string url = "http://apis.baidu.com/heweather/pro/weather";
        //    //string[] param = { "city=zhengzhou" };
        //    //var weatherresult = HeWeatherContract.TrimResult(actual);
        //    //var wa = JsonHelper.FromJson<HeWeatherContract>(weatherresult);
        //    //HeWeatherModel m = new HeWeatherModel(wa);
        //    //string[] param = { "search=allworld" };
        //    //await FileIOHelper.SaveFile(actual, "cityid.txt");
        //    #endregion
        //    //ThreadPool.RunAsync(async work =>
        //    //{
        //    //    var str = await FileIOHelper.ReadStringFromAssets("cityid.txt");
        //    //    var result = JsonHelper.FromJson<CityIdContract>(str);
        //    //    var citys = Models.HeWeather.CityInfo.CreateList(result);
        //    //    allcity = citys;
        //    //    this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
        //    //    {
        //    //        SuggestionBox.TextChanged += SuggestionBox_TextChanged;
        //    //    });
        //    //});
        //}

        //private async void SuggestionBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        //{
        //    // Only get results when it was a user typing, 
        //    // otherwise assume the value got filled in by TextMemberPath 
        //    // or the handler for SuggestionChosen.
        //    if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        //    {
        //        var text = sender.Text;
        //        await ThreadPool.RunAsync(async work =>
        //         {
        //             var searcharray = text.Split(' ');
        //             StringBuilder searchsb = new StringBuilder(@".*");
        //             foreach (var search in searcharray)
        //             {
        //                 searchsb.Append(search);
        //                 searchsb.Append(@".*");
        //             }
        //             var pattern = searchsb.ToString();
        //             var dataset = allcity.FindAll(x =>
        //              {
        //                  if (x.Country == "中国")
        //                  {
        //                      var pin = PinYinHelper.GetPinyin(x.City);
        //                      return (Regex.IsMatch(pin, pattern, RegexOptions.IgnoreCase) || Regex.IsMatch(x.City, pattern, RegexOptions.IgnoreCase));
        //                  }
        //                  return Regex.IsMatch(x.City, pattern, RegexOptions.IgnoreCase);
        //              });
        //             //Set the ItemsSource to be your filtered dataset
        //             await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
        //               {
        //                   sender.ItemsSource = dataset;
        //               });
        //         });
        //    }
        //}

        //private void SuggestionBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        //{
        //    if (args.ChosenSuggestion != null)
        //    {
        //        // User selected an item from the suggestion list, take an action on it here.
        //    }
        //    else
        //    {
        //        // Use args.QueryText to determine what to do.
        //    }
        //}

        //private void SuggestionBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        //{
        //    // Set sender.Text. You can use args.SelectedItem to build your text string.
        //}
    }
}
