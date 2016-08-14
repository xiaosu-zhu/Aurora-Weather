// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.Shared.Extensions;
using Windows.System.Threading;
using Com.Aurora.AuWeather.ViewModels.Events;
using Windows.Foundation;
using System.Threading.Tasks;
using Com.Aurora.Shared.MVVM;
using Windows.UI.Xaml;
using Windows.Globalization;
using Windows.UI;
using Windows.ApplicationModel.Core;
using Windows.UI.Core;
using Windows.UI.Xaml.Media;

namespace Com.Aurora.AuWeather.ViewModels
{
    internal class PreferencesSettingViewModel : ViewModelBase
    {
        private Preferences Preferences;
        private ElementTheme theme;

        public PreferencesSettingViewModel()
        {
            Preferences = Preferences.Get();
            Theme1 = Preferences.GetTheme();
            var task = ThreadPool.RunAsync(async (work) =>
            {
                work.Completed = new AsyncActionCompletedHandler(WorkComplete);

                Data = new DataList();
                Temperature = new TemperatureList();
                Wind = new WindList();
                Speed = new SpeedList();
                Length = new LengthList();
                Pressure = new PressureList();
                Theme = new ThemeList();
                Year = new FormatList();
                Month = new FormatList();
                Day = new FormatList();
                Hour = new FormatList();
                Minute = new FormatList();
                Week = new FormatList();
                Languages = new FormatList();

                Year.AddRange(Preferences.YearFormat);
                Month.AddRange(Preferences.MonthFormat);
                Day.AddRange(Preferences.DayFormat);
                Hour.AddRange(Preferences.HourFormat);
                Minute.AddRange(Preferences.MinuteFormat);
                Week.AddRange(Preferences.WeekFormat);
                Separator = Preferences.DateSeparator.ToString();
                Languages.AddRange(ApplicationLanguages.ManifestLanguages);

                Data.SelectedIndex = Data.FindIndex(x =>
                {
                    return (DataSource)x.Value == Preferences.DataSource;
                });
                Temperature.SelectedIndex = Temperature.FindIndex(x =>
                {
                    return (TemperatureParameter)x.Value == Preferences.TemperatureParameter;
                });
                Wind.SelectedIndex = Wind.FindIndex(x =>
                {
                    return (WindParameter)x.Value == Preferences.WindParameter;
                });
                Speed.SelectedIndex = Speed.FindIndex(x =>
                {
                    return (SpeedParameter)x.Value == Preferences.SpeedParameter;
                });
                Length.SelectedIndex = Length.FindIndex(x =>
                {
                    return (LengthParameter)x.Value == Preferences.LengthParameter;
                });
                Pressure.SelectedIndex = Pressure.FindIndex(x =>
                {
                    return (PressureParameter)x.Value == Preferences.PressureParameter;
                });
                Theme.SelectedIndex = Theme.FindIndex(x =>
                {
                    return (RequestedTheme)x.Value == Preferences.Theme;
                });

                RefreshFreq = Preferences.RefreshFrequency;

                Year.SelectedIndex = (int)Preferences.YearNumber;
                Month.SelectedIndex = (int)Preferences.MonthNumber;
                Day.SelectedIndex = (int)Preferences.DayNumber;
                Hour.SelectedIndex = (int)Preferences.HourNumber;
                Minute.SelectedIndex = (int)Preferences.MinuteNumber;
                Week.SelectedIndex = (int)Preferences.WeekNumber;
                Languages.SelectedIndex = Languages.FindIndex(x =>
                {
                    return x == ApplicationLanguages.Languages[0];
                });
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
                {
                    DisableDynamic = Preferences.DisableDynamic;
                    EnableAlarm = Preferences.EnableAlarm;
                    EnableSecond = Preferences.EnableImmersiveSecond;
                    UseWeekDay = Preferences.UseWeekDayforForecast;
                    UseLunarCalendar = Preferences.UseLunarCalendarPrimary;
                    Showtt = Preferences.DecorateNumber == 1;
                    ShowImmersivett = Preferences.ShowImmersivett;
                    EnableEveryDay = Preferences.EnableEveryDay;
                    EnableMorning = Preferences.EnableMorning;
                    EnableEvening = Preferences.EnableEvening;
                    EnablePulltoRefresh = Preferences.EnablePulltoRefresh;
                    ThemeasRiseSet = Preferences.ThemeasRiseSet;
                    EnableFullScreen = Preferences.EnableFullScreen;
                    AlwaysShowBackground = Preferences.AlwaysShowBackground;
                    SetWallPaper = Preferences.SetWallPaper;
                    TransparentTile = Preferences.TransparentTile;
                    AlwaysBlur = Preferences.AlwaysBlur;
                    EnableDebug = Preferences.EnableCrashReport;

                    StartTime = Preferences.StartTime;
                    EndTime = Preferences.EndTime;
                    MorningTime = Preferences.MorningNoteTime;
                    EveningTime = Preferences.EveningNoteTime;

                    NowPanelHeight = Preferences.NowPanelHeight;
                    IsNowPanelLowStyle = Preferences.IsNowPanelLowStyle;
                    ForecastHide = Preferences.ForecastHide;
                    AQIHide = Preferences.AQIHide;
                    DetailsHide = Preferences.DetailsHide;
                    SuggestHide = Preferences.SuggestHide;
                    if (Preferences.MainColor.A == 0)
                    {
                        MainColor = new SolidColorBrush((Color)App.Current.Resources["SystemAccentColor"]);
                    }
                    else
                    {
                        MainColor = new SolidColorBrush(Preferences.MainColor);
                    }

                }));
            });
        }

        private async void WorkComplete(IAsyncAction asyncInfo, AsyncStatus asyncStatus)
        {
            await Task.Delay(500);
            OnFetchDataComplete();
        }

        public EventHandler<FetchDataCompleteEventArgs> FetchDataComplete;


        private uint refresFreq;
        private TimeSpan startTime;
        private TimeSpan endTime;
        private TimeSpan eveningNoteTime;
        private string separator;
        private bool disableDynamic;
        private bool enableAlarm;
        private bool enableSecond;
        private bool useWeekDay;
        private bool useLunarCalendar;
        private bool showtt;
        private bool showImmersivett;
        private bool enableEveryDay;
        private bool enableDebug;

        private bool enableMorning;
        private bool enableEvening;

        private bool enablePulltoRefresh;
        private SolidColorBrush mainColor;
        private bool themeasRiseSet;
        private bool enableFullScreen;
        private bool alwaysShowBackground;
        private uint nowPanelHeight;
        private bool isNowPanelLowStyle;
        private bool forecastHide;
        private bool aqiHide;
        private bool detailsHide;
        private bool suggestHide;
        private bool setWallPaper;
        private bool transparentTile;
        private bool alwaysBlur;
        private TimeSpan morningNoteTime;

        private void OnFetchDataComplete()
        {
            FetchDataComplete?.Invoke(this, new FetchDataCompleteEventArgs());
        }

        internal void SetFormatValue(string name, string v)
        {
            if (name == "Year")
            {
                Preferences.YearNumber = (uint)Array.IndexOf(Preferences.YearFormat, v);
            }
            if (name == "Month")
            {
                Preferences.MonthNumber = (uint)Array.IndexOf(Preferences.MonthFormat, v);
            }
            if (name == "Day")
            {
                Preferences.DayNumber = (uint)Array.IndexOf(Preferences.DayFormat, v);
            }
            if (name == "Hour")
            {
                Preferences.HourNumber = (uint)Array.IndexOf(Preferences.HourFormat, v);
            }
            if (name == "Minute")
            {
                Preferences.MinuteNumber = (uint)Array.IndexOf(Preferences.MinuteFormat, v);
            }
            if (name == "Week")
            {
                Preferences.WeekNumber = (uint)Array.IndexOf(Preferences.WeekFormat, v);
            }
            SaveAll();
        }

        internal void ReloadTheme()
        {
            Theme1 = Preferences.GetTheme();
        }

        internal void SetEnumValue(Enum value)
        {
            if (value is TemperatureParameter)
            {
                Preferences.Set((TemperatureParameter)value);
            }
            else if (value is WindParameter)
            {
                Preferences.Set((WindParameter)value);
            }
            else if (value is SpeedParameter)
            {
                Preferences.Set((SpeedParameter)value);
            }
            else if (value is LengthParameter)
            {
                Preferences.Set((LengthParameter)value);
            }
            else if (value is PressureParameter)
            {
                Preferences.Set((PressureParameter)value);
            }
            else if (value is RequestedTheme)
            {
                Preferences.Set((RequestedTheme)value);
            }
            SaveAll();
        }

        internal void SaveAll()
        {
            Preferences.Save();
        }

        public DataList Data { get; private set; }
        public TemperatureList Temperature { get; private set; }
        public WindList Wind { get; private set; }
        public SpeedList Speed { get; private set; }
        public LengthList Length { get; private set; }
        public PressureList Pressure { get; private set; }
        public ThemeList Theme { get; private set; }
        public FormatList Languages { get; private set; }
        public FormatList Hour { get; private set; }
        public FormatList Minute { get; private set; }
        public FormatList Year { get; private set; }
        public FormatList Month { get; private set; }
        public FormatList Day { get; private set; }
        public FormatList Week { get; private set; }

        public uint RefreshFreq
        {
            get
            {
                return refresFreq;
            }
            set
            {
                SetProperty(ref refresFreq, value);
                Preferences.Set(value);
                Preferences.Save();
                RegBG();
            }
        }
        public TimeSpan StartTime
        {
            get
            {
                return startTime;
            }
            set
            {
                SetProperty(ref startTime, value);
                Preferences.StartTime = value;
                Preferences.Save();
                ReloadTheme();
                SettingOptionsPage.Current.ReloadTheme();
            }
        }
        public TimeSpan EndTime
        {
            get
            {
                return endTime;
            }
            set
            {
                SetProperty(ref endTime, value);
                Preferences.EndTime = value;
                Preferences.Save();
                ReloadTheme();
                SettingOptionsPage.Current.ReloadTheme();
            }
        }
        public TimeSpan MorningTime
        {
            get
            {
                return morningNoteTime;
            }
            set
            {
                SetProperty(ref morningNoteTime, value);
                Preferences.MorningNoteTime = value;
                Preferences.Save();
            }
        }
        public TimeSpan EveningTime
        {
            get
            {
                return eveningNoteTime;
            }
            set
            {
                SetProperty(ref eveningNoteTime, value);
                Preferences.EveningNoteTime = value;
                Preferences.Save();
            }
        }
        public string Separator
        {
            get
            {
                return separator;
            }
            set
            {
                if (value.Length > 0)
                {
                    if (Preferences.DateSeparator != value[0])
                    {
                        Preferences.DateSeparator = value[0];
                        SaveAll();
                        MainPage.Current.ReCalcPaneFormat();
                    }
                    SetProperty(ref separator, value[0].ToString());
                }
            }
        }
        public bool DisableDynamic
        {
            get
            {
                return disableDynamic;
            }
            set
            {
                SetProperty(ref disableDynamic, value);
                Preferences.DisableDynamic = value;
                Preferences.Save();
            }
        }
        public bool EnableAlarm
        {
            get
            {
                return enableAlarm;
            }
            set
            {
                SetProperty(ref enableAlarm, value);
                Preferences.EnableAlarm = value;
                Preferences.Save();
            }
        }
        public bool EnableSecond
        {
            get
            {
                return enableSecond;
            }
            set
            {
                SetProperty(ref enableSecond, value);
                Preferences.EnableImmersiveSecond = value;
                Preferences.Save();
            }
        }
        public bool UseWeekDay
        {
            get
            {
                return useWeekDay;
            }
            set
            {
                SetProperty(ref useWeekDay, value);
                Preferences.UseWeekDayforForecast = value;
                Preferences.Save();
            }
        }
        public bool UseLunarCalendar
        {
            get
            {
                return useLunarCalendar;
            }
            set
            {
                if (useLunarCalendar != value)
                {
                    SetProperty(ref useLunarCalendar, value);
                    Preferences.UseLunarCalendarPrimary = value;
                    Preferences.Save();
                    MainPage.Current.ReCalcPaneFormat();
                }
            }
        }
        public bool Showtt
        {
            get
            {
                return showtt;
            }
            set
            {
                if (showtt != value)
                {
                    SetProperty(ref showtt, value);
                    Preferences.DecorateNumber = value ? 1u : 0u;
                    Preferences.Save();
                    MainPage.Current.ReCalcPaneFormat();
                }
            }
        }
        public bool ShowImmersivett
        {
            get
            {
                return showImmersivett;
            }
            set
            {
                SetProperty(ref showImmersivett, value);
                Preferences.ShowImmersivett = value;
                Preferences.Save();
            }
        }
        public bool EnableEveryDay
        {
            get
            {
                return enableEveryDay;
            }
            set
            {
                SetProperty(ref enableEveryDay, value);
            }
        }
        public bool EnablePulltoRefresh
        {
            get
            {
                return enablePulltoRefresh;
            }
            set
            {
                SetProperty(ref enablePulltoRefresh, value);
                Preferences.EnablePulltoRefresh = value;
                Preferences.Save();
            }
        }
        public SolidColorBrush MainColor
        {
            get
            {
                return mainColor;
            }
            set
            {
                SetProperty(ref mainColor, value);
            }
        }
        public bool ThemeasRiseSet
        {
            get
            {
                return themeasRiseSet;
            }
            set
            {
                SetProperty(ref themeasRiseSet, value);
                Preferences.ThemeasRiseSet = value;
                Preferences.Save();
                ReloadTheme();
                SettingOptionsPage.Current.ReloadTheme();
            }
        }
        public bool EnableFullScreen
        {
            get
            {
                return enableFullScreen;
            }
            set
            {
                SetProperty(ref enableFullScreen, value);
                Preferences.EnableFullScreen = value;
                Preferences.Save();
            }
        }

        internal void SetColor(Color transparent)
        {
            Preferences.MainColor = transparent;
            Preferences.Save();
        }

        public bool AlwaysShowBackground
        {
            get
            {
                return alwaysShowBackground;
            }
            set
            {
                SetProperty(ref alwaysShowBackground, value);
                Preferences.AlwaysShowBackground = value;
                Preferences.Save();
            }
        }
        public ElementTheme Theme1
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

        public uint NowPanelHeight
        {
            get
            {
                return nowPanelHeight;
            }
            set
            {
                SetProperty(ref nowPanelHeight, value);
                Preferences.NowPanelHeight = value;
                Preferences.Save();
            }
        }
        public bool IsNowPanelLowStyle
        {
            get
            {
                return isNowPanelLowStyle;
            }
            set
            {
                SetProperty(ref isNowPanelLowStyle, value);
                Preferences.IsNowPanelLowStyle = value;
                Preferences.Save();
            }
        }
        public bool ForecastHide
        {
            get
            {
                return forecastHide;
            }
            set
            {
                SetProperty(ref forecastHide, value);
                Preferences.ForecastHide = value;
                Preferences.Save();
            }
        }
        public bool AQIHide
        {
            get
            {
                return aqiHide;
            }
            set
            {
                SetProperty(ref aqiHide, value);
                Preferences.AQIHide = value;
                Preferences.Save();
            }
        }
        public bool DetailsHide
        {
            get
            {
                return detailsHide;
            }
            set
            {
                SetProperty(ref detailsHide, value);
                Preferences.DetailsHide = value;
                Preferences.Save();
            }
        }
        public bool SuggestHide
        {
            get
            {
                return suggestHide;
            }
            set
            {
                SetProperty(ref suggestHide, value);
                Preferences.SuggestHide = value;
                Preferences.Save();
            }
        }

        public bool SetWallPaper
        {
            get
            {
                return setWallPaper;
            }
            set
            {
                SetProperty(ref setWallPaper, value);
                Preferences.SetWallPaper = value;
                Preferences.Save();
            }
        }

        public bool TransparentTile
        {
            get
            {
                return transparentTile;
            }
            set
            {
                SetProperty(ref transparentTile, value);
                Preferences.TransparentTile = value;
                Preferences.Save();
            }
        }

        public bool AlwaysBlur
        {
            get
            {
                return alwaysBlur;
            }

            set
            {
                SetProperty(ref alwaysBlur, value);
                Preferences.AlwaysBlur = value;
                Preferences.Save();
            }
        }

        public bool EnableDebug
        {
            get
            {
                return enableDebug;
            }

            set
            {
                SetProperty(ref enableDebug, value);
                Preferences.EnableCrashReport = value;
                Preferences.Save();
            }
        }

        public bool EnableMorning
        {
            get
            {
                return enableMorning;
            }

            set
            {
                SetProperty(ref enableMorning, value);
                Preferences.EnableMorning = value;
                Preferences.Save();
            }
        }

        public bool EnableEvening
        {
            get
            {
                return enableEvening;
            }

            set
            {
                SetProperty(ref enableEvening, value);
                Preferences.EnableEvening = value;
                Preferences.Save();
            }
        }


        internal async void RegBG()
        {
            var lic = new License.License();
            await Core.Models.BGTask.RegBGTask(Preferences.RefreshFrequency, lic.IsPurchased);
        }

        internal async Task SetSource(DataSource caiyun)
        {
            await Preferences.Set(caiyun);
            SaveAll();
        }
    }




    public class FormatList : List<string>
    {
        public int SelectedIndex { get; internal set; } = 0;
    }

    public class TemperatureList : List<EnumSelector>
    {
        public TemperatureList()
        {
            foreach (TemperatureParameter item in Enum.GetValues(typeof(TemperatureParameter)))
            {
                Add(new EnumSelector(item, item.GetDisplayName()));
            }
        }

        public int SelectedIndex { get; internal set; } = 0;
    }

    public class DataList : List<EnumSelector>
    {
        public DataList()
        {
            foreach (DataSource item in Enum.GetValues(typeof(DataSource)))
            {
                Add(new EnumSelector(item, item.GetDisplayName()));
            }
        }

        public int SelectedIndex { get; internal set; } = 0;
    }

    public class PressureList : List<EnumSelector>
    {
        public PressureList()
        {
            foreach (PressureParameter item in Enum.GetValues(typeof(PressureParameter)))
            {
                Add(new EnumSelector(item, item.GetDisplayName()));
            }
        }

        public int SelectedIndex { get; internal set; } = 0;
    }

    public class WindList : List<EnumSelector>
    {
        public WindList()
        {
            foreach (WindParameter item in Enum.GetValues(typeof(WindParameter)))
            {
                Add(new EnumSelector(item, item.GetDisplayName()));
            }
        }
        public int SelectedIndex { get; internal set; } = 0;
    }

    public class SpeedList : List<EnumSelector>
    {
        public SpeedList()
        {
            foreach (SpeedParameter item in Enum.GetValues(typeof(SpeedParameter)))
            {
                Add(new EnumSelector(item, item.GetDisplayName()));
            }
        }
        public int SelectedIndex { get; internal set; } = 0;
    }

    public class LengthList : List<EnumSelector>
    {
        public LengthList()
        {
            foreach (LengthParameter item in Enum.GetValues(typeof(LengthParameter)))
            {
                Add(new EnumSelector(item, item.GetDisplayName()));
            }
        }
        public int SelectedIndex { get; internal set; } = 0;
    }

    public class ThemeList : List<EnumSelector>
    {
        public ThemeList()
        {
            foreach (RequestedTheme item in Enum.GetValues(typeof(RequestedTheme)))
            {
                Add(new EnumSelector(item, item.GetDisplayName()));
            }
        }
        public int SelectedIndex { get; internal set; } = 0;
    }

    public class EnumSelector
    {
        public Enum Value { get; private set; }
        public string Title { get; private set; }

        public EnumSelector(Enum e, string t)
        {
            Value = e;
            Title = t;
        }
    }
}
