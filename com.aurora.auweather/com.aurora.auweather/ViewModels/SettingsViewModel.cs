using Com.Aurora.AuWeather.Models.Settings;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

namespace Com.Aurora.AuWeather.ViewModels
{
    internal class SettingsViewModel
    {
        public SettingsList SettingsList { get; set; } = new SettingsList();
        public SettingsViewModel()
        {

        }
    }

    internal class SettingsList : ObservableCollection<SettingOption>
    {
        public SettingsList()
        {
            Add(new SettingOption(Symbol.Calculator, "Preferences", typeof(Preferences)));
            Add(new SettingOption(Symbol.BlockContact, "Immersive Background", typeof(Immersive)));
            Add(new SettingOption(Symbol.Accept, "Cities Management", typeof(Cities)));
            
            Add(new SettingOption(Symbol.Page, "About", typeof(About)));
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
