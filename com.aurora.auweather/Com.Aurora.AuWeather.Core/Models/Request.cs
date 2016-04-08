using Com.Aurora.AuWeather.License;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.Shared.Helpers;
using System;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Core.Models
{
    public static class Request
    {
        public static async Task<string> GetRequest(SettingsModel settings, CitySettingsModel city)
        {
            return await GetRequest(settings, city.Id, city.Longitude, city.Latitude);
        }

        public static async Task<string> GetRequest(SettingsModel settings, string id, float lon, float lat)
        {
            var keys = Key.key.Split(new string[] { ":|:" }, StringSplitOptions.RemoveEmptyEntries);
            var param = new string[] { "cityid=" + id };
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
                default:
                    resstr = null;
                    break;
            }

            return resstr;
        }
    }
}
