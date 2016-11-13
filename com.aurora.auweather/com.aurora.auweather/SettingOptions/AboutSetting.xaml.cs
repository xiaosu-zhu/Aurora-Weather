// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.Shared;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Com.Aurora.AuWeather.Models;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather.SettingOptions
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AboutSetting : Page, INotifyPropertyChanged
    {
        public AboutSetting()
        {
            this.InitializeComponent();
            var p = SettingsModel.Current.Preferences;
            Theme = p.GetTheme();
        }

        private ElementTheme theme;

        public event PropertyChangedEventHandler PropertyChanged;

        public ElementTheme Theme
        {
            get
            {
                return theme;
            }

            set
            {
                theme = value;
                OnPropertyChanged();
            }
        }

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        private async void RateButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName));
        }

        private void Version_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as TextBlock).Text = SystemInfoHelper.GetPackageVer();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("mailto:" + Utils.MAIL_ADDRESS));
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

        private async void FeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            ReportBox.Text = string.Empty;
            EmailBox.Text = string.Empty;
            await FeedbackDialog.ShowAsync();
        }

        private async void FeedbackDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            try
            {
                var log = await FileIOHelper.CreateCacheFileAsync(Guid.NewGuid().ToString() + "_Feedback");
                if (!ReportBox.Text.IsNullorEmpty())
                {
                    await FileIO.AppendTextAsync(log, "User Voice = " + ReportBox.Text + Environment.NewLine);
                    if (!EmailBox.Text.IsNullorEmpty())
                    {
                        if (new EmailAddressAttribute().IsValid(EmailBox.Text))
                        {
                            await FileIO.AppendTextAsync(log, "Email = " + EmailBox.Text + Environment.NewLine);
                            var fileBytes = await FileIOHelper.GetBytesAsync(log);
                            WebHelper.UploadFilesToServer(new Uri(Utils.UPLOAD_CRASH), null, log.Name, "application/octet-stream", fileBytes);
                        }
                        else
                        {
                            EmailBox.PlaceholderText = "Invalid Email address!";
                            await FeedbackDialog.ShowAsync();
                        }

                    }
                    else
                    {
                        EmailBox.PlaceholderText = "Invalid Email address!";
                        await FeedbackDialog.ShowAsync();
                    }
                }
                else
                {
                    ReportBox.PlaceholderText = "Input some opinion";
                    await FeedbackDialog.ShowAsync();
                }

            }
            catch (Exception)
            {
                FeedbackDialog.PrimaryButtonText = "Failed";
                await FeedbackDialog.ShowAsync();
            }
        }

        private void FeedbackDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            FeedbackDialog.Hide();
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("https://github.com/aurora-lzzp/Aurora-Weather"));
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://publisher/?name=Aurora-Studio"));
        }
    }
}
