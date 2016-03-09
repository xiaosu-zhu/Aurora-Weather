using Com.Aurora.Shared.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace Com.Aurora.Shared.Helpers
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
                var container = ApplicationData.Current.RoamingSettings;
                SettingsHelper.DirectWrite(key, value, container);
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
                var container = ApplicationData.Current.LocalSettings;
                SettingsHelper.DirectWrite(key, value, container);
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
        public static void WriteSettingsValue(this ApplicationDataContainer container, string key, object value)
        {
            if (value is DateTime)
            {
                container.Values[key] = ((DateTime)value).ToBinary();
            }
            else if (value is Enum)
            {
                container.Values[key] = ((Enum)value).ToString();
            }
            else
            {
                container.Values[key] = value;
            }
        }

        public static ApplicationDataContainer GetContainer(this ApplicationDataContainer container, string key)
        {
            return container.CreateContainer(key, ApplicationDataCreateDisposition.Always);
        }

        public static object DirectRead(string key, ApplicationDataContainer subContainer)
        {
            return subContainer.Values[key];
        }

        /// <summary>
        /// 如果值是 DateTime, 作处理（work around）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="container"></param>
        public static void DirectWrite(string key, object value, ApplicationDataContainer container)
        {
            if (value is DateTime)
            {
                container.Values[key] = ((DateTime)value).ToBinary();
            }
            else if (value is Enum)
            {
                container.Values[key] = ((Enum)value).ToString();
            }
            else
            {
                container.Values[key] = value;
            }
        }

        public static bool ReadGroupSettings<T>(this ApplicationDataContainer mainContainer, out T source)
        {
            try
            {
                var type = typeof(T);
                var obj = Activator.CreateInstance(type);
                foreach (var member in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
                {
                    if (member.PropertyType.IsArray)
                    {
                        var subContainer = mainContainer.CreateContainer(member.Name, ApplicationDataCreateDisposition.Always);
                        var res = ReadArraySettings(subContainer);
                        if (member.PropertyType == typeof(DateTime[]))
                        {
                            List<DateTime> times = new List<DateTime>();
                            foreach (var time in res)
                            {
                                times.Add(DateTime.FromBinary((long)time));
                            }
                            member.SetValue(obj, times.ToArray());
                        }
                        else
                        {
                            if (res.IsNullorEmpty())
                                member.SetValue(obj, null);
                            member.SetValue(obj, res);
                        }
                    }
                    else if (member.PropertyType == typeof(DateTime))
                    {
                        member.SetValue(obj, DateTime.FromBinary((long)DirectRead(member.Name, mainContainer)));
                    }
                    // Holy shit! WinRT's type is really different from the legacy type.
                    else if (member.PropertyType.GetTypeInfo().IsEnum)
                    {
                        member.SetValue(obj, Enum.Parse(member.PropertyType, (string)DirectRead(member.Name, mainContainer)));
                    }
                    else
                    {
                        member.SetValue(obj, DirectRead(member.Name, mainContainer));
                    }
                }
                source = (T)obj;
                return true;
            }
            catch (Exception)
            {
                source = default(T);
                return false;
            }
        }

        /// <summary>
        /// 读取数组数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="subContainer"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Array ReadArraySettings(ApplicationDataContainer subContainer)
        {
            int i = (int)subContainer.Values["Count"];
            var list = Array.CreateInstance(DirectRead("0", subContainer).GetType(), i);
            for (int j = 0; j < i; j++)
            {
                list.SetValue(DirectRead(j.ToString(), subContainer), j);
            }
            return list;
        }

        /// <summary>
        /// 写入数组值
        /// </summary>
        /// <param name="subContainer"></param>
        /// <param name="value"></param>
        public static void WriteArraySettings(ApplicationDataContainer subContainer, Array value)
        {
            int i = 0;
            if (value.IsNullorEmpty())
            {
                subContainer.Values["Count"] = 0;
                return;
            }
            foreach (var item in value)
            {
                DirectWrite(i.ToString(), item, subContainer);
                i++;
            }
            subContainer.Values["Count"] = i;
        }

        public static void WriteGroupSettings<T>(this ApplicationDataContainer mainContainer, T source)
        {
            var type = typeof(T);
            foreach (var member in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var value = member.GetValue(source);
                if (value is Array)
                {
                    var subContainer = mainContainer.CreateContainer(member.Name, ApplicationDataCreateDisposition.Always);
                    WriteArraySettings(subContainer, value as Array);
                }
                else
                {
                    DirectWrite(member.Name, value, mainContainer);
                }
            }
        }
    }
}
