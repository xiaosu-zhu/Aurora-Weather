using Com.Aurora.AuWeather.LunarCalendar;
using Com.Aurora.Shared.MVVM;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using System;
using Com.Aurora.AuWeather.Models.Settings;
using Windows.UI.Xaml;

namespace Com.Aurora.AuWeather.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public Pane PaneList { get; set; } = new Pane();
        private ElementTheme theme;

        public MainPageViewModel()
        {
            var p = Preferences.Get();
            Theme = p.GetTheme();
        }

        public CalendarInfo Calendar
        {
            get; set;
        } = new CalendarInfo();

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

        internal void ReloadTheme()
        {
            var p = Preferences.Get();
            Theme = p.GetTheme();
        }
    }

    public class Pane : ObservableCollection<PaneOption>
    {
        public Pane()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            Add(new PaneOption(Symbol.Street, loader.GetString("Now"), typeof(NowWeatherPage)));
            Add(new PaneOption(Symbol.World, loader.GetString("Cities"), typeof(CitiesPage)));
            Add(new PaneOption(Symbol.Setting, loader.GetString("Settings"), typeof(SettingsPage)));
        }
    }

    public class PaneOption
    {
        public Symbol Symbol { get; set; }
        public string Title { get; set; }
        public Type Page { get; set; }

        public PaneOption(Symbol s, string t, Type p)
        {
            Symbol = s;
            Title = t;
            Page = p;
        }
    }
}
