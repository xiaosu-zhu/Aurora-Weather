using System;
using System.Collections.Generic;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.Shared.Extensions;
using Windows.System.Threading;
using Com.Aurora.AuWeather.ViewModels.Events;

namespace Com.Aurora.AuWeather.ViewModels
{
    internal class PreferencesSettingViewModel
    {
        private Preferences Preferences;

        public PreferencesSettingViewModel()
        {
            var task = ThreadPool.RunAsync((work) =>
            {
                Preferences = Preferences.Get();
                Temperature = new TemperatureList();
                Wind = new WindList();
                Speed = new SpeedList();
                Length = new LengthList();
                Pressure = new PressureList();
                Theme = new ThemeList();
                RefreshFreq = new RefreshFreqList();
                Year = new FormatList();
                Month = new FormatList();
                Day = new FormatList();
                Hour = new FormatList();
                Minute = new FormatList();
                Week = new FormatList();

                Year.AddRange(Preferences.YearFormat);
                Month.AddRange(Preferences.MonthFormat);
                Day.AddRange(Preferences.DayFormat);
                Hour.AddRange(Preferences.HourFormat);
                Minute.AddRange(Preferences.MinuteFormat);
                Week.AddRange(Preferences.WeekFormat);
                Separator = Preferences.DateSeparator.ToString();

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
                RefreshFreq.SelectedIndex = RefreshFreq.FindIndex(x =>
                {
                    return (RefreshState)x.Value == Preferences.RefreshFrequency;
                });

                Year.SelectedIndex = (int)Preferences.YearNumber;
                Month.SelectedIndex = (int)Preferences.MonthNumber;
                Day.SelectedIndex = (int)Preferences.DayNumber;
                Hour.SelectedIndex = (int)Preferences.HourNumber;
                Minute.SelectedIndex = (int)Preferences.MinuteNumber;
                Week.SelectedIndex = (int)Preferences.WeekNumber;

                DisableDynamic = Preferences.DisableDynamic;
                EnableAlarm = Preferences.EnableAlarm;
                EnableSecond = Preferences.EnableImmersiveSecond;
                UseWeekDay = Preferences.UseWeekDayforForecast;
                UseLunarCalendar = Preferences.UseLunarCalendarPrimary;
                Showtt = Preferences.DecorateNumber == 1;
                ShowImmersivett = Preferences.ShowImmersivett;
                EnableEveryDay = Preferences.EnableEveryDay;
                EnablePulltoRefresh = Preferences.EnablePulltoRefresh;

                OnFetchDataComplete();
            });
        }

        public EventHandler<FetchDataCompleteEventArgs> FetchDataComplete;

        private void OnFetchDataComplete()
        {
            var h = FetchDataComplete;
            if (h != null)
            {
                h(this, new FetchDataCompleteEventArgs());
            }
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
            if(name == "Week")
            {
                Preferences.WeekNumber = (uint)Array.IndexOf(Preferences.WeekFormat, v);
            }
            SaveAll();
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
            else if (value is RefreshState)
            {
                Preferences.Set((RefreshState)value);
            }
            else if (value is RequestedTheme)
            {
                Preferences.Set((RequestedTheme)value);
            }
            SaveAll();
        }

        internal void SaveAll()
        {
            var task = ThreadPool.RunAsync((work) =>
            {
                Preferences.Save();
            });
        }

        internal void SetSeparator(string text)
        {
            if (text.Length > 0)
                Preferences.DateSeparator = text[0];
            SaveAll();
        }

        public TemperatureList Temperature { get; private set; }
        public WindList Wind { get; private set; }
        public SpeedList Speed { get; private set; }
        public LengthList Length { get; private set; }
        public PressureList Pressure { get; private set; }
        public ThemeList Theme { get; private set; }
        public RefreshFreqList RefreshFreq { get; private set; }

        public FormatList Hour { get; private set; }
        public FormatList Minute { get; private set; }
        public FormatList Year { get; private set; }
        public FormatList Month { get; private set; }
        public FormatList Day { get; private set; }
        public FormatList Week { get; private set; }

        public string Separator { get; internal set; }
        public bool DisableDynamic { get; private set; }
        public bool EnableAlarm { get; private set; }
        public bool EnableSecond { get; private set; }
        public bool UseWeekDay { get; private set; }
        public bool UseLunarCalendar { get; private set; }
        public bool Showtt { get; private set; }
        public bool ShowImmersivett { get; private set; }
        public bool EnableEveryDay { get; private set; }
        public bool EnablePulltoRefresh { get; private set; }

        internal void Settt(bool isOn)
        {
            Showtt = isOn;
            if (isOn)
            {
                Preferences.DecorateNumber = 1;
            }
            else
            {
                Preferences.DecorateNumber = 0;
            }
            SaveAll();
        }

