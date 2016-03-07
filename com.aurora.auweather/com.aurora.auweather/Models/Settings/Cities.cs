using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Models.Settings
{
    internal class Cities
    {
        public CitySettingsModel[] SavedCities { get; private set; }

        public uint CurrentIndex { get; private set; }

        public bool EnableLocate { get; private set; } = true;

        public static Cities Get()
        {
            Cities c = new Cities();
            try
            {
                var container = RoamingSettingsHelper.GetContainer("Cities");
                c.CurrentIndex = (uint)container.Values["CurrentIndex"];
                c.EnableLocate = (bool)container.Values["EnableLocate"];
                int i = (int)container.Values["Count"];
                List<CitySettingsModel> cs = new List<CitySettingsModel>();
                for (int j = 0; j < i; j++)
                {
                    CitySettingsModel m;
                    var sub = RoamingSettingsHelper.GetContainer(j.ToString());
                    sub.ReadGroupSettings(out m);
                    cs.Add(m);
                }
                c.SavedCities = cs.ToArray();
                return c;
            }
            catch (Exception)
            {
                return new Cities();
            }
        }

        public void Pick(uint index)
        {
            CurrentIndex = index;
        }

        public void Save()
        {
            var container = RoamingSettingsHelper.GetContainer("Cities");
            container.Values["CurrentIndex"] = CurrentIndex;
            container.Values["EnableLocate"] = EnableLocate;
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
                    sub.WriteGroupSettings(item);
                    i++;
                }
            }
            container.Values["Count"] = i;
        }

        public void Save(CitySettingsModel[] citys)
        {
            this.SavedCities = citys;
            var container = RoamingSettingsHelper.GetContainer("Cities");
            SaveCities(container);
        }

        internal void Set(CitySettingsModel[] citys)
        {
            SavedCities = citys;
        }

        internal void Set(bool e)
        {
            EnableLocate = e;
        }

        internal async Task SaveDataAsync(string currentId, string resstr)
        {
            await FileIOHelper.SaveStringtoStorageAsync(currentId, resstr);
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
            LastUpdate = DateTime.Now;
        }
    }
}
