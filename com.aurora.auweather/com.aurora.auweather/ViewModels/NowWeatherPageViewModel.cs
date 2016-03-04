using Com.Aurora.AuWeather.LunarCalendar;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.AuWeather.Models.HeWeather.JsonContract;
using Com.Aurora.AuWeather.ViewModels.Events;
using Com.Aurora.Shared.Converters;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using Com.Aurora.Shared.MVVM;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.System.Threading;
using Windows.UI.Core;

namespace Com.Aurora.AuWeather.ViewModels
{
    public class NowWeatherPageViewModel : ViewModelBase
    {
        #region private members
        private Temprature temprature;
        private Temprature bodyTemprature;
        private Temprature nowH;
        private Temprature nowL;
        private Wind wind;
        private DateTime currentTime;
        private WeatherCondition condition;
        private string currentCity;
        private string currentId;
        private HeWeatherModel fetchresult;
        private uint humidity;
        private float precipitation;
        private uint proportion;
        private double sunProgress;
        private double moonPhase;
        private Pressure pressure;
        private Length visibility;
        private AQI aqi;
        private Suggestion comf;
        private Suggestion cw;
        private Suggestion drsg;
        private Suggestion flu;
        private Suggestion sport;
        private Suggestion trav;
        private Suggestion uv;

        private float tempraturePath0;
        private float tempraturePath1;
        private float tempraturePath2;
        private float tempraturePath3;
        private float tempraturePath4;
        private float tempraturePath5;

        private Temprature hourlyTemp0;
        private Temprature hourlyTemp1;
        private Temprature hourlyTemp2;
        private Temprature hourlyTemp3;
        private Temprature hourlyTemp4;
        private Temprature hourlyTemp5;

        private DateTime hour0;
        private DateTime hour1;
        private DateTime hour2;
        private DateTime hour3;
        private DateTime hour4;
        private DateTime hour5;

        private DateTime forecastDate1;
        private DateTime forecastDate2;
        private DateTime forecastDate3;
        private DateTime forecastDate4;
        private string forecastDateConverterParameter;

        private float pop0;
        private float pop1;
        private float pop2;
        private float pop3;
        private float pop4;
        private float pop5;

        private WeatherCondition forecast0;
        private WeatherCondition forecast1;
        private WeatherCondition forecast2;
        private WeatherCondition forecast3;
        private WeatherCondition forecast4;
        private Temprature forecast0H;
        private Temprature forecast0L;
        private Temprature forecast1H;
        private Temprature forecast1L;
        private Temprature forecast2H;
        private Temprature forecast2L;
        private Temprature forecast3H;
        private Temprature forecast3L;
        private Temprature forecast4H;
        private Temprature forecast4L;

        private DateTime updateTime;

        private TimeSpan sunRise;
        private TimeSpan sunSet;

        private bool isNight;
        private bool isSummer;
        private CitySettingsModel[] citys;
        private List<KeyValuePair<string, string>> storedDatas;
        private SettingsModel settings;
        private CitySettingsModel currentCityModel;
        private CalendarInfo calendar;
        private ThreadPoolTimer currentTimer;
        private TimeZoneInfo currentTimeZone;
        private string glance;
        #endregion
        #region public binded properties
        public Temprature Temprature
        {
            get
            {
                return temprature;
            }

            set
            {
                SetProperty(ref temprature, value);
            }
        }

        public DateTime CurrentTime
        {
            get
            {
                return currentTime;
            }

            set
            {
                SetProperty(ref currentTime, value);
            }
        }

        public Wind Wind
        {
            get
            {
                return wind;
            }

            set
            {
                SetProperty(ref wind, value);
            }
        }

        public WeatherCondition Condition
        {
            get
            {
                return condition;
            }

            set
            {
                SetProperty(ref condition, value);
            }
        }

        public string City
        {
            get
            {
                return currentCity;
            }

            set
            {
                SetProperty(ref currentCity, value);
            }
        }

        public float TempraturePath0
        {
            get
            {
                return tempraturePath0;
            }

            set
            {
                SetProperty(ref tempraturePath0, value);
            }
        }

