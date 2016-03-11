using Com.Aurora.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using System.Collections;

namespace Com.Aurora.AuWeather.Models.Settings
{
    internal class Immersive
    {
        private const string sunny = "Background\\Sunny";
        private const string starry = "Background\\Starry";
        private const string cloudy = "Background\\Cloudy";
        private const string rainny = "Background\\Rainny";
        private const string snowy = "Background\\Snowy";
        private const string foggy = "Background\\Foggy";
        private const string haze = "Background\\Haze";

        private const string local = "l.png";

        public ImmersiveBackgroundState Sunny { get; internal set; }
        public ImmersiveBackgroundState Cloudy { get; internal set; }
        public ImmersiveBackgroundState Rainny { get; internal set; }
        public ImmersiveBackgroundState Snowy { get; internal set; }
        public ImmersiveBackgroundState Foggy { get; internal set; }
        public ImmersiveBackgroundState Haze { get; internal set; }
        public ImmersiveBackgroundState Starry { get; internal set; }

        public int SunnyPicked { get; internal set; } = 0;
        public int CloudyPicked { get; internal set; } = 0;
        public int RainnyPicked { get; internal set; } = 0;
        public int SnowyPicked { get; internal set; } = 0;
        public int FoggyPicked { get; internal set; } = 0;
        public int HazePicked { get; internal set; } = 0;
        public int StarryPicked { get; internal set; } = 0;


        internal static async Task<List<KeyValuePair<Uri, string>>> GetThumbnailsFromAssetsAsync(string title)
        {
            var thumbs = new List<KeyValuePair<Uri, string>>();
            switch (title)
            {
                case "Sunny":
                    thumbs = await FileIOHelper.GetThumbnailUrisFromAssetsAsync(sunny);
                    break;
                case "Starry":
                    thumbs = await FileIOHelper.GetThumbnailUrisFromAssetsAsync(starry);
                    break;
                case "Cloudy":
                    thumbs = await FileIOHelper.GetThumbnailUrisFromAssetsAsync(cloudy);
                    break;
                case "Rainny":
                    thumbs = await FileIOHelper.GetThumbnailUrisFromAssetsAsync(rainny);
                    break;
                case "Snowy":
                    thumbs = await FileIOHelper.GetThumbnailUrisFromAssetsAsync(snowy);
                    break;
                case "Foggy":
                    thumbs = await FileIOHelper.GetThumbnailUrisFromAssetsAsync(foggy);
                    break;
                case "Haze":
                    thumbs = await FileIOHelper.GetThumbnailUrisFromAssetsAsync(haze);
                    break;
                default:
                    return null;
            }
            if (thumbs == null || thumbs.Count == 0)
                return null;

            return thumbs;
        }

        internal static async Task<Uri> GetFileFromLocalAsync(string title)
        {
            Uri lFile;
            switch (title)
            {
                case "Sunny":
                    lFile = await FileIOHelper.GetFileUriFromLocalAsync(sunny, local);
                    break;
                case "Starry":
                    lFile = await FileIOHelper.GetFileUriFromLocalAsync(starry, local);
                    break;
                case "Cloudy":
                    lFile = await FileIOHelper.GetFileUriFromLocalAsync(cloudy, local);
                    break;
                case "Rainny":
                    lFile = await FileIOHelper.GetFileUriFromLocalAsync(rainny, local);
                    break;
                case "Snowy":
                    lFile = await FileIOHelper.GetFileUriFromLocalAsync(snowy, local);
                    break;
                case "Foggy":
                    lFile = await FileIOHelper.GetFileUriFromLocalAsync(foggy, local);
                    break;
                case "Haze":
                    lFile = await FileIOHelper.GetFileUriFromLocalAsync(haze, local);
                    break;
                default:
                    return null;
            }
            return lFile;
        }

        public static Immersive Get()
        {
            Immersive instance;
            var container = RoamingSettingsHelper.GetContainer("Immersive");
            container.ReadGroupSettings(out instance);
            return instance;
        }

