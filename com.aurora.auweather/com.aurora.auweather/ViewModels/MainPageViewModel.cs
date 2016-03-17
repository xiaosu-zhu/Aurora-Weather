using Com.Aurora.AuWeather.LunarCalendar;
using Com.Aurora.Shared.MVVM;
using System.Collections.ObjectModel;
using Windows.UI.Xaml.Controls;
using System;

namespace Com.Aurora.AuWeather.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public Pane PaneList { get; set; } = new Pane();

        public MainPageViewModel()
        {
        }

        public CalendarInfo Calendar
        {
            get; set;
        } = new CalendarInfo();
    }

    public class Pane : ObservableCollection<PaneOption>
    {
        public Pane()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            Add(new PaneOption(Symbol.Favorite, loader.GetString("Now"), typeof(NowWeatherPage)));
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
