// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Core.Models;
using Com.Aurora.AuWeather.License;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.AuWeather.Models.HeWeather.JsonContract;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.AuWeather.Tile;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using NotificationsExtensions.Tiles;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Com.Aurora.AuWeather.Background
{
    public sealed class BackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            await FileIOHelper.AppendLogtoCacheAsync("Background Task Run Once");
            var settings = SettingsModel.Get();
            if (settings.Cities.SavedCities.IsNullorEmpty())
            {

            }
            else
            {
                if (settings.Cities.CurrentIndex == -1 && settings.Cities.EnableLocate)
                {
                    if (settings.Cities.LocatedCity == null)
                    {
                        settings.Cities.CurrentIndex = 0;
                    }
                    else
                    {
                        var currentCity = settings.Cities.LocatedCity;
                        await Init(settings, currentCity);
                    }
                }
                else
                {
                    var currentCity = settings.Cities.SavedCities[settings.Cities.CurrentIndex];
                    await Init(settings, currentCity);
                }
            }
            await FileIOHelper.AppendLogtoCacheAsync("Background Task Completed");
            deferral.Complete();
        }

        private async Task Init(SettingsModel settings, CitySettingsModel currentCity)
        {
            string resstr = await Request.GetRequest(settings, currentCity);
            var resjson1 = HeWeatherContract.Generate(resstr);
            var fetchresult = new HeWeatherModel(resjson1);
            Sender.CreateMainTileQueue(await Generator.CreateAll(fetchresult, DateTime.Now));
            if (settings.Preferences.EnableEveryDay)
            {
                var tomorrow8 = DateTime.Now.Hour > 8 ? (DateTime.Today.AddDays(1)).AddHours(8) : (DateTime.Today.AddHours(8));
                Sender.CreateScheduledToastNotification(Generator.CreateToast(fetchresult, currentCity, settings, tomorrow8).GetXml(), tomorrow8, "EveryDayToast");
            }
            if (!fetchresult.Alarms.IsNullorEmpty() && settings.Preferences.EnableAlarm)
            {
                Sender.CreateBadge(Generator.GenerateAlertBadge());
                Sender.CreateToast(Generator.CreateAlertToast(fetchresult, currentCity));
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

        public void InstantUpdateSecondary(TileContent content)
        {

        }
    }
}
