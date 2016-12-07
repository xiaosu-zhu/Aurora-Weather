using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.AuWeather.ViewModels.Events;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using Com.Aurora.Shared.MVVM;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.Shared.Converters;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using Windows.UI;
using Com.Aurora.Shared;
using Windows.ApplicationModel.Resources;
using Com.Aurora.AuWeather.LunarCalendar;
using System.Text;

namespace Com.Aurora.AuWeather.ViewModels
{
    class DetailsPageViewModel : ViewModelBase
    {
        private BitmapImage currentBG;
        private ElementTheme theme;
        private SettingsModel settings;
        private string storedDatas;
        private DataSource source;
        private HeWeatherModel fetchresult;
        private TimeSpan utcOffset;
        private CitySettingsModel currentCityModel;
        private string city;
        private string hum;
        private string pop;
        private string pcpn;
        private string vis;
        private string scale;
        private string dir;
        private string cityGlance;
        private string lunarCalendar;
        private string date;


        public string City
        {
            get
            {
                return city;
            }
            set
            {
                SetProperty(ref city, value);
            }
        }
        public BitmapImage CurrentBG
        {
            get
            {
                return currentBG;
            }
            set
            {
                SetProperty(ref currentBG, value);
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

        public DateTime CurrentTime
        {
            get
            {
                return currenTime;
            }
            set
            {
                SetProperty(ref currenTime, value);
            }
        }
        public DateTime UpdateTime
        {
            get
            {
                return updateTime;
            }
            set
            {
                SetProperty(ref updateTime, value);
            }
        }

        public string Glance
        {
            get
            {
                return glance;
            }
            set
            {
                SetProperty(ref glance, value);
            }
        }

        public string Hum
        {
            get
            {
                return hum;
            }

            set
            {
                SetProperty(ref hum, value);
            }
        }

        public string Pop
        {
            get
            {
                return pop;
            }

            set
            {
                SetProperty(ref pop, value);
            }
        }

        public string Pcpn
        {
            get
            {
                return pcpn;
            }

            set
            {
                SetProperty(ref pcpn, value);
            }
        }

        public string Vis
        {
            get
            {
                return vis;
            }

            set
            {
                SetProperty(ref vis, value);
            }
        }

        public string Scale
        {
            get
            {
                return scale;
            }

            set
            {
                SetProperty(ref scale, value);
            }
        }

        public string Dir
        {
            get
            {
                return dir;
            }

            set
            {
                SetProperty(ref dir, value);
            }
        }

        public string CityGlance
        {
            get
            {
                return cityGlance;
            }

            set
            {
                SetProperty(ref cityGlance, value);
            }
        }

        public string LunarCalendar
        {
            get
            {
                return lunarCalendar;
            }

            set
            {
                SetProperty(ref lunarCalendar, value);
            }
        }

        public string Date
        {
            get
            {
                return date;
            }

            set
            {
                SetProperty(ref date, value);
            }
        }

        public string Pressure
        {
            get
            {
                return pressure;
            }
            set
            {
                SetProperty(ref pressure, value);
            }
        }

        public string SunRise
        {
            get
            {
                return rise;
            }
            set
            {
                SetProperty(ref rise, value);
            }
        }
        public string SunSet
        {
            get
            {
                return set;
            }
            set
            {
                SetProperty(ref set, value);
            }
        }
        public string Location
        {
            get
            {
                return location;
            }
            set
            {
                SetProperty(ref location, value);
            }
        }
        public string Offset
        {
            get
            {
                return offset;
            }
            set
            {
                SetProperty(ref offset, value);
            }
        }

        public event EventHandler<FetchDataCompleteEventArgs> FetchDataCompleted;
        public event EventHandler<FetchDataFailedEventArgs> FetchDataFailed;


        public ObservableCollection<GraphViewModel> Forecasts = new ObservableCollection<GraphViewModel>();
        private int todayIndex;
        private int nowHourIndex;
        private bool isNight;
        private TimeSpan sunRise;
        private TimeSpan sunSet;
        private DateTime updateTime;
        private DateTime currenTime;
        private string glance;
        private string pressure;
        private string rise;
        private string set;
        private string offset;
        private string location;
        private ThreadPoolTimer currentTimer;

        public DetailsPageViewModel()
        {
            Init();
        }

        private void Init()
        {
            try
            {
                ReadSettings();
                Theme = settings.Preferences.GetTheme();
            }
            catch (ArgumentNullException)
            {
                var taskkkk = ThreadPool.RunAsync(async (work) =>
                {
                    await Task.Delay(1000);
                    OnFetchDataFailed(this, new FetchDataFailedEventArgs("Cities_null"));
                    return;
                });
                return;
            }

            var task = ThreadPool.RunAsync(async (work) =>
            {
                try
                {
                    storedDatas = string.Empty;
                    source = settings.Preferences.DataSource;
                    await FetchDataAsync();
                    if (fetchresult == null || fetchresult.DailyForecast == null || fetchresult.HourlyForecast == null)
                    {
                        await Task.Delay(1000);
                        this.OnFetchDataFailed(this, new FetchDataFailedEventArgs("Service_Unavailable"));
                        return;
                    }
                    var t = ThreadPool.RunAsync(async (w) =>
                    {
                        if (fetchresult == null || fetchresult.DailyForecast == null || fetchresult.HourlyForecast == null)
                        {
                            await Task.Delay(1000);
                            this.OnFetchDataFailed(this, new FetchDataFailedEventArgs("Service_Unavailable"));
                            return;
                        }

                    });

                }
                catch (ArgumentNullException)
                {
                    await Task.Delay(1000);
                    OnFetchDataFailed(this, new FetchDataFailedEventArgs("Cities_null"));
                    return;
                }
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(async () =>
                {
                    if (fetchresult == null)
                    {
                        return;
                    }
                    await InitialViewModel();
                }));
            });
        }

        internal void SetPanelWidth(double v)
        {
            foreach (var item in Forecasts)
            {
                item.PanelWidth = v;
            }
        }

        private async Task InitialViewModel()
        {
            CurrentTime = DateTime.Now;
            UpdateTime = fetchresult.Location.UpdateTime;
            utcOffset = UpdateTime - fetchresult.Location.UtcTime;
            RefreshCurrentTime();
            CurrentTimeRefreshTask();
            todayIndex = Array.FindIndex(fetchresult.DailyForecast, x =>
            {
                return x.Date.Date == CurrentTime.Date;
            });
            nowHourIndex = Array.FindIndex(fetchresult.HourlyForecast, x =>
            {
                return (x.DateTime - CurrentTime).TotalSeconds > 0;
            });
            if (CurrentTime.Hour <= sunRise.Hours)
            {
                todayIndex--;
            }
            if (todayIndex < 0)
            {
                todayIndex = 0;
            }
            if (nowHourIndex < 0)
            {
                nowHourIndex = 0;
            }
            if (fetchresult.DailyForecast[todayIndex].SunRise == default(TimeSpan) || fetchresult.DailyForecast[todayIndex].SunSet == default(TimeSpan))
            {
                sunRise = Core.LunarCalendar.SunRiseSet.GetRise(new Models.Location(currentCityModel.Latitude, currentCityModel.Longitude), CurrentTime);
                sunSet = Core.LunarCalendar.SunRiseSet.GetSet(new Models.Location(currentCityModel.Latitude, currentCityModel.Longitude), CurrentTime);
            }
            else
            {
                sunRise = fetchresult.DailyForecast[todayIndex].SunRise;
                sunSet = fetchresult.DailyForecast[todayIndex].SunSet;
            }
            City = currentCityModel.City;
            isNight = WeatherModel.CalculateIsNight(CurrentTime, sunRise, sunSet);
            this.Glance = Models.Glance.GenerateGlanceDescription(fetchresult, isNight, TemperatureDecoratorConverter.Parameter, DateTime.Now);
            CityGlance = (City + "  " + Glance);
            Date = CurrentTime.ToString(settings.Preferences.GetDateFormat());

            var calendar = new CalendarInfo(CurrentTime);
            var loader = new ResourceLoader();


            LunarCalendar = settings.Preferences.UseLunarCalendarPrimary ? (("农历 " + calendar.LunarYearSexagenary + "年" + calendar.LunarMonthText + "月" + calendar.LunarDayText + "    " + calendar.SolarTermStr).Trim()) : string.Empty;
            Hum = ": " + fetchresult.HourlyForecast[nowHourIndex].Humidity + "%";
            Pop = ": " + fetchresult.HourlyForecast[nowHourIndex].Pop + "%";
            Pcpn = ": " + fetchresult.NowWeather.Precipitation + " mm";
            var v = new VisibilityConverter();
            Vis = ": " + (fetchresult.NowWeather.Visibility == null ? "N/A" : v.Convert(fetchresult.NowWeather.Visibility, null, null, null));
            var w = new WindSpeedConverter();
            Scale = ": " + (fetchresult.NowWeather.Wind == null ? "N/A" : w.Convert(fetchresult.NowWeather.Wind, null, null, null));
            var d = new WindDirectionConverter();
            Dir = ": " + (fetchresult.NowWeather.Wind == null ? "N/A" : d.Convert(fetchresult.NowWeather.Wind, null, null, null));
            var p = new PressureConverter();
            Pressure = ": " + (fetchresult.NowWeather.Pressure == null ? "N/A" : p.Convert(fetchresult.NowWeather.Pressure, null, null, null));

            var t = new TimeSpanConverter();
            SunRise = ": " + (string)t.Convert(sunRise, null, null, null);
            SunSet = ": " + (string)t.Convert(sunSet, null, null, null);
            this.Location = ": " + new Models.Location(currentCityModel.Latitude, currentCityModel.Longitude).ToString();
            var off = utcOffset.Hours;
            Offset = ": UTC" + (off >= 0 ? " +" : " -") + t.Convert(utcOffset, null, null, null);

            var uri = await settings.Immersive.GetCurrentBackgroundAsync(fetchresult.NowWeather.Now.Condition, isNight);
            if (uri != null)
            {
                try
                {
                    CurrentBG = new BitmapImage(uri);
                }
                catch (Exception)
                {
                }
            }
            List<KeyValuePair<int, double>> doubles0 = new List<KeyValuePair<int, double>>();
            List<KeyValuePair<int, double>> doubles1 = new List<KeyValuePair<int, double>>();
            List<KeyValuePair<int, double>> doubles2 = new List<KeyValuePair<int, double>>();
            List<KeyValuePair<int, double>> doubles3 = new List<KeyValuePair<int, double>>();
            List<KeyValuePair<int, double>> doubles5 = new List<KeyValuePair<int, double>>();
            List<KeyValuePair<int, double>> doubles4 = new List<KeyValuePair<int, double>>();
            if (!fetchresult.HourlyForecast.IsNullorEmpty())
            {
                for (int i = nowHourIndex + 1; i < fetchresult.HourlyForecast.Length; i++)
                {
                    if (fetchresult.HourlyForecast[i].Temprature != null)
                    {
                        doubles0.Add(new KeyValuePair<int, double>(i, fetchresult.HourlyForecast[i].Temprature.ActualDouble(TemperatureDecoratorConverter.Parameter)));
                    }
                    if (fetchresult.HourlyForecast[i].Pop != default(uint))
                    {
                        doubles1.Add(new KeyValuePair<int, double>(i, fetchresult.HourlyForecast[i].Pop));
                    }
                    if (fetchresult.HourlyForecast[i].Wind != null)
                    {
                        doubles4.Add(new KeyValuePair<int, double>(i, fetchresult.HourlyForecast[i].Wind.Speed.ActualDouble(WindSpeedConverter.SpeedParameter)));
                    }
                }
                var sb = new StringBuilder();
                if (doubles0 != null && doubles0.Count > 1)
                {
                    GetHourlyXText(doubles0, sb);
                    Forecasts.Add(new GraphViewModel(doubles0, null, new SolidColorBrush(Palette.GetRandom()), new SolidColorBrush(Palette.Cyan), string.Format(loader.GetString("HourlyDetailsTemperature"), doubles0.Count), Temperature.GetFormat(TemperatureDecoratorConverter.Parameter), -280, 9999, sb.ToString()));
                }
                if (doubles1 != null && doubles1.Count > 1)
                {
                    GetHourlyXText(doubles1, sb);
                    Forecasts.Add(new GraphViewModel(doubles1, null, new SolidColorBrush(Palette.GetRandom()), new SolidColorBrush(Colors.Transparent), string.Format(loader.GetString("HourlyDetailsPop"), doubles1.Count), "%", 0, 100, sb.ToString()));
                }
                if (doubles4 != null && doubles4.Count > 1)
                {
                    GetHourlyXText(doubles4, sb);
                    Forecasts.Add(new GraphViewModel(doubles4, null, new SolidColorBrush(Palette.GetRandom()), new SolidColorBrush(Colors.Transparent), string.Format(loader.GetString("HourlyDetailsWind"), doubles4.Count), Wind.GetSpeedFormat(WindSpeedConverter.SpeedParameter), 0, 1000, sb.ToString()));
                }
            }

            doubles0.Clear();
            doubles1.Clear();
            doubles2.Clear();
            doubles3.Clear();
            doubles4.Clear();
            doubles5.Clear();

            if (!fetchresult.DailyForecast.IsNullorEmpty())
            {
                for (int i = todayIndex + 1; i < fetchresult.DailyForecast.Length; i++)
                {
                    if (fetchresult.DailyForecast[i].HighTemp != null && fetchresult.DailyForecast[i].LowTemp != null)
                    {
                        doubles0.Add(new KeyValuePair<int, double>(i, fetchresult.DailyForecast[i].HighTemp.ActualDouble(TemperatureDecoratorConverter.Parameter)));
                        doubles1.Add(new KeyValuePair<int, double>(i, fetchresult.DailyForecast[i].LowTemp.ActualDouble(TemperatureDecoratorConverter.Parameter)));
                    }
                    if (fetchresult.DailyForecast[i].Pop != default(uint))
                    {
                        doubles2.Add(new KeyValuePair<int, double>(i, fetchresult.DailyForecast[i].Pop));
                    }
                    if (fetchresult.DailyForecast[i].Precipitation != default(float))
                    {
                        doubles3.Add(new KeyValuePair<int, double>(i, fetchresult.DailyForecast[i].Precipitation));
                    }
                    if (fetchresult.DailyForecast[i].Visibility != null)
                    {
                        doubles5.Add(new KeyValuePair<int, double>(i, fetchresult.DailyForecast[i].Visibility.ActualDouble(VisibilityConverter.LengthParameter)));
                    }
                    if (fetchresult.DailyForecast[i].Wind != null)
                    {
                        doubles4.Add(new KeyValuePair<int, double>(i, fetchresult.DailyForecast[i].Wind.Speed.ActualDouble(WindSpeedConverter.SpeedParameter)));
                    }
                }
                var sb = new StringBuilder();
                if (!doubles0.IsNullorEmpty() && !doubles1.IsNullorEmpty())
                {
                    GetDailyXText(doubles0, sb);
                    Forecasts.Add(new GraphViewModel(doubles0, doubles1, new SolidColorBrush(Palette.Orange), new SolidColorBrush(Palette.Cyan), string.Format(loader.GetString("DailyDetailsTemp"), doubles0.Count), Temperature.GetFormat(TemperatureDecoratorConverter.Parameter), -280, 9999, sb.ToString()));
                }
                if (doubles2 != null && doubles2.Count > 1)
                {
                    GetDailyXText(doubles2, sb);
                    Forecasts.Add(new GraphViewModel(doubles2, null, new SolidColorBrush(Palette.GetRandom()), new SolidColorBrush(Colors.Transparent), string.Format(loader.GetString("DailyDetailsPop"), doubles2.Count), "%", 0, 100, sb.ToString()));
                }
                if (doubles3 != null && doubles3.Count > 1)
                {
                    GetDailyXText(doubles3, sb);
                    Forecasts.Add(new GraphViewModel(doubles3, null, new SolidColorBrush(Palette.GetRandom()), new SolidColorBrush(Colors.Transparent), string.Format(loader.GetString("DailyDetailsPrep"), doubles3.Count), "mm", 0, 100, sb.ToString()));
                }
                if (doubles5 != null && doubles5.Count > 1)
                {
                    GetDailyXText(doubles5, sb);
                    Forecasts.Add(new GraphViewModel(doubles5, null, new SolidColorBrush(Palette.GetRandom()), new SolidColorBrush(Colors.Transparent), string.Format(loader.GetString("DailyDetailsVis"), doubles5.Count), Length.GetFormat(VisibilityConverter.LengthParameter), 0, 1000, sb.ToString()));
                }
                if (doubles4 != null && doubles4.Count > 1)
                {
                    GetDailyXText(doubles4, sb);
                    Forecasts.Add(new GraphViewModel(doubles4, null, new SolidColorBrush(Palette.GetRandom()), new SolidColorBrush(Colors.Transparent), string.Format(loader.GetString("DailyDetailsWind"), doubles4.Count), Wind.GetSpeedFormat(WindSpeedConverter.SpeedParameter), 0, 1000, sb.ToString()));
                }
            }

            OnFetchDataComplete();
        }

        private void GetHourlyXText(List<KeyValuePair<int, double>> list, StringBuilder sb)
        {
            sb.Clear();
            if (list.Count == 2)
            {
                sb.Append(' ');
                sb.Append(',');
                sb.Append(fetchresult.HourlyForecast[list[0].Key].DateTime.ToString("H:mm"));
                sb.Append(',');
                sb.Append(',');
                sb.Append(fetchresult.HourlyForecast[list[1].Key].DateTime.ToString("H:mm"));
                sb.Append(',');
                sb.Append(' ');
            }
            else if (list.Count == 3)
            {
                sb.Append(' ');
                sb.Append(',');
                sb.Append(fetchresult.HourlyForecast[list[0].Key].DateTime.ToString("H:mm"));
                sb.Append(',');
                sb.Append(fetchresult.HourlyForecast[list[1].Key].DateTime.ToString("H:mm"));
                sb.Append(',');
                sb.Append(fetchresult.HourlyForecast[list[2].Key].DateTime.ToString("H:mm"));
                sb.Append(',');
                sb.Append(' ');
            }
            else if (list.Count == 4)
            {
                var step = fetchresult.HourlyForecast[list[2].Key].DateTime - fetchresult.HourlyForecast[list[1].Key].DateTime;
                step = TimeSpan.FromSeconds(step.TotalSeconds / 2);
                sb.Append((fetchresult.HourlyForecast[list[0].Key].DateTime - step).ToString("H:mm"));
                sb.Append(',');
                sb.Append((fetchresult.HourlyForecast[list[1].Key].DateTime - step).ToString("H:mm"));
                sb.Append(',');
                sb.Append((fetchresult.HourlyForecast[list[2].Key].DateTime - step).ToString("H:mm"));
                sb.Append(',');
                sb.Append((fetchresult.HourlyForecast[list[3].Key].DateTime - step).ToString("H:mm"));
                sb.Append(',');
                sb.Append((fetchresult.HourlyForecast[list[3].Key].DateTime + step).ToString("H:mm"));
            }
            else
            {
                var st = list.Count / 10;
                sb.Append(fetchresult.HourlyForecast[list[st].Key].DateTime.ToString("H:mm"));
                sb.Append(',');
                sb.Append(fetchresult.HourlyForecast[list[st * 3].Key].DateTime.ToString("H:mm"));
                sb.Append(',');
                sb.Append(fetchresult.HourlyForecast[list[st * 4].Key].DateTime.ToString("H:mm"));
                sb.Append(',');
                sb.Append(fetchresult.HourlyForecast[list[st * 7].Key].DateTime.ToString("H:mm"));
                sb.Append(',');
                sb.Append(fetchresult.HourlyForecast[list[st * 9].Key].DateTime.ToString("H:mm"));
            }
        }

        private void GetDailyXText(List<KeyValuePair<int, double>> list, StringBuilder sb)
        {
            sb.Clear();
            if (list.Count == 2)
            {
                sb.Append(' ');
                sb.Append(',');
                sb.Append(fetchresult.DailyForecast[list[0].Key].Date.ToString("M/dd"));
                sb.Append(',');
                sb.Append(',');
                sb.Append(fetchresult.DailyForecast[list[1].Key].Date.ToString("M/dd"));
                sb.Append(',');
                sb.Append(' ');
            }
            else if (list.Count == 3)
            {
                sb.Append(' ');
                sb.Append(',');
                sb.Append(fetchresult.DailyForecast[list[0].Key].Date.ToString("M/dd"));
                sb.Append(',');
                sb.Append(fetchresult.DailyForecast[list[1].Key].Date.ToString("M/dd"));
                sb.Append(',');
                sb.Append(fetchresult.DailyForecast[list[2].Key].Date.ToString("M/dd"));
                sb.Append(',');
                sb.Append(' ');
            }
            else if (list.Count == 4)
            {
                var step = fetchresult.DailyForecast[list[2].Key].Date - fetchresult.DailyForecast[list[1].Key].Date;
                step = TimeSpan.FromSeconds(step.TotalSeconds / 2);
                sb.Append((fetchresult.DailyForecast[list[0].Key].Date - step).ToString("M/dd"));
                sb.Append(',');
                sb.Append((fetchresult.DailyForecast[list[1].Key].Date - step).ToString("M/dd"));
                sb.Append(',');
                sb.Append((fetchresult.DailyForecast[list[2].Key].Date - step).ToString("M/dd"));
                sb.Append(',');
                sb.Append((fetchresult.DailyForecast[list[3].Key].Date - step).ToString("M/dd"));
                sb.Append(',');
                sb.Append((fetchresult.DailyForecast[list[3].Key].Date + step).ToString("M/dd"));
            }
            else
            {
                var st = list.Count / 10d;
                sb.Append(fetchresult.DailyForecast[list[(int)Math.Floor(st)].Key].Date.ToString("M/dd"));
                sb.Append(',');
                sb.Append(fetchresult.DailyForecast[list[(int)Math.Floor(st * 3)].Key].Date.ToString("M/dd"));
                sb.Append(',');
                sb.Append(fetchresult.DailyForecast[list[(int)Math.Floor(st * 4)].Key].Date.ToString("M/dd"));
                sb.Append(',');
                sb.Append(fetchresult.DailyForecast[list[(int)Math.Floor(st * 7)].Key].Date.ToString("M/dd"));
                sb.Append(',');
                sb.Append(fetchresult.DailyForecast[list[(int)Math.Floor(st * 9)].Key].Date.ToString("M/dd"));
            }
        }

        private void RefreshCurrentTime()
        {
            CurrentTime = DateTimeHelper.ReviseLoc(utcOffset);
        }

        public void CurrentTimeRefreshTask()
        {
            if (currentTimer != null)
            {
                currentTimer.Cancel();
                currentTimer = null;
            }
            var nextupdate = 1.02 - DateTime.Now.Millisecond / 1000d;
            currentTimer = ThreadPoolTimer.CreateTimer(
                async (task) =>
                {
                    var locTime = DateTimeHelper.ReviseLoc(utcOffset);

                    await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
                    {
                        CurrentTime = locTime;
                    }));
                }, TimeSpan.FromSeconds(nextupdate),
            (compelte) =>
            {
                CurrentTimeRefreshTask();
            });
        }



