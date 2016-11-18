// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.LunarCalendar;
using Com.Aurora.Shared.MVVM;
using System.Collections.ObjectModel;
using System;
using Windows.UI.Xaml;
using Com.Aurora.AuWeather.Models;

namespace Com.Aurora.AuWeather.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public Pane PaneList { get; set; } = new Pane();
        private ElementTheme theme;
        private CalendarInfo calendar;

        public object Date;

        public MainPageViewModel()
        {
            var p = SettingsModel.Current.Preferences;
            Theme = p.GetTheme();
            Calendar = p.UseLunarCalendarPrimary ? new CalendarInfo() : null;
        }

        public CalendarInfo Calendar
        {
            get
            {
                return calendar;
            }
            set
            {
                SetProperty(ref calendar, value);
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

        internal void ReloadTheme()
        {
            var p = SettingsModel.Current.Preferences;
            Theme = p.GetTheme();
        }
    }

    public class Pane : ObservableCollection<PaneOption>
    {
        public Pane()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            Add(new PaneOption("\uEA8A", loader.GetString("Now"), typeof(NowWeatherPage)));
            Add(new PaneOption("\uE81E", loader.GetString("Details"), typeof(DetailsPage)));
            Add(new PaneOption("\uEC06", loader.GetString("Cities"), typeof(CitiesPage)));
            Add(new PaneOption("\uE713", loader.GetString("Settings"), typeof(SettingsPage)));
        }
    }

    public class PaneOption
    {
        public string Symbol { get; set; }
        public string Title { get; set; }
        public Type Page { get; set; }

        public PaneOption(string s, string t, Type p)
        {
            Symbol = s;
            Title = t;
            Page = p;
        }
    }
}
