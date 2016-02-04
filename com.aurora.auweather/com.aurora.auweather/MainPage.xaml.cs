using Com.Aurora.AuWeather.Models;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using System.Windows;
using Windows.UI.Xaml;
using System;
using Com.Aurora.AuWeather.ViewModels;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Effects;
using System.Numerics;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Com.Aurora.AuWeather.Effects;
using Com.Aurora.Shared.Helpers;

namespace Com.Aurora.AuWeather
{
    public sealed partial class MainPage : Page
    {
        RainParticleSystem rain = new RainParticleSystem(RainLevel.light);
        //StarParticleSystem star = new StarParticleSystem();
        private bool useSpriteBatch = false;
        SmokeParticleSystem smoke = new SmokeParticleSystem();
        private float timeToCreate = 0;

        public MainPage()
        {
            this.InitializeComponent();

        }

        private void canvas_CreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            useSpriteBatch = CanvasSpriteBatch.IsSupported(sender.Device);
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }

        async Task CreateResourcesAsync(CanvasAnimatedControl sender)
        {
            await rain.CreateResourcesAsync(sender);
            //await star.CreateResourcesAsync(sender);
            await smoke.CreateResourcesAsync(sender);
        }

        private void canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            var elapsedTime = (float)args.Timing.ElapsedTime.TotalSeconds;
            timeToCreate -= elapsedTime;
            CreateRain();
            rain.Update(elapsedTime, canvas.Size.ToVector2());
            var k = canvas.Size.Height * canvas.Size.Width;
            //if (star.ActiveParticles.Count < 1e-4 * k)
            //{
            //    CreateStar();
            //}
            if (timeToCreate < 0)
            {
                timeToCreate = Tools.RandomBetween(10, 20);
                timeToCreate += elapsedTime;
                CreateSmoke();
            }
            //star.Update(elapsedTime);
            smoke.Update(elapsedTime);
        }

        private void CreateSmoke()
        {
            Vector2 size = canvas.Size.ToVector2();
            // 在左侧生成
            var where = new Vector2(size.X * 0.5f, size.Y);
            smoke.AddParticles(where);
        }

        private void CreateRain()
        {
            Vector2 size = canvas.Size.ToVector2();
            rain.AddRainDrop(size);
        }

        private void CreateStar()
        {
            Vector2 size = canvas.Size.ToVector2();
            var where = new Vector2(Tools.RandomBetween(0, size.X), Tools.RandomBetween(0, size.Y * 0.65f));
            //star.AddParticles(where);
        }

        private void canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var ds = args.DrawingSession;
            rain.Draw(ds, useSpriteBatch);
            //star.Draw(ds, useSpriteBatch);
            smoke.Draw(ds, useSpriteBatch);
            //GC.Collect();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            canvas.RemoveFromVisualTree();
            canvas = null;
        }

        //Random rnd = new Random();
        //private Vector2 RndPosition()
        //{
        //    double x = rnd.NextDouble() * 500f;
        //    double y = rnd.NextDouble() * 500f;
        //    return new Vector2((float)x, (float)y);
        //}

        //private float RndRadius()
        //{
        //    return (float)rnd.NextDouble() * 150f;
        //}

        //private byte RndByte()
        //{
        //    return (byte)rnd.Next(256);
        //}

        //private void TitleBar_Loaded(object sender, RoutedEventArgs e)
        //{
        //    SettingsModel origin = new SettingsModel();
        //    origin.AllowLocation = false;
        //    origin.RefreshState = RefreshState.none;
        //    origin.SavedCities = new CitySettingsModel[] {new CitySettingsModel(new Models.HeWeather.CityInfo
        //    {
        //        City = "北京", Id = "CN101010100"
        //    }),new CitySettingsModel(new Models.HeWeather.CityInfo
        //    {
        //        City = "郑州", Id = "CN101180101"
        //    }) };
        //    origin.SaveSettings();
        //    var actual = SettingsModel.ReadSettings();
        //}

        //private void MainCanvas_Draw(Microsoft.Graphics.Canvas.UI.Xaml.ICanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedDrawEventArgs args)
        //{
        //    float radius = (float)(1 + Math.Sin(args.Timing.TotalTime.TotalSeconds)) * 10f;
        //    blur.BlurAmount = radius;
        //    args.DrawingSession.DrawImage(blur);
        //}
        //GaussianBlurEffect blur;
        //private double width;

        //private void MainCanvas_CreateResources(
        //    Microsoft.Graphics.Canvas.UI.Xaml.CanvasAnimatedControl sender,
        //    Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        //{
        //    CanvasCommandList cl = new CanvasCommandList(sender);
        //    using (CanvasDrawingSession clds = cl.CreateDrawingSession())
        //    {
        //        for (int i = 0; i < 100; i++)
        //        {
        //            clds.DrawText("Hello, World!", RndPosition(), Color.FromArgb(255, RndByte(), RndByte(), RndByte()));
        //            clds.DrawCircle(RndPosition(), RndRadius(), Color.FromArgb(255, RndByte(), RndByte(), RndByte()));
        //            clds.DrawLine(RndPosition(), RndPosition(), Color.FromArgb(255, RndByte(), RndByte(), RndByte()));
        //        }
        //    }

        //    blur = new GaussianBlurEffect()
        //    {
        //        Source = cl,
        //        BlurAmount = 10.0f
        //    };
        //}



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
