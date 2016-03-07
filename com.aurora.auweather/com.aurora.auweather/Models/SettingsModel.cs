using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Models
{
    internal class SettingsModel
    {
        public Cities Cities { get; private set; }
        public Immersive Immersive { get; private set; }
        public Preferences Preferences { get; private set; }

        public static SettingsModel Get()
        {
            var s = new SettingsModel();
            s.Cities = Cities.Get();
            s.Immersive = Immersive.Get();
            s.Preferences = Preferences.Get();
            return s;
        }

        internal void SaveSettings()
        {
            Cities.Save();
            Immersive.Save();
            Preferences.Save();
        }
    }

}
