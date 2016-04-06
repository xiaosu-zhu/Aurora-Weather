// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Effects;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.Shared.Helpers;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System.Numerics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.Foundation;
using System;
using Windows.System.Threading;

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
        BackBlur backBlur;

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



        public bool EnableBGBlur
        {
            get { return (bool)GetValue(EnableBGBlurProperty); }
            set { SetValue(EnableBGBlurProperty, value); }
        }

        // Using a DependencyProperty as the backing store for EnableBGBlur.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty EnableBGBlurProperty =
            DependencyProperty.Register("EnableBGBlur", typeof(bool), typeof(WeatherCanvas), new PropertyMetadata(false, new PropertyChangedCallback(OnEnableBGBlurChanged)));

        private static void OnEnableBGBlurChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var weatherCanvas = d as WeatherCanvas;
            if ((bool)e.NewValue)
            {
                weatherCanvas.backBlur.BlurIn();
                weatherCanvas.stopUpdate();
            }
            else
            {
                weatherCanvas.backBlur.BlurOut();
                weatherCanvas.continueUpdate();
            }
        }

        private void continueUpdate()
        {
            if (stopUpdateTimer != null)
            {
                stopUpdateTimer.Cancel();
            }
            Canvas.Paused = false;
        }

        private void stopUpdate()
        {
            if (stopUpdateTimer != null)
            {
                stopUpdateTimer.Cancel();
            }
            if (!EnableDynamic)
                stopUpdateTimer = ThreadPoolTimer.CreateTimer((x) =>
                {
                    Canvas.Paused = true;
                }, TimeSpan.FromMilliseconds(2100));
        }

        private static void OnEnableDynamicChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var weatherCanvas = d as WeatherCanvas;
            if ((bool)e.NewValue)
            {
                if (weatherCanvas.stopUpdateTimer != null)
                {
                    weatherCanvas.stopUpdateTimer.Cancel();
                }
                weatherCanvas.Canvas.Paused = false;
                weatherCanvas.sun = new SolarSystem();
                weatherCanvas.star = new StarParticleSystem();
                weatherCanvas.smoke = new SmokeParticleSystem();
                weatherCanvas.rain = new RainParticleSystem();
                weatherCanvas.thunderGen = new ThunderGenerator();
                weatherCanvas.ChangeCondition(weatherCanvas.condition, weatherCanvas.isNight, weatherCanvas.isSummer);
            }
            else
            {
                weatherCanvas.Canvas.Paused = false;
                weatherCanvas.sun.Dispose();
                weatherCanvas.star.Dispose();
                weatherCanvas.smoke.smokeDispose();
                weatherCanvas.rain.Dispose();
                if (weatherCanvas.stopUpdateTimer != null)
                {
                    weatherCanvas.stopUpdateTimer.Cancel();
                }
                if (weatherCanvas.EnableBGBlur)
                    weatherCanvas.stopUpdateTimer = ThreadPoolTimer.CreateTimer((x) =>
                    {
                        weatherCanvas.Canvas.Paused = true;
                    }, TimeSpan.FromMilliseconds(128));
            }
        }

        private WeatherCondition condition = WeatherCondition.unknown;
        private ThreadPoolTimer stopUpdateTimer;
        private float timeToCreateRain = Tools.RandomBetween(1, 5);

        public WeatherCanvas()
        {
            InitializeComponent();
            timeToCreate = Tools.RandomBetween(0, 2);
            sun = new SolarSystem();
            star = new StarParticleSystem();
            smoke = new SmokeParticleSystem();
            rain = new RainParticleSystem();
            backBlur = new BackBlur();
            thunderGen = new ThunderGenerator();
        }

        private void WeatherCanvas_Unloaded(object sender, RoutedEventArgs e)
        {
            Canvas.RemoveFromVisualTree();
            Canvas = null;
        }

        private void Canvas_Update(ICanvasAnimatedControl sender, CanvasAnimatedUpdateEventArgs args)
        {
            if (Canvas == null)
                return;
            backBlur.update(Canvas.Size.ToVector2());
            var elapsedTime = (float)args.Timing.ElapsedTime.TotalSeconds;
            if (isRain || isThunder)
            {
                if (rainLevel != RainLevel.shower)
                {
                    CreateRain();
                }
                else
                {
                    timeToCreateRain -= elapsedTime;
                    if (timeToCreateRain < 0)
                    {
                        if (timeToCreateRain > -Tools.RandomBetween(7, 15))
                            CreateRain();
                        else
                        {
                            timeToCreateRain = Tools.RandomBetween(2, 15);
                        }
                    }
                }
            }


            if ((!isRain) && isNight)
            {
                var k = Canvas.Size.Height * Canvas.Size.Width;
                if (star.ActiveParticles.Count < 1e-4 * k)
                {
                    CreateStar();
                }
            }

            if (isThunder || isHaze || isFog || isCloudy)
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
            rain.Update(elapsedTime, Canvas.Size.ToVector2());
            star.Update(elapsedTime);
            smoke.Update(elapsedTime);
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
                try
                {
                    backBlur.Draw(ds);
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
                catch (Exception)
                {

                }
            }
        }

        public void ChangeCondition(WeatherCondition condition, bool isnight, bool issummer)
        {
            ResetCondition();
            this.condition = condition;
            if (Canvas == null)
                return;
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
                    SetRain(0);
                    break;
                case WeatherCondition.moderate_rain:
                    SetRain(1);
                    break;
                case WeatherCondition.heavy_rain:
                case WeatherCondition.extreme_rain:
                    SetRain(2);
                    break;
                case WeatherCondition.drizzle_rain:
                    SetRain(0);
                    break;
                case WeatherCondition.storm_rain:
                case WeatherCondition.heavy_storm_rain:
                case WeatherCondition.severe_storm_rain:
                    SetRain(3);
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
            if (EnableDynamic)
            {
                var task = Canvas.RunOnGameLoopThreadAsync(async () =>
                {
                    await smoke.LoadSurfaceAsync(Canvas);
                });
            }
            isHaze = true;
            SetHazeBG();
        }

        private void SetFog()
        {
            if (EnableDynamic)
            {
                var task = Canvas.RunOnGameLoopThreadAsync(async () =>
                {
                    await smoke.LoadSurfaceAsync(Canvas);
                });
            }
            isFog = true;
            SetFogBG();
        }

        private void SetSnow(int v)
        {
            isRain = true;
            if (EnableDynamic)
            {
                var task = Canvas.RunOnGameLoopThreadAsync(async () =>
                {
                    await rain.LoadSurfaceAsync(Canvas);
                });
            }
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
            rain.ChangeConstants(rainLevel);
        }

        private void SetRain(int v)
        {
            isRain = true;
            if (EnableDynamic)
            {
                var task = Canvas.RunOnGameLoopThreadAsync(async () =>
                {
                    await rain.LoadSurfaceAsync(Canvas);
                });
            }
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
                case 3:
                    rainLevel = RainLevel.extreme;
                    break;
            }
            SetCloudyBG();
            rain.ChangeConstants(rainLevel);
        }

        private void SetThunderShower()
        {
            if (EnableDynamic)
            {
                var task = Canvas.RunOnGameLoopThreadAsync(async () =>
                {
                    await rain.LoadSurfaceAsync(Canvas);
                });
            }
            isRain = true;
            isThunder = true;
            rainLevel = RainLevel.shower;
            SetCloudyBG();
            rain.ChangeConstants(rainLevel);
        }

        private void SetShower()
        {
            if (EnableDynamic)
            {
                var task = Canvas.RunOnGameLoopThreadAsync(async () =>
                {
                    await rain.LoadSurfaceAsync(Canvas);
                });
            }
            isRain = true;
            rainLevel = RainLevel.shower;
            SetCloudyBG();
            rain.ChangeConstants(rainLevel);
        }

        private void SetCloudy(int v)
        {
            if (EnableDynamic)
            {
                var task = Canvas.RunOnGameLoopThreadAsync(async () =>
                                {
                                    await smoke.LoadSurfaceAsync(Canvas);
                                });
            }
            isCloudy = true;
            SetCloudyBG();
        }

        private void SetSunny()
        {
            isSunny = !isNight;
            SetSunnyBG();
            if (EnableDynamic)
            {
                if (isSunny)
                {
                    var task = Canvas.RunOnGameLoopThreadAsync(async () =>
                     {
                         await sun.LoadSurfaceAsync(Canvas);
                     });
                }
                else
                {
                    //await backBlur.LoadSurfaceAsync(Canvas, await FileIOHelper.ReadRandomAccessStreamFromAssetsAsync("Background/Todd Quackenbush.png"));
                    //backBlur.ImmersiveIn();
                    var task1 = Canvas.RunOnGameLoopThreadAsync(async () =>
                     {
                         await star.LoadSurfaceAsync(Canvas);
                     });
                }
            }
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

        internal void ImmersiveIn(Uri uri)
        {
            if (uri == null)
            {
                backBlur.ImmersiveOut();
                return;
            }
            var task = Canvas.RunOnGameLoopThreadAsync(async () =>
                        {
                            using (var stream = await FileIOHelper.ReadRandomAccessStreamByUriAsync(uri))
                            {
                                await backBlur.LoadSurfaceAsync(Canvas, stream);
                            }
                            backBlur.ImmersiveIn();
                        });
        }

        internal void ImmersiveOut()
        {
            var task = Canvas.RunOnGameLoopThreadAsync(() =>
            {
                backBlur.ImmersiveOut();
            });
        }
    }
}
