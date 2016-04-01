// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using NotificationsExtensions.Tiles;
using NotificationsExtensions.Badges;
using System;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.UI.StartScreen;
using System.Collections.Generic;
using NotificationsExtensions.Toasts;

namespace Com.Aurora.AuWeather.Tile
{
    public static class Sender
    {
        public static void CreateMainTileNotification(TileContent content)
        {
            var notification = new TileNotification(content.GetXml());
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        public static void CreateMainTileNotification(TileContent content, TimeSpan duration)
        {
            var notification = new TileNotification(content.GetXml());
            notification.ExpirationTime = DateTimeOffset.UtcNow + duration;
            TileUpdateManager.CreateTileUpdaterForApplication().Update(notification);
        }

        public static void CreateSubTileNotification(TileContent content, string tileName)
        {
            if (SecondaryTile.Exists(tileName))
            {
                var notification = new TileNotification(content.GetXml());
                var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileName);
                updater.Update(notification);
            }
        }

        public static void CreateSubTileNotification(TileContent content, string tileName, TimeSpan duration)
        {
            if (SecondaryTile.Exists(tileName))
            {
                var notification = new TileNotification(content.GetXml());
                notification.ExpirationTime = DateTimeOffset.UtcNow + duration;
                var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileName);
                updater.Update(notification);
            }
        }

        public static void CreateScheduledTileNotification(TileContent content, DateTime dueTime, string id)
        {
            ScheduledTileNotification scheduledTile = new ScheduledTileNotification(content.GetXml(), dueTime);
            scheduledTile.Id = id;
            RemoveSpecScheduledTileNotification(id);
            TileUpdateManager.CreateTileUpdaterForApplication().AddToSchedule(scheduledTile);
        }

        public static void RemoveSpecScheduledTileNotification(string id)
        {
            var notifier = TileUpdateManager.CreateTileUpdaterForApplication();
            var scheduled = notifier.GetScheduledTileNotifications();
            for (var i = 0; i < scheduled.Count; i++)
            {
                if (scheduled[i].Id == id)
                {
                    notifier.RemoveFromSchedule(scheduled[i]);
                }
            }
        }

        public static void CreateScheduledToastNotification(XmlDocument toastXml, DateTime dueTime, string id)
        {
            ScheduledToastNotification scheduledToast = new ScheduledToastNotification(toastXml, dueTime);
            scheduledToast.Id = id;
            RemoveSpecScheduledToastNotification(id);
            ToastNotificationManager.CreateToastNotifier().AddToSchedule(scheduledToast);
        }

        public static void CreateToast(ToastContent content)
        {
            ToastNotification t = new ToastNotification(content.GetXml());
            ToastNotificationManager.CreateToastNotifier().Show(t);
        }

        public static void RemoveSpecScheduledToastNotification(string id)
        {
            var notifier = ToastNotificationManager.CreateToastNotifier();
            var list = notifier.GetScheduledToastNotifications();
            foreach (var toast in list)
            {
                if (toast.Id == id)
                {
                    notifier.RemoveFromSchedule(toast);
                }
            }
        }

        public static void Clear()
        {
            TileUpdateManager.CreateTileUpdaterForApplication().Clear();
        }

        public static void ClearSub(string tileName)
        {
            if (SecondaryTile.Exists(tileName))
            {
                var updater = TileUpdateManager.CreateTileUpdaterForSecondaryTile(tileName);
                updater.Clear();
            }
        }

        public static void CreateMainTileQueue(List<TileContent> list)
        {
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            int i = 0;
            foreach (var item in list)
            {
                if(item == null)
                {
                    continue;
                }
                TileNotification n = new TileNotification(item.GetXml());
                n.Tag = i.ToString();
                TileUpdateManager.CreateTileUpdaterForApplication().Update(n);
                i++;
            }
        }

        public static void CreateBadge(BadgeNumericNotificationContent badge)
        {
            BadgeNotification b = new BadgeNotification(badge.GetXml());
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(b);
        }

        public static void CreateBadge(BadgeGlyphNotificationContent badge)
        {
            BadgeNotification b = new BadgeNotification(badge.GetXml());
            BadgeUpdateManager.CreateBadgeUpdaterForApplication().Update(b);
        }
    }
}
