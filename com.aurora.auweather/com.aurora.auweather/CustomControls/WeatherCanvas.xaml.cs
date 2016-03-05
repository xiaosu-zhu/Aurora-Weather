using Com.Aurora.AuWeather.Effects;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.Shared.Helpers;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Com.Aurora.AuWeather.CustomControls
{
    public sealed partial class WeatherCanvas : UserControl
    {
        RainParticleSystem rain;
        StarParticleSystem star;
        SmokeParticleSystem smoke;
        ThunderGenerator thunderGen;
        SolarSystem sun;
        BGBlur bgBlur;

        private bool useSpriteBatch = true;

        private bool isCloudy;
        private bool isRain;
        private RainLevel rainLevel;
        private bool isThunder;
        private bool isFog;
        private bool isHaze;
        private bool isSunny;
        private float timeToCreate = 0;

        private bool isSummer = false;

        private bool isNight = false;


        public bool EnableDynamic
        {
            get { return (bool)GetValue(EnableDynamicProperty); }
            set { SetValue(EnableDynamicProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableDynamic.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableDynamicProperty =
            DependencyProperty.Register("EnableDynamic", typeof(bool), typeof(WeatherCanvas), new PropertyMetadata(true, new PropertyChangedCallback(OnEnableDynamicChanged)));

        private static void OnEnableDynamicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var weatherCanvas = d as WeatherCanvas;
            if ((bool)e.NewValue)
            {
                weatherCanvas.sun = new SolarSystem();
                weatherCanvas.star = new StarParticleSystem();
                weatherCanvas.smoke = new SmokeParticleSystem();
                weatherCanvas.rain = new RainParticleSystem();
                weatherCanvas.bgBlur = new BGBlur();
                weatherCanvas.thunderGen = new ThunderGenerator();
                weatherCanvas.Canvas.Draw += weatherCanvas.Canvas_Draw;
                weatherCanvas.Canvas.Update += weatherCanvas.Canvas_Update;
            }
            else
            {
                weatherCanvas.Canvas.Draw -= weatherCanvas.Canvas_Draw;
                weatherCanvas.Canvas.Update -= weatherCanvas.Canvas_Update;
                weatherCanvas.sun.Dispose();
                weatherCanvas.star.Dispose();
                weatherCanvas.smoke.smokeDispose();
                weatherCanvas.rain.Dispose();
                weatherCanvas.bgBlur.Dispose();
                weatherCanvas.sun = null;
                weatherCanvas.star = null;
                weatherCanvas.smoke = null;
                weatherCanvas.rain = null;
                weatherCanvas.bgBlur = null;
                weatherCanvas.thunderGen = null;
            }
        }

        private WeatherCondition condition = WeatherCondition.unknown;

        public WeatherCanvas()
        {
            InitializeComponent();
            timeToCreate = Tools.RandomBetween(0, 2);
            sun = new SolarSystem();
            star = new StarParticleSystem();
            smoke = new SmokeParticleSystem();
            rain = new RainParticleSystem();
            bgBlur = new BGBlur();
            thunderGen = new ThunderGenerator();
        }

        private void WeatherCanvas_Unloaded(object sender, RoutedEventArgs e)
        {
            Canvas.RemoveFromVisualTree();
            Canvas = null;
        }

        private void Canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            bgBlur.update(Canvas.Size.ToVector2());
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

            if (isThunder || isHaze || isFog)
            {
                timeToCreate -= elapsedTime;
                if (timeToCreate < 0)
                {
                    CreateSmoke();
                    timeToCreate = Tools.RandomBetween(5, 7);
                    if (isThunder)
                    {
                        thunderGen.Generate(Canvas.Size.ToVector2());
                    }
                }
            }
            star.Update(elapsedTime);
            smoke.Update(elapsedTime);
            rain.Update(elapsedTime, Canvas.Size.ToVector2());
            thunderGen.Update(elapsedTime, Canvas.Size.ToVector2());
            if (isSunny)
                sun.Update();
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
            using (var ds = args.DrawingSession)
            {
                bgBlur.Draw(ds);
                if ((!isRain) && isNight)
                    star.Draw(ds, useSpriteBatch);
                if (isThunder)
                    thunderGen.Draw(sender, args.DrawingSession);
                if (isCloudy || isRain || isThunder || isHaze || isFog)
                    smoke.Draw(ds, useSpriteBatch);
                if (isRain)
                    rain.Draw(ds, useSpriteBatch);
                if (isSunny)
                {
                    sun.Draw(ds, useSpriteBatch);
                }
            }
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
            BGAnimation.Begin();
        }

        private void SetWind()
        {
            SetRain(2);
            SetCloudyBG();
        }

        private void ResetCondition()
        {
            isFog = false;
            isHaze = false;
            isRain = false;
            isThunder = false;
            isCloudy = false;
            if (EnableDynamic)
            {
                sun.Dispose();
                star.Dispose();
                smoke.smokeDispose();
                rain.Dispose();
                bgBlur.Dispose();
            }
        }

        private void SetCold()
        {
            isSummer = false;
            SetSunnyBG();
        }

        private void SetHot()
        {
            isSummer = true;
            SetSunnyBG();
        }

        private void SetHaze()
        {
            isHaze = true;
            SetHazeBG();
        }

        private void SetFog()
        {
            isFog = true;
            SetFogBG();
        }

        private async void SetSnow(int v)
        {
            isRain = true;
            if (EnableDynamic)
                await rain.LoadSurfaceAsync(Canvas);
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
            SetCloudyBG();
        }

        private async void SetRain(int v)
        {
            isRain = true;
            if (EnableDynamic)
                await rain.LoadSurfaceAsync(Canvas);
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
            SetCloudyBG();
        }

        private async void SetThunderShower()
        {
            if (EnableDynamic)
                await rain.LoadSurfaceAsync(Canvas);
            isRain = true;
            isThunder = true;
            rainLevel = RainLevel.moderate;
            SetCloudyBG();
        }

        private async void SetShower()
        {
            if (EnableDynamic)
                await rain.LoadSurfaceAsync(Canvas);
            isRain = true;
            rainLevel = RainLevel.moderate;
            SetCloudyBG();
        }

        private async void SetCloudy(int v)
        {
            if (EnableDynamic)
                await smoke.LoadSurfaceAsync(Canvas);
            isCloudy = true;
            SetCloudyBG();
        }

        private async void SetSunny()
        {
            isSunny = !isNight;
            SetSunnyBG();
            if (EnableDynamic)
                await bgBlur.LoadSurfaceAsync(Canvas, await FileIOHelper.ReadRandomAccessStreamFromAssetsAsync("BG/thomas shellberg.jpg"));
        }

        #region set gradient background
        private void SetSunnyBG()
        {
            if (isSummer)
            {
                if (isNight)
                {
                    GradientAni0.To = Color.FromArgb(255, 0, 0, 0);
                    GradientAni1.To = Color.FromArgb(255, 0x24, 0x08, 0);
                    BGPointAni0.To = new Point(0.5, 0);
                    BGPointAni1.To = new Point(0.5, 1);
                }
                else
                {
                    GradientAni0.To = Color.FromArgb(255, 0xe2, 0xce, 0x60);
                    GradientAni1.To = Color.FromArgb(255, 0xd5, 0x60, 0x28);
                    BGPointAni0.To = new Point(0, 0);
                    BGPointAni1.To = new Point(1, 1);
                }
            }
            else
            {
                if (isNight)
                {
                    GradientAni0.To = Color.FromArgb(255, 0x37, 0x4e, 0x80);
                    GradientAni1.To = Color.FromArgb(255, 0x17, 0x2e, 0x44);
                    BGPointAni0.To = new Point(0.5, 0);
                    BGPointAni1.To = new Point(0.5, 1);
                }
                else
                {
                    GradientAni0.To = Color.FromArgb(255, 0x4b, 0x9a, 0xdc);
                    GradientAni1.To = Color.FromArgb(255, 0x20, 0x6d, 0xcb);
                    BGPointAni0.To = new Point(0, 0);
                    BGPointAni1.To = new Point(1, 1);
                }
            }
        }
        private void SetFogBG()
        {
            if (isNight)
            {
                if (isSummer)
                {
                    GradientAni0.To = Color.FromArgb(255, 0x0d, 0x00, 0x00);
                    GradientAni1.To = Color.FromArgb(255, 0x2a, 0x00, 0x00);
                    BGPointAni0.To = new Point(0.5, 0);
                    BGPointAni1.To = new Point(0.5, 1);
                }
                else
                {
                    GradientAni0.To = Color.FromArgb(255, 0x50, 0x50, 0x50);
                    GradientAni1.To = Color.FromArgb(255, 0x20, 0x20, 0x20);
                    BGPointAni0.To = new Point(0.5, 0);
                    BGPointAni1.To = new Point(0.5, 1);
                }
            }
            else
            {
                GradientAni0.To = Color.FromArgb(255, 0xa0, 0xa0, 0xa0);
                GradientAni1.To = Color.FromArgb(255, 0x5a, 0x50, 0x50);
                BGPointAni0.To = new Point(0.5, 0);
                BGPointAni1.To = new Point(0.5, 1);
            }
        }
        private void SetHazeBG()
        {
            GradientAni0.To = Color.FromArgb(255, 0x94, 0x8b, 0x62);
            GradientAni1.To = Color.FromArgb(255, 0x70, 0x5a, 0x41);
            BGPointAni0.To = new Point(0.5, 0);
            BGPointAni1.To = new Point(0.5, 1);
        }
        private void SetCloudyBG()
        {
            GradientAni0.To = Color.FromArgb(255, 0xa0, 0xa0, 0xa0);
            GradientAni1.To = Color.FromArgb(255, 0x80, 0x80, 0x80);
            BGPointAni0.To = new Point(0.5, 0);
            BGPointAni1.To = new Point(0.5, 1);
        }
        #endregion

        public void ImmersiveIn()
        {

        }
        public void ImmersiveOut()
        {

        }
    }
}
