// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Activation;
using Windows.UI.Core;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.Shared.Extensions;
using Windows.Devices.Geolocation;
using System;
using Com.Aurora.Shared.Helpers;
using Com.Aurora.AuWeather.Models.HeWeather.JsonContract;
using Com.Aurora.AuWeather.Models.HeWeather;
using System.Collections.Generic;
using System.Linq;
using Windows.System.Threading;
using Windows.UI.Popups;
using Windows.ApplicationModel.Resources;
using System.Threading.Tasks;
using Com.Aurora.AuWeather.License;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SplashScreenEx : Page
    {
        internal Rect splashImageRect; // Rect to store splash screen image coordinates.
        private SplashScreen splash; // Variable to hold the splash screen object.
        internal bool dismissed = false; // Variable to track splash screen dismissal status.
        internal Frame rootFrame;
        private string args;
        private ThreadPoolTimer timer;

        public SplashScreenEx(SplashScreen splashscreen, string args)
        {
            this.InitializeComponent();
            Window.Current.SizeChanged += new WindowSizeChangedEventHandler(Splash_OnResize);

            splash = splashscreen;
            if (splash != null)
            {
                splash.Dismissed += new TypedEventHandler<SplashScreen, object>(DismissedEventHandler);

                splashImageRect = splash.ImageLocation;
                PositionImage();
                PositionRing();
            }
            this.args = args;
            rootFrame = new Frame();
        }

        private void PositionRing()
        {
            SplashProgressRing.SetValue(Canvas.LeftProperty, splashImageRect.X + (splashImageRect.Width * 0.5) - (SplashProgressRing.Width * 0.5));
            SplashProgressRing.SetValue(Canvas.TopProperty, (splashImageRect.Y + splashImageRect.Height + splashImageRect.Height * 0.1));
        }

        private void PositionImage()
        {
            SplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
            SplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
            SplashImage.Height = splashImageRect.Height;
            SplashImage.Width = splashImageRect.Width;
        }

        private async void DismissedEventHandler(SplashScreen sender, object args)
        {
            dismissed = true;
            SetLongTimeTimer();
#if DEBUG
            await Task.Delay(4000);
#endif
            var settings = SettingsModel.Get();
            var license = new License.License();
            if (!license.IsPurchased)
            {
                settings.Preferences.EnableAlarm = false;
                settings.Preferences.EnableEveryDay = false;
                settings.Preferences.Set(RefreshState.one);
            }
            if (!settings.Inited)
            {
                NavigatetoStart();
                return;
            }
            if (this.args != "" && this.args != null)
            {
                if (this.args == settings.Cities.LocatedCity.Id)
                {
                    settings.Cities.CurrentIndex = -1;
                }
                else
                {
                    var index = Array.FindIndex(settings.Cities.SavedCities, x =>
                     {
                         return x.Id == this.args;
                     });
                    if (index >= 0)
                    {
                        settings.Cities.CurrentIndex = index;
                    }
                }
            }
            if (settings.Cities.SavedCities == null && settings.Cities.EnableLocate)
            {
                settings.Cities.CurrentIndex = -1;
            }
            else if (settings.Cities.SavedCities == null && !settings.Cities.EnableLocate)
            {
                NavigatetoStart();
            }
            if (settings.Cities.CurrentIndex == -1 && settings.Cities.EnableLocate)
            {
                var r = ThreadPool.RunAsync(async (work) =>
                {
                    var str = await FileIOHelper.ReadStringFromAssetsAsync("cityid.txt");
                    var result = JsonHelper.FromJson<CityIdContract>(str);
                    var citys = CityInfo.CreateList(result);
                    str = null;
                    result = null;
                    var p = Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(async () =>
                     {
                         var accessStatus = await Geolocator.RequestAccessAsync();
                         if (accessStatus == GeolocationAccessStatus.Allowed)
                         {
                             var _geolocator = new Geolocator();
                             var pos = await _geolocator.GetGeopositionAsync();
                             var t = ThreadPool.RunAsync(async (w) =>
                             {
                                 CalcPosition(pos, citys, settings);
                                 if ((DateTime.Now - settings.Cities.LocatedCity.LastUpdate).TotalMinutes >= 60)
                                 {
                                     var keys = Key.key.Split(new string[] { ":|:" }, StringSplitOptions.RemoveEmptyEntries);
                                     var param = new string[] { "cityid=" + settings.Cities.LocatedCity.Id };
                                     CreateTimeOutTimer();
                                     var resstr = await BaiduRequestHelper.RequestWithKeyAsync("http://apis.baidu.com/heweather/pro/weather", param, keys[0]);
                                     if (resstr == null || resstr == "")
                                     {
                                         await Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(async () =>
                                         {
                                             var loader = new ResourceLoader();
                                             var d = new MessageDialog(loader.GetString("Network_Error"));
                                             d.Title = loader.GetString("Error");
                                             d.Commands.Add(new UICommand(loader.GetString("Quit"), new UICommandInvokedHandler(QuitAll)));
                                             await d.ShowAsync();
                                         }));
                                     }

                                     var task = ThreadPool.RunAsync(async (m) =>
                                     {
                                         m.Completed = new AsyncActionCompletedHandler(SplashComplete);
                                         await settings.Cities.SaveDataAsync(settings.Cities.LocatedCity.Id, resstr);
                                         settings.Cities.LocatedCity.Update();
                                         settings.Cities.Save();
                                     });
                                 }
                                 else
                                 {
                                     SplashComplete(null, AsyncStatus.Completed);
                                 }
                             });
                         }
                     }));
                });
            }
            else if (settings.Cities.CurrentIndex >= 0 && !settings.Cities.SavedCities.IsNullorEmpty())
            {
                if ((DateTime.Now - settings.Cities.SavedCities[settings.Cities.CurrentIndex].LastUpdate).TotalMinutes >= 60)
                {
                    var keys = Key.key.Split(new string[] { ":|:" }, StringSplitOptions.RemoveEmptyEntries);
                    var param = new string[] { "cityid=" + settings.Cities.SavedCities[settings.Cities.CurrentIndex].Id };
                    CreateTimeOutTimer();
                    var resstr = await BaiduRequestHelper.RequestWithKeyAsync("http://apis.baidu.com/heweather/pro/weather", param, keys[0]);
                    if (resstr == null || resstr == "")
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(async () =>
                        {
                            var loader = new ResourceLoader();
                            var d = new MessageDialog(loader.GetString("Network_Error"));
                            d.Title = loader.GetString("Error");
                            d.Commands.Add(new UICommand(loader.GetString("Quit"), new UICommandInvokedHandler(QuitAll)));
                            await d.ShowAsync();
                        }));
                    }
                    var task = ThreadPool.RunAsync(async (work) =>
                    {
                        work.Completed = new AsyncActionCompletedHandler(SplashComplete);
                        await settings.Cities.SaveDataAsync(settings.Cities.SavedCities[settings.Cities.CurrentIndex].Id, resstr);
                        settings.Cities.SavedCities[settings.Cities.CurrentIndex].Update();
                        settings.Cities.Save();
                    });

                }
                else
                {
                    SplashComplete(null, AsyncStatus.Completed);
                }
            }
            else
            {
                NavigatetoStart();
                return;
            }
        }

        private void SetLongTimeTimer()
        {
            if (timer != null)
            {
                timer.Cancel();
            }
            timer = ThreadPoolTimer.CreateTimer(async (x) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
                {
                    var loader = new ResourceLoader();
                    SplashWelcome.Text = loader.GetString("LongTime");
                }));
            }, TimeSpan.FromMilliseconds(5000));
        }

        private void CreateTimeOutTimer()
        {
            if (timer != null)
            {
                timer.Cancel();
            }
            timer = ThreadPoolTimer.CreateTimer(async (x) =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(async () =>
                {
                    var loader = new ResourceLoader();
                    var d = new MessageDialog(loader.GetString("Network_Error"));
                    d.Title = loader.GetString("Error");
                    d.Commands.Add(new UICommand(loader.GetString("Quit"), new UICommandInvokedHandler(QuitAll)));
                    await d.ShowAsync();
                }));
            }, TimeSpan.FromMilliseconds(10000));
        }

        private void QuitAll(IUICommand command)
        {
            App.Current.Exit();
        }

        private async void NavigatetoStart()
        {
            if (timer != null)
            {
                timer.Cancel();
            }
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
            {
                rootFrame.Navigate(typeof(StartPage));
                Window.Current.Content = rootFrame;
            }));
        }

        private async void SplashComplete(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
        {
            if (timer != null)
            {
                timer.Cancel();
            }
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
             {
                 DismissExtendedSplash();
             }));
        }

        private void CalcPosition(Geoposition pos, List<CityInfo> citys, SettingsModel settings)
        {
            var final = Models.Location.GetNearsetLocation(citys,
                new Models.Location((float)pos.Coordinate.Point.Position.Latitude, (float)pos.Coordinate.Point.Position.Longitude));

            if (settings.Cities.LocatedCity != null && settings.Cities.LocatedCity.Id == final.ToArray()[0].Id)
            {
                final = null;
                citys.Clear();
                citys = null;
                return;
            }
            settings.Cities.LocatedCity = new Models.Settings.CitySettingsModel(final.ToArray()[0]);
            final = null;
            citys.Clear();
            citys = null;
        }

        void DismissExtendedSplash()
        {
            rootFrame.Navigate(typeof(MainPage));
            Window.Current.Content = rootFrame;
        }

        private void Splash_OnResize(object sender, WindowSizeChangedEventArgs e)
        {
            if (splash != null)
            {
                splashImageRect = splash.ImageLocation;
                PositionImage();
                PositionRing();
            }
        }
    }
}
