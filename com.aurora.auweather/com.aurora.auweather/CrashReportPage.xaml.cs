// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CrashReportPage : Page
    {
        private UnhandledExceptionEventArgs crash;

        public CrashReportPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            crash = e.Parameter as UnhandledExceptionEventArgs;
        }

        private async void CrashDialog_Loaded(object sender, RoutedEventArgs e)
        {
            var crashLOG = await FileIOHelper.AppendLogtoCacheAsync(crash.Exception);

            CrashInfo.Text = crash.Message + '\n' + "Error Code:" + crash.Exception.HResult.ToHexString();
            CrashDialog.PrimaryButtonClick += async (m, v) =>
            {
                await Core.CrashReport.Sender.SendFileEmail(crashLOG);
                Application.Current.Exit();
            };
            CrashDialog.SecondaryButtonClick += (a, q) =>
            {
                Application.Current.Exit();
            };
            await CrashDialog.ShowAsync();
        }
    }
}