        public float TempraturePath1
        {
            get
            {
                return tempraturePath1;
            }

            set
            {
                SetProperty(ref tempraturePath1, value);
            }
        }

        public float TempraturePath2
        {
            get
            {
                return tempraturePath2;
            }

            set
            {
                SetProperty(ref tempraturePath2, value);
            }
        }

        public float TempraturePath3
        {
            get
            {
                return tempraturePath3;
            }

            set
            {
                SetProperty(ref tempraturePath3, value);
            }
        }

        public float TempraturePath4
        {
            get
            {
                return tempraturePath4;
            }

            set
            {
                SetProperty(ref tempraturePath4, value);
            }
        }

        public float TempraturePath5
        {
            get
            {
                return tempraturePath5;
            }

            set
            {
                SetProperty(ref tempraturePath5, value);
            }
        }

        public Temprature HourlyTemp0
        {
            get
            {
                return hourlyTemp0;
            }

            set
            {
                SetProperty(ref hourlyTemp0, value);
            }
        }

        public Temprature HourlyTemp1
        {
            get
            {
                return hourlyTemp1;
            }

            set
            {
                SetProperty(ref hourlyTemp1, value);
            }
        }

        public Temprature HourlyTemp2
        {
            get
            {
                return hourlyTemp2;
            }

            set
            {
                SetProperty(ref hourlyTemp2, value);
            }
        }

        public Temprature HourlyTemp3
        {
            get
            {
                return hourlyTemp3;
            }

            set
            {
                SetProperty(ref hourlyTemp3, value);
            }
        }

        public Temprature HourlyTemp4
        {
            get
            {
                return hourlyTemp4;
            }

            set
            {
                SetProperty(ref hourlyTemp4, value);
            }
        }

        public Temprature HourlyTemp5
        {
            get
            {
                return hourlyTemp5;
            }

            set
            {
                SetProperty(ref hourlyTemp5, value);
            }
        }

        public DateTime Hour0
        {
            get
            {
                return hour0;
            }

            set
            {
                SetProperty(ref hour0, value);
            }
        }

        public DateTime Hour1
        {
            get
            {
                return hour1;
            }

            set
            {
                SetProperty(ref hour1, value);
            }
        }

        public DateTime Hour2
        {
            get
            {
                return hour2;
            }

            set
            {
                SetProperty(ref hour2, value);
            }
        }

        public DateTime Hour3
        {
            get
            {
                return hour3;
            }

            set
            {
                SetProperty(ref hour3, value);
            }
        }

        public DateTime Hour4
        {
            get
            {
                return hour4;
            }

            set
            {
                SetProperty(ref hour4, value);
            }
        }

        public DateTime Hour5
        {
            get
            {
                return hour5;
            }

            set
            {
                SetProperty(ref hour5, value);
            }
        }

        public float Pop0
        {
            get
            {
                return pop0;
            }

            set
            {
                SetProperty(ref pop0, value);
            }
        }

        public float Pop1
        {
            get
            {
                return pop1;
            }

            set
            {
                SetProperty(ref pop1, value);
            }
        }

        public float Pop2
        {
            get
            {
                return pop2;
            }

            set
            {
                SetProperty(ref pop2, value);
            }
        }

        public float Pop3
        {
            get
            {
                return pop3;
            }

            set
            {
                SetProperty(ref pop3, value);
            }
        }

        public float Pop4
        {
            get
            {
                return pop4;
            }

            set
            {
                SetProperty(ref pop4, value);
            }
        }

        public float Pop5
        {
            get
            {
                return pop5;
            }

            set
            {
                SetProperty(ref pop5, value);
            }
        }

        public WeatherCondition Forecast0
        {
            get
            {
                return forecast0;
            }

            set
            {
                SetProperty(ref forecast0, value);
            }
        }

        public WeatherCondition Forecast1
        {
            get
            {
                return forecast1;
            }

            set
            {
                SetProperty(ref forecast1, value);
            }
        }

        public WeatherCondition Forecast2
        {
            get
            {
                return forecast2;
            }

            set
            {
                SetProperty(ref forecast2, value);
            }
        }

        public WeatherCondition Forecast3
        {
            get
            {
                return forecast3;
            }

            set
            {
                SetProperty(ref forecast3, value);
            }
        }

