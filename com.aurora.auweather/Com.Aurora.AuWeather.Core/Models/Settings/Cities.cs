// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Core.Models;
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
                var subContainer = container.GetContainer("Locate");
                c.CurrentIndex = (int)container.Values["CurrentIndex"];
                c.EnableLocate = (bool)container.Values["EnableLocate"];
                CitySettingsModel loc;

                if (subContainer.ReadGroupSettings(out loc))
                {
                    if (loc.City != null && loc.Id != null)
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
                if (c.SavedCities == null)
                {
                    c.SavedCities = new CitySettingsModel[] { };
                }
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
            var subContainer = container.GetContainer("Locate");
            container.Values["CurrentIndex"] = CurrentIndex;
            container.Values["EnableLocate"] = EnableLocate;
            if (LocatedCity != null)
            {
                try
                {
                    subContainer.WriteGroupSettings(LocatedCity);
                }
                catch (Exception)
                {
                }
            }
            else
            {
                try
                {
                    container.DeleteContainer("Locate");
                }
                catch (Exception)
                {
                }
                
            }
            SaveCities(container);
        }

        public CitySettingsModel GetCurrentCity()
        {
            if (EnableLocate && CurrentIndex == -1 && LocatedCity != null)
            {
                return LocatedCity;
            }
            else if (EnableLocate && CurrentIndex == -1 && LocatedCity == null)
            {
                throw new NullReferenceException();
            }
            else if (!EnableLocate && CurrentIndex == -1 && !SavedCities.IsNullorEmpty())
            {
                CurrentIndex = 0;
                return SavedCities[CurrentIndex];
            }
            else if (!SavedCities.IsNullorEmpty() && CurrentIndex >= 0 && SavedCities.Length > CurrentIndex)
            {
                return SavedCities[CurrentIndex];
            }
            else
            {
                throw new NullReferenceException();
            }
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

        public async Task SaveDataAsync(string currentId, string resstr, DataSource source)
        {
            try
            {
                switch (source)
                {
                    case DataSource.HeWeather:
                        await FileIOHelper.SaveStringtoStorageAsync(currentId + "_H", resstr);
                        break;
                    case DataSource.Caiyun:
                        await FileIOHelper.SaveStringtoStorageAsync(currentId + "_C", resstr);
                        break;
                    case DataSource.Wunderground:
                        await FileIOHelper.SaveStringtoStorageAsync(currentId + "_W", resstr);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
                
            }
            
        }

        public async Task<string> ReadDataAsync(string id, DataSource dataSource)
        {
            switch (dataSource)
            {
                case DataSource.HeWeather:
                    return await FileIOHelper.ReadStringFromStorageAsync(id + "_H");
                case DataSource.Caiyun:
                    return await FileIOHelper.ReadStringFromStorageAsync(id + "_C");
                case DataSource.Wunderground:
                    return await FileIOHelper.ReadStringFromStorageAsync(id + "_W");
                default:
                    return null;
            }
        }
    }
    public class CitySettingsModel
    {
        public string City { get; set; }
        public string Id { get; set; }
        public bool IsPinned { get; set; } = false;
        public float Longitude { get; set; } = 0;
        public float Latitude { get; set; } = 0;

        public bool IsCurrent = false;

        public DateTime LastUpdate { get; set; } = new DateTime(1970, 1, 1);
        public string ZMW { get; internal set; } = "";

        public CitySettingsModel(CityInfo info)
        {
            City = info.City;
            Id = info.Id;
            Longitude = info.Location.Longitude;
            Latitude = info.Location.Latitude;
        }

        public CitySettingsModel()
        {
            //Use this to Create Default Instance. Otherwise you can't use reflection.
        }

        public void Update()
        {
            LastUpdate = DateTime.Now;
        }

        internal void RequestZMW(string s)
        {
            if (s.IsNullorEmpty())
            {
                ZMW = "";
            }
            else
            {
                ZMW = s;
            }

        }
    }
}
