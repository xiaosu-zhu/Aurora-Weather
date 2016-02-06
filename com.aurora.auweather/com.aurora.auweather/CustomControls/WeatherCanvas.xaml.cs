using Com.Aurora.AuWeather.Effects;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.Shared.Helpers;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Numerics;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Media.Imaging;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Com.Aurora.AuWeather.CustomControls
{
    public sealed partial class WeatherCanvas : UserControl
    {
        RainParticleSystem rain = new RainParticleSystem(RainLevel.light);
        StarParticleSystem star = new StarParticleSystem();
        SmokeParticleSystem smoke = new SmokeParticleSystem();
        ThunderGenerator thunderGen = new ThunderGenerator();
        private bool useSpriteBatch = false;

        private bool isCloudy;
        private bool isRain;
        private RainLevel rainLevel;
        private bool isThunder;
        private bool isFog;
        private bool isHaze;
        private float timeToCreate = 0;

        private bool isSummer = false;

        private bool isNight = false;


        private WeatherCondition condition = WeatherCondition.unknown;

        public WeatherCanvas()
        {
            this.InitializeComponent();
        }

        private void WeatherCanvas_Unloaded(object sender, RoutedEventArgs e)
        {
            Canvas.RemoveFromVisualTree();
            Canvas = null;
        }

        private void Canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            var elapsedTime = (float)args.Timing.ElapsedTime.TotalSeconds;

            if (isRain || isThunder)
                CreateRain();

            if ((!isRain) && isNight)
            {
                var k = Canvas.Size.Height * Canvas.Size.Width;
                if (star.ActiveParticles.Count < 1e-4 * k)
                {
                    CreateStar();
                }
            }

            if (isCloudy || isRain || isThunder || isHaze || isFog)
            {
                timeToCreate -= elapsedTime;
                if (timeToCreate < 0)
                {
                    timeToCreate = Tools.RandomBetween(5, 7);
                    CreateSmoke();
                }
            }

            star.Update(elapsedTime);
            smoke.Update(elapsedTime);
            rain.Update(elapsedTime, Canvas.Size.ToVector2());
            thunderGen.Update(elapsedTime, Canvas.Size.ToVector2());
        }

        private void CreateSmoke()
        {
            Vector2 size = Canvas.Size.ToVector2();
            // 在左侧生成
            var where = new Vector2(size.X * 0.5f, size.Y);
            smoke.AddParticles(where);
        }

        private void CreateRain()
        {
            Vector2 size = Canvas.Size.ToVector2();
            rain.AddRainDrop(size);
        }

        private void CreateStar()
        {
            Vector2 size = Canvas.Size.ToVector2();
            var where = new Vector2(Tools.RandomBetween(0, size.X), Tools.RandomBetween(0, size.Y * 0.65f));
            star.AddParticles(where);
        }

        private void Canvas_Draw(ICanvasAnimatedControl sender, CanvasAnimatedDrawEventArgs args)
        {
            var ds = args.DrawingSession;

            if ((!isRain) && isNight)
                star.Draw(ds, useSpriteBatch);
            if (isThunder)
                thunderGen.Draw(sender, args.DrawingSession);
            if (isCloudy || isRain || isThunder || isHaze || isFog)
                smoke.Draw(ds, useSpriteBatch);
            if (isRain)
                rain.Draw(ds, useSpriteBatch);
        }

        private void Canvas_CreateResources(CanvasAnimatedControl sender, Microsoft.Graphics.Canvas.UI.CanvasCreateResourcesEventArgs args)
        {
            useSpriteBatch = CanvasSpriteBatch.IsSupported(sender.Device);
            args.TrackAsyncAction(CreateResourcesAsync(sender).AsAsyncAction());
        }

        async Task CreateResourcesAsync(CanvasAnimatedControl sender)
        {
            await rain.CreateResourcesAsync(sender);
            await star.CreateResourcesAsync(sender);
            await smoke.CreateResourcesAsync(sender);
        }

        public void ChangeCondition(WeatherCondition condition, bool isnight, bool issummer)
        {
            ResetCondition();
            this.condition = condition;
            isNight = isnight;
            isSummer = issummer;
            switch (condition)
            {
                case WeatherCondition.unknown:
                    return;
                case WeatherCondition.sunny:
                    SetSunny();
                    break;
                case WeatherCondition.cloudy:
                case WeatherCondition.few_clouds:
                case WeatherCondition.partly_cloudy:
                    SetCloudy(0);
                    break;
                case WeatherCondition.overcast:
                    SetCloudy(1);
                    break;
                case WeatherCondition.windy:
                case WeatherCondition.calm:
                case WeatherCondition.light_breeze:
                case WeatherCondition.moderate:
                case WeatherCondition.fresh_breeze:
                case WeatherCondition.strong_breeze:
                case WeatherCondition.high_wind:
                case WeatherCondition.gale:
                    SetSunny();
                    break;
                case WeatherCondition.strong_gale:
                case WeatherCondition.storm:
                case WeatherCondition.violent_storm:
                case WeatherCondition.hurricane:
                case WeatherCondition.tornado:
                case WeatherCondition.tropical_storm:
                    SetWind();
                    break;
                case WeatherCondition.shower_rain:
                case WeatherCondition.heavy_shower_rain:
                    SetShower();
                    break;
                case WeatherCondition.thundershower:
                case WeatherCondition.heavy_thunderstorm:
                case WeatherCondition.hail:
                    SetThunderShower();
                    break;
                case WeatherCondition.light_rain:
                case WeatherCondition.moderate_rain:
                    SetRain(0);
                    break;
                case WeatherCondition.heavy_rain:
                case WeatherCondition.extreme_rain:
                    SetRain(1);
                    break;
                case WeatherCondition.drizzle_rain:
                    SetRain(0);
                    break;
                case WeatherCondition.storm_rain:
                case WeatherCondition.heavy_storm_rain:
                case WeatherCondition.severe_storm_rain:
                    SetRain(2);
                    break;
                case WeatherCondition.freezing_rain:
                    SetRain(0);
                    break;
                case WeatherCondition.light_snow:
                case WeatherCondition.moderate_snow:
                    SetSnow(0);
                    break;
                case WeatherCondition.heavy_snow:
                case WeatherCondition.snowstorm:
                    SetSnow(1);
                    break;
                case WeatherCondition.sleet:
                case WeatherCondition.rain_snow:
                case WeatherCondition.shower_snow:
                case WeatherCondition.snow_flurry:
                    SetSnow(0);
                    break;
                case WeatherCondition.mist:
                case WeatherCondition.foggy:
                    SetFog();
                    break;
                case WeatherCondition.haze:
                case WeatherCondition.sand:
                case WeatherCondition.dust:
                case WeatherCondition.volcanic_ash:
                case WeatherCondition.duststorm:
                case WeatherCondition.sandstorm:
                    SetHaze();
                    break;
                case WeatherCondition.hot:
                    SetHot();
                    break;
                case WeatherCondition.cold:
                    SetCold();
                    break;
                default:
                    break;
            }
            rain.ChangeConstants(rainLevel);
        }

        private void SetWind()
        {
            SetRain(2);
        }

        private void ResetCondition()
        {
            isFog = false;
            isHaze = false;
            isRain = false;
            isThunder = false;
            isCloudy = false;
        }

        private void SetCold()
        {
            isSummer = false;
        }

        private void SetHot()
        {
            isSummer = true;
        }

        private void SetHaze()
        {
            isHaze = true;
        }

        private void SetFog()
        {
            isFog = true;
        }

        private void SetSnow(int v)
        {
            isRain = true;
            BG.Source = new Uri("ms-appx:///Assets/rain_cloud.mp4");
            switch (v)
            {
                default:
                    rainLevel = RainLevel.sSnow;
                    break;
                case 0:
                    rainLevel = RainLevel.sSnow;
                    break;
                case 1:
                    rainLevel = RainLevel.lSnow;
                    break;
            }
        }

        private void SetRain(int v)
        {
            isRain = true;
            BG.Source = new Uri("ms-appx:///Assets/rain_cloud.mp4");
            switch (v)
            {
                default:
                    rainLevel = RainLevel.light;
                    break;
                case 0:
                    rainLevel = RainLevel.light;
                    break;
                case 1:
                    rainLevel = RainLevel.moderate;
                    break;
                case 2:
                    rainLevel = RainLevel.heavy;
                    break;
            }

        }

        private void SetThunderShower()
        {
            isRain = true;
            isThunder = true;
            rainLevel = RainLevel.moderate;
        }

        private void SetShower()
        {
            isRain = true;
            rainLevel = RainLevel.moderate;
        }

        private void SetCloudy(int v)
        {
            BG.Source = new Uri("ms-appx:///Assets/rain_cloud.mp4");
            isCloudy = true;
        }

        private void SetSunny()
        {
            if (!isNight)
                SunAnimation.Begin();
        }
    }
}
