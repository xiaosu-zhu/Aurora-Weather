using Com.Aurora.AuWeather.ViewModels;
using Com.Aurora.Shared.Converters;
using Com.Aurora.Shared.Helpers;
using NotificationsExtensions.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Tile
{
    internal static class Generator
    {
        public static void GenerateNormalTile(Models.HeWeather.HeWeatherModel model, DateTime desiredDate)
        {
#if DEBUG
            var todayIndex = 0;
            var hourIndex = 0;
#else
            var todayIndex = Array.FindIndex(model.DailyForecast, x =>
            {
                return (x.Date - desiredDate).TotalSeconds > 0;
            }) - 1;
            var hourIndex = Array.FindIndex(model.HourlyForecast, x =>
            {
                return (x.DateTime - desiredDate).TotalSeconds > 0;
            });
#endif
            var updateTime = model.Location.UpdateTime;
            var currentTimeZone = DateTimeHelper.GetTimeZone(updateTime, model.Location.UtcTime);
            var sunRise = model.DailyForecast[todayIndex].SunRise;
            var sunSet = model.DailyForecast[todayIndex].SunSet;
            var currentTime = DateTimeHelper.RevisetoLoc(currentTimeZone, desiredDate);
            var isNight = CalcIsNight(currentTime, sunRise, sunSet);

            var ctosConverter = new ConditiontoTextConverter();
            var ctoiConverter = new ConditiontoImageConverter();
            ctoiConverter.IsNight = isNight;
            TileContent content = new TileContent()
            {
                Visual = new TileVisual()
                {
                    Branding = TileBranding.NameAndLogo,
                    DisplayName = "Glance",
                    TileMedium = new TileBinding()
                    {
                        DisplayName = "Now",
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileGroup()
                                {
                                    Children =
                                            {
                                                    CreateSubgroup((string)ctosConverter.Convert(model.NowWeather.Now.Condition,null,null,null),
                                                  (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,null,null),
                                                 model.DailyForecast[todayIndex].HighTemp.Celsius.ToString(),
                                                 model.DailyForecast[todayIndex].LowTemp.Celsius.ToString())
                                            }
                                }
                            },
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                                       {
                                          new TileGroup()
                                {
                                    Children =
                                            {
                                                    CreateSubgroup(model.DailyForecast[todayIndex].Date.ToString("dddd"),
                                                  (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,null,null),
                                                 model.DailyForecast[todayIndex].HighTemp.Celsius.ToString(),
                                                 model.DailyForecast[todayIndex].LowTemp.Celsius.ToString()),

                                                    CreateSubgroup(model.DailyForecast[todayIndex+1].Date.ToString("dddd"),
                                                  (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,null,null),
                                                 model.DailyForecast[todayIndex+1].HighTemp.Celsius.ToString(),
                                                 model.DailyForecast[todayIndex+1].LowTemp.Celsius.ToString()),

                                                    CreateSubgroup(model.DailyForecast[todayIndex+2].Date.ToString("dddd"),
                                                  (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,null,null),
                                                 model.DailyForecast[todayIndex+2].HighTemp.Celsius.ToString(),
                                                 model.DailyForecast[todayIndex+2].LowTemp.Celsius.ToString()),

                                                  CreateSubgroup(model.DailyForecast[todayIndex+3].Date.ToString("dddd"),
                                                  (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,null,null),
                                                 model.DailyForecast[todayIndex+3].HighTemp.Celsius.ToString(),
                                                 model.DailyForecast[todayIndex+3].LowTemp.Celsius.ToString()),

                                                  CreateSubgroup(model.DailyForecast[todayIndex+4].Date.ToString("dddd"),
                                                  (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,null,null),
                                                 model.DailyForecast[todayIndex+4].HighTemp.Celsius.ToString(),
                                                 model.DailyForecast[todayIndex+4].LowTemp.Celsius.ToString())
                                            }
                                  }
                                       }
                        }
                    },
                    TileLarge = new TileBinding()
                    {

                    }
                }
            };
        }

        private static bool CalcIsNight(DateTime updateTime, TimeSpan sunRise, TimeSpan sunSet)
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

        private static TileSubgroup CreateSubgroup(string day, string image, string highTemp, string lowTemp)
        {
            return new TileSubgroup()
            {
                Weight = 1,

                Children =
        {
            new TileText()
            {
                Text = day,
                Align = TileTextAlign.Center
            },

            new TileImage()
            {
                Source = new TileImageSource("Assets/Weather/" + image),
                RemoveMargin = true
            },

            new TileText()
            {
                Text = highTemp,
                Align = TileTextAlign.Center
            },

            new TileText()
            {
                Text = lowTemp,
                Align = TileTextAlign.Center,
                Style = TileTextStyle.CaptionSubtle
            }
        }
            };
        }
    }
}
