using Com.Aurora.AuWeather.Models;
using Com.Aurora.Shared.Converters;
using Com.Aurora.Shared.Helpers;
using NotificationsExtensions.Tiles;
using System;
using System.Threading.Tasks;

namespace Com.Aurora.AuWeather.Tile
{
    internal static class Generator
    {
        public static async Task<TileContent> GenerateNormalTile(Models.HeWeather.HeWeatherModel model, DateTime desiredDate)
        {
            #region
#if DEBUG
            var todayIndex = 0;
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
            var settings = SettingsModel.Get();
            var currentCity = settings.Cities.CurrentIndex < 0 ? settings.Cities.LocatedCity : settings.Cities.SavedCities[settings.Cities.CurrentIndex];
            Uri uri = await settings.Immersive.GetCurrentBackgroundAsync(model.NowWeather.Now.Condition, isNight);
            var ctosConverter = new ConditiontoTextConverter();
            var ctoiConverter = new ConditiontoImageConverter();
            var glance = Glance.GenerateShortDescription(model, isNight);
            var glanceFull = Glance.GenerateGlanceDescription(model, isNight, settings.Preferences.TemperatureParameter, desiredDate);
            TileContent NowContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    Branding = TileBranding.NameAndLogo,
                    DisplayName = currentCity.City,
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                                 new TileText()
            {
                Text = (string)ctosConverter.Convert(model.NowWeather.Now.Condition,null,null,null),
                Align = TileTextAlign.Center
            },


            new TileText()
            {
                Text = model.DailyForecast[todayIndex].HighTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
                Align = TileTextAlign.Center
            },

            new TileText()
            {
                Text = model.DailyForecast[todayIndex].LowTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
                Align = TileTextAlign.Center,
                Style = TileTextStyle.CaptionSubtle
            }
                                            }
                        }
                    },

                    TileWide = new TileBinding()
                    {
                        DisplayName = glance,
                        Branding = TileBranding.Name,
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                                       {
                                new TileText()
                                                {
                                                    Text = currentCity.City,
                                                    Align = TileTextAlign.Auto,
                                                    Style = TileTextStyle.Caption
                                                },
                                new TileGroup()
                                {
                                    Children=
                                    {
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            Children =
                                            {
                                                new TileImage()
                                                {
                                                    Source = new TileImageSource("Assets/Tile/" + (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,isNight,null))
                                                }
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 3,
                                            TextStacking = TileTextStacking.Center,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex].HighTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].LowTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
                                                    Style = TileTextStyle.CaptionSubtle,
                                                    Align = TileTextAlign.Center
                                                }
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 3,
                                            TextStacking = TileTextStacking.Center,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.NowWeather.Wind.Speed.Actual(settings.Preferences.SpeedParameter) +
                                                    model.NowWeather.Wind.Speed.DanWei(settings.Preferences.SpeedParameter),
                                                    Align = TileTextAlign.Left,
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex].Humidity.ToString() + "%",
                                                    Align = TileTextAlign.Left,
                                                    Style = TileTextStyle.Caption
                                                }
                                            }
                                        }
                                    }
                                }
                                //          new TileGroup()
                                //{
                                //    Children =
                                //            {
                                //                    CreateSubgroup(model.DailyForecast[todayIndex].Date.ToString(settings.Preferences.GetForecastFormat()),
                                //                  (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,isNight,null),
                                //                 model.DailyForecast[todayIndex].HighTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
                                //                 model.DailyForecast[todayIndex].LowTemp.Actual(settings.Preferences.TemperatureParameter).ToString()),

                                //                    CreateSubgroup(model.DailyForecast[todayIndex+1].Date.ToString(settings.Preferences.GetForecastFormat()),
                                //                  (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,isNight,null),
                                //                 model.DailyForecast[todayIndex+1].HighTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
                                //                 model.DailyForecast[todayIndex+1].LowTemp.Actual(settings.Preferences.TemperatureParameter).ToString()),

                                //                    CreateSubgroup(model.DailyForecast[todayIndex+2].Date.ToString(settings.Preferences.GetForecastFormat()),
                                //                  (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,isNight,null),
                                //                 model.DailyForecast[todayIndex+2].HighTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
                                //                 model.DailyForecast[todayIndex+2].LowTemp.Actual(settings.Preferences.TemperatureParameter).ToString()),
                                //              }
                                //          }
                                       },
                            PeekImage = uri == null ? null : new TilePeekImage()
                            {
                                Source = new TileImageSource(uri.ToString()),
                            }
                        }
                    },
                    TileLarge = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText()
                                                {
                                                    Text = currentCity.City,
                                                    Align = TileTextAlign.Auto,
                                                    Style = TileTextStyle.Caption
                                                },
                                            new TileGroup()
                                            {
                                             Children =
                                             {
                                                 new TileSubgroup()
                                        {
                                            Weight = 1,
                                            Children =
                                            {
                                                new TileImage()
                                                {
                                                    Source = new TileImageSource("Assets/Tile/" + (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,isNight,null))
                                                },
                                                new TileText()
                                               {
                                                   Text = model.NowWeather.Temprature.Actual(settings.Preferences.TemperatureParameter).ToString(),
                                                   Align = TileTextAlign.Center,
                                                   Style = TileTextStyle.Caption
                                               },
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 3,
                                            TextStacking = TileTextStacking.Center,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex].HighTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].LowTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
                                                    Style = TileTextStyle.CaptionSubtle,
                                                    Align = TileTextAlign.Center
                                                }
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 2,
                                            TextStacking = TileTextStacking.Center,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.NowWeather.Wind.Speed.Actual(settings.Preferences.SpeedParameter) +
                                                    model.NowWeather.Wind.Speed.DanWei(settings.Preferences.SpeedParameter),
                                                    Align = TileTextAlign.Left,
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex].Humidity.ToString() + "%",
                                                    Align = TileTextAlign.Left,
                                                    Style = TileTextStyle.Caption
                                                }
                                            }
                                        }
                                            },
                                           },
                                            new TileGroup()
                                            {
                                                Children =
                                                {
                                                    new TileSubgroup()
                                                    {
                                                        TextStacking = TileTextStacking.Bottom,
                                                        Children =
                                                        {
                                      new TileText()
                                      {
                                         Text = glanceFull,
                                         Align = TileTextAlign.Auto,
                                         Style = TileTextStyle.CaptionSubtle,
                                         Wrap = true
                                      }
                                                        }
                                                    }
                                                }
                                            }

                            },
                            PeekImage = uri == null ? null : new TilePeekImage()
                            {
                                Source = new TileImageSource(uri.ToString()),
                            }
                        }
                    }
                }
            };
            #endregion
            return NowContent;
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
                Source = new TileImageSource("Assets/Tile/" + image),
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
