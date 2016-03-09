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
            if (container.ReadGroupSettings(out instance))
            {
                return instance;
            }
            return new Immersive();
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

        private static async Task<List<string>> Getstring(string path)
        {
            var list = new List<string>();
            IReadOnlyList<StorageFile> files;
            try
            {
                files = await FileIOHelper.GetFilesFromAssetsAsync(path);
            }
            catch (Exception)
            {
                files = null;
            }

            AddPath(path, false, files, list);
            try
            {
                files = await FileIOHelper.GetFilesFromLocalAsync(path);
            }
            catch (Exception)
            {
                files = null;
            }
            AddPath(path, true, files, list);
            return list;
        }

        private static void AddPath(string path, bool isLocal, IReadOnlyList<StorageFile> files, List<string> list)
        {
            if (files != null)
                foreach (var f in files)
                {
                    var s = isLocal ? "l" : "";
                    list.Add(s + path + "\\" + f.Name);
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
