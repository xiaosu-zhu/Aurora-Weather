// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Core.Models
{
    public static class Request
    {
        public static async Task<string> GetRequestAsync(SettingsModel settings, CitySettingsModel city)
        {
            try
            {
                return await GetRequestAsync(settings, city.Id, city.City, city.Longitude, city.Latitude, city.ZMW);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static async Task<string> RequestbyIpAsync(string ip)
        {
            if (WebHelper.IsInternet())
            {
                var keys = Key.key.Split(new string[] { ":|:" }, StringSplitOptions.RemoveEmptyEntries);
                var param = new string[] { "cityip=" + ip };
                var resstr = await BaiduRequestHelper.RequestWithKeyAsync("http://apis.baidu.com/heweather/pro/weather", param, keys[0]);
                return resstr;
            }
            else
            {
                return null;
            }
        }

        public static async Task<string> GetRequestAsync(SettingsModel settings, string id, string city, float lon, float lat, string zmw)
        {
            try
            {
                if (WebHelper.IsInternet())
                {
                    var keys = Key.key.Split(new string[] { ":|:" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] param;
                    if (id.IsNullorEmpty())
                    {
                        param = new string[] { "city=" + city };
                    }
                    else
                    {
                        param = new string[] { "cityid=" + id };

                    }
                    string resstr;
                    switch (settings.Preferences.DataSource)
                    {
                        case DataSource.HeWeather:
                            resstr = await BaiduRequestHelper.RequestWithKeyAsync("http://apis.baidu.com/heweather/pro/weather", param, keys[0]);
                            break;
                        case DataSource.Caiyun:
                            resstr = await CaiyunRequestHelper.RequestNowWithKeyAsync(lon, lat, keys[1]);
                            resstr += ":|:";
                            resstr += await CaiyunRequestHelper.RequestForecastWithKeyAsync(lon, lat, keys[1]);
                            break;
                        case DataSource.Wunderground:

                            if (!zmw.IsNullorEmpty())
                            {
                                resstr = await WundergroundRequestHelper.GetResult(keys[2], zmw);
                            }
                            else
                            {
                                resstr = await GeoLookup(lat, lon);
                                if (resstr.Length < 40)
                                {
                                    if (!settings.Cities.SavedCities.IsNullorEmpty() && !id.IsNullorEmpty())
                                    {
                                        foreach (var item in settings.Cities.SavedCities)
                                        {
                                            if (item.Id == id)
                                            {
                                                item.RequestZMW(resstr);
                                                zmw = item.ZMW;
                                            }
                                        }
                                    }
                                    else if (settings.Cities.LocatedCity != null)
                                    {
                                        settings.Cities.LocatedCity.RequestZMW(resstr);
                                        zmw = settings.Cities.LocatedCity.ZMW;
                                    }
                                    settings.Cities.Save();
                                    resstr = await WundergroundRequestHelper.GetResult(keys[2], zmw);
                                }
                            }
                            break;
                        default:
                            resstr = null;
                            break;
                    }
                    return resstr;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        internal static async Task<string> GeoLookup(float latitude, float longitude)
        {
            var keys = Key.key.Split(new string[] { ":|:" }, StringSplitOptions.RemoveEmptyEntries);
            var s = await WundergroundRequestHelper.GeoLookup(keys[2], latitude, longitude);
            var w = WunderGround.JsonContract.WunderGroundContract.Generate(s);
            if (w != null && w.location != null && w.forecast != null)
            {
                return s;
            }
            if (w != null && w.response != null && !w.response.results.IsNullorEmpty())
            {
                return w.response.results[0].zmw;
            }
            else if (w != null && w.location != null)
            {
                return w.location.l.TrimStart("/q/".ToCharArray());
            }
            else
            {
                return null;
            }
        }
    }

}