        internal void SetBool(string name, bool isOn)
        {
            switch (name)
            {
                case "UseWeekDay":
                    Preferences.UseWeekDayforForecast = isOn;
                    UseWeekDay = isOn;
                    break;
                case "EnableEveryDay":
                    Preferences.EnableEveryDay = isOn;
                    EnableEveryDay = isOn;
                    break;
                case "EnableAlarm":
                    Preferences.EnableAlarm = isOn;
                    EnableAlarm = isOn;
                    break;
                case "EnableSecond":
                    Preferences.EnableImmersiveSecond = isOn;
                    EnableSecond = isOn;
                    break;
                case "ShowImmersivett":
                    Preferences.ShowImmersivett = isOn;
                    ShowImmersivett = isOn;
                    break;
                case "DisableDynamic":
                    Preferences.DisableDynamic = isOn;
                    DisableDynamic = isOn;
                    break;
                case "EnablePulltoRefresh":
                    Preferences.EnablePulltoRefresh = isOn;
                    EnablePulltoRefresh = isOn;
                    break;
                default:
                    break;
            }
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
            Add(new EnumSelector(TemperatureParameter.Celsius, TemperatureParameter.Celsius.GetDisplayName()));
            Add(new EnumSelector(TemperatureParameter.Fahrenheit, TemperatureParameter.Fahrenheit.GetDisplayName()));
            Add(new EnumSelector(TemperatureParameter.Kelvin, TemperatureParameter.Kelvin.GetDisplayName()));
        }

        public int SelectedIndex { get; internal set; } = 0;
    }

    public class PressureList : List<EnumSelector>
    {
        public PressureList()
        {
            Add(new EnumSelector(PressureParameter.Atm, PressureParameter.Atm.GetDisplayName()));
            Add(new EnumSelector(PressureParameter.CmHg, PressureParameter.CmHg.GetDisplayName()));
            Add(new EnumSelector(PressureParameter.Hpa, PressureParameter.Hpa.GetDisplayName()));
            Add(new EnumSelector(PressureParameter.Torr, PressureParameter.Torr.GetDisplayName()));
        }

        public int SelectedIndex { get; internal set; } = 0;
    }

    public class WindList : List<EnumSelector>
    {
        public WindList()
        {
            Add(new EnumSelector(WindParameter.BeaufortandDegree, WindParameter.BeaufortandDegree.GetDisplayName()));
            Add(new EnumSelector(WindParameter.BeaufortandText, WindParameter.BeaufortandText.GetDisplayName()));
            Add(new EnumSelector(WindParameter.SpeedandDegree, WindParameter.SpeedandDegree.GetDisplayName()));
            Add(new EnumSelector(WindParameter.SpeedandText, WindParameter.SpeedandText.GetDisplayName()));
        }
        public int SelectedIndex { get; internal set; } = 0;
    }

    public class SpeedList : List<EnumSelector>
    {
        public SpeedList()
        {
            Add(new EnumSelector(SpeedParameter.KMPH, SpeedParameter.KMPH.GetDisplayName()));
            Add(new EnumSelector(SpeedParameter.MPS, SpeedParameter.MPS.GetDisplayName()));
            Add(new EnumSelector(SpeedParameter.Knot, SpeedParameter.Knot.GetDisplayName()));
        }
        public int SelectedIndex { get; internal set; } = 0;
    }

    public class LengthList : List<EnumSelector>
    {
        public LengthList()
        {
            Add(new EnumSelector(LengthParameter.KM, LengthParameter.KM.GetDisplayName()));
            Add(new EnumSelector(LengthParameter.M, LengthParameter.M.GetDisplayName()));
            Add(new EnumSelector(LengthParameter.Mile, LengthParameter.Mile.GetDisplayName()));
            Add(new EnumSelector(LengthParameter.NM, LengthParameter.NM.GetDisplayName()));
        }
        public int SelectedIndex { get; internal set; } = 0;
    }

    public class RefreshFreqList : List<EnumSelector>
    {
        public RefreshFreqList()
        {
            Add(new EnumSelector(RefreshState.one, RefreshState.one.GetDisplayName()));
            Add(new EnumSelector(RefreshState.two, RefreshState.two.GetDisplayName()));
            Add(new EnumSelector(RefreshState.three, RefreshState.three.GetDisplayName()));
            Add(new EnumSelector(RefreshState.four, RefreshState.four.GetDisplayName()));
        }
        public int SelectedIndex { get; internal set; } = 0;
    }

    public class ThemeList : List<EnumSelector>
    {
        public ThemeList()
        {
            Add(new EnumSelector(RequestedTheme.Auto, RequestedTheme.Auto.GetDisplayName()));
            Add(new EnumSelector(RequestedTheme.Light, RequestedTheme.Light.GetDisplayName()));
            Add(new EnumSelector(RequestedTheme.Dark, RequestedTheme.Dark.GetDisplayName()));
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
