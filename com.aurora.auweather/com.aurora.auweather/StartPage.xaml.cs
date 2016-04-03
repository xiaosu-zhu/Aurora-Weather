// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.AuWeather.SettingOptions;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class StartPage : Page, INotifyPropertyChanged
    {
        public StartPage()
        {
            this.InitializeComponent();
            var p = Preferences.Get();
            Theme = p.GetTheme();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                TitleBlock.Visibility = Visibility.Collapsed;
            }
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
            var h = PropertyChanged;
            if (h != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {

            MainFrame.Navigate(typeof(CitiesSetting));
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            if ((MainFrame.Content as CitiesSetting).CheckCompleted())
            {
                (MainFrame.Content as CitiesSetting).Complete();
                (Window.Current.Content as Frame).Navigate(typeof(MainPage));
            }

        }

        private void FlipView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var v = sender as FlipView;
            if (v.SelectedIndex == 0)
            {
                GotoPage0.Begin();
            }
            if (v.SelectedIndex == 1)
            {
                GotoPage1.Begin();
            }
            if (v.SelectedIndex == 2)
            {
                GotoPage2.Begin();
            }
            if (v.SelectedIndex == 3)
            {
                GotoPage3.Begin();
            }
        }

        private void FlipView_Loaded(object sender, RoutedEventArgs e)
        {
            GotoPage0.Begin();
            FlipView.SelectionChanged += FlipView_SelectionChanged;
        }
    }
}
