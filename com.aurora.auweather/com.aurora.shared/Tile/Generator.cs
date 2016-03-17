using Com.Aurora.AuWeather.Models;
using Com.Aurora.Shared.Converters;
using Com.Aurora.Shared.Helpers;
using NotificationsExtensions.Tiles;
using NotificationsExtensions.Badges;
using System;
using System.Threading.Tasks;
using Com.Aurora.AuWeather.Models.HeWeather;
using System.Collections.Generic;
using Com.Aurora.AuWeather.Models.Settings;
using NotificationsExtensions.Toasts;

namespace Com.Aurora.AuWeather.Tile
{
    public static class Generator
    {
        public static TileContent GenerateNormalTile(HeWeatherModel model, bool isNight, string glance, string glanceFull, Uri uri, int todayIndex, CitySettingsModel currentCity, SettingsModel settings)
        {
            var ctosConverter = new ConditiontoTextConverter();
            var ctoiConverter = new ConditiontoImageConverter();

            #region
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
                                new TileText(),
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
                        Branding = TileBranding.NameAndLogo,
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
                                //                    CreateSubgroup(model.DailyForecast[todayIndex].Date.ToString(settings.Preferences.GetTileFormat()),
                                //                  (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,isNight,null),
                                //                 model.DailyForecast[todayIndex].HighTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
                                //                 model.DailyForecast[todayIndex].LowTemp.Actual(settings.Preferences.TemperatureParameter).ToString()),

                                //                    CreateSubgroup(model.DailyForecast[todayIndex+1].Date.ToString(settings.Preferences.GetTileFormat()),
                                //                  (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,isNight,null),
                                //                 model.DailyForecast[todayIndex+1].HighTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
                                //                 model.DailyForecast[todayIndex+1].LowTemp.Actual(settings.Preferences.TemperatureParameter).ToString()),

                                //                    CreateSubgroup(model.DailyForecast[todayIndex+2].Date.ToString(settings.Preferences.GetTileFormat()),
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
                                    Text = model.NowWeather.Temprature.Actual(settings.Preferences.TemperatureParameter).ToString(),
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
                                            new TileText(),
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

        public static ToastContent CreateToast(HeWeatherModel model, CitySettingsModel currentCity, SettingsModel settings, DateTime DueTime)
        {
            var glance = Glance.GenerateGlanceDescription(model, false, settings.Preferences.TemperatureParameter, DueTime);
            var ctos = new ConditiontoTextConverter();
            var todayIndex = Array.FindIndex(model.DailyForecast, x =>
            {
                return x.Date.Date == DueTime.Date;
            });
            var toast = new ToastContent()
            {
                Scenario = ToastScenario.Default,
                ActivationType = ToastActivationType.Foreground,
                Duration = ToastDuration.Long,
                Launch = currentCity.Id,
                Visual = new ToastVisual()
                {
                    TitleText = new ToastText()
                    {
                        Text = "Today's Weather for " + currentCity.City
                    },
                    BodyTextLine1 = new ToastText()
                    {
                        Text = ctos.Convert(model.DailyForecast[todayIndex].Condition.DayCond, null, null, null) + ", "
                        + ((model.DailyForecast[todayIndex].HighTemp + model.DailyForecast[todayIndex].LowTemp) / 2).Actual(settings.Preferences.TemperatureParameter)
                    },
                    BodyTextLine2 = new ToastText()
                    {
                        Text = glance
                    },
                }
            };
            return toast;
        }

        public static bool CalcIsNight(DateTime updateTime, TimeSpan sunRise, TimeSpan sunSet)
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

        public static async Task<List<TileContent>> CreateAll(HeWeatherModel model, DateTime desiredDate)
        {
            var todayIndex = Array.FindIndex(model.DailyForecast, x =>
            {
                return x.Date.Date == desiredDate.Date;
            });
            var hourIndex = Array.FindIndex(model.HourlyForecast, x =>
            {
                return (x.DateTime - desiredDate).TotalSeconds > 0;
            });
            var updateTime = model.Location.UpdateTime;
            var currentTimeZone = DateTimeHelper.GetTimeZone(updateTime, model.Location.UtcTime);
            var sunRise = model.DailyForecast[todayIndex].SunRise;
            var sunSet = model.DailyForecast[todayIndex].SunSet;
            var currentTime = DateTimeHelper.RevisetoLoc(currentTimeZone, desiredDate);
            var isNight = CalcIsNight(currentTime, sunRise, sunSet);
            var settings = SettingsModel.Get();
            var currentCity = settings.Cities.CurrentIndex < 0 ? settings.Cities.LocatedCity : settings.Cities.SavedCities[settings.Cities.CurrentIndex];
            Uri uri = await settings.Immersive.GetCurrentBackgroundAsync(model.NowWeather.Now.Condition, isNight);

            var glance = Glance.GenerateShortDescription(model, isNight);
            var glanceFull = Glance.GenerateGlanceDescription(model, isNight, settings.Preferences.TemperatureParameter, desiredDate);
            var noramlTile = GenerateNormalTile(model, isNight, glance, glanceFull, uri, todayIndex, currentCity, settings);
            var nowTile = GenerateNowTile(model, isNight, uri, glanceFull, todayIndex, currentCity, settings);
            var forecastTile = GenerateForecastTile(model, isNight, uri, glanceFull, todayIndex, currentCity, settings);
            var list = new List<TileContent>();
            list.Add(nowTile);
            list.Add(noramlTile);
            list.Add(forecastTile);
            return list;
        }

        public static BadgeGlyphNotificationContent GenerateAlertBadge()
        {
            var b = new BadgeGlyphNotificationContent(GlyphValue.Alert);
            return b;
        }

        private static TileContent GenerateForecastTile(HeWeatherModel model, bool isNight, Uri uri, string glanceFull, int todayIndex, CitySettingsModel currentCity, SettingsModel settings)
        {
            var ctosConverter = new ConditiontoTextConverter();
            var ctoiConverter = new ConditiontoImageConverter();
            var forecaset = new TileContent()
            {
                Visual = new TileVisual()
                {
                    DisplayName = currentCity.City,
                    Branding = TileBranding.NameAndLogo,
                    LockDetailedStatus1 = currentCity.City,
                    LockDetailedStatus2 = model.NowWeather.Temprature.Actual(settings.Preferences.TemperatureParameter),
                    LockDetailedStatus3 = glanceFull,
                    TileLarge = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText(),
                                new TileGroup()
                                {
                                    Children =
                                    {
                                        new TileSubgroup()
                                        {
                                            Weight = 1,

                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            Children=
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].Date.ToString("ddd"),
                                                },
                                                new TileImage()
                                                {
                                                    Source = new TileImageSource("Assets/Tile/" + (string)ctoiConverter.Convert(isNight?model.DailyForecast[todayIndex+1].Condition.NightCond:model.DailyForecast[todayIndex+1].Condition.DayCond,null,isNight,null)),
                                                },
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            TextStacking = TileTextStacking.Bottom,
                                            Children=
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].HighTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].LowTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.CaptionSubtle
                                                },
                                            }
                                        },

                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            Children=
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+2].Date.ToString("ddd"),
                                                },
                                                new TileImage()
                                                {
                                                    Source = new TileImageSource("Assets/Tile/" + (string)ctoiConverter.Convert(isNight?model.DailyForecast[todayIndex+2].Condition.NightCond:model.DailyForecast[todayIndex+2].Condition.DayCond,null,isNight,null)),
                                                },
                                            }

                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            TextStacking = TileTextStacking.Bottom,
                                            Children=
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+2].HighTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+2].LowTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.CaptionSubtle
                                                },
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,

                                        },
                                    }
                                },
                                new TileText(),
                                new TileGroup()
                                {
                                    Children =
                                    {
                                        new TileSubgroup()
                                        {
                                            Weight = 1,

                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            Children=
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+3].Date.ToString("ddd"),
                                                },
                                                new TileImage()
                                                {
                                                    Source = new TileImageSource("Assets/Tile/" + (string)ctoiConverter.Convert(isNight?model.DailyForecast[todayIndex+3].Condition.NightCond:model.DailyForecast[todayIndex+3].Condition.DayCond,null,isNight,null)),
                                                },
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            TextStacking = TileTextStacking.Bottom,
                                            Children=
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+3].HighTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+3].LowTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.CaptionSubtle
                                                },
                                            }
                                        },

                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            Children=
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+4].Date.ToString("ddd"),

                                                },
                                                new TileImage()
                                                {
                                                    Source = new TileImageSource("Assets/Tile/" + (string)ctoiConverter.Convert(isNight?model.DailyForecast[todayIndex+4].Condition.NightCond:model.DailyForecast[todayIndex+4].Condition.DayCond,null,isNight,null)),
                                                },
                                            }

                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            TextStacking = TileTextStacking.Bottom,
                                            Children=
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+4].HighTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+4].LowTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.CaptionSubtle
                                                },
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,

                                        },
                                    }
                                }
                            }
                        }
                    },
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText()
                                {

                                },
                                new TileText()
                                {
                                    Text = model.DailyForecast[todayIndex+1].Date.ToString("ddd"),
                                    Align = TileTextAlign.Center
                                },
                                 new TileText()
                                 {
                                                    Text = model.DailyForecast[todayIndex+1].HighTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.Caption,
                                                    Align = TileTextAlign.Center
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].LowTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.CaptionSubtle,
                                                    Align = TileTextAlign.Center
                                                },
                            }
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
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].Date.ToString("ddd"),
                                                },
                                                new TileImage()
                                                {
                                                    Source = new TileImageSource("Assets/Tile/" + (string)ctoiConverter.Convert(isNight?model.DailyForecast[todayIndex+1].Condition.NightCond:model.DailyForecast[todayIndex+1].Condition.DayCond,null,isNight,null)),
                                                },
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            TextStacking = TileTextStacking.Bottom,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].HighTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].LowTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.CaptionSubtle
                                                },
                                            }
                                        },
                                       new TileSubgroup()
                                        {
                                            Weight = 1,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+2].Date.ToString("ddd"),
                                                },
                                                new TileImage()
                                                {
                                                    Source = new TileImageSource("Assets/Tile/" + (string)ctoiConverter.Convert(isNight?model.DailyForecast[todayIndex+2].Condition.NightCond:model.DailyForecast[todayIndex+2].Condition.DayCond,null,isNight,null)),
                                                },
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            TextStacking = TileTextStacking.Bottom,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+2].HighTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+2].LowTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.CaptionSubtle
                                                },
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+3].Date.ToString("ddd"),
                                                },
                                                new TileImage()
                                                {
                                                    Source = new TileImageSource("Assets/Tile/" + (string)ctoiConverter.Convert(isNight?model.DailyForecast[todayIndex+3].Condition.NightCond:model.DailyForecast[todayIndex+3].Condition.DayCond,null,isNight,null)),
                                                },
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            TextStacking = TileTextStacking.Bottom,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+3].HighTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+3].LowTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.CaptionSubtle
                                                },
                                            }
                                        },
                                    }
                                },
                            }
                        }
                    }
                }
            };
            return forecaset;
        }

        public static ToastContent CreateAlertToast(HeWeatherModel fetchresult, CitySettingsModel currentCityModel)
        {
            var alarm = fetchresult.Alarms[0];
            ToastContent t = new ToastContent()
            {
                Duration = ToastDuration.Long,
                Scenario = ToastScenario.Alarm,
                Launch = currentCityModel.Id,
                Visual = new ToastVisual()
                {
                    TitleText = new ToastText()
                    {
                        Text = "气象灾害预警"
                    },
                    BodyTextLine1 = new ToastText()
                    {
                        Text = alarm.Title
                    },
                    BodyTextLine2 = new ToastText()
                    {
                        Text = alarm.Text
                    }
                }
            };
            return t;
        }

        private static TileContent GenerateNowTile(HeWeatherModel model, bool isNight, Uri uri, string glanceFull, int todayIndex, CitySettingsModel currentCity, SettingsModel settings)
        {
            var ctosConverter = new ConditiontoTextConverter();
            var ctoiConverter = new ConditiontoImageConverter();

            var now = new TileContent()
            {
                Visual = new TileVisual()
                {
                    DisplayName = currentCity.City,
                    Branding = TileBranding.NameAndLogo,
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText(),
                                new TileGroup()
                                {
                                    Children =
                                    {
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            Children =
                                            {
                                                new TileImage()
                                                {
                                                    Source = new TileImageSource("Assets/Tile/" + (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,isNight,null)),
                                                },
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                        }
                                    }
                                },
                                new TileText()
                                {
                                    Text = model.NowWeather.Temprature.Actual(settings.Preferences.TemperatureParameter),
                                    Style = TileTextStyle.Body,
                                    Align = TileTextAlign.Center
                                }
                            }
                        },
                    },
                    TileLarge = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText(),
                                new TileGroup()
                                {
                                    Children =
                                    {
                                        new TileSubgroup()
                                        {
                                            Weight = 3,
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 2,
                                            Children =
                                            {
                                                new TileImage()
                                                {
                                                    Source = new TileImageSource("Assets/Tile/" + (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,isNight,null)),
                                                },
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 3,
                                        }
                                    }
                                },
                                new TileText()
                                {
                                    Text = model.NowWeather.Temprature.Actual(settings.Preferences.TemperatureParameter),
                                    Style = TileTextStyle.Subheader,
                                    Align = TileTextAlign.Center
                                },
                                new TileText()
                                {
                                    Text = glanceFull,
                                    Align = TileTextAlign.Center,
                                    Style = TileTextStyle.CaptionSubtle,
                                    Wrap = true
                                }
                            }
                        }
                    },
                    TileSmall = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            Children =
                            {
                                new TileText()
                                {
                                    Text = model.NowWeather.Temprature.Actual(settings.Preferences.TemperatureParameter),
                                    Align = TileTextAlign.Center
                                }
                            }
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
                                    Children=
                                    {
                                        new TileSubgroup()
                                        {
                                            TextStacking = TileTextStacking.Top,
                                            Weight =1,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = "今日",
                                                    Style = TileTextStyle.Caption,
                                                    Align = TileTextAlign.Center
                                                },
                                                new TileText()
                                                {
                                                    Text = (string)ctosConverter.Convert(model.NowWeather.Now.Condition,null,null,null),
                                                    Style = TileTextStyle.Caption,
                                                    Align = TileTextAlign.Center
                                                },
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            TextStacking = TileTextStacking.Center,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex].HighTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.Caption,
                                                    Align = TileTextAlign.Center
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex].LowTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.CaptionSubtle,
                                                    Align = TileTextAlign.Center
                                                },
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            TextStacking = TileTextStacking.Top,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = "明日",
                                                    Style = TileTextStyle.Caption,
                                                    Align = TileTextAlign.Center
                                                },
                                                new TileText()
                                                {
                                                    Text = (string)ctosConverter.Convert(isNight? model.DailyForecast[todayIndex+1].Condition.NightCond:model.DailyForecast[todayIndex+1].Condition.DayCond,null,null,null),
                                                    Style = TileTextStyle.Caption,
                                                    Align = TileTextAlign.Center
                                                },
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            TextStacking = TileTextStacking.Center,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].HighTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.Caption,
                                                    Align = TileTextAlign.Center
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].LowTemp.Actual(settings.Preferences.TemperatureParameter),
                                                    Style = TileTextStyle.CaptionSubtle,
                                                    Align = TileTextAlign.Center
                                                },
                                            }
                                        },

                                    }
                                },
                                new TileText()
                                        {
                                            Text = glanceFull,
                                            Wrap = true,
                                            Style = TileTextStyle.CaptionSubtle
                                        }
                            }
                        }
                    }
                }
            };
            return now;
        }
    }
}
