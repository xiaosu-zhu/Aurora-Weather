// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Com.Aurora.Shared.Helpers;
using Windows.UI.Xaml;
using Com.Aurora.AuWeather.Core.LunarCalendar;
using System.Threading.Tasks;
using Windows.UI;

namespace Com.Aurora.AuWeather.Models.Settings
{
    public class Preferences
    {
        public TemperatureParameter TemperatureParameter { get; private set; } = TemperatureParameter.Celsius;
        public PressureParameter PressureParameter { get; private set; } = PressureParameter.Atm;
        public LengthParameter LengthParameter { get; private set; } = LengthParameter.KM;
        public SpeedParameter SpeedParameter { get; private set; } = SpeedParameter.KMPH;
        public WindParameter WindParameter { get; private set; } = WindParameter.SpeedandText;

        public DataSource DataSource { get; private set; } = DataSource.HeWeather;

        public bool EnableEveryDay
        {
            get
            {
                return EnableMorning || EnableEvening;
            }
        }

        public bool EnableMorning { get; set; } = false;
        public bool EnableEvening { get; set; } = false;

        public uint RefreshFrequency { get; set; } = 90;
        public uint NowPanelHeight { get; set; } = 320;
        public double ImmersiveTimeout { get; set; } = 2;

        public RequestedTheme Theme { get; private set; } = RequestedTheme.Auto;

        public bool DisableDynamic { get; set; } = false;
        public bool EnableImmersiveSecond { get; set; } = false;
        public bool UseLunarCalendarPrimary { get; set; } = true;
        public bool EnableAlarm { get; set; } = false;
        public bool UseWeekDayforForecast { get; set; } = false;
        public bool EnablePulltoRefresh { get; set; } = false;
        public bool AlwaysShowBackground { get; set; } = false;
        public bool ThemeasRiseSet { get; set; } = true;
        public bool EnableFullScreen { get; set; } = false;
        public bool IsNowPanelLowStyle { get; set; } = false;
        public bool ForecastHide { get; set; } = false;
        public bool AQIHide { get; set; } = false;
        public bool DetailsHide { get; set; } = false;
        public bool SuggestHide { get; set; } = false;
        public bool AlwaysBlur { get; set; } = false;

        public readonly string[] YearFormat = new string[] { " ", "yy", "yyyy" };
        public readonly string[] MonthFormat = new string[] { " ", "M", "MM" };
        public readonly string[] DayFormat = new string[] { " ", "d", "dd" };
        public readonly string[] WeekFormat = new string[] { " ", "ddd", "dddd" };
        public readonly string[] HourFormat = new string[] { "H", "HH", "h", "hh" };
        public readonly string[] MinuteFormat = new string[] { "m", "mm" };
        public readonly string[] DecorateFormat = new string[] { " ", "tt" };

        public char DateSeparator { get; set; } = '/';
        public uint YearNumber { get; set; } = 2;
        public uint MonthNumber { get; set; } = 1;
        public uint DayNumber { get; set; } = 2;
        public uint WeekNumber { get; set; } = 0;
        public uint HourNumber { get; set; } = 1;
        public uint MinuteNumber { get; set; } = 1;
        public uint DecorateNumber { get; set; } = 0;
        public bool ShowImmersivett { get; set; } = false;

        public Color MainColor { get; set; } = Colors.Transparent;

        public TimeSpan StartTime { get; set; } = new TimeSpan(19, 30, 0);
        public TimeSpan EndTime { get; set; } = new TimeSpan(7, 30, 0);
        public TimeSpan MorningNoteTime { get; set; } = new TimeSpan(7, 30, 0);
        public TimeSpan EveningNoteTime { get; set; } = new TimeSpan(19, 30, 0);
        public bool SetWallPaper { get; set; } = false;
        public bool TransparentTile { get; set; } = false;
        public DateTime LastAlertTime { get; set; } = new DateTime(1970, 1, 1);
        public DateTime LastAlarmTime { get; set; } = new DateTime(1970, 1, 1);
        public bool EnableCrashReport { get; set; } = false;

        private static DateTime themeTime = new DateTime(1970, 1, 1);
        private static TimeSpan setTime = new TimeSpan(19, 30, 0);
        private static TimeSpan riseTime = new TimeSpan(7, 30, 0);

        public static Preferences Get()
        {
            Preferences ins;
            var container = RoamingSettingsHelper.GetContainer("Preferences");
            container.ReadGroupSettings(out ins);
            if (ins.Theme != RequestedTheme.Default)
            {
                ins.MainColor = Colors.Transparent;
            }
            return ins;
        }

