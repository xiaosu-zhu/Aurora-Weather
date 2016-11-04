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
using Com.Aurora.AuWeather.ViewModels;
using Com.Aurora.AuWeather.CustomControls;
using Windows.UI;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SplashScreenEx : Page, IThemeble
    {
        internal bool dismissed = false; // Variable to track splash screen dismissal status.
        internal Frame rootFrame;
        private string args;
        private ThreadPoolTimer timer;
        private Rect splashImageRect;

        public SplashScreenEx(SplashScreen splash, string args)
        {
            this.InitializeComponent();
            this.splash = splash;
            splash.Dismissed += DismissedEventHandler;
            this.args = args;
            splashImageRect = splash.ImageLocation;
            rootFrame = new Frame();
            rootFrame.Navigated += RootFrame_Navigated1;
        }

        private void RootFrame_Navigated1(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (rootFrame.Content is IThemeble)
            {
                if (App.MainColor.A > 0)
                    if (rootFrame.Content is IThemeble)
                    {
                        (rootFrame.Content as IThemeble).ChangeThemeColor(App.MainColor);
                    }
            }
            PositionImage();
        }

        private DispatcherTimer showWindowTimer;
        private SplashScreen splash;

        private void OnShowWindowTimer(object sender, object e)
        {
            showWindowTimer.Stop();

            // Activate/show the window, now that the splash image has rendered
            Window.Current.Activate();
        }

        private void extendedSplashImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            // ImageOpened means the file has been read, but the image hasn't been painted yet.
            // Start a short timer to give the image a chance to render, before showing the window
            // and starting the animation.
            showWindowTimer = new DispatcherTimer();
            showWindowTimer.Interval = TimeSpan.FromMilliseconds(50);
            showWindowTimer.Tick += OnShowWindowTimer;
            showWindowTimer.Start();
        }

        void PositionImage()
        {
            var deviceType = SystemInfoHelper.GetDeviceFormFactorType();
            // desktop
            if (deviceType == DeviceFormFactorType.Desktop)
            {
                extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
                extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
                extendedSplashImage.Height = splashImageRect.Height;
                extendedSplashImage.Width = splashImageRect.Width;
            }
            else if (deviceType == DeviceFormFactorType.Tablet)
            {
                extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
                extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
                extendedSplashImage.Height = splashImageRect.Height;
                extendedSplashImage.Width = splashImageRect.Width;
            }
            // mobile
            else if (deviceType == DeviceFormFactorType.Phone)
            {
                // 获取一个值，该值表示每个视图（布局）像素的原始（物理）像素数。
                double density = Windows.Graphics.Display.DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;

                extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X / density);
                extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y / density);
                extendedSplashImage.Height = splashImageRect.Height / density;
                extendedSplashImage.Width = splashImageRect.Width / density;
            }
            // xbox等没试过，编不出来
            else
            {
                extendedSplashImage.SetValue(Canvas.LeftProperty, splashImageRect.X);
                extendedSplashImage.SetValue(Canvas.TopProperty, splashImageRect.Y);
                extendedSplashImage.Height = splashImageRect.Height;
                extendedSplashImage.Width = splashImageRect.Width;
            }
        }

        private async void DismissedEventHandler(SplashScreen sender, object args)
        {
            dismissed = true;
            SetLongTimeTimer();
            var settings = SettingsModel.Get();
            var license = new License.License();
            if (!license.IsPurchased)
            {
                settings.Preferences.EnableAlarm = false;
                settings.Preferences.EnableEvening = false;
                settings.Preferences.EnableMorning = false;
                settings.Preferences.Set(240);
            }
            if (settings.Preferences.MainColor.A > 0)
            {
                App.MainColor = settings.Preferences.MainColor;
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
                {
                    App.ChangeThemeColor(settings.Preferences.MainColor);
                    ChangeThemeColor(settings.Preferences.MainColor);
                }));

            }
            if (!settings.Inited)
            {
                NavigatetoStart();
                return;
            }
            if (!this.args.IsNullorEmpty())
            {
                settings.Cities.ChangeCurrent(this.args);
            }
            try
            {
                if (settings.Cities.GetCurrentCity() == null)
                {
                    NavigatetoStart();
                    return;
                }
            }
            catch (Exception)
            {

            }

            if (settings.Cities.CurrentIndex == -1 && settings.Cities.EnableLocate)
            {
                try
                {
                    var accessStatus = await Geolocator.RequestAccessAsync();
                    if (accessStatus == GeolocationAccessStatus.Allowed)
                    {
                        await Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(async () =>
                        {
                            var l = new ResourceLoader();
                            SplashWelcome.Text = l.GetString("Locating");
                            try
                            {
                                var _geolocator = new Geolocator();
                                var pos = await _geolocator.GetGeopositionAsync();
                                if ((_geolocator.LocationStatus != PositionStatus.NoData) && (_geolocator.LocationStatus != PositionStatus.NotAvailable) && (_geolocator.LocationStatus != PositionStatus.Disabled))
                                {
                                    var t = ThreadPool.RunAsync(async (w) =>
                                    {
                                        var str = await FileIOHelper.ReadStringFromAssetsAsync("cityid.txt");
                                        var cityids = JsonHelper.FromJson<CityIdContract>(str);
                                        var citys = CityInfo.CreateList(cityids);
                                        str = null;
                                        cityids = null;
                                        await CalcPosition(pos, citys, settings);
                                        GetCityListandCompelte(settings);
                                    });
                                }
                                else
                                {
                                    GetCityListandCompelte(settings);
                                }
                            }
                            catch (Exception)
                            {
                                GetCityListandCompelte(settings);
                            }

                        }));
                    }
                    else
                    {
                        if (!settings.Cities.SavedCities.IsNullorEmpty())
                            settings.Cities.CurrentIndex = 0;
                        else
                        {
                            GetCityListandCompelte(settings);
                        }
                    }
                }
                catch (Exception)
                {
                    NavigatetoStart();
                }
            }
            else if (settings.Cities.CurrentIndex >= 0 && !settings.Cities.SavedCities.IsNullorEmpty())
            {
                List<CityInfo> citys = null;
                foreach (var citty in settings.Cities.SavedCities)
                {
                    if (citty.Latitude == 0 && citty.Longitude == 0)
                    {
                        if (citys == null)
                        {
                            var str = await FileIOHelper.ReadStringFromAssetsAsync("cityid.txt");
                            var result = JsonHelper.FromJson<CityIdContract>(str);
                            citys = CityInfo.CreateList(result);
                            str = null;
                            result = null;
                        }
                        var tar = citys.Find(x =>
                        {
                            return x.Id == citty.Id;
                        });
                        citty.Latitude = tar.Location.Latitude;
                        citty.Longitude = tar.Location.Longitude;
                    }
                }
                settings.Cities.Save();
                if (citys != null)
                {
                    citys.Clear();
                    citys = null;
                }
                GetCityListandCompelte(settings);
            }
            else
            {
                NavigatetoStart();
                return;
            }
        }

        private void GetCityListandCompelte(SettingsModel settings)
        {
            var cs = new List<string>();
            if (settings.Cities.LocatedCity != null && !settings.Cities.LocatedCity.City.IsNullorEmpty())
            {
                cs.Add(settings.Cities.LocatedCity.City);
            }
            if (!settings.Cities.SavedCities.IsNullorEmpty())
                cs.AddRange(settings.Cities.SavedCities.Select((a, b) =>
                {
                    return a.City;
                }));
            SplashComplete(cs);
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
                    CreateTimeOutTimer();
                }));
            }, TimeSpan.FromMilliseconds(8000));
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
            }, TimeSpan.FromMilliseconds(7000));
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

        private async void SplashComplete(IEnumerable<string> cs)
        {
            if (timer != null)
            {
                timer.Cancel();
            }
            var task = ThreadPool.RunAsync(async (w) =>
            {
                await CortanaHelper.EditPhraseListAsync("AuroraWeatherCommandSet_zh-cn", "where", cs.ToArray());
            });
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
            {
                DismissExtendedSplash();
            }));
        }

        private async Task CalcPosition(Geoposition pos, List<CityInfo> citys, SettingsModel settings)
        {
            var ocontract = await Models.Location.OpenMapReGeoAsync(pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);
            List<CityInfo> final = null;
            if (ocontract != null)
                final = citys.FindAll(x =>
                {
                    return x.City == ocontract.address.city;
                });
            if (final.IsNullorEmpty())
            {
                var acontract = await Models.Location.AmapReGeoAsync(pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);
                if (acontract != null)
                    final = citys.FindAll(x =>
                    {
                        return x.City == acontract.regeocode.addressComponent.district;
                    });
            }
            if (final.IsNullorEmpty())
            {
                var id = await Models.Location.ReGeobyIpAsync();
                if (id != null)
                    final = citys.FindAll(x =>
                    {
                        return x.Id == id.CityId;
                    });
            }
            if (final.IsNullorEmpty())
            {
                if (settings.Cities.LocatedCity == null)
                {
                    FailtoPos();
                    return;
                }
                return;
            }
            if (final.IsNullorEmpty())
            {
                var near = Models.Location.GetNearsetLocation(citys, new Models.Location((float)pos.Coordinate.Point.Position.Latitude, (float)pos.Coordinate.Point.Position.Longitude));
                final = near.ToList();
            }
            if (settings.Cities.LocatedCity != null && settings.Cities.LocatedCity.Id == final.ToArray()[0].Id)
            {
                if (settings.Cities.LocatedCity.Latitude == 0)
                {
                    settings.Cities.LocatedCity.Latitude = (float)pos.Coordinate.Point.Position.Latitude;
                    settings.Cities.LocatedCity.Longitude = (float)pos.Coordinate.Point.Position.Longitude;
                }
            }
            else
            {
                var p = final.ToArray()[0];
                p.Location = new Models.Location((float)pos.Coordinate.Point.Position.Latitude, (float)pos.Coordinate.Point.Position.Longitude);
                settings.Cities.LocatedCity = new Models.Settings.CitySettingsModel(p);
            }
            final = null;
            citys.Clear();
            citys = null;
            settings.Cities.Save();
        }

        private void FailtoPos()
        {

        }

        void DismissExtendedSplash()
        {
            if (Window.Current.Content == rootFrame)
            {
                return;
            }
            rootFrame.Navigated += RootFrame_Navigated;
            rootFrame.Navigate(typeof(MainPage));
            Window.Current.Content = rootFrame;
        }

        private void RootFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (rootFrame.Content is MainPage)
            {
                var s = SettingsModel.Get();
                var c = s.Cities.SavedCities;
                List<CitySettingsViewModel> l = new List<CitySettingsViewModel>();
                if (s.Cities.EnableLocate && s.Cities.LocatedCity != null)
                {
                    l.Add(new CitySettingsViewModel(s.Cities.LocatedCity.City, s.Cities.LocatedCity.Id, s.Cities.LocatedCity.IsCurrent));
                }
                foreach (var item in c)
                {
                    l.Add(new CitySettingsViewModel(item.City, item.Id, item.IsCurrent));
                }
                ((rootFrame.Content) as MainPage).SetCitiesPanel(l, (s.Cities.EnableLocate && s.Cities.LocatedCity != null) ? s.Cities.CurrentIndex + 1 : s.Cities.CurrentIndex);
            }
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (timer != null)
            {
                timer.Cancel();
                timer = null;
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

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (splash != null)
            {
                // Update the coordinates of the splash screen image.
                splashImageRect = splash.ImageLocation;
                PositionImage();
            }
        }
    }
}
