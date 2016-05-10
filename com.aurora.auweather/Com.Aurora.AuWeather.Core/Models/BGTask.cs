using Com.Aurora.AuWeather.Models;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Com.Aurora.AuWeather.Core.Models
{
    public static class BGTask
    {
        private const string BACKGROUND_ENTRY = "Com.Aurora.AuWeather.Background.BackgroundTask";
        private const string BACKGROUND_NAME = "Aurora Weather";
        public async static Task RegBGTask(uint frequency, bool isEnabled)
        {
            uint freshTime = frequency;
            string entryPoint = BACKGROUND_ENTRY;
            string taskName = BACKGROUND_NAME;
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity)
            {
                foreach (var t in BackgroundTaskRegistration.AllTasks)
                {
                    if (t.Value.Name == taskName)
                    {
                        t.Value.Unregister(true);
                    }
                }
                if (freshTime < 30 || !isEnabled)
                {
                    return;
                }
                TimeTrigger hourlyTrigger = new TimeTrigger(freshTime, false);
                SystemCondition userCondition = new SystemCondition(SystemConditionType.InternetAvailable);
                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = entryPoint;
                taskBuilder.AddCondition(userCondition);
                taskBuilder.SetTrigger(hourlyTrigger);
                var registration = taskBuilder.Register();
            }
        }
    }
}
