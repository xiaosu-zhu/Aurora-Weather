using NotificationsExtensions.Tiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace Com.Aurora.AuWeather.Background
{
    public sealed class Background : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();
            
            deferral.Complete();
        }

        public void InstantUpdate(TileContent content)
        {
            Sender.CreateMainTileNotification(content);
        }

        public void InstantUpdateSecondary(TileContent content)
        {

        }
    }
}
