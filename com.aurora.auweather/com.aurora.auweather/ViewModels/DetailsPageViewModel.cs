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

        private async Task InitialViewModel()
        {
            CurrentTime = DateTime.Now;
            UpdateTime = fetchresult.Location.UpdateTime;
            utcOffset = UpdateTime - fetchresult.Location.UtcTime;
            RefreshCurrentTime();
            todayIndex = Array.FindIndex(fetchresult.DailyForecast, x =>
            {
                return x.Date.Date == CurrentTime.Date;
            });
            nowHourIndex = Array.FindIndex(fetchresult.HourlyForecast, x =>
            {
                return (x.DateTime - CurrentTime).TotalSeconds > 0;
            });
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
            isNight = CalculateIsNight(CurrentTime, sunRise, sunSet);
            this.Glance = Models.Glance.GenerateGlanceDescription(fetchresult, isNight, TempratureandDegreeConverter.Parameter, DateTime.Now);
            CityGlance = (City + "  " + Glance);
            Date = CurrentTime.ToString(settings.Preferences.GetDateFormat());
            var calendar = new CalendarInfo(CurrentTime);
            var loader = new ResourceLoader();
            LunarCalendar = settings.Preferences.UseLunarCalendarPrimary ? (("农历 " + calendar.LunarYearSexagenary + "年" + calendar.LunarMonthText + "月" + calendar.LunarDayText + "    " + calendar.SolarTermStr).Trim()) : string.Empty;
            Hum = loader.GetString("Hum") + ": " + fetchresult.HourlyForecast[nowHourIndex].Humidity + "%";
            Pop = loader.GetString("Pop") + ": " + fetchresult.HourlyForecast[nowHourIndex].Pop + "%";
            Pcpn = loader.GetString("Pcpn") + ": " + fetchresult.NowWeather.Precipitation + "mm";
            var v = new VisibilityConverter();
            Vis = loader.GetString("Vis") + ": " + (fetchresult.NowWeather.Visibility == null ? "N/A" : v.Convert(fetchresult.NowWeather.Visibility, null, null, null));
            var w = new WindSpeedConverter();
            Scale = loader.GetString("Scale") + ": " + (fetchresult.NowWeather.Wind == null ? "N/A" : w.Convert(fetchresult.NowWeather.Wind, null, null, null));
            var d = new WindDirectionConverter();
            Dir = loader.GetString("Dir") + ": " + (fetchresult.NowWeather.Wind == null ? "N/A" : d.Convert(fetchresult.NowWeather.Wind, null, null, null));
            var p = new PressureConverter();
            Pressure = loader.GetString("Pres") + ": " + (fetchresult.NowWeather.Pressure == null ? "N/A" : p.Convert(fetchresult.NowWeather.Pressure, null, null, null));
            CurrentBG = new BitmapImage(await settings.Immersive.GetCurrentBackgroundAsync(fetchresult.NowWeather.Now.Condition, isNight));
            List<double> doubles0 = new List<double>();
            List<double> doubles1 = new List<double>();
            List<double> doubles2 = new List<double>();
            List<double> doubles3 = new List<double>();
            List<double> doubles5 = new List<double>();
            if (!fetchresult.HourlyForecast.IsNullorEmpty())
            {
                for (int i = nowHourIndex + 1; i < fetchresult.HourlyForecast.Length; i++)
                {
                    if (fetchresult.HourlyForecast[i].Temprature != null)
                    {
                        doubles0.Add(fetchresult.HourlyForecast[i].Temprature.ActualDouble(TempratureandDegreeConverter.Parameter));
                    }
                    if (fetchresult.HourlyForecast[i].Pop != default(uint))
                    {
                        doubles1.Add(fetchresult.HourlyForecast[i].Pop);
                    }
                }
                if (!doubles0.IsNullorEmpty() && !doubles1.IsNullorEmpty())
                {
                    Forecasts.Add(new GraphViewModel(doubles0, null, new SolidColorBrush(Pallette.GetRandom()), new SolidColorBrush(Pallette.Cyan), string.Format(loader.GetString("HourlyDetailsTemperature"), doubles0.Count), Temperature.GetFormat(TempratureandDegreeConverter.Parameter)));
                }
                if (doubles1 != null && doubles1.Count > 1)
                {
                    Forecasts.Add(new GraphViewModel(doubles1, null, new SolidColorBrush(Pallette.GetRandom()), new SolidColorBrush(Colors.Transparent), string.Format(loader.GetString("HourlyDetailsPop"), doubles1.Count), "%"));
                }
            }

            doubles0.Clear();
            doubles1.Clear();

            if (!fetchresult.DailyForecast.IsNullorEmpty())
            {
                for (int i = todayIndex + 1; i < fetchresult.DailyForecast.Length; i++)
                {
                    if (fetchresult.DailyForecast[i].HighTemp != null && fetchresult.DailyForecast[i].LowTemp != null)
                    {
                        doubles0.Add(fetchresult.DailyForecast[i].HighTemp.ActualDouble(TempratureandDegreeConverter.Parameter));
                        doubles1.Add(fetchresult.DailyForecast[i].LowTemp.ActualDouble(TempratureandDegreeConverter.Parameter));
                    }
                    if (fetchresult.DailyForecast[i].Pop != default(uint))
                    {
                        doubles2.Add(fetchresult.DailyForecast[i].Pop);
                    }
                    if (fetchresult.DailyForecast[i].Precipitation != default(float))
                    {
                        doubles3.Add(fetchresult.DailyForecast[i].Precipitation);
                    }
                    if (fetchresult.DailyForecast[i].Visibility != null)
                    {
                        doubles5.Add(fetchresult.DailyForecast[i].Visibility.ActualDouble(VisibilityConverter.LengthParameter));
                    }
                }
                if (!doubles0.IsNullorEmpty() && !doubles1.IsNullorEmpty())
                {
                    Forecasts.Add(new GraphViewModel(doubles0, doubles1, new SolidColorBrush(Pallette.Orange), new SolidColorBrush(Pallette.Cyan), string.Format(loader.GetString("DailyDetailsTemp"), doubles0.Count), Temperature.GetFormat(TempratureandDegreeConverter.Parameter)));
                }
                if (doubles2 != null && doubles2.Count > 1)
                {
                    Forecasts.Add(new GraphViewModel(doubles2, null, new SolidColorBrush(Pallette.GetRandom()), new SolidColorBrush(Colors.Transparent), string.Format(loader.GetString("DailyDetailsPop"), doubles2.Count), "%"));
                }
                if (doubles3 != null && doubles3.Count > 1)
                {
                    Forecasts.Add(new GraphViewModel(doubles3, null, new SolidColorBrush(Pallette.GetRandom()), new SolidColorBrush(Colors.Transparent), string.Format(loader.GetString("DailyDetailsPrep"), doubles3.Count), "mm"));
                }
                if (doubles5 != null && doubles5.Count > 1)
                {
                    Forecasts.Add(new GraphViewModel(doubles5, null, new SolidColorBrush(Pallette.GetRandom()), new SolidColorBrush(Colors.Transparent), string.Format(loader.GetString("DailyDetailsVis"), doubles5.Count), Length.GetFormat(VisibilityConverter.LengthParameter)));
                }
            }

            OnFetchDataComplete();
        }

        private void RefreshCurrentTime()
        {
            CurrentTime = DateTimeHelper.ReviseLoc(utcOffset);
        }

        private bool CalculateIsNight(DateTime updateTime, TimeSpan sunRise, TimeSpan sunSet)
        {
            var updateMinutes = updateTime.Hour * 60 + updateTime.Minute;
            if (updateMinutes < sunRise.TotalMinutes)
            {
            }
            else if (updateMinutes >= sunSet.TotalMinutes)
            {
            }
            else
            {
                return false;
            }
            return true;
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
                    resstr = await Core.Models.Request.GetRequest(settings, currentCityModel);
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
                        await settings.Cities.SaveDataAsync(currentCityModel.Id, resstr, settings.Preferences.DataSource);
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
                    var data = await settings.Cities.ReadDataAsync(currentCityModel.Id, settings.Preferences.DataSource);
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
            settings = SettingsModel.Get();
            currentCityModel = settings.Cities.GetCurrentCity();
            InitialConverterParameter(settings);
        }

        private void InitialConverterParameter(SettingsModel settings)
        {
            TempratureandDegreeConverter.ChangeParameter(settings.Preferences.TemperatureParameter);
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
        private string title;
        private DoubleCollection values0;
        private DoubleCollection values1;
        private Brush stroke0;
        private Brush stroke1;
        private string decorate;

        public string Title
        {
            get
            {
                return title;
            }

            set
            {
                SetProperty(ref title, value);
            }
        }

        public DoubleCollection Values0
        {
            get
            {
                return values0;
            }

            set
            {
                SetProperty(ref values0, value);
            }
        }

        public DoubleCollection Values1
        {
            get
            {
                return values1;
            }

            set
            {
                SetProperty(ref values1, value);
            }
        }

        public Brush Stroke0
        {
            get
            {
                return stroke0;
            }

            set
            {
                SetProperty(ref stroke0, value);
            }
        }

        public Brush Stroke1
        {
            get
            {
                return stroke1;
            }

            set
            {
                SetProperty(ref stroke1, value);
            }
        }

        public string Decorate
        {
            get
            {
                return decorate;
            }
            set
            {
                SetProperty(ref decorate, value);
            }
        }

        public GraphViewModel(ICollection<double> values0, ICollection<double> values1, Brush stroke0, Brush stroke1, string title, string decorate)
        {
            if (values0 != null && values0.Count > 0)
            {
                this.Values0 = new DoubleCollection();
                foreach (var v0 in values0)
                {
                    Values0.Add(v0);
                }
            }
            if (values1 != null && values1.Count > 0)
            {
                this.Values1 = new DoubleCollection();
                foreach (var v1 in values1)
                {
                    Values1.Add(v1);
                }
            }
            Stroke0 = stroke0;
            Stroke1 = stroke1;
            Title = title;
            Decorate = decorate;
        }
    }
}
