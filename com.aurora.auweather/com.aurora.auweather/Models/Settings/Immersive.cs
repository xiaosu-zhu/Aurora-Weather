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
        private const string cloudy = "Background\\Cloudy";
        private const string rainny = "Background\\Rainny";
        private const string snowy = "Background\\Snowy";
        private const string foggy = "Background\\Foggy";
        private const string haze = "Background\\Haze";

        public ImmersiveBackgroundState Sunny { get; private set; }
        public ImmersiveBackgroundState Cloudy { get; private set; }
        public ImmersiveBackgroundState Rainny { get; private set; }
        public ImmersiveBackgroundState Snowy { get; private set; }
        public ImmersiveBackgroundState Foggy { get; private set; }
        public ImmersiveBackgroundState Haze { get; private set; }

        public int SunnyPicked { get; private set; } = 0;
        public int CloudyPicked { get; private set; } = 0;
        public int RainnyPicked { get; private set; } = 0;
        public int SnowyPicked { get; private set; } = 0;
        public int FoggyPicked { get; private set; } = 0;
        public int HazePicked { get; private set; } = 0;

        public static Immersive Get()
        {
            Immersive instance;
            var container = RoamingSettingsHelper.GetContainer("Immersive");
            container.ReadGroupSettings(out instance);
            if (instance == default(Immersive))
            {
                instance = new Immersive();
            }
            return instance;
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
                    SunnyPicked = number;
                    break;
                case "Cloudy":
                    SunnyPicked = number;
                    break;
                case "Rainny":
                    SunnyPicked = number;
                    break;
                case "Snowy":
                    SunnyPicked = number;
                    break;
                case "Foggy":
                    SunnyPicked = number;
                    break;
                case "Haze":
                    SunnyPicked = number;
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
                files = await FileIOHelper.GetFilesFromAssets(path);
            }
            catch (Exception)
            {
                files = null;
            }

            AddPath(path, false, files, list);
            try
            {
                files = await FileIOHelper.GetFilesFromLocal(path);
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
    }
}