        public ElementTheme GetTheme()
        {
            if (Theme == RequestedTheme.Auto)
            {
                if (!ThemeasRiseSet)
                {
                    if (StartTime < EndTime)
                    {
                        if (DateTime.Now > (DateTime.Today + EndTime) || DateTime.Now < (DateTime.Today + StartTime))
                        {
                            return ElementTheme.Light;
                        }
                        else
                        {
                            return ElementTheme.Dark;
                        }
                    }
                    else
                    {
                        if (DateTime.Now > (DateTime.Today + EndTime) && DateTime.Now < (DateTime.Today + StartTime))
                        {
                            return ElementTheme.Light;
                        }
                        else
                        {
                            return ElementTheme.Dark;
                        }
                    }
                }
                else
                {
                    var c = Cities.Get();
                    if (c.EnableLocate && c.LocatedCity != null && c.LocatedCity.Longitude != 0 && c.LocatedCity.Latitude != 0)
                    {
                        if ((DateTime.Now - themeTime).TotalDays > 0.5)
                        {
                            riseTime = SunRiseSet.GetRise(new Location(c.LocatedCity.Latitude, c.LocatedCity.Longitude), DateTime.Now);
                            setTime = SunRiseSet.GetSet(new Location(c.LocatedCity.Latitude, c.LocatedCity.Longitude), DateTime.Now);
                            themeTime = DateTime.Now;
                        }
                        if (DateTime.Now > (DateTime.Today + riseTime) && DateTime.Now < (DateTime.Today + setTime))
                        {
                            return ElementTheme.Light;
                        }
                        else
                        {
                            return ElementTheme.Dark;
                        }
                    }
                    else
                    {
                        if (DateTime.Now > (DateTime.Today + riseTime) && DateTime.Now < (DateTime.Today + setTime))
                        {
                            return ElementTheme.Light;
                        }
                        else
                        {
                            return ElementTheme.Dark;
                        }
                    }
                }
            }
            if (Theme == RequestedTheme.Dark)
            {
                return ElementTheme.Dark;
            }
            if (Theme == RequestedTheme.Light)
            {
                return ElementTheme.Light;
            }
            if (Theme == RequestedTheme.Default)
                return ElementTheme.Default;
            return ElementTheme.Default;
        }

        public void Set(TemperatureParameter t)
        {
            TemperatureParameter = t;
        }
        public void Set(PressureParameter p)
        {
            PressureParameter = p;
        }
        public void Set(LengthParameter l)
        {
            LengthParameter = l;
        }
        public void Set(SpeedParameter s)
        {
            SpeedParameter = s;
        }
        public void Set(WindParameter w)
        {
            WindParameter = w;
        }
        public void Set(uint r)
        {
            RefreshFrequency = r;
        }
        public void Set(RequestedTheme r)
        {
            Theme = r;
            if (Theme != RequestedTheme.Default)
            {
                MainColor = Colors.Transparent;
            }
        }
        public async Task Set(DataSource d)
        {
            DataSource = d;
            try
            {
                switch (d)
                {
                    case DataSource.HeWeather:
                        await FileIOHelper.RemoveLocalFilesWithKeywordAsync("_H");
                        break;
                    case DataSource.Caiyun:
                        await FileIOHelper.RemoveLocalFilesWithKeywordAsync("_C");
                        break;
                    case DataSource.Wunderground:
                        await FileIOHelper.RemoveLocalFilesWithKeywordAsync("_W");
                        break;
                    default:
                        break;
                }
            }
            catch (Exception)
            {
            }

        }


        public void Save()
        {
            var container = RoamingSettingsHelper.GetContainer("Preferences");
            container.WriteGroupSettings(this);
        }

        public string GetHourlyFormat()
        {
            return (DecorateFormat[DecorateNumber] + "\n" + HourFormat[HourNumber] + ':' + MinuteFormat[MinuteNumber]).Trim(new char[] { ' ', ':' });
        }

        public string GetImmersiveHourFormat()
        {
            return (DecorateFormat[ShowImmersivett ? 1 : 0] + ' ' + HourFormat[HourNumber]).Trim();
        }

        public string GetImmersiveMinFormat()
        {
            return (MinuteFormat[MinuteNumber]).Trim();
        }

        public string GetForecastFormat()
        {
            if (!UseWeekDayforForecast)
                return MonthFormat[MonthNumber] + DateSeparator + DayFormat[DayNumber];
            else
            {
                if (WeekNumber > 0)
                {
                    return WeekFormat[WeekNumber];
                }
                return "dddd";
            }
        }

        public string GetTileFormat()
        {
            if (WeekNumber > 0)
            {
                return WeekFormat[WeekNumber];
            }
            return "ddd";
        }

        public string GetDateFormat()
        {
            return (YearFormat[YearNumber] + DateSeparator + MonthFormat[MonthNumber] + DateSeparator + DayFormat[DayNumber] + "  " + WeekFormat[WeekNumber]).Trim(new char[] { ' ', DateSeparator });
        }


    }
}