        internal async Task<Uri> SaveLocalFile(string title, StorageFile file)
        {
            switch (title)
            {
                case "Sunny":
                    Sunny = ImmersiveBackgroundState.Local;
                    return await FileIOHelper.SaveFiletoLocalAsync(sunny, file, local);
                case "Starry":

                    Starry = ImmersiveBackgroundState.Local;
                    return await FileIOHelper.SaveFiletoLocalAsync(starry, file, local);
                case "Cloudy":

                    Cloudy = ImmersiveBackgroundState.Local;
                    return await FileIOHelper.SaveFiletoLocalAsync(cloudy, file, local);
                case "Rainny":

                    Rainny = ImmersiveBackgroundState.Local;
                    return await FileIOHelper.SaveFiletoLocalAsync(rainny, file, local);
                case "Snowy":

                    Snowy = ImmersiveBackgroundState.Local;
                    return await FileIOHelper.SaveFiletoLocalAsync(snowy, file, local);
                case "Foggy":

                    Foggy = ImmersiveBackgroundState.Local;
                    return await FileIOHelper.SaveFiletoLocalAsync(foggy, file, local);
                case "Haze":

                    Haze = ImmersiveBackgroundState.Local;
                    return await FileIOHelper.SaveFiletoLocalAsync(haze, file, local);
                default:
                    return null;
            }
        }

        public void Save()
        {
            var container = RoamingSettingsHelper.GetContainer("Immersive");
            container.WriteGroupSettings(this);
        }

