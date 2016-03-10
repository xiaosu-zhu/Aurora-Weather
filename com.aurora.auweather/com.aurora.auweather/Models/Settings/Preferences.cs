using Com.Aurora.Shared.Helpers;

namespace Com.Aurora.AuWeather.Models.Settings
{
    internal class Preferences
    {
        public TemperatureParameter TemperatureParameter { get; private set; } = TemperatureParameter.Celsius;
        public PressureParameter PressureParameter { get; private set; } = PressureParameter.Atm;
        public LengthParameter LengthParameter { get; private set; } = LengthParameter.KM;
        public SpeedParameter SpeedParameter { get; private set; } = SpeedParameter.KMPH;
        public WindParameter WindParameter { get; private set; } = WindParameter.SpeedandText;

        public bool EnableEveryDay { get; set; } = false;
        public RefreshState RefreshFrequency { get; private set; } = RefreshState.one;
        public RequestedTheme Theme { get; private set; } = RequestedTheme.Auto;

        public bool DisableDynamic { get; set; } = false;
        public bool EnableImmersiveSecond { get; set; } = false;
        public bool UseLunarCalendarPrimary { get; set; } = false;
        public bool EnableAlarm { get; set; } = false;
        public bool UseWeekDayforForecast { get; set; } = false;
        public bool EnablePulltoRefresh { get; set; } = false;

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

        public static Preferences Get()
        {
            Preferences ins;
            var container = RoamingSettingsHelper.GetContainer("Preferences");
            container.ReadGroupSettings(out ins);
            return ins;
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
        public void Set(RefreshState r)
        {
            RefreshFrequency = r;
        }
        public void Set(RequestedTheme r)
        {
            Theme = r;
        }


        public void Save()
        {
            var container = RoamingSettingsHelper.GetContainer("Preferences");
            container.WriteGroupSettings(this);
        }

        internal string GetHourlyFormat()
        {
            return (DecorateFormat[DecorateNumber] + "\n" + HourFormat[HourNumber] + ':' + MinuteFormat[MinuteNumber]).Trim();
        }

        public string GetImmersiveFormat()
        {
            if (EnableImmersiveSecond)
            {
                return (DecorateFormat[ShowImmersivett ? 1 : 0] + "\n" + HourFormat[HourNumber] + ':' + MinuteFormat[MinuteNumber] + ":ss").Trim();
            }
            else
            {
                return (DecorateFormat[ShowImmersivett ? 1 : 0] + "\n" + HourFormat[HourNumber] + ':' + MinuteFormat[MinuteNumber]).Trim();
            }
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

        public string GetDateFormat()
        {
            return (YearFormat[YearNumber] + DateSeparator + MonthFormat[MonthNumber] + DateSeparator + DayFormat[DayNumber] + "  " + WeekFormat[WeekNumber]).Trim();
        }
    }
}
