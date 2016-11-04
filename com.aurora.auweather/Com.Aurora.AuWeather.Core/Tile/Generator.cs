// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Models;
using Com.Aurora.Shared.Converters;
using NotificationsExtensions.Tiles;
using NotificationsExtensions.Badges;
using System;
using System.Threading.Tasks;
using Com.Aurora.AuWeather.Models.HeWeather;
using System.Collections.Generic;
using Com.Aurora.AuWeather.Models.Settings;
using NotificationsExtensions.Toasts;
using Windows.ApplicationModel.Resources;
using Com.Aurora.AuWeather.Core.LunarCalendar;
using Windows.Data.Xml.Dom;
using Com.Aurora.Shared.Extensions;
using Windows.UI.StartScreen;
using Com.Aurora.AuWeather.Core.Models;
using Com.Aurora.Shared.Helpers;
using System.Linq;

namespace Com.Aurora.AuWeather.Tile
{
    public static class Generator
    {

        public static async Task UpdateSubTiles(SettingsModel settings)
        {
            var tiles = await SecondaryTile.FindAllForPackageAsync();
            var list = tiles.ToList();
            foreach (var item in settings.Cities.SavedCities)
            {
                try
                {
                    var tile = list.Find(x =>
                    {
                        return x.TileId == item.Id;
                    });
                    if (tile != null)
                    {
                        string resstr = await Request.GetRequestAsync(settings, item);
                        if (!resstr.IsNullorEmpty())
                        {
                            var fetchresult = HeWeatherModel.Generate(resstr, settings.Preferences.DataSource);
                            if (fetchresult == null || fetchresult.DailyForecast == null || fetchresult.HourlyForecast == null)
                            {
                                return;
                            }
                            var utcOffset = fetchresult.Location.UpdateTime - fetchresult.Location.UtcTime;
                            var current = DateTimeHelper.ReviseLoc(utcOffset);
                            try
                            {
                                Sender.CreateSubTileNotification(await Generator.CreateAll(item, fetchresult, current), item.Id);
                            }
                            catch (Exception)
                            { }
                        }
                    }
                }
                catch (Exception)
                {

                }

            }

        }


