using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.Shared.MVVM;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;

namespace Com.Aurora.AuWeather.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        public SettingsList SettingsList { get; set; } = new SettingsList();

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

        public SettingsViewModel()
        {
            var license = new License.License();
            IsNotPurchased = license.IsPurchased;
        }

        private bool isnotPurchased;
    }

    internal class SettingsList : ObservableCollection<SettingOption>
    {
        public SettingsList()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            Add(new SettingOption(Symbol.Calculator, loader.GetString("Preferences"), typeof(Preferences)));
            Add(new SettingOption(Symbol.BlockContact, loader.GetString("Immersive_Background"), typeof(Immersive)));
            Add(new SettingOption(Symbol.Accept, loader.GetString("Cities_Management"), typeof(Models.Settings.Cities)));
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
