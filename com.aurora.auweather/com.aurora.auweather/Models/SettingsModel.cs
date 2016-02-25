using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Models
{
    public class SettingsModel
    {
        public bool AllowLocation { get; set; } = true;

        public uint CurrentCityIndex { get; set; } = 0;

        public CitySettingsModel[] SavedCities { get; set; }

        public RefreshState RefreshState { get; set; } = RefreshState.daily;

        public int TempratureParameter { get; set; } = 0;

        public string ForecastDateParameter { get; set; } = "MM-dd";

        public void SaveSettings()
        {
            RoamingSettingsHelper.WriteSettingsValue("AllowLocation", AllowLocation);
            RoamingSettingsHelper.WriteSettingsValue("RefreshState", (byte)RefreshState);
            RoamingSettingsHelper.WriteSettingsValue("CurrentIndex", CurrentCityIndex);
            RoamingSettingsHelper.WriteSettingsValue("TempratureParameter", TempratureParameter);
            RoamingSettingsHelper.WriteSettingsValue("ForecastDateParameter", ForecastDateParameter);
            if (!SavedCities.IsNullorEmpty())
            {
                SaveCities();
            }
        }

        private void SaveCities()
        {
            int i = 0;
            RoamingSettingsHelper.WriteSettingsValue("CityNumber", SavedCities.Length);
            foreach (var item in SavedCities)
            {
                RoamingSettingsHelper.GetContainer("City" + i).WriteGroupSettings(item);
                i++;
            }
        }

        public static SettingsModel ReadSettings()
        {
            SettingsModel set = new SettingsModel();
            try
            {
                set.TempratureParameter = (int)RoamingSettingsHelper.ReadSettingsValue("TempratureParameter");
                set.ForecastDateParameter = (string)RoamingSettingsHelper.ReadSettingsValue("ForecastDateParameter");
                set.AllowLocation = (bool)RoamingSettingsHelper.ReadSettingsValue("AllowLocation");
                set.RefreshState = (RefreshState)Enum.Parse(typeof(RefreshState),
                    RoamingSettingsHelper.ReadSettingsValue("RefreshState").ToString());
                set.CurrentCityIndex = (uint)RoamingSettingsHelper.ReadSettingsValue("CurrentIndex");
                int? i = (int?)RoamingSettingsHelper.ReadSettingsValue("CityNumber");
                if (i != null)
                {
                    List<CitySettingsModel> cities = new List<CitySettingsModel>();
                    for (int j = 0; j < i; j++)
                    {
                        CitySettingsModel city;
                        RoamingSettingsHelper.GetContainer("City" + j).ReadGroupSettings(out city);
                        cities.Add(city);
                    }
                    set.SavedCities = cities.ToArray();
                }
                return set;
            }
            catch (Exception)
            {
                return set;
            }
        }
        public void UpdateCity(CitySettingsModel[] city)
        {
            if (!city.IsNullorEmpty())
            {
                this.SavedCities = city;
                SaveCities();
            }

        }

        public async Task SaveData(string currentId, string fetchresult)
        {
            await FileIOHelper.SaveStringtoStorage(currentId, fetchresult);
        }
    }
}

public class CitySettingsModel
{
    public string City { get; set; }

    public string Id { get; set; }

    public DateTime LastUpdate { get; set; } = new DateTime(1970, 1, 1);

    public CitySettingsModel(CityInfo info)
    {
        City = info.City;
        Id = info.Id;
    }
    public CitySettingsModel()
    {

    }

    internal void Update()
    {
        this.LastUpdate = DateTime.Now;
    }
}
