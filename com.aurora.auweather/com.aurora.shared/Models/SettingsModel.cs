using Com.Aurora.AuWeather.Models.Settings;
using Windows.System.Threading;

namespace Com.Aurora.AuWeather.Models
{
    public class SettingsModel
    {
        public Cities Cities { get; private set; }
        public Immersive Immersive { get; private set; }
        public Preferences Preferences { get; private set; }

        public static SettingsModel Get()
        {
            var s = new SettingsModel();
            s.Cities = Cities.Get();
            s.Immersive = Immersive.Get();
            s.Preferences = Preferences.Get();
            return s;
        }

        public void SaveSettings()
        {
            var task = ThreadPool.RunAsync((work) =>
             {
                 Cities.Save();
                 Immersive.Save();
                 Preferences.Save();
             });
        }
    }

}