        private async Task FetchDataAsync()
        {
            await SearchExistingDataAsync();
            string resstr;
            if (currentCityModel.Id != null)
            {
                try
                {
                    if (!storedDatas.IsNullorEmpty())
                    {
                        resstr = storedDatas;
                        fetchresult = HeWeatherModel.Generate(resstr, settings.Preferences.DataSource);
                        return;
                    }
                    resstr = await Core.Models.Request.GetRequestAsync(settings, currentCityModel);
                    if (resstr.IsNullorEmpty())
                    {
                        await Task.Delay(1000);
                        this.OnFetchDataFailed(this, new FetchDataFailedEventArgs("Network_Error"));
                        return;
                    }
                    fetchresult = HeWeatherModel.Generate(resstr, settings.Preferences.DataSource);
                    if (fetchresult.Status != HeWeatherStatus.ok)
                    {
                        await Task.Delay(1000);
                        this.OnFetchDataFailed(this, new FetchDataFailedEventArgs("Service_Unavailable"));
                        return;
                    }
                    var task = ThreadPool.RunAsync(async (work) =>
                    {
                        await settings.Cities.SaveDataAsync(currentCityModel.Id.IsNullorEmpty() ? currentCityModel.City : currentCityModel.Id, resstr, settings.Preferences.DataSource);
                        currentCityModel.Update();
                        if (settings.Cities.CurrentIndex != -1)
                        {
                            settings.Cities.SavedCities[settings.Cities.CurrentIndex] = currentCityModel;
                        }
                        else
                        {
                            settings.Cities.LocatedCity = currentCityModel;
                        }
                        settings.Cities.Save();
                    });
                    return;
                }
                catch (Exception)
                {
                    await Task.Delay(1000);
                    this.OnFetchDataFailed(this, new FetchDataFailedEventArgs("Service_Unavailable"));
                    return;
                }

            }
            else throw new NullReferenceException();
        }