        public static TileContent GenerateNormalTile(HeWeatherModel model, bool isNight, string glance, string glanceFull, Uri uri, int todayIndex, CitySettingsModel currentCity, SettingsModel settings)
        {
            var ctosConverter = new ConditiontoTextConverter();
            var ctoiConverter = new ConditiontoImageConverter();
            var loader = new ResourceLoader();
            #region
            TileContent NowContent = new TileContent()
            {
                Visual = new TileVisual()
                {
                    Branding = TileBranding.Auto,
                    DisplayName = currentCity.City,
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            PeekImage = uri == null ? null : (settings.Preferences.TransparentTile ? null : new TilePeekImage()
                            {
                                Source = new TileImageSource(uri.ToString()),
                            }),
                            BackgroundImage = uri == null ? null : (settings.Preferences.TransparentTile ? null : new TileBackgroundImage()
                            {
                                Source = new TileImageSource(uri.ToString()),
                                Overlay = 70
                            }),
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
                            BackgroundImage = uri == null ? null : (settings.Preferences.TransparentTile ? null : new TileBackgroundImage()
                            {
                                Source = new TileImageSource(uri.ToString()),
                                Overlay = 70
                            }),
                            Children =
                                       {
                                new TileGroup()
                                {
                                    Children=
                                    {
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex].Date.ToString("ddd"),
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileImage()
                                                {
                                                    Source = new TileImageSource("Assets/Tile/" + (string)ctoiConverter.Convert(model.NowWeather.Now.Condition,null,isNight,null))
                                                },
                                                new TileText()
                                                {
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.Caption
                                                },
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
                                                    Style = TileTextStyle.CaptionSubtle,
                                                    Align = TileTextAlign.Center
                                                },
                                                new TileText()
                                                {
                                                    Text = (string)ctosConverter.Convert(model.NowWeather.Now.Condition,null,null,null),
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.Caption
                                                },

                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex].HighTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex].LowTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
                                                    Style = TileTextStyle.CaptionSubtle,
                                                    Align = TileTextAlign.Center
                                                }
                                            }
                                        },
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.CaptionSubtle
                                                },
                                                new TileText()
                                                {
                                                    Text = loader.GetString("ScaleText"),
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.CaptionSubtle
                                                },
                                                new TileText()
                                                {
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.CaptionSubtle
                                                },
                                                new TileText()
                                                {
                                                    Text = loader.GetString("HumText"),
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.CaptionSubtle
                                                },
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

                                                    Style = TileTextStyle.CaptionSubtle,
                                                    Align = TileTextAlign.Left
                                                },
                                                new TileText()
                                                {
                                                    Text = model.NowWeather.Wind.Speed.Actual(settings.Preferences.SpeedParameter) +
                                                    model.NowWeather.Wind.Speed.DanWei(settings.Preferences.SpeedParameter),
                                                    Align = TileTextAlign.Left,
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {

                                                    Style = TileTextStyle.CaptionSubtle,
                                                    Align = TileTextAlign.Left
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex].Humidity.ToString() + "%",
                                                    Align = TileTextAlign.Left,
                                                    Style = TileTextStyle.Caption
                                                },
                                            }
                                        },
                                    }
                                }
                                       },
                            PeekImage = uri == null ? null : (settings.Preferences.TransparentTile ? null : new TilePeekImage()
                            {
                                Source = new TileImageSource(uri.ToString()),
                            }),
                        }
                    },
                    TileLarge = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = uri == null ? null : (settings.Preferences.TransparentTile ? null : new TileBackgroundImage()
                            {
                                Source = new TileImageSource(uri.ToString()),
                                Overlay = 70
                            }),
                            Children =
                            {
                                            new TileGroup()
                                            {
                                             Children =
                                             {
                                                 new TileSubgroup()
                                        {
                                            Weight = 30,
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
                                            TextStacking = TileTextStacking.Center,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex].Date.ToString("ddd"),
                                                    Style = TileTextStyle.Base
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex].HighTemp.Actual(settings.Preferences.TemperatureParameter).ToString() + '~' + model.DailyForecast[todayIndex].LowTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
                                                    Align = TileTextAlign.Auto,
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                     Text = glanceFull,
                                                     Align = TileTextAlign.Auto,
                                                     Style = TileTextStyle.CaptionSubtle,
                                                     Wrap = true
                                                }
                                            }
                                        },
                                            },
                                           },
                                            new TileText(),
                                            new TileGroup()
                                            {
                                             Children =
                                             {
                                                 new TileSubgroup()
                                        {
                                            Weight = 30,
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
                                            TextStacking = TileTextStacking.Center,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].Date.ToString("ddd"),
                                                    Style = TileTextStyle.Base
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex + 1].HighTemp.Actual(settings.Preferences.TemperatureParameter).ToString() + '~' + model.DailyForecast[todayIndex + 1].LowTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
                                                    Align = TileTextAlign.Auto,
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                     Text = loader.GetString("ScaleText")+": "+model.DailyForecast[todayIndex+1].Wind.Speed.Actual(settings.Preferences.SpeedParameter) +
                                                    model.DailyForecast[todayIndex+1].Wind.Speed.DanWei(settings.Preferences.SpeedParameter),
                                                     Align = TileTextAlign.Auto,
                                                     Style = TileTextStyle.CaptionSubtle,
                                                },
                                                new TileText()
                                                {
                                                     Text = loader.GetString("HumText")+": "+model.DailyForecast[todayIndex+1].Humidity.ToString() + "%",
                                                     Align = TileTextAlign.Auto,
                                                     Style = TileTextStyle.CaptionSubtle,
                                                     Wrap = true
                                                }
                                            }
                                        },
                                                }
                                            }

                            },
                            PeekImage = uri == null ? null : (settings.Preferences.TransparentTile ? null : new TilePeekImage()
                            {
                                Source = new TileImageSource(uri.ToString()),
                            }),
                        }
                    }
                }
            };
            #endregion
            return NowContent;
        }

        public static async Task<XmlDocument> CreateToast(HeWeatherModel model, CitySettingsModel currentCity, SettingsModel settings, DateTime DueTime)
        {
            var glance = Glance.GenerateGlanceDescription(model, false, settings.Preferences.TemperatureParameter, DueTime);
            var ctos = new ConditiontoTextConverter();

            var dueIndex = Array.FindIndex(model.DailyForecast, x =>
            {
                return x.Date.Date == DueTime.Date;
            });
            var uri = await settings.Immersive.GetCurrentBackgroundAsync(model.DailyForecast[dueIndex].Condition.DayCond, false);
            var loader = new ResourceLoader();
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
                        Text = string.Format(loader.GetString("Today_Weather"), currentCity.City)
                    },
                    BodyTextLine1 = new ToastText()
                    {
                        Text = ctos.Convert(model.DailyForecast[dueIndex].Condition.DayCond, null, null, null) + ", "
                        + ((model.DailyForecast[dueIndex].HighTemp + model.DailyForecast[dueIndex].LowTemp) / 2).Actual(settings.Preferences.TemperatureParameter)
                    },
                    BodyTextLine2 = new ToastText()
                    {
                        Text = glance
                    },
                }
            };
            var xml = toast.GetXml();
            var e = xml.CreateElement("image");
            e.SetAttribute("placement", "hero");
            e.SetAttribute("src", uri.AbsoluteUri);
            var b = xml.GetElementsByTagName("binding");
            b.Item(0).AppendChild(e);
            return xml;
        }

        public static bool CalcIsNight(DateTime updateTime, TimeSpan sunRise, TimeSpan sunSet, Models.Location geoPoint)
        {
            if (sunRise == default(TimeSpan) || sunSet == default(TimeSpan))
            {
                sunRise = SunRiseSet.GetRise(geoPoint, updateTime);
                sunSet = SunRiseSet.GetSet(geoPoint, updateTime);
            }
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

        public static async Task<List<TileContent>> CreateAll(CitySettingsModel currentCity, HeWeatherModel model, DateTime desiredDateTimeinThatRegion)
        {
            var todayIndex = Array.FindIndex(model.DailyForecast, x =>
            {
                return x.Date.Date == desiredDateTimeinThatRegion.Date;
            });
            var hourIndex = Array.FindIndex(model.HourlyForecast, x =>
            {
                return (x.DateTime - desiredDateTimeinThatRegion).TotalSeconds > 0;
            });
            if (desiredDateTimeinThatRegion.Hour < 7)
            {
                todayIndex--;
            }
            if (todayIndex < 0)
            {
                todayIndex = 0;
            }
            if (hourIndex < 0)
            {
                hourIndex = 0;
            }
            var sunRise = model.DailyForecast[todayIndex].SunRise;
            var sunSet = model.DailyForecast[todayIndex].SunSet;
            var currentTime = desiredDateTimeinThatRegion;

            var settings = SettingsModel.Get();
            var isNight = CalcIsNight(currentTime, sunRise, sunSet, new Models.Location(currentCity.Latitude, currentCity.Longitude));
            Uri uri = await settings.Immersive.GetCurrentBackgroundAsync(model.NowWeather.Now.Condition, isNight);

            var glance = Glance.GenerateShortDescription(model, isNight);
            var glanceFull = Glance.GenerateGlanceDescription(model, isNight, settings.Preferences.TemperatureParameter, desiredDateTimeinThatRegion);
            var lockdetial = Glance.GenerateLockDetailDescription(model, isNight, settings.Preferences.TemperatureParameter, desiredDateTimeinThatRegion);
            var noramlTile = GenerateNormalTile(model, isNight, glance, glanceFull, uri, todayIndex, currentCity, settings);
            var nowTile = GenerateNowTile(model, isNight, uri, glanceFull, lockdetial, todayIndex, currentCity, settings);
            var forecastTile = GenerateForecastTile(model, isNight, uri, glanceFull, lockdetial, todayIndex, currentCity, settings);
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

        private static TileContent GenerateForecastTile(HeWeatherModel model, bool isNight, Uri uri, string glanceFull, string lockdetial, int todayIndex, CitySettingsModel currentCity, SettingsModel settings)
        {
            var ctosConverter = new ConditiontoTextConverter();
            var ctoiConverter = new ConditiontoImageConverter();
            var forecaset = new TileContent()
            {
                Visual = new TileVisual()
                {
                    DisplayName = currentCity.City,
                    Branding = TileBranding.NameAndLogo,
                    LockDetailedStatus1 = currentCity.City + "  " + model.NowWeather.Temprature.Actual(settings.Preferences.TemperatureParameter) + "\r\n" + lockdetial,
                    TileLarge = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = uri == null ? null : (settings.Preferences.TransparentTile ? null : new TileBackgroundImage()
                            {
                                Source = new TileImageSource(uri.ToString()),
                                Overlay = 70
                            }),
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
                                                    Text = model.DailyForecast.Length > todayIndex + 3 ? model.DailyForecast[todayIndex+3].Date.ToString("ddd") : "",
                                                },
                                                new TileImage()
                                                {
                                                    Source = model.DailyForecast.Length > todayIndex + 3 ? new TileImageSource("Assets/Tile/" + (string)ctoiConverter.Convert(isNight?model.DailyForecast[todayIndex+3].Condition.NightCond:model.DailyForecast[todayIndex+3].Condition.DayCond,null,isNight,null)) : new TileImageSource(""),
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
                                                    Text = model.DailyForecast.Length > todayIndex + 3 ? model.DailyForecast[todayIndex+3].HighTemp.Actual(settings.Preferences.TemperatureParameter) : "",
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast.Length > todayIndex + 3 ? model.DailyForecast[todayIndex+3].LowTemp.Actual(settings.Preferences.TemperatureParameter) : "",
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
                                                    Text = model.DailyForecast.Length > todayIndex + 4 ? model.DailyForecast[todayIndex+4].Date.ToString("ddd") : "",

                                                },
                                                new TileImage()
                                                {
                                                    Source = model.DailyForecast.Length > todayIndex + 4 ?  new TileImageSource("Assets/Tile/" + (string)ctoiConverter.Convert(isNight ? model.DailyForecast[todayIndex+4].Condition.NightCond : model.DailyForecast[todayIndex+4].Condition.DayCond, null, isNight, null)) : new TileImageSource(""),
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
                                                    Text = model.DailyForecast.Length > todayIndex + 4 ? model.DailyForecast[todayIndex+4].HighTemp.Actual(settings.Preferences.TemperatureParameter) : "",
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast.Length > todayIndex + 4 ? model.DailyForecast[todayIndex+4].LowTemp.Actual(settings.Preferences.TemperatureParameter) : "",
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
                            BackgroundImage = uri == null ? null : (settings.Preferences.TransparentTile ? null : new TileBackgroundImage()
                            {
                                Source = new TileImageSource(uri.ToString()),
                                Overlay = 70
                            }),
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
                            BackgroundImage = uri == null ? null : (settings.Preferences.TransparentTile ? null : new TileBackgroundImage()
                            {
                                Source = new TileImageSource(uri.ToString()),
                                Overlay = 70
                            }),
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


        public static string[] CalculateWeatherAlarm(HeWeatherModel model, CitySettingsModel currentCityModel, SettingsModel settings, DateTime desiredDateTimeinThatRegion)
        {
            var l = new ResourceLoader();
            var todayIndex = Array.FindIndex(model.DailyForecast, x =>
            {
                return x.Date.Date == desiredDateTimeinThatRegion.Date;
            });
            var hourIndex = Array.FindIndex(model.HourlyForecast, x =>
            {
                return (x.DateTime - desiredDateTimeinThatRegion).TotalSeconds > 0;
            });
            if (desiredDateTimeinThatRegion.Hour < 7)
            {
                todayIndex--;
            }
            if (todayIndex < 0)
            {
                todayIndex = 0;
            }
            if (hourIndex < 0)
            {
                hourIndex = 0;
            }
            List<string> str = new List<string>();
            var ctos = new ConditiontoTextConverter();
            switch (model.DailyForecast[todayIndex + 1].Condition.DayCond)
            {
                case WeatherCondition.gale:
                case WeatherCondition.strong_gale:
                case WeatherCondition.storm:
                case WeatherCondition.violent_storm:
                case WeatherCondition.hurricane:
                case WeatherCondition.tornado:
                case WeatherCondition.tropical_storm:
                    str.Add(currentCityModel.City + l.GetString("ToastGale"));
                    break;
                case WeatherCondition.heavy_shower_rain:
                    str.Add(currentCityModel.City + l.GetString("ToastRain"));
                    break;
                case WeatherCondition.thundershower:
                case WeatherCondition.heavy_thunderstorm:
                    str.Add(currentCityModel.City + l.GetString("ToastThunder"));
                    break;
                case WeatherCondition.hail:
                    str.Add(currentCityModel.City + l.GetString("ToastHail"));
                    break;
                case WeatherCondition.heavy_rain:
                case WeatherCondition.extreme_rain:
                case WeatherCondition.storm_rain:
                case WeatherCondition.heavy_storm_rain:
                case WeatherCondition.severe_storm_rain:
                    str.Add(currentCityModel.City + l.GetString("ToastRain"));
                    break;
                case WeatherCondition.freezing_rain:
                    str.Add(currentCityModel.City + l.GetString("ToastFreeze"));
                    break;
                case WeatherCondition.heavy_snow:
                case WeatherCondition.snowstorm:
                    str.Add(currentCityModel.City + l.GetString("ToastSnow"));
                    break;
                case WeatherCondition.volcanic_ash:
                case WeatherCondition.duststorm:
                case WeatherCondition.sandstorm:
                    str.Add(currentCityModel.City + l.GetString("ToastSand"));
                    break;
                default:
                    break;
            }
            if (str.IsNullorEmpty())
            {
                switch (model.DailyForecast[todayIndex + 1].Condition.NightCond)
                {
                    case WeatherCondition.gale:
                    case WeatherCondition.strong_gale:
                    case WeatherCondition.storm:
                    case WeatherCondition.violent_storm:
                    case WeatherCondition.hurricane:
                    case WeatherCondition.tornado:
                    case WeatherCondition.tropical_storm:
                        str.Add(currentCityModel.City + l.GetString("ToastGale"));
                        break;
                    case WeatherCondition.heavy_shower_rain:
                        str.Add(currentCityModel.City + l.GetString("ToastRain"));
                        break;
                    case WeatherCondition.thundershower:
                    case WeatherCondition.heavy_thunderstorm:
                        str.Add(currentCityModel.City + l.GetString("ToastThunder"));
                        break;
                    case WeatherCondition.hail:
                        str.Add(currentCityModel.City + l.GetString("ToastHail"));
                        break;
                    case WeatherCondition.heavy_rain:
                    case WeatherCondition.extreme_rain:
                    case WeatherCondition.storm_rain:
                    case WeatherCondition.heavy_storm_rain:
                    case WeatherCondition.severe_storm_rain:
                        str.Add(currentCityModel.City + l.GetString("ToastRain"));
                        break;
                    case WeatherCondition.freezing_rain:
                        str.Add(currentCityModel.City + l.GetString("ToastFreeze"));
                        break;
                    case WeatherCondition.heavy_snow:
                    case WeatherCondition.snowstorm:
                        str.Add(currentCityModel.City + l.GetString("ToastSnow"));
                        break;
                    case WeatherCondition.volcanic_ash:
                    case WeatherCondition.duststorm:
                    case WeatherCondition.sandstorm:
                        str.Add(currentCityModel.City + l.GetString("ToastSand"));
                        break;
                    default:
                        break;
                }
            }
            if (str.IsNullorEmpty())
            {
                if ((((model.DailyForecast[todayIndex + 1].LowTemp + model.DailyForecast[todayIndex + 1].HighTemp) / 2).Celsius - ((model.DailyForecast[todayIndex].LowTemp + model.DailyForecast[todayIndex].HighTemp) / 2).Celsius) > 8)
                {
                    str.Add(currentCityModel.City + l.GetString("ToastHot"));
                }
                else if ((((model.DailyForecast[todayIndex + 1].LowTemp + model.DailyForecast[todayIndex + 1].HighTemp) / 2).Celsius - ((model.DailyForecast[todayIndex].LowTemp + model.DailyForecast[todayIndex].HighTemp) / 2).Celsius) < -8)
                {
                    str.Add(currentCityModel.City + l.GetString("ToastCold"));
                }
            }
            if (!str.IsNullorEmpty())
            {
                str.Add(ctos.Convert(model.DailyForecast[todayIndex + 1].Condition.DayCond, null, null, null) + "  " + model.DailyForecast[todayIndex + 1].LowTemp.Actual(settings.Preferences.TemperatureParameter).ToString() + "~" + model.DailyForecast[todayIndex + 1].HighTemp.Actual(settings.Preferences.TemperatureParameter).ToString());
            }
            return str.ToArray();
        }

        public static ToastContent CreateAlertToast(HeWeatherModel fetchresult, CitySettingsModel currentCityModel)
        {
            var lo = new ResourceLoader();
            var action = new ToastActionsCustom();
            var button = new ToastButton(lo.GetString("Known"), "Today_Alert_Dismiss");
            button.ActivationType = ToastActivationType.Background;
            action.Buttons.Add(button);
            action.Buttons.Add(new ToastButtonDismiss(lo.GetString("Dismiss")));
            var alarm = fetchresult.Alarms[0];
            ToastContent t = new ToastContent()
            {
                Scenario = ToastScenario.Reminder,
                Launch = currentCityModel.Id,
                Actions = action,
                Visual = new ToastVisual()
                {
                    TitleText = new ToastText()
                    {
                        Text = alarm.Title
                    },
                    BodyTextLine1 = new ToastText()
                    {
                        Text = alarm.Text
                    }
                }
            };
            return t;
        }


        public static ToastContent CreateAlarmToast(string[] str, CitySettingsModel currentCityModel)
        {
            var lo = new ResourceLoader();
            var action = new ToastActionsCustom();
            var button = new ToastButton(lo.GetString("Known"), "Today_Alarm_Dismiss");
            button.ActivationType = ToastActivationType.Background;
            action.Buttons.Add(button);
            action.Buttons.Add(new ToastButtonDismiss(lo.GetString("Okay")));
            ToastContent t = new ToastContent()
            {
                Scenario = ToastScenario.Reminder,
                Launch = currentCityModel.Id,
                Actions = action,
                Visual = new ToastVisual()
                {
                    TitleText = new ToastText()
                    {
                        Text = str[0]
                    },
                    BodyTextLine1 = new ToastText()
                    {
                        Text = str[1]
                    }
                }
            };
            return t;
        }

        private static TileContent GenerateNowTile(HeWeatherModel model, bool isNight, Uri uri, string glanceFull, string lockdetial, int todayIndex, CitySettingsModel currentCity, SettingsModel settings)
        {
            var ctosConverter = new ConditiontoTextConverter();
            var ctoiConverter = new ConditiontoImageConverter();
            var loader = new ResourceLoader();
            var now = new TileContent()
            {
                Visual = new TileVisual()
                {
                    LockDetailedStatus1 = currentCity.City + "  " + model.NowWeather.Temprature.Actual(settings.Preferences.TemperatureParameter) + "\r\n" + lockdetial,
                    DisplayName = currentCity.City,
                    Branding = TileBranding.NameAndLogo,
                    TileMedium = new TileBinding()
                    {
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = uri == null ? null : (settings.Preferences.TransparentTile ? null : new TileBackgroundImage()
                            {
                                Source = new TileImageSource(uri.ToString()),
                                Overlay = 70
                            }),
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
                            BackgroundImage = uri == null ? null : (settings.Preferences.TransparentTile ? null : new TileBackgroundImage()
                            {
                                Source = new TileImageSource(uri.ToString()),
                                Overlay = 70
                            }),
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
                            TextStacking = TileTextStacking.Center,
                            Children =
                            {
                                new TileText()
                                {
                                    Text = model.NowWeather.Temprature.Actual(settings.Preferences.TemperatureParameter),
                                    Align = TileTextAlign.Center,
                                    Style = TileTextStyle.Body
                                }
                            }
                        }
                    },
                    TileWide = new TileBinding()
                    {
                        DisplayName = currentCity.City,
                        Branding = TileBranding.NameAndLogo,
                        Content = new TileBindingContentAdaptive()
                        {
                            BackgroundImage = uri == null ? null : (settings.Preferences.TransparentTile ? null : new TileBackgroundImage()
                            {
                                Source = new TileImageSource(uri.ToString()),
                                Overlay = 70
                            }),
                            Children =
                                       {
                                new TileGroup()
                                {
                                    Children=
                                    {
                                        new TileSubgroup()
                                        {
                                            Weight = 1,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].Date.ToString("ddd"),
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileImage()
                                                {
                                                    Source = new TileImageSource("Assets/Tile/" + (string)ctoiConverter.Convert(model.DailyForecast[todayIndex+1].Condition.DayCond,null,isNight,null))
                                                },
                                                new TileText()
                                                {
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.Caption
                                                },
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
                                                    Style = TileTextStyle.CaptionSubtle,
                                                    Align = TileTextAlign.Center
                                                },
                                                new TileText()
                                                {
                                                    Text = (string)ctosConverter.Convert(model.DailyForecast[todayIndex+1].Condition.DayCond,null,null,null),
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.Caption
                                                },

                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].HighTemp.Actual(settings.Preferences.TemperatureParameter).ToString(),
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
                                            Weight = 1,
                                            Children =
                                            {
                                                new TileText()
                                                {
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.CaptionSubtle
                                                },
                                                new TileText()
                                                {
                                                    Text = loader.GetString("ScaleText"),
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.CaptionSubtle
                                                },
                                                new TileText()
                                                {
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.CaptionSubtle
                                                },
                                                new TileText()
                                                {
                                                    Text = loader.GetString("HumText"),
                                                    Align = TileTextAlign.Center,
                                                    Style = TileTextStyle.CaptionSubtle
                                                },
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

                                                    Style = TileTextStyle.CaptionSubtle,
                                                    Align = TileTextAlign.Left
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].Wind.Speed.Actual(settings.Preferences.SpeedParameter) +
                                                    model.NowWeather.Wind.Speed.DanWei(settings.Preferences.SpeedParameter),
                                                    Align = TileTextAlign.Left,
                                                    Style = TileTextStyle.Caption
                                                },
                                                new TileText()
                                                {

                                                    Style = TileTextStyle.CaptionSubtle,
                                                    Align = TileTextAlign.Left
                                                },
                                                new TileText()
                                                {
                                                    Text = model.DailyForecast[todayIndex+1].Humidity.ToString() + "%",
                                                    Align = TileTextAlign.Left,
                                                    Style = TileTextStyle.Caption
                                                },
                                            }
                                        },
                                    }
                                }
                                       },
                        }
                    },
                }
            };
            return now;
        }
    }
}
