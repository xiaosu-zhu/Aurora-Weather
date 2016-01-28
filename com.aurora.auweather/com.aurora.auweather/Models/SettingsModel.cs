using com.aurora.auweather.Models.HeWeather;
using com.aurora.shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace com.aurora.auweather.Models
{
    public class SettingsModel
    {
        private bool allowLocation = true;
        private CitySettingsModel[] savedCities;
        private RefreshState refreshState = RefreshState.daily;

        public bool AllowLocation
        {
            get
            {
                return allowLocation;
            }

            set
            {
                allowLocation = value;
            }
        }

        public CitySettingsModel[] SavedCities
        {
            get
            {
                return savedCities;
            }

            set
            {
                savedCities = value;
            }
        }

        public RefreshState RefreshState
        {
            get
            {
                return refreshState;
            }

            set
            {
                refreshState = value;
            }
        }

        public void SaveSettings()
        {
            RoamingSettingsHelper.WriteSettingsValue("AllowLocation", AllowLocation);
            RoamingSettingsHelper.WriteSettingsValue("RefreshState", (byte)RefreshState);
            if (SavedCities != null && SavedCities.Length > 0)
            {
                int i = 0;
                RoamingSettingsHelper.WriteSettingsValue("CityNumber", SavedCities.Length);
                foreach (var item in SavedCities)
                {
                    RoamingSettingsHelper.GetContainer("City" + i).WriteGroupSettings(item);
                    i++;
                }
            }
        }

        public static SettingsModel ReadSettings()
        {
            SettingsModel set = new SettingsModel();
            try
            {
                set.AllowLocation = (bool)RoamingSettingsHelper.ReadSettingsValue("AllowLocation");
                set.RefreshState = (RefreshState)Enum.Parse(typeof(RefreshState),
                    RoamingSettingsHelper.ReadSettingsValue("RefreshState").ToString());
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
                return new SettingsModel();
            }
        }
    }
}

public class CitySettingsModel
{
    private string city;
    private string id;

    public string City
    {
        get
        {
            return city;
        }

        set
        {
            city = value;
        }
    }

    public string Id
    {
        get
        {
            return id;
        }

        set
        {
            id = value;
        }
    }

    public CitySettingsModel(CityInfo info)
    {
        City = info.City;
        Id = info.Id;
    }
    public CitySettingsModel()
    {

    }
}
