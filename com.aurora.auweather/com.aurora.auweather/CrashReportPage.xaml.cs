// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.Storage;
using System.ComponentModel.DataAnnotations;
using Com.Aurora.AuWeather.Shared;
using Com.Aurora.Shared;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CrashReportPage : Page
    {
        private CrashLog crash;


        public CrashReportPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            crash = e.Parameter as CrashLog;
        }

        private async void CrashDialog_Loaded(object sender, RoutedEventArgs e)
        {
            CrashInfo.Text = crash.Message + '\n' + "Error Code:" + crash.HResult.ToHexString();
            CrashDialog.PrimaryButtonClick += async (m, v) =>
            {
                try
                {
                    var crashLOG = await FileIOHelper.AppendLogtoCacheAsync(crash, Guid.NewGuid().ToString());
                    if (!ReportBox.Text.IsNullorEmpty())
                        await FileIO.AppendTextAsync(crashLOG, "User Voice = " + ReportBox.Text + Environment.NewLine);
                    if (!EmailBox.Text.IsNullorEmpty())
                    {
                        if (new EmailAddressAttribute().IsValid(EmailBox.Text))
                        {
                            await FileIO.AppendTextAsync(crashLOG, "Email = " + EmailBox.Text + Environment.NewLine);
                        }
                    }
                    var fileBytes = await FileIOHelper.GetBytesAsync(crashLOG);
                    WebHelper.UploadFilesToServer(new Uri(Utils.UPLOAD_CRASH), null, crashLOG.Name, "application/octet-stream", fileBytes);
                }
                catch (Exception)
                {
                    CrashDialog.PrimaryButtonText = "Failed";
                    await CrashDialog.ShowAsync();
                }
            };
            CrashDialog.SecondaryButtonClick += (a, q) =>
            {
                Application.Current.Exit();
            };
            await CrashDialog.ShowAsync();
        }



        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            switch (ConfirmDelete.Visibility)
            {
                case Visibility.Visible:
                    RoamingSettingsHelper.ClearAllSettings();
                    LocalSettingsHelper.ClearAllSettings();
                    App.Current.Exit();
                    break;
                case Visibility.Collapsed:
                    ConfirmDelete.Visibility = Visibility.Visible;
                    DeleteButton.Content = "OK!";
                    break;
                default:
                    break;
            }
        }
    }
}
