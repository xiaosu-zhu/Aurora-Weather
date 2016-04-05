// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.ViewModels;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        private MainPage baba;

        public SettingsPage()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            baba = e.Parameter as MainPage;
            Color c;
            SolidColorBrush s;
            if (Context.Theme == ElementTheme.Dark)
            {
                var d = this.Resources.ThemeDictionaries["Dark"] as ResourceDictionary;
                c = (Color)d["SystemBaseHighColor"];
                s = (SolidColorBrush)d["SystemControlForegroundBaseHighBrush"];
            }
            else
            {
                c = (Color)Resources["SystemBaseHighColor"];
                s = (SolidColorBrush)Resources["SystemControlForegroundBaseHighBrush"];
            }
            baba.ChangeColor(Colors.Transparent, c, s);
        }

        private void SettingsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            baba.NavigatetoSettings((SettingsList.SelectedItem as SettingOption).Option);
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualWidth >= 720)
            {
                baba.NavigatetoSettings((SettingsList.Items[0] as SettingOption).Option);
            }
        }

        private void DonateButton_Click(object sender, RoutedEventArgs e)
        {
            baba.NavigatetoSettings(typeof(DonationPage));
        }
    }
}
