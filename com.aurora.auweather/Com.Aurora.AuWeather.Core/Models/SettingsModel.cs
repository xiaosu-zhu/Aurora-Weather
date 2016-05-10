// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.Shared.Helpers;
using Windows.System.Threading;

namespace Com.Aurora.AuWeather.Models
{
    public class SettingsModel
    {
        public Cities Cities { get; private set; }
        public Immersive Immersive { get; private set; }
        public Preferences Preferences { get; private set; }
        public bool Inited { get; private set; }

        public static SettingsModel Get()
        {
            var s = new SettingsModel();
            s.Cities = Cities.Get();
            s.Immersive = Immersive.Get();
            s.Preferences = Preferences.Get();
            var init = RoamingSettingsHelper.ReadSettingsValue("Inited");
            if (init == null)
            {
                init = LocalSettingsHelper.ReadSettingsValue("Inited");
                if (init == null)
                    s.Inited = false;
                else
                {
                    s.Inited = (bool)init;
                }
            }
            else
            {
                s.Inited = (bool)init;
            }
            return s;
        }

        public void SaveSettings()
        {
            var task = ThreadPool.RunAsync((work) =>
             {
                 Cities.Save();
                 Immersive.Save();
                 Preferences.Save();
                 RoamingSettingsHelper.WriteSettingsValue("Inited", Inited);
             });
        }

        public void Set(bool v)
        {
            Inited = v;
        }
    }

}