        public async Task<Uri> GetCurrentBackgroundAsync(WeatherCondition condition, bool isNight)
        {
            Uri uri;
            switch (condition)
            {
                case WeatherCondition.unknown:
                    return null;
                case WeatherCondition.sunny:
                case WeatherCondition.windy:
                case WeatherCondition.calm:
                case WeatherCondition.light_breeze:
                case WeatherCondition.moderate:
                case WeatherCondition.fresh_breeze:
                case WeatherCondition.strong_breeze:
                case WeatherCondition.high_wind:
                case WeatherCondition.gale:
                case WeatherCondition.hot:
                    if (isNight)
                    {
                        if (Starry == ImmersiveBackgroundState.Assets)
                        {
                            uri = await FileIOHelper.GetFileUriFromAssetsAsync(starry, StarryPicked);
                        }
                        else if (Starry == ImmersiveBackgroundState.Local)
                        {
                            uri = await FileIOHelper.GetFileUriFromLocalAsync(starry, local);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        if (Sunny == ImmersiveBackgroundState.Assets)
                        {
                            uri = await FileIOHelper.GetFileUriFromAssetsAsync(sunny, SunnyPicked);
                        }
                        else if (Sunny == ImmersiveBackgroundState.Local)
                        {
                            uri = await FileIOHelper.GetFileUriFromLocalAsync(sunny, local);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    break;
                case WeatherCondition.cloudy:
                case WeatherCondition.few_clouds:
                case WeatherCondition.partly_cloudy:
                case WeatherCondition.overcast:
                    if (Cloudy == ImmersiveBackgroundState.Assets)
                    {
                        uri = await FileIOHelper.GetFileUriFromAssetsAsync(cloudy, CloudyPicked);
                    }
                    else if (Cloudy == ImmersiveBackgroundState.Local)
                    {
                        uri = await FileIOHelper.GetFileUriFromLocalAsync(cloudy, local);
                    }
                    else
                    {
                        return null;
                    }
                    break;
                case WeatherCondition.strong_gale:
                case WeatherCondition.storm:
                case WeatherCondition.violent_storm:
                case WeatherCondition.hurricane:
                case WeatherCondition.tornado:
                case WeatherCondition.tropical_storm:
                case WeatherCondition.shower_rain:
                case WeatherCondition.heavy_shower_rain:
                case WeatherCondition.thundershower:
                case WeatherCondition.heavy_thunderstorm:
                case WeatherCondition.hail:
                case WeatherCondition.light_rain:
                case WeatherCondition.moderate_rain:
                case WeatherCondition.heavy_rain:
                case WeatherCondition.extreme_rain:
                case WeatherCondition.drizzle_rain:
                case WeatherCondition.storm_rain:
                case WeatherCondition.heavy_storm_rain:
                case WeatherCondition.severe_storm_rain:
                case WeatherCondition.freezing_rain:
                    if (Rainny == ImmersiveBackgroundState.Assets)
                    {
                        uri = await FileIOHelper.GetFileUriFromAssetsAsync(rainny, RainnyPicked);
                    }
                    else if (Rainny == ImmersiveBackgroundState.Local)
                    {
                        uri = await FileIOHelper.GetFileUriFromLocalAsync(rainny, local);
                    }
                    else
                    {
                        return null;
                    }
                    break;
                case WeatherCondition.light_snow:
                case WeatherCondition.moderate_snow:
                case WeatherCondition.heavy_snow:
                case WeatherCondition.snowstorm:
                case WeatherCondition.sleet:
                case WeatherCondition.rain_snow:
                case WeatherCondition.shower_snow:
                case WeatherCondition.snow_flurry:
                case WeatherCondition.cold:
                    if (Snowy == ImmersiveBackgroundState.Assets)
                    {
                        uri = await FileIOHelper.GetFileUriFromAssetsAsync(snowy, SnowyPicked);
                    }
                    else if (Snowy == ImmersiveBackgroundState.Local)
                    {
                        uri = await FileIOHelper.GetFileUriFromLocalAsync(snowy, local);
                    }
                    else
                    {
                        return null;
                    }
                    break;
                case WeatherCondition.mist:
                case WeatherCondition.foggy:
                    if (Foggy == ImmersiveBackgroundState.Assets)
                    {
                        uri = await FileIOHelper.GetFileUriFromAssetsAsync(foggy, FoggyPicked);
                    }
                    else if (Foggy == ImmersiveBackgroundState.Local)
                    {
                        uri = await FileIOHelper.GetFileUriFromLocalAsync(foggy, local);
                    }
                    else
                    {
                        return null;
                    }
                    break;
                case WeatherCondition.haze:
                case WeatherCondition.sand:
                case WeatherCondition.dust:
                case WeatherCondition.volcanic_ash:
                case WeatherCondition.duststorm:
                case WeatherCondition.sandstorm:
                    if (Haze == ImmersiveBackgroundState.Assets)
                    {
                        uri = await FileIOHelper.GetFileUriFromAssetsAsync(haze, HazePicked);
                    }
                    else if (Haze == ImmersiveBackgroundState.Local)
                    {
                        uri = await FileIOHelper.GetFileUriFromLocalAsync(haze, local);
                    }
                    else
                    {
                        return null;
                    }
                    break;
                default:
                    return null;
            }
            return uri;
        }

        public void Pick(string name, int number)
        {
            switch (name)
            {
                case "Sunny":
                    if (Sunny == ImmersiveBackgroundState.Assets)
                        SunnyPicked = number;
                    break;
                case "Starry":
                    if (Sunny == ImmersiveBackgroundState.Assets)
                        StarryPicked = number;
                    break;
                case "Cloudy":
                    if (Sunny == ImmersiveBackgroundState.Assets)
                        CloudyPicked = number;
                    break;
                case "Rainny":
                    if (Sunny == ImmersiveBackgroundState.Assets)
                        RainnyPicked = number;
                    break;
                case "Snowy":
                    if (Sunny == ImmersiveBackgroundState.Assets)
                        SnowyPicked = number;
                    break;
                case "Foggy":
                    if (Sunny == ImmersiveBackgroundState.Assets)
                        FoggyPicked = number;
                    break;
                case "Haze":
                    if (Sunny == ImmersiveBackgroundState.Assets)
                        HazePicked = number;
                    break;
                default:
                    break;
            }
        }

        internal void CheckLocal(string title, Uri lUri)
        {
            if (lUri == null)
                switch (title)
                {
                    case "Sunny":
                        if (Sunny == ImmersiveBackgroundState.Local)
                            Sunny = ImmersiveBackgroundState.Assets;
                        break;
                    case "Starry":
                        if (Sunny == ImmersiveBackgroundState.Local)
                            Sunny = ImmersiveBackgroundState.Assets;
                        break;
                    case "Cloudy":
                        if (Sunny == ImmersiveBackgroundState.Local)
                            Sunny = ImmersiveBackgroundState.Assets;
                        break;
                    case "Rainny":
                        if (Sunny == ImmersiveBackgroundState.Local)
                            Sunny = ImmersiveBackgroundState.Assets;
                        break;
                    case "Snowy":
                        if (Sunny == ImmersiveBackgroundState.Local)
                            Sunny = ImmersiveBackgroundState.Assets;
                        break;
                    case "Foggy":
                        if (Sunny == ImmersiveBackgroundState.Local)
                            Sunny = ImmersiveBackgroundState.Assets;
                        break;
                    case "Haze":
                        if (Sunny == ImmersiveBackgroundState.Local)
                            Sunny = ImmersiveBackgroundState.Assets;
                        break;
                    default:
                        break;
                }
        }
    }
}