        public WeatherCondition Forecast4
        {
            get
            {
                return forecast4;
            }

            set
            {
                SetProperty(ref forecast4, value);
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

        public TimeSpan SunRise
        {
            get
            {
                return sunRise;
            }

            set
            {
                SetProperty(ref sunRise, value);
            }
        }

        public TimeSpan SunSet
        {
            get
            {
                return sunSet;
            }

            set
            {
                SetProperty(ref sunSet, value);
            }
        }

        public bool IsNight
        {
            get
            {
                return isNight;
            }

            set
            {
                SetProperty(ref isNight, value);
            }
        }

        public bool IsSummer
        {
            get
            {
                return isSummer;
            }

            set
            {
                SetProperty(ref isSummer, value);
            }
        }

        public DateTime ForecastDate1
        {
            get
            {
                return forecastDate1;
            }

            set
            {
                SetProperty(ref forecastDate1, value);
            }
        }

        public DateTime ForecastDate2
        {
            get
            {
                return forecastDate2;
            }

            set
            {
                SetProperty(ref forecastDate2, value);
            }
        }

        public DateTime ForecastDate3
        {
            get
            {
                return forecastDate3;
            }

            set
            {
                SetProperty(ref forecastDate3, value);
            }
        }

        public DateTime ForecastDate4
        {
            get
            {
                return forecastDate4;
            }

            set
            {
                SetProperty(ref forecastDate4, value);
            }
        }

        public string ForecastDateConverterParameter
        {
            get
            {
                return forecastDateConverterParameter;
            }

            set
            {
                SetProperty(ref forecastDateConverterParameter, value);
            }
        }

        public Temprature Forecast0H
        {
            get
            {
                return forecast0H;
            }

            set
            {
                SetProperty(ref forecast0H, value);
            }
        }

        public Temprature Forecast0L
        {
            get
            {
                return forecast0L;
            }

            set
            {
                SetProperty(ref forecast0L, value);
            }
        }

        public Temprature Forecast1H
        {
            get
            {
                return forecast1H;
            }

            set
            {
                SetProperty(ref forecast1H, value);
            }
        }

        public Temprature Forecast1L
        {
            get
            {
                return forecast1L;
            }

            set
            {
                SetProperty(ref forecast1L, value);
            }
        }

        public Temprature Forecast2H
        {
            get
            {
                return forecast2H;
            }

            set
            {
                SetProperty(ref forecast2H, value);
            }
        }

        public Temprature Forecast2L
        {
            get
            {
                return forecast2L;
            }

            set
            {
                SetProperty(ref forecast2L, value);
            }
        }

        public Temprature Forecast3H
        {
            get
            {
                return forecast3H;
            }

            set
            {
                SetProperty(ref forecast3H, value);
            }
        }

        public Temprature Forecast3L
        {
            get
            {
                return forecast3L;
            }

            set
            {
                SetProperty(ref forecast3L, value);
            }
        }

        public Temprature Forecast4H
        {
            get
            {
                return forecast4H;
            }

            set
            {
                SetProperty(ref forecast4H, value);
            }
        }

        public Temprature Forecast4L
        {
            get
            {
                return forecast4L;
            }

            set
            {
                SetProperty(ref forecast4L, value);
            }
        }

        public Temprature BodyTemprature
        {
            get
            {
                return bodyTemprature;
            }

            set
            {
                SetProperty(ref bodyTemprature, value);
            }
        }

        public Temprature NowH
        {
            get
            {
                return nowH;
            }

            set
            {
                SetProperty(ref nowH, value);
            }
        }

        public Temprature NowL
        {
            get
            {
                return nowL;
            }

            set
            {
                SetProperty(ref nowL, value);
            }
        }

        public uint Humidity
        {
            get
            {
                return humidity;
            }

            set
            {
                SetProperty(ref humidity, value);
            }
        }

        public float Precipitation
        {
            get
            {
                return precipitation;
            }

            set
            {
                SetProperty(ref precipitation, value);
            }
        }

        public uint Proportion
        {
            get
            {
                return proportion;
            }

            set
            {
                SetProperty(ref proportion, value);
            }
        }

        public double SunProgress
        {
            get
            {
                return sunProgress;
            }

            set
            {
                SetProperty(ref sunProgress, value);
            }
        }

        public double MoonPhase
        {
            get
            {
                return moonPhase;
            }

            set
            {
                SetProperty(ref moonPhase, value);
            }
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

        public Pressure Pressure
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

        public Length Visibility
        {
            get
            {
                return visibility;
            }

            set
            {
                SetProperty(ref visibility, value);
            }
        }

        public AQI Aqi
        {
            get
            {
                return aqi;
            }

            set
            {
                SetProperty(ref aqi, value);
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
        public Suggestion Comf
        {
            get
            {
                return comf;
            }

            set
            {
                SetProperty(ref comf, value);
            }
        }

        public Suggestion Cw
        {
            get
            {
                return cw;
            }

            set
            {
                SetProperty(ref cw, value);
            }
        }

        public Suggestion Drsg
        {
            get
            {
                return drsg;
            }

            set
            {
                SetProperty(ref drsg, value);
            }
        }

        public Suggestion Flu
        {
            get
            {
                return flu;
            }

            set
            {
                SetProperty(ref flu, value);
            }
        }

        public Suggestion Sport
        {
            get
            {
                return sport;
            }

            set
            {
                SetProperty(ref sport, value);
            }
        }

        public Suggestion Trav
        {
            get
            {
                return trav;
            }

            set
            {
                SetProperty(ref trav, value);
            }
        }

        public Suggestion Uv
        {
            get
            {
                return uv;
            }

            set
            {
                SetProperty(ref uv, value);
            }
        }
        #endregion
        #region events
        public event EventHandler<FetchDataCompleteEventArgs> FetchDataComplete;
        public event EventHandler<ParameterChangedEventArgs> ParameterChanged;
        public event EventHandler<FetchDataFailedEventArgs> FetchDataFailed;
        #endregion

        public DelegateCommand RefreshCommand
        {
            get
            {
                return new DelegateCommand(async () =>
                {
                    await RefreshAsync();
                });
            }
        }



        public async Task RefreshAsync()
        {
            try
            {
                storedDatas.Clear();
                await FetchDataAsync();
                CalcCalendar();
            }
            catch (ArgumentNullException)
            {
                this.OnFetchDataFailed(this, new FetchDataFailedEventArgs("未设置城市"));
                return;
            }
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
            {
                InitialViewModel();
            }));
        }

        public NowWeatherPageViewModel()
        {
            var task = ThreadPool.RunAsync(async (work) =>
            {
                try
                {
                    ReadSettings();
                    await FetchDataAsync();
                    CalcCalendar();
                    GenerateGlance();
                }
                catch (ArgumentNullException)
                {
                    this.OnFetchDataFailed(this, new FetchDataFailedEventArgs("未设置城市"));
                    return;
                }
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
                  {
                      InitialViewModel();
                  }));
            });
        }

        private void GenerateGlance()
        {
            var time = fetchresult.Location.UpdateTime;
            var updateMinutes = time.Hour * 60 + time.Minute;
            glance = Models.Glance.GenerateGlanceDescription(fetchresult,
                 updateMinutes >= fetchresult.DailyForecast[0].SunSet.TotalMinutes - 30,
                 TempratureParameter.Celsius);
        }

        private void CalcCalendar()
        {
            calendar = new CalendarInfo(fetchresult.Location.UpdateTime);
            moonPhase = Calendar.LunarDay / 30d;
        }

        private void OnFetchDataFailed(object sender, FetchDataFailedEventArgs e)
        {
            var h = this.FetchDataFailed;
            if (h != null)
            {
                h(sender, e);
            }
        }

        private async Task FetchDataAsync()
        {
            await SearchExistingDataAsync();
            string resstr;
            if (currentId != null)
            {
                if (!storedDatas.IsNullorEmpty())
                {
                    var lastFetchedData = storedDatas.Find(x =>
                     {
                         return x.Key == currentId;
                     });
                    if (!lastFetchedData.Equals(default(KeyValuePair<string, string>)))
                    {
                        resstr = lastFetchedData.Value;
                        var resjson = HeWeatherContract.Generate(resstr);
                        fetchresult = new HeWeatherModel(resjson);
                        return;
                    }
                }
#if DEBUG
                await Task.Delay(5000);
                resstr = await FileIOHelper.ReadStringFromAssetsAsync("testdata");
#else
                var keys = (await FileIOHelper.ReadStringFromAssets("Key")).Split(new string[] { ":|:" }, StringSplitOptions.RemoveEmptyEntries);
                var param = new string[] { "cityid=" + currentId };
                resstr = await BaiduRequestHelper.RequestWithKey("http://apis.baidu.com/heweather/pro/weather", param, keys[0]);    
#endif
                var resjson1 = HeWeatherContract.Generate(resstr);
                fetchresult = new HeWeatherModel(resjson1);
                var task = ThreadPool.RunAsync(async (work) =>
                {
                    await settings.SaveDataAsync(currentId, resstr);
                });
                currentCityModel.Update();
                citys[settings.CurrentCityIndex] = currentCityModel;
                settings.UpdateCity(citys);
            }
            else throw new NullReferenceException();

        }

        private void NotifyParameterChanged(object parameter)
        {
            var h = ParameterChanged;
            if (h != null)
            {
                ParameterChanged(this, new ParameterChangedEventArgs(parameter));
            }
        }
        private void NotifyFetchDataComplete()
        {
            var h = FetchDataComplete;
            if (h != null)
            {
                FetchDataComplete(this, new FetchDataCompleteEventArgs());
            }
        }

        private void InitialViewModel()
        {
            var c = currentCity;
            currentCity = null;
            City = c;
            var g = glance;
            glance = null;
            Glance = g;
            SetNow();
            SetTime();
            CalculatePath();
            SetHourlyTemp();
            SetHour();
            SetProportion();
            SetDailyForecast();
            SetSuggestion();

            this.NotifyFetchDataComplete();
        }

        private void SetSuggestion()
        {
            Comf = fetchresult.WeatherSuggestion.Comfortable;
            Cw = fetchresult.WeatherSuggestion.CarWashing;
            Drsg = fetchresult.WeatherSuggestion.Dressing;
            Uv = fetchresult.WeatherSuggestion.UV;
            Sport = fetchresult.WeatherSuggestion.Sport;
            Flu = fetchresult.WeatherSuggestion.Flu;
            Trav = fetchresult.WeatherSuggestion.Trav;
        }

        private async Task SearchExistingDataAsync()
        {
            if (!citys.IsNullorEmpty())
            {
                var currentTime = DateTime.Now;
                storedDatas = new List<KeyValuePair<string, string>>();
                foreach (var c in citys)
                {

                    if ((currentTime - c.LastUpdate).TotalMinutes > 15)
                    {
                        continue;
                    }
                    try
                    {
                        var data = await FileIOHelper.ReadStringFromStorageAsync(c.Id);
                        if (data != null)
                            storedDatas.Add(new KeyValuePair<string, string>(c.Id, data));
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
        }

        private void InitialConverterParameter(SettingsModel settings)
        {
            TempratureConverter.ChangeParameter(settings.TempratureParameter);
            DateTimeConverter.ChangeParameter(settings.ForecastDateParameter);
        }
        #region Set Properties
        private void SetDailyForecast()
        {
            //json 中第一个dailyforecast 是今天的
            if (!IsNight)
            {
                Forecast0 = fetchresult.DailyForecast[1].Condition.DayCond;
                Forecast1 = fetchresult.DailyForecast[2].Condition.DayCond;
                Forecast2 = fetchresult.DailyForecast[3].Condition.DayCond;
                Forecast3 = fetchresult.DailyForecast[4].Condition.DayCond;
                Forecast4 = fetchresult.DailyForecast[5].Condition.DayCond;
            }
            else
            {
                Forecast0 = fetchresult.DailyForecast[1].Condition.NightCond;
                Forecast1 = fetchresult.DailyForecast[2].Condition.NightCond;
                Forecast2 = fetchresult.DailyForecast[3].Condition.NightCond;
                Forecast3 = fetchresult.DailyForecast[4].Condition.NightCond;
                Forecast4 = fetchresult.DailyForecast[5].Condition.NightCond;
            }
            ForecastDate1 = fetchresult.DailyForecast[2].Date;
            ForecastDate2 = fetchresult.DailyForecast[3].Date;
            ForecastDate3 = fetchresult.DailyForecast[4].Date;
            ForecastDate4 = fetchresult.DailyForecast[5].Date;
            Forecast0H = fetchresult.DailyForecast[1].HighTemp;
            Forecast0L = fetchresult.DailyForecast[1].LowTemp;
            Forecast1H = fetchresult.DailyForecast[2].HighTemp;
            Forecast1L = fetchresult.DailyForecast[2].LowTemp;
            Forecast2H = fetchresult.DailyForecast[3].HighTemp;
            Forecast2L = fetchresult.DailyForecast[3].LowTemp;
            Forecast3H = fetchresult.DailyForecast[4].HighTemp;
            Forecast3L = fetchresult.DailyForecast[4].LowTemp;
            Forecast4H = fetchresult.DailyForecast[5].HighTemp;
            Forecast4L = fetchresult.DailyForecast[5].LowTemp;
        }

        private void SetProportion()
        {
            Pop0 = fetchresult.HourlyForecast[0].Pop / 100f;
            Pop1 = fetchresult.HourlyForecast[1].Pop / 100f;
            Pop2 = fetchresult.HourlyForecast[2].Pop / 100f;
            Pop3 = fetchresult.HourlyForecast[3].Pop / 100f;
            Pop4 = fetchresult.HourlyForecast[4].Pop / 100f;
            Pop5 = fetchresult.HourlyForecast[5].Pop / 100f;
        }

        private void SetHour()
        {
            Hour0 = fetchresult.HourlyForecast[0].DateTime;
            Hour1 = fetchresult.HourlyForecast[1].DateTime;
            Hour2 = fetchresult.HourlyForecast[2].DateTime;
            Hour3 = fetchresult.HourlyForecast[3].DateTime;
            Hour4 = fetchresult.HourlyForecast[4].DateTime;
            Hour5 = fetchresult.HourlyForecast[5].DateTime;
        }

        private void SetHourlyTemp()
        {
            HourlyTemp0 = fetchresult.HourlyForecast[0].Temprature;
            HourlyTemp1 = fetchresult.HourlyForecast[1].Temprature;
            HourlyTemp2 = fetchresult.HourlyForecast[2].Temprature;
            HourlyTemp3 = fetchresult.HourlyForecast[3].Temprature;
            HourlyTemp4 = fetchresult.HourlyForecast[4].Temprature;
            HourlyTemp5 = fetchresult.HourlyForecast[5].Temprature;
        }

        private void CalculatePath()
        {
            List<float> pathResults = new List<float>();
            for (int i = 0; i < 6; i++)
            {
                pathResults.Add(fetchresult.HourlyForecast[i].Temprature.Celsius);
            }
            var min = 0f;
            var max = min;
            foreach (var data in pathResults)
            {
                if (data < min)
                {
                    min = data;
                    continue;
                }
                if (data > max)
                {
                    max = data;
                }
            }
            var avg = (max + min) / 2;
            for (int j = 0; j < pathResults.Count; j++)
            {
                pathResults[j] -= avg;
            }

            TempraturePath0 = pathResults[0] / (max - min);
            TempraturePath1 = pathResults[1] / (max - min);
            TempraturePath2 = pathResults[2] / (max - min);
            TempraturePath3 = pathResults[3] / (max - min);
            TempraturePath4 = pathResults[4] / (max - min);
            TempraturePath5 = pathResults[5] / (max - min);
        }

        private void SetNow()
        {
            var p = moonPhase;
            moonPhase = double.NaN;
            MoonPhase = p;
            var c = calendar;
            calendar = null;
            Calendar = c;
            Temprature = fetchresult.NowWeather.Temprature;
            NowH = fetchresult.DailyForecast[0].HighTemp;
            NowL = fetchresult.DailyForecast[0].LowTemp;
            BodyTemprature = fetchresult.NowWeather.BodyTemprature;
            Humidity = fetchresult.HourlyForecast[0].Humidity;
            Precipitation = fetchresult.NowWeather.Precipitation;
            Proportion = fetchresult.HourlyForecast[0].Pop;
            Pressure = fetchresult.NowWeather.Pressure;
            Visibility = fetchresult.NowWeather.Visibility;
            Aqi = fetchresult.Aqi;

            if (Temprature.Celsius > 20)
            {
                IsSummer = true;
            }
            else
            {
                IsSummer = false;
            }
            Wind = fetchresult.NowWeather.Wind;
            Condition = fetchresult.NowWeather.Now.Condition;
        }

        private void SetTime()
        {
            UpdateTime = fetchresult.Location.UpdateTime;
            currentTimeZone = DateTimeHelper.GetTimeZone(UpdateTime, fetchresult.Location.UtcTime);
            RefreshCurrentTime();
            CurrentTimeRefreshTask();
            SunRise = fetchresult.DailyForecast[0].SunRise;
            SunSet = fetchresult.DailyForecast[0].SunSet;
            IsNight = CalculateIsNight(UpdateTime, SunRise, SunSet);
        }

        public void RefreshCurrentTime()
        {
            CurrentTime = DateTimeHelper.RevisetoLoc(currentTimeZone);
        }

        public void CurrentTimeRefreshTask()
        {
            if (currentTimer != null)
            {
                currentTimer.Cancel();
                currentTimer = null;
            }
            var nextupdate = 60.05 - (DateTime.Now.Second + DateTime.Now.Millisecond / 1000f);
            currentTimer = ThreadPoolTimer.CreateTimer(
                async (task) =>
            {
                var locTime = DateTimeHelper.RevisetoLoc(currentTimeZone);
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

        private bool CalculateIsNight(DateTime updateTime, TimeSpan sunRise, TimeSpan sunSet)
        {
            var updateMinutes = updateTime.Hour * 60 + updateTime.Minute;
            if (updateMinutes < sunRise.TotalMinutes)
            {
                SunProgress = (1440 + updateMinutes - sunSet.TotalMinutes) / (sunRise.Add(TimeSpan.FromHours(24)) - sunSet).TotalMinutes;
            }
            else if (updateMinutes >= sunSet.TotalMinutes)
            {
                SunProgress = (updateMinutes - sunSet.TotalMinutes) / (sunRise.Add(TimeSpan.FromHours(24)) - sunSet).TotalMinutes;
            }
            else
            {
                SunProgress = (updateMinutes - sunRise.TotalMinutes) / (sunSet - sunRise).TotalMinutes;
                return false;
            }
            return true;
        }
        #endregion
        private void ReadSettings()
        {
#if DEBUG
            settings = SettingsModel.ReadSettings();
            if (settings.SavedCities.IsNullorEmpty())
            {
                currentCityModel = new CitySettingsModel();
                currentCityModel.City = "北京";
                currentCityModel.Id = "CA1000011";
                currentCityModel.LastUpdate = DateTime.Now;
                currentCity = "北京";
                currentId = "CA1000011";
                citys = new CitySettingsModel[] { new CitySettingsModel() };
                citys[0].City = "北京";
                citys[0].Id = "CA1000011";
                citys[0].LastUpdate = DateTime.Now;
                settings.CurrentCityIndex = 0;
                settings.SavedCities = citys;
                settings.AllowLocation = true;
                settings.ForecastDateParameter = "dddd";
                settings.TempratureParameter = 1;
                settings.SaveSettings();
            }
            else
            {
                currentCityModel = settings.SavedCities[settings.CurrentCityIndex];
                currentCity = currentCityModel.City;
                citys = settings.SavedCities;
                currentId = currentCityModel.Id;
            }
            InitialConverterParameter(settings);
#else
            settings = SettingsModel.ReadSettings();
            if (!settings.SavedCities.IsNullorEmpty())
            {
            currentCity = settings.CurrentCity.City;
            citys = settings.SavedCities;
            currentId = settings.CurrentCity.Id;
            InitialConverterParameter(settings);
            }
            else
            {
            throw new ArgumentNullException();
            }
#endif

        }
    }
}