        private async Task SearchExistingDataAsync()
        {
            var currentTime = DateTime.Now;
            if ((currentTime - currentCityModel.LastUpdate).TotalMinutes < 30)
            {
                try
                {
                    var data = await settings.Cities.ReadDataAsync(currentCityModel.Id.IsNullorEmpty() ? currentCityModel.City : currentCityModel.Id, settings.Preferences.DataSource);
                    if (data != null)
                        storedDatas = data;
                }
                catch (Exception)
                {

                }
            }
        }

        private void ReadSettings()
        {
            settings = SettingsModel.Current;
            currentCityModel = settings.Cities.GetCurrentCity();
            InitialConverterParameter(settings);
        }

        private void InitialConverterParameter(SettingsModel settings)
        {
            TemperatureDecoratorConverter.ChangeParameter(settings.Preferences.TemperatureParameter);
            DateTimeConverter.ChangeParameter(settings.Preferences.GetForecastFormat());
            var p = settings.Preferences.GetHourlyFormat();
            HourMinuteConverter.ChangeParameter(p);
            UpdateTimeConverter.ChangeParameter(p);
            RefreshCompleteConverter.ChangeParameter(p);
            WindSpeedConverter.ChangeParameter(settings.Preferences.WindParameter, settings.Preferences.SpeedParameter);
            PressureConverter.ChangeParameter(settings.Preferences.PressureParameter);
            VisibilityConverter.ChangeParameter(settings.Preferences.LengthParameter);
            ImmersiveHourConverter.ChangeParameter(settings.Preferences.GetImmersiveHourFormat());
            ImmersiveMinConverter.ChangeParameter(settings.Preferences.GetImmersiveMinFormat());
        }

