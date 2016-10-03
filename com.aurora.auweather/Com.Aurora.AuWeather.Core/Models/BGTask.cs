using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Com.Aurora.AuWeather.Core.Models
{
    public static class BGTask
    {
        private const string BACKGROUND_ENTRY = "Com.Aurora.AuWeather.Background.BackgroundTask";
        private const string BACKGROUND_NAME = "Aurora Weather Background Service";
        private const string TOAST_HANDLER_NAME = "Aurora Weather Toast Service";
        private const string TOAST_HANDLER_ENTRY = "Com.Aurora.AuWeather.Background.ToastHandler";
        public async static Task RegBGTask(uint frequency, bool isEnabled)
        {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy)
            {
                foreach (var t in BackgroundTaskRegistration.AllTasks)
                {
                    if (t.Value.Name == BACKGROUND_NAME || t.Value.Name == TOAST_HANDLER_NAME)
                    {
                        t.Value.Unregister(true);
                    }
                }


                BackgroundTaskBuilder builder = new BackgroundTaskBuilder()
                {
                    Name = TOAST_HANDLER_NAME,
                    TaskEntryPoint = TOAST_HANDLER_ENTRY
                };
                builder.SetTrigger(new ToastNotificationActionTrigger());
                BackgroundTaskRegistration toastreg = builder.Register();


                if (frequency < 30 || !isEnabled)
                {
                    return;
                }
                TimeTrigger hourlyTrigger = new TimeTrigger(frequency, false);
                SystemCondition userCondition = new SystemCondition(SystemConditionType.InternetAvailable);
                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = BACKGROUND_NAME;
                taskBuilder.TaskEntryPoint = BACKGROUND_ENTRY;
                taskBuilder.AddCondition(userCondition);
                taskBuilder.SetTrigger(hourlyTrigger);
                var registration = taskBuilder.Register();
            }
        }
    }
}
