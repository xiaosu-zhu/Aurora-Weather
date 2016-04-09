// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.AuWeather.SettingOptions;
using Com.Aurora.AuWeather.ViewModels;
using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingOptionsPage : Page
    {
        private Type curretType;
        private bool isWideState;

        public SettingOptionsPage()
        {
            this.InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            curretType = (e.Parameter as Type);
        }

        private void SettingsList_Loaded(object sender, RoutedEventArgs e)
        {
            if (curretType == typeof(DonationPage))
            {
                SettingsList.SelectionChanged -= SettingsList_SelectionChanged;
                SettingsList.SelectedIndex = -1;
                SettingsList.SelectionChanged += SettingsList_SelectionChanged;
                MainFrame.Navigate((typeof(DonationPage)));
                return;
            }
            SettingsList.SelectionChanged -= SettingsList_SelectionChanged;
            SettingsList.SelectedItem = (SettingsList.ItemsSource as SettingsList).First(x =>
            {
                return x.Option == curretType;
            });
            var loader = new ResourceLoader();
            if (curretType == typeof(Preferences))
            {
                if (this.ActualWidth < 720)
                {
                    PageTitle.Text = loader.GetString("Preferences");
                }
                else
                {
                    PageTitle.Text = loader.GetString("Settings");
                }
                MainFrame.Navigate((typeof(PreferencesSetting)));
            }
            else if (curretType == typeof(Immersive))
            {
                if (this.ActualWidth < 720)
                {
                    PageTitle.Text = loader.GetString("Immersive_Background");
                }
                else
                {
                    PageTitle.Text = loader.GetString("Settings");
                }
                MainFrame.Navigate((typeof(ImmersiveSetting)));
            }
            else if (curretType == typeof(Models.Settings.Cities))
            {
                if (this.ActualWidth < 720)
                {
                    PageTitle.Text = loader.GetString("Cities_Management");
                }
                else
                {
                    PageTitle.Text = loader.GetString("Settings");
                }
                MainFrame.Navigate((typeof(CitiesSetting)));
            }
            else if (curretType == typeof(About))
            {
                if (this.ActualWidth < 720)
                {
                    PageTitle.Text = loader.GetString("About");
                }
                else
                {
                    PageTitle.Text = loader.GetString("Settings");
                }
                MainFrame.Navigate((typeof(AboutSetting)));
            }
            SettingsList.SelectionChanged += SettingsList_SelectionChanged;
        }

        private void SettingsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            curretType = (SettingsList.SelectedItem as SettingOption).Option;
            SettingsList_Loaded(null, null);
        }

        private void DonateButton_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is DonationPage)
            {
                return;
            }
            SettingsList.SelectionChanged -= SettingsList_SelectionChanged;
            SettingsList.SelectedIndex = -1;
            SettingsList.SelectionChanged += SettingsList_SelectionChanged;
            MainFrame.Navigate(typeof(DonationPage));
        }

        internal void ReloadTheme()
        {
            Context.ReloadTheme();
            ((Window.Current.Content as Frame).Content as MainPage).ReloadTheme();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.ActualWidth >= 720)
            {
                GotoWideState();
            }
            else
            {
                GotoNormalState();
            }
        }

        private void GotoWideState()
        {
            if (isWideState)
            {
                return;
            }
            isWideState = true;
            var loader = new ResourceLoader();
            PageTitle.Text = loader.GetString("Settings");
        }

        private void GotoNormalState()
        {
            if (!isWideState)
            {
                return;
            }
            isWideState = false;
            var loader = new ResourceLoader();
            if (MainFrame.Content is PreferencesSetting)
            {
                PageTitle.Text = loader.GetString("Preferences");
            }
            else if (MainFrame.Content is ImmersiveSetting)
            {
                PageTitle.Text = loader.GetString("Immersive_Background");
            }
            else if (MainFrame.Content is CitiesSetting)
            {
                PageTitle.Text = loader.GetString("Cities_Management");
            }
            else if (MainFrame.Content is AboutSetting)
            {
                PageTitle.Text = loader.GetString("About");
            }
            else
            {
                PageTitle.Text = loader.GetString("Settings");
            }
        }
    }
}