        private void OnFetchDataComplete()
        {
            this.FetchDataCompleted?.Invoke(this, new FetchDataCompleteEventArgs());
        }
        private void OnFetchDataFailed(object sender, FetchDataFailedEventArgs e)
        {
            this.FetchDataFailed?.Invoke(sender, e);
        }
    }

    class GraphViewModel : ViewModelBase
    {

        public string Title { get; private set; }

        public DoubleCollection Values0 { get; private set; }

        public DoubleCollection Values1 { get; private set; }

        public Brush Stroke0 { get; private set; }

        public Brush Stroke1 { get; private set; }

        public string Decorate { get; private set; }

        public double Minimum { get; private set; }
        public double Maximum { get; private set; }

        public string XText { get; private set; }


        private double panelWidth = 480d;

        public double PanelWidth
        {
            get { return panelWidth; }
            set { SetProperty(ref panelWidth, value); }
        }

        public GraphViewModel(ICollection<KeyValuePair<int, double>> values0, ICollection<KeyValuePair<int, double>> values1, Brush stroke0, Brush stroke1, string title, string decorate, double min, double max, string xtext)
        {
            if (values0 != null && values0.Count > 0)
            {
                this.Values0 = new DoubleCollection();
                foreach (var v0 in values0)
                {
                    Values0.Add(v0.Value);
                }
            }
            if (values1 != null && values1.Count > 0)
            {
                this.Values1 = new DoubleCollection();
                foreach (var v1 in values1)
                {
                    Values1.Add(v1.Value);
                }
            }
            Stroke0 = stroke0;
            Stroke1 = stroke1;
            Title = title;
            Decorate = decorate;
            Minimum = min;
            Maximum = max;
            XText = xtext;
        }
    }
}
