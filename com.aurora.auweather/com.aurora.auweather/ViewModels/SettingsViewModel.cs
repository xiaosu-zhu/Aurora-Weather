// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.Shared.MVVM;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI;
using Com.Aurora.AuWeather.Models;

namespace Com.Aurora.AuWeather.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        public SettingsList SettingsList { get; set; } = new SettingsList();
        private ElementTheme theme;
        public bool IsNotPurchased
        {
            get
            {
                return isnotPurchased;
            }

            set
            {
                SetProperty(ref isnotPurchased, value);
            }
        }

        public ElementTheme Theme
        {
            get
            {
                return theme;
            }

            set
            {
                SetProperty(ref theme, value);
            }
        }

        public SettingsViewModel()
        {
            var license = new License.License();
            IsNotPurchased = license.IsPurchased;
            var p = SettingsModel.Current.Preferences;
            Theme = p.GetTheme();
        }

        private bool isnotPurchased;

        internal void ReloadTheme()
        {
            var p = SettingsModel.Current.Preferences;
            Theme = p.GetTheme();
        }
    }

    internal class SettingsList : ObservableCollection<SettingOption>
    {
        public SettingsList()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            Add(new SettingOption(Symbol.Repair, loader.GetString("Preferences"), typeof(Preferences)));
            Add(new SettingOption(Symbol.BrowsePhotos, loader.GetString("Immersive_Background"), typeof(Immersive)));
            Add(new SettingOption(Symbol.World, loader.GetString("Cities_Management"), typeof(Models.Settings.Cities)));
            Add(new SettingOption(Symbol.Page, loader.GetString("About"), typeof(About)));
        }
    }

    internal class SettingOption
    {
        public Symbol Symbol
        {
            get; set;
        }
        public string Title
        {
            get;
            set;
        }
        public Type Option
        {
            get;
            set;
        }

        public SettingOption(Symbol s, string t, Type p)
        {
            Symbol = s;
            Title = t;
            Option = p;
        }
    }
}
