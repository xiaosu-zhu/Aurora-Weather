// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.Shared.MVVM;
using Com.Aurora.AuWeather.Models;
using Windows.UI;
using Windows.UI.Xaml.Media.Imaging;

namespace Com.Aurora.AuWeather.ViewModels
{
    public class ConditionPathViewModel : ViewModelBase
    {
        #region
        private static readonly string[] conditionEnums = new string[]
        {
            //sunny 0
            "ms-appx:///Assets/Tile/Sun.png",
            //moon 1
            "ms-appx:///Assets/Tile/Moon.png",
            //cloudy 2
            "ms-appx:///Assets/Tile/Cloud.png",
            //sun cloudy 3
            "ms-appx:///Assets/Tile/Sun Cloud.png",
            //moon cloudy 4
            "ms-appx:///Assets/Tile/Moon Cloud.png",
            //sun cloud rain 5
            "ms-appx:///Assets/Tile/Sun Cloud Rain.png",
            //moon cloud rain 6
            "ms-appx:///Assets/Tile/Moon Cloud Rain.png",
            //small rain 7
            "ms-appx:///Assets/Tile/Small Rain.png",
            //rain 8
            "ms-appx:///Assets/Tile/Rain.png",
            //thunder 9
            "ms-appx:///Assets/Tile/Thunder Rain.png",
            //snow 10
            "ms-appx:///Assets/Tile/Snow.png",
            //snow rain 11
            "ms-appx:///Assets/Tile/Snow Rain.png",
            //fog 12
            "ms-appx:///Assets/Tile/Fog.png",
            //haze 13
            "ms-appx:///Assets/Tile/Haze.png",
            //wind 14
            "ms-appx:///Assets/Tile/Wind.png",
            //hot 15
            "ms-appx:///Assets/Tile/Hot.png",
            //cold 16
            "ms-appx:///Assets/Tile/Cold.png",
            //moderate 17
            "ms-appx:///Assets/Tile/Moderate.png"
        };
        #endregion

        internal void SetCondition(WeatherCondition condition, bool isNight = false)
        {
            switch (condition)
            {
                case WeatherCondition.unknown:
                    Source = null; break;
                case WeatherCondition.sunny:
                    SetSunny(isNight);
                    break;
                case WeatherCondition.cloudy:
                case WeatherCondition.few_clouds:
                case WeatherCondition.partly_cloudy:
                    SetSunCloudy(isNight);
                    break;
                case WeatherCondition.overcast:
                    SetCloudy();
                    break;
                case WeatherCondition.windy:
                case WeatherCondition.calm:
                case WeatherCondition.light_breeze:
                    SetSunny(isNight);
                    break;
                case WeatherCondition.moderate:
                case WeatherCondition.fresh_breeze:
                case WeatherCondition.strong_breeze:
                case WeatherCondition.high_wind:
                case WeatherCondition.gale:
                case WeatherCondition.strong_gale:
                case WeatherCondition.storm:
                case WeatherCondition.violent_storm:
                case WeatherCondition.hurricane:
                case WeatherCondition.tornado:
                case WeatherCondition.tropical_storm:
                    SetWind();
                    break;
                case WeatherCondition.shower_rain:
                case WeatherCondition.heavy_shower_rain:
                    SetShower(isNight);
                    break;
                case WeatherCondition.thundershower:
                case WeatherCondition.heavy_thunderstorm:
                case WeatherCondition.hail:
                    SetThunderShower();
                    break;
                case WeatherCondition.light_rain:
                    SetRain(0);
                    break;
                case WeatherCondition.moderate_rain:
                    SetRain(1);
                    break;
                case WeatherCondition.heavy_rain:
                case WeatherCondition.extreme_rain:
                    SetRain(2);
                    break;
                case WeatherCondition.drizzle_rain:
                    SetRain(0);
                    break;
                case WeatherCondition.storm_rain:
                case WeatherCondition.heavy_storm_rain:
                case WeatherCondition.severe_storm_rain:
                    SetRain(2);
                    break;
                case WeatherCondition.freezing_rain:
                    SetSnowRain();
                    break;
                case WeatherCondition.light_snow:
                case WeatherCondition.moderate_snow:
                    SetSnow();
                    break;
                case WeatherCondition.heavy_snow:
                case WeatherCondition.snowstorm:
                    SetSnow();
                    break;
                case WeatherCondition.sleet:
                case WeatherCondition.rain_snow:
                    SetSnowRain();
                    break;
                case WeatherCondition.shower_snow:
                case WeatherCondition.snow_flurry:
                    SetSnow();
                    break;
                case WeatherCondition.mist:
                case WeatherCondition.foggy:
                    SetFog();
                    break;
                case WeatherCondition.haze:
                case WeatherCondition.sand:
                case WeatherCondition.dust:
                case WeatherCondition.volcanic_ash:
                case WeatherCondition.duststorm:
                case WeatherCondition.sandstorm:
                    SetHaze();
                    break;
                case WeatherCondition.hot:
                    SetHot();
                    break;
                case WeatherCondition.cold:
                    SetCold();
                    break;
                default:
                    break;
            }

        }

        private void SetSnowRain()
        {
            Source = new BitmapImage(new System.Uri(conditionEnums[11]));
        }

        private void SetCold()
        {
            Source = new BitmapImage(new System.Uri(conditionEnums[16]));
        }

        private void SetHot()
        {
            Source = new BitmapImage(new System.Uri(conditionEnums[15]));
        }

        private void SetHaze()
        {
            Source = new BitmapImage(new System.Uri(conditionEnums[13]));
        }

        private void SetFog()
        {
            Source = new BitmapImage(new System.Uri(conditionEnums[12]));
        }

        private void SetSnow()
        {
            Source = new BitmapImage(new System.Uri(conditionEnums[10]));
        }

        private void SetRain(int v)
        {
            if (v > 1)
            {
                Source = new BitmapImage(new System.Uri(conditionEnums[8]));
            }
            else if (v > 0)
            {
                Source = new BitmapImage(new System.Uri(conditionEnums[17]));
            }
            else
            {
                Source = new BitmapImage(new System.Uri(conditionEnums[7]));
            }
        }

        private void SetThunderShower()
        {
            Source = new BitmapImage(new System.Uri(conditionEnums[9]));
        }

        private void SetShower(bool isNight)
        {
            if (isNight)
            {
                Source = new BitmapImage(new System.Uri(conditionEnums[6]));
            }
            else
            {
                Source = new BitmapImage(new System.Uri(conditionEnums[5]));
            }
        }

        private void SetWind()
        {
            Source = new BitmapImage(new System.Uri(conditionEnums[14]));
        }

        private void SetCloudy()
        {
            Source = new BitmapImage(new System.Uri(conditionEnums[2]));
        }

        private void SetSunCloudy(bool isNight)
        {
            if (isNight)
            {
                Source = new BitmapImage(new System.Uri(conditionEnums[4]));
            }
            else
            {
                Source = new BitmapImage(new System.Uri(conditionEnums[3]));
            }
        }

        private void SetSunny(bool isNight)
        {
            if (isNight)
            {
                Source = new BitmapImage(new System.Uri(conditionEnums[1]));
            }
            else
            {
                Source = new BitmapImage(new System.Uri(conditionEnums[0]));
            }
        }

        private BitmapImage source;

        public BitmapImage Source
        {
            get
            {
                return source;
            }

            set
            {
                SetProperty(ref source, value);
            }
        }
    }
}
