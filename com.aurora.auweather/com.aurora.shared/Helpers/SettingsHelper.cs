using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace com.aurora.shared.Helpers
{

    public static class RoamingSettingsHelper
    {
        /// <summary>
        /// Read a RoamingSetting's Value and clear it
        /// </summary>
        /// <param name="key">Setting's Key</param>
        /// <returns>Setting's Value</returns>
        public static object ReadResetSettingsValue(string key)
        {
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(key))
            {
                var value = ApplicationData.Current.RoamingSettings.Values[key];
                ApplicationData.Current.RoamingSettings.Values.Remove(key);
                return value;
            }
            return null;
        }

        /// <summary>
        /// Read a RoamingSetting's Value
        /// </summary>
        /// <param name="key">Setting's Key</param>
        /// <returns>Setting's Value</returns>
        public static object ReadSettingsValue(string key)
        {
            if (ApplicationData.Current.RoamingSettings.Values.ContainsKey(key))
            {
                return ApplicationData.Current.RoamingSettings.Values[key];
            }
            return null;
        }

        /// <summary>
        /// Save/Overwrite a RoamingSetting
        /// </summary>
        /// <param name="key">Setting's Key</param>
        /// <param name="value">Setting's Value</param>
        /// <returns></returns>
        public static bool WriteSettingsValue(string key, object value)
        {
            try
            {
                ApplicationData.Current.RoamingSettings.Values[key] = value;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static ApplicationDataContainer GetContainer(string key)
        {
            return ApplicationData.Current.
                RoamingSettings.CreateContainer(key, ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// Reads a RoamingSettings' container and sets all public instanced properties to source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual">all properties must be ApplicationSettings acceptable, and must have a default constructor</param>
        public static void ReadGroupSettings<T>(out T source)
        {
            var type = typeof(T);
            var obj = Activator.CreateInstance(type);
            var mainContainer = ApplicationData.Current.
                RoamingSettings.CreateContainer(type.Name, ApplicationDataCreateDisposition.Always);
            foreach (var member in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var value = mainContainer.Values[member.Name];
                member.SetValue(obj, value);
            }
            source = (T)obj;
        }

        /// <summary>
        /// Write the source's all public instanced properties to RoamingSettings using container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">all properties must be ApplicationSettings acceptable, and must have a default constructor</param>
        public static void WriteGroupSettings<T>(T source)
        {
            var type = typeof(T);
            var mainContainer = ApplicationData.Current.
                RoamingSettings.CreateContainer(type.Name, ApplicationDataCreateDisposition.Always);
            foreach (var member in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var value = member.GetValue(source, null);
                mainContainer.Values[member.Name] = value;
            }
        }

    }

    public static class LocalSettingsHelper
    {

        /// <summary>
        /// Read a LocalSetting's Value and clear it
        /// </summary>
        /// <param name="key">Setting's Key</param>
        /// <returns>Setting's Value</returns>
        public static object ReadResetSettingsValue(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                var value = ApplicationData.Current.LocalSettings.Values[key];
                ApplicationData.Current.LocalSettings.Values.Remove(key);
                return value;
            }

            return null;
        }

        /// <summary>
        /// Read a LocalSetting's Value
        /// </summary>
        /// <param name="key">Setting's Key</param>
        /// <returns>Setting's Value</returns>
        public static object ReadSettingsValue(string key)
        {
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey(key))
            {
                return ApplicationData.Current.LocalSettings.Values[key];
            }
            return null;
        }

        /// <summary>
        /// Save/Overwrite a LocalSetting
        /// </summary>
        /// <param name="key">Setting's Key</param>
        /// <param name="value">Setting's Value</param>
        /// <returns></returns>
        public static bool WriteSettingsValue(string key, object value)
        {
            try
            {
                ApplicationData.Current.LocalSettings.Values[key] = value;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static ApplicationDataContainer GetContainer(string key)
        {
            return ApplicationData.Current.
                LocalSettings.CreateContainer(key, ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// Reads a LocalSettings' container and sets all public instanced properties to source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual">all properties must be ApplicationSettings acceptable, and must have a default constructor</param>
        public static void ReadGroupSettings<T>(out T source)
        {
            var type = typeof(T);
            var obj = Activator.CreateInstance(type);
            var mainContainer = ApplicationData.Current.
                LocalSettings.CreateContainer(type.Name, ApplicationDataCreateDisposition.Always);
            foreach (var member in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var value = mainContainer.Values[member.Name];
                member.SetValue(obj, value);
            }
            source = (T)obj;
        }

        /// <summary>
        /// Write the source's all public instanced properties to LocalSettings using container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">all properties must be ApplicationSettings acceptable, and must have a default constructor</param>
        public static void WriteGroupSettings<T>(T source)
        {
            var type = typeof(T);
            var mainContainer = ApplicationData.Current.
                LocalSettings.CreateContainer(type.Name, ApplicationDataCreateDisposition.Always);
            foreach (var member in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var value = member.GetValue(source, null);
                mainContainer.Values[member.Name] = value;
            }
        }
    }

    public static class SettingsHelper
    {
        /// <summary>
        /// Read a LocalSetting's Value and clear it
        /// </summary>
        /// <param name="key">Setting's Key</param>
        /// <returns>Setting's Value</returns>
        public static object ReadResetSettingsValue(this ApplicationDataContainer container, string key)
        {
            if (container.Values.ContainsKey(key))
            {
                var value = container.Values[key];
                container.Values.Remove(key);
                return value;
            }

            return null;
        }

        /// <summary>
        /// Read a LocalSetting's Value
        /// </summary>
        /// <param name="key">Setting's Key</param>
        /// <returns>Setting's Value</returns>
        public static object ReadSettingsValue(this ApplicationDataContainer container, string key)
        {
            if (container.Values.ContainsKey(key))
            {
                return container.Values[key];
            }
            return null;
        }

        /// <summary>
        /// Save/Overwrite a LocalSetting
        /// </summary>
        /// <param name="key">Setting's Key</param>
        /// <param name="value">Setting's Value</param>
        /// <returns></returns>
        public static bool WriteSettingsValue(this ApplicationDataContainer container, string key, object value)
        {
            try
            {
                container.Values[key] = value;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static ApplicationDataContainer GetContainer(this ApplicationDataContainer container, string key)
        {
            return container.CreateContainer(key, ApplicationDataCreateDisposition.Always);
        }

        /// <summary>
        /// Reads a LocalSettings' container and sets all public instanced properties to source
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="actual">all properties must be ApplicationSettings acceptable, and must have a default constructor</param>
        public static void ReadGroupSettings<T>(this ApplicationDataContainer container, out T source)
        {
            var type = typeof(T);
            var obj = Activator.CreateInstance(type);
            var mainContainer = container.CreateContainer(type.Name, ApplicationDataCreateDisposition.Always);
            foreach (var member in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var value = mainContainer.Values[member.Name];
                member.SetValue(obj, value);
            }
            source = (T)obj;
        }

        /// <summary>
        /// Write the source's all public instanced properties to LocalSettings using container
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source">all properties must be ApplicationSettings acceptable, and must have a default constructor</param>
        public static void WriteGroupSettings<T>(this ApplicationDataContainer container, T source)
        {
            var type = typeof(T);
            var mainContainer = container.CreateContainer(type.Name, ApplicationDataCreateDisposition.Always);
            foreach (var member in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var value = member.GetValue(source, null);
                mainContainer.Values[member.Name] = value;
            }
        }
    }
}
