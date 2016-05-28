// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Core.Models;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.AuWeather.Tile;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using NotificationsExtensions.Tiles;
using System;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Graphics.Display;
using Windows.System.UserProfile;
using Windows.UI.Xaml.Media.Imaging;

namespace Com.Aurora.AuWeather.Background
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            var settings = SettingsModel.Get();

            await FileIOHelper.AppendLogtoCacheAsync("Background Task Run Once");

            if (settings.Cities.CurrentIndex == -1 && settings.Cities.EnableLocate)
            {
                if (settings.Cities.LocatedCity == null && !settings.Cities.SavedCities.IsNullorEmpty())
                {
                    settings.Cities.CurrentIndex = 0;
                    var currentCity = settings.Cities.SavedCities[settings.Cities.CurrentIndex];
                    await Init(settings, currentCity);
                }
                else
                {
                    var currentCity = settings.Cities.LocatedCity;
                    await Init(settings, currentCity);
                }
            }
            else if (settings.Cities.CurrentIndex == 0)
            {
                var currentCity = settings.Cities.SavedCities[settings.Cities.CurrentIndex];
                await Init(settings, currentCity);
            }
            await FileIOHelper.AppendLogtoCacheAsync("Background Task Completed");
            deferral.Complete();
        }

        private async Task Init(SettingsModel settings, CitySettingsModel currentCity)
        {
            string resstr = await Request.GetRequest(settings, currentCity);
            if (!resstr.IsNullorEmpty())
            {
                var fetchresult = HeWeatherModel.Generate(resstr, settings.Preferences.DataSource);
                if (fetchresult == null || fetchresult.DailyForecast == null || fetchresult.HourlyForecast == null)
                {
                    return;
                }
                var utcOffset = fetchresult.Location.UpdateTime - fetchresult.Location.UtcTime;
                var current = DateTimeHelper.ReviseLoc(utcOffset);
                try
                {
                    Sender.CreateMainTileQueue(await Generator.CreateAll(fetchresult, current));
                }
                catch (Exception)
                {

                }

                var todayIndex = Array.FindIndex(fetchresult.DailyForecast, x =>
                {
                    return x.Date == current.Date;
                });
                TimeSpan sunRise, sunSet;
                if (fetchresult.DailyForecast[todayIndex].SunRise == default(TimeSpan) || fetchresult.DailyForecast[todayIndex].SunSet == default(TimeSpan))
                {
                    sunRise = Core.LunarCalendar.SunRiseSet.GetRise(new Models.Location(currentCity.Latitude, currentCity.Longitude), current);
                    sunSet = Core.LunarCalendar.SunRiseSet.GetSet(new Models.Location(currentCity.Latitude, currentCity.Longitude), current);
                }
                else
                {
                    sunRise = fetchresult.DailyForecast[todayIndex].SunRise;
                    sunSet = fetchresult.DailyForecast[todayIndex].SunSet;
                }
                try
                {
                    if (UserProfilePersonalizationSettings.IsSupported() && settings.Preferences.SetWallPaper)
                    {
                        var file = await FileIOHelper.GetFilebyUriAsync(await settings.Immersive.GetCurrentBackgroundAsync(fetchresult.NowWeather.Now.Condition, WeatherModel.CalculateIsNight(current, sunRise, sunSet)));
                        var lFile = await FileIOHelper.CreateWallPaperFileAsync(Guid.NewGuid().ToString() + ".png");
                        var d = PointerDevice.GetPointerDevices();
                        var m = d.ToArray();
                        var scaleFactor = DisplayInformation.GetForCurrentView().RawPixelsPerViewPixel;
                        var size = new Size(m[0].PhysicalDeviceRect.Width, m[0].PhysicalDeviceRect.Height);
                        var ratio = size.Height / size.Width;
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

                if (settings.Preferences.EnableMorning)
                {
                    var shu1 = settings.Preferences.MorningNoteTime.TotalHours;

                    var tomorrow8 = DateTime.Now.Hour > shu1 ? (DateTime.Today.AddDays(1)).AddHours(shu1) : (DateTime.Today.AddHours(shu1));

                    try
                    {
                        Sender.CreateScheduledToastNotification(await (Generator.CreateToast(fetchresult, currentCity, settings, DateTimeHelper.ReviseLoc(tomorrow8, utcOffset))), tomorrow8, "MorningToast");
                    }
                    catch (Exception)
                    {

                    }
                }
                if (settings.Preferences.EnableEvening)
                {
                    var shu2 = settings.Preferences.EveningNoteTime.TotalHours;
                    var tomorrow20 = DateTime.Now.Hour > shu2 ? (DateTime.Today.AddDays(1)).AddHours(shu2) : (DateTime.Today.AddHours(shu2));
                    try
                    {
                        Sender.CreateScheduledToastNotification(await (Generator.CreateToast(fetchresult, currentCity, settings, DateTimeHelper.ReviseLoc(tomorrow20, utcOffset))), tomorrow20, "EveningToast");
                    }
                    catch (Exception)
                    {

                    }
                }
                if (settings.Preferences.EnableAlarm)
                {
                    if (!fetchresult.Alarms.IsNullorEmpty() && (DateTime.Now - settings.Preferences.LastAlertTime).TotalDays > 1)
                    {
                        Sender.CreateBadge(Generator.GenerateAlertBadge());
                        Sender.CreateToast(Generator.CreateAlertToast(fetchresult, currentCity).GetXml());
                    }
                    var str = Generator.CalculateWeatherAlarm(fetchresult, currentCity, settings, DateTimeHelper.ReviseLoc(DateTime.Now, utcOffset));
                    if (!str.IsNullorEmpty() && str.Length > 1 && (DateTime.Now - settings.Preferences.LastAlarmTime).TotalDays > 1)
                    {
                        Sender.CreateToast(Generator.CreateAlarmToast(str, currentCity).GetXml());
                    }
                }
                await settings.Cities.SaveDataAsync(currentCity.Id, resstr, settings.Preferences.DataSource);
                currentCity.Update();
                if (settings.Cities.CurrentIndex != -1)
                {
                    settings.Cities.SavedCities[settings.Cities.CurrentIndex] = currentCity;
                }
                else
                {
                    settings.Cities.LocatedCity = currentCity;
                }
                settings.Cities.Save();
            }
        }

        public void InstantUpdateSecondary(TileContent content)
        {

        }
    }
}
