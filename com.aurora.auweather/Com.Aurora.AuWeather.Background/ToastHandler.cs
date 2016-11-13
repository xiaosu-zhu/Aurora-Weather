using System;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.Shared.Extensions;
using Windows.ApplicationModel.Background;
using Windows.UI.Notifications;

namespace Com.Aurora.AuWeather.Background
{
    public sealed class ToastHandler : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            var settings = SettingsModel.Current;
            var details = taskInstance.TriggerDetails as ToastNotificationActionTriggerDetail;
            var arguments = details.Argument;
            var input = details.UserInput;
            if (!arguments.IsNullorEmpty())
            {
                switch (arguments)
                {
                    case "Today_Alert_Dismiss":
                        settings.Preferences.LastAlertTime = DateTime.Now;
                        break;
                    case "Today_Alarm_Dismiss":
                        settings.Preferences.LastAlarmTime = DateTime.Now;
                        break;
                }
                settings.Preferences.Save();
                return;
            }

        }
    }
}
