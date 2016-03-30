using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Models.Settings
{
    public class Cities
    {
        public CitySettingsModel[] SavedCities { get; private set; }

        public int CurrentIndex { get; set; }

        public bool EnableLocate { get; set; } = true;
        public CitySettingsModel LocatedCity { get; set; }

        public static Cities Get()
        {
            Cities c = new Cities();
            try
            {
                var container = RoamingSettingsHelper.GetContainer("Cities");
                c.CurrentIndex = (int)container.Values["CurrentIndex"];
                c.EnableLocate = (bool)container.Values["EnableLocate"];
                CitySettingsModel loc;

                if (container.ReadGroupSettings(out loc))
                {

                    c.LocatedCity = loc;
                    if (c.CurrentIndex == -1)
                    {
                        if (c.EnableLocate == true)
                            c.LocatedCity.IsCurrent = true;
                        else
                        {
                            c.CurrentIndex = 0;
                        }
                    }
                }
                int i = (int)container.Values["Count"];
                List<CitySettingsModel> cs = new List<CitySettingsModel>();
                for (int j = 0; j < i; j++)
                {
                    CitySettingsModel m;
                    var sub = RoamingSettingsHelper.GetContainer(j.ToString());
                    if (sub.ReadGroupSettings(out m))
                    {
                        cs.Add(m);
                    }
                }
                c.SavedCities = cs.ToArray();
                if (c.CurrentIndex != -1 && !c.SavedCities.IsNullorEmpty())
                {
                    c.SavedCities[c.CurrentIndex].IsCurrent = true;
                }
                return c;
            }
            catch (Exception)
            {
                return c;
            }
        }

        public void Pick(int index)
        {
            CurrentIndex = index;
        }

        public void Save()
        {
            var container = RoamingSettingsHelper.GetContainer("Cities");
            container.Values["CurrentIndex"] = CurrentIndex;
            container.Values["EnableLocate"] = EnableLocate;
            try
            {
                container.WriteGroupSettings(LocatedCity);
            }
            catch (Exception)
            {
            }
            SaveCities(container);
        }

        private void SaveCities(Windows.Storage.ApplicationDataContainer container)
        {
            int i = 0;
            if (!SavedCities.IsNullorEmpty())
            {
                foreach (var item in SavedCities)
                {
                    var sub = RoamingSettingsHelper.GetContainer(i.ToString());
                    try
                    {
                        sub.WriteGroupSettings(item);
                        i++;
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
            container.Values["Count"] = i;
        }

        public void Save(CitySettingsModel[] citys)
        {
            this.SavedCities = citys;
            Save();
        }

        public void Set(CitySettingsModel[] citys)
        {
            SavedCities = citys;
        }

        public async Task SaveDataAsync(string currentId, string resstr)
        {
            await FileIOHelper.SaveStringtoStorageAsync(currentId, resstr);
        }
    }
    public class CitySettingsModel
    {
        public string City { get; set; }
        public string Id { get; set; }
        public bool IsPinned { get; set; } = false;

        public bool IsCurrent = false;

        public DateTime LastUpdate { get; set; } = new DateTime(1970, 1, 1);

        public CitySettingsModel(CityInfo info)
        {
            City = info.City;
            Id = info.Id;
        }
        public CitySettingsModel()
        {

        }

        public void Update()
        {
            LastUpdate = DateTime.Now;
        }
    }
}
