// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Com.Aurora.Shared.Helpers;
using Windows.UI;
using Windows.UI.Xaml.Media;
using System;
using Com.Aurora.AuWeather.ViewModels;
using Com.Aurora.Shared.Converters;
using Windows.ApplicationModel.Resources;
using Windows.System.Threading;
using Windows.UI.Popups;

namespace Com.Aurora.AuWeather
{
    public sealed partial class MainPage : Page
    {
        private License.License license;

        public MainPage()
        {
            InitializeComponent();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                TitleBlock.Visibility = Visibility.Collapsed;
            }
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            MainFrame.Navigate(typeof(NowWeatherPage), this);
            license = new License.License();
            var t = ThreadPool.RunAsync(async (w) =>
            {
                var b = (int?)RoamingSettingsHelper.ReadSettingsValue("MeetDataSourceOnce");
                if (b == null || b < 3)
                {
                    RoamingSettingsHelper.WriteSettingsValue("MeetDataSourceOnce", 3);
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(async () =>
                     {
                         var loader = new ResourceLoader();
                         var d = new MessageDialog(string.Format(loader.GetString("MeetNewDataSource"), loader.GetString("CaiyunWeather")), loader.GetString("MeetNewDataSourceTitle"));
                         await d.ShowAsync();
                     }));
                }
            });
        }

        private void Hamburger_Click(object sender, RoutedEventArgs e)
        {
            Root.IsPaneOpen = !Root.IsPaneOpen;
        }

        internal void NavigatetoSettings(Type option)
        {
            var loader = new ResourceLoader();
            MainFrame.Navigate(typeof(SettingOptionsPage), option);
            Refresh.Visibility = Visibility.Collapsed;
            Settings.Icon = new SymbolIcon(Symbol.Setting);
            Cities.Icon = new SymbolIcon(Symbol.World);
            Settings.Label = loader.GetString("Settings");
            Cities.Label = loader.GetString("Cities");
            Refresh.Label = loader.GetString("Refresh");
            PaneList.SelectionChanged -= PaneList_SelectionChanged;
            PaneList.SelectedIndex = 2;
            PaneList.SelectionChanged += PaneList_SelectionChanged;
        }

        internal void Navigate(Type page)
        {
            if (page == typeof(NowWeatherPage))
            {
                PaneList.SelectedIndex = 0;
            }
            if (page == typeof(CitiesPage))
            {
                PaneList.SelectedIndex = 1;
            }
            if (page == typeof(SettingsPage))
            {
                PaneList.SelectedIndex = 2;
            }
        }

        internal void ChangeColor(Color back, Color fore, SolidColorBrush foreground)
        {
            UIHelper.ChangeTitlebarButtonColor(back, fore);
            Hamburger.Foreground = foreground;
            TitleBlock.Foreground = foreground;
        }

        internal void ChangeColor(SolidColorBrush foreground)
        {
            Hamburger.Foreground = foreground;
            TitleBlock.Foreground = foreground;
        }

        private void MainPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            if (MainFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                if (MainFrame.Content is SettingOptionsPage && this.ActualWidth >= 720)
                {
                    PaneList.SelectedIndex = 0;
                    return;
                }
                if (MainFrame.Content is NowWeatherPage)
                {
                    e.Handled = false;
                    return;
                }
                MainFrame.GoBack();
            }
        }

        internal void ReloadTheme()
        {
            Context.ReloadTheme();
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
            if (MainFrame.Content is NowWeatherPage)
            {
                return;
            }
            ChangeColor(Colors.Transparent, c, s);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is CitiesPage)
            {
                (MainFrame.Content as CitiesPage).Add();
            }
            else
            {
                var loader = new ResourceLoader();
                PaneList.SelectedIndex = 2;
                Refresh.Visibility = Visibility.Collapsed;
                Settings.Icon = new SymbolIcon(Symbol.Setting);
                Cities.Icon = new SymbolIcon(Symbol.World);
                Settings.Label = loader.GetString("Settings");
                Cities.Label = loader.GetString("Cities");
                Refresh.Label = loader.GetString("Refresh");
            }
        }

        private void Cities_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is CitiesPage)
            {
                (MainFrame.Content as CitiesPage).Edit();
            }
            else
            {
                var loader = new ResourceLoader();
                PaneList.SelectedIndex = 1;
                Settings.Icon = new SymbolIcon(Symbol.Add);
                Cities.Icon = new SymbolIcon(Symbol.Edit);
                Cities.Label = loader.GetString("Edit");
                Refresh.Label = loader.GetString("Refresh");
                Settings.Label = loader.GetString("Add");
                Refresh.Visibility = Visibility.Visible;
                if (!license.IsPurchased)
                {
                    Refresh.IsEnabled = false;
                }
            }
        }

        internal void CitiesPageQuitEditMode()
        {
            var loader = new ResourceLoader();
            Cities.Icon = new SymbolIcon(Symbol.Edit);
            Refresh.Icon = new SymbolIcon(Symbol.Refresh);
            Settings.Icon = new SymbolIcon(Symbol.Add);
            Cities.Label = loader.GetString("Edit");
            Refresh.Label = loader.GetString("Refresh");
            Settings.Label = loader.GetString("Add");
            if (!license.IsPurchased)
            {
                Refresh.IsEnabled = false;
            }
            Settings.IsEnabled = true;
        }

        private void PaneList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainFrame.Navigate((PaneList.SelectedItem as PaneOption).Page, this);
            if ((PaneList.SelectedItem as PaneOption).Page == typeof(CitiesPage))
            {
                var loader = new ResourceLoader();
                Settings.Icon = new SymbolIcon(Symbol.Add);
                Cities.Icon = new SymbolIcon(Symbol.Edit);
                Cities.Label = loader.GetString("Edit");
                Refresh.Label = loader.GetString("Refresh");
                Settings.Label = loader.GetString("Add");
                if (!license.IsPurchased)
                {
                    Refresh.IsEnabled = false;
                }
            }
            else
            {
                var loader = new ResourceLoader();
                Settings.Icon = new SymbolIcon(Symbol.Setting);
                Cities.Icon = new SymbolIcon(Symbol.World);
                Settings.Label = loader.GetString("Settings");
                Cities.Label = loader.GetString("Cities");
                Refresh.Label = loader.GetString("Refresh");
                Refresh.IsEnabled = true;
            }
            if ((PaneList.SelectedItem as PaneOption).Page == typeof(SettingsPage))
            {
                Refresh.Visibility = Visibility.Collapsed;
            }
            else
            {
                Refresh.Visibility = Visibility.Visible;
            }
        }

        internal void ReCalcPaneFormat()
        {
            var i = PaneList.SelectedIndex;
            PaneList.SelectionChanged -= PaneList_SelectionChanged;
            this.DataContext = null;
            DateNowConverter.Refresh();
            this.DataContext = new MainPageViewModel();
            PaneList.SelectedIndex = i;
            PaneList.SelectionChanged += PaneList_SelectionChanged;
        }

        private void Today_Click(object sender, RoutedEventArgs e)
        {
            var loader = new ResourceLoader();
            Settings.Icon = new SymbolIcon(Symbol.Setting);
            Cities.Icon = new SymbolIcon(Symbol.World);
            Settings.Label = loader.GetString("Settings");
            Cities.Label = loader.GetString("Cities");
            Refresh.Label = loader.GetString("Refresh");
            Refresh.Visibility = Visibility.Visible;
            Refresh.IsEnabled = true;
            PaneList.SelectedIndex = 0;
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is NowWeatherPage)
            {
                (MainFrame.Content as NowWeatherPage).Refresh();
            }
            else if (MainFrame.Content is CitiesPage)
            {
                (MainFrame.Content as CitiesPage).Refresh();
            }
        }

        internal void CitiesPageGotoEditMode()
        {
            var loader = new ResourceLoader();
            Cities.Icon = new SymbolIcon(Symbol.Delete);
            Refresh.Icon = new SymbolIcon(Symbol.Cancel);
            Settings.Icon = new SymbolIcon(Symbol.Pin);
            Cities.Label = loader.GetString("Delete");
            Refresh.Label = loader.GetString("Cacel");
            Settings.Label = loader.GetString("Pin");
            if (!license.IsPurchased)
            {
                Settings.IsEnabled = false;
            }
        }

        private async void Like_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName));
        }

        private void PaneList_Loaded(object sender, RoutedEventArgs e)
        {
            PaneList.SelectionChanged -= PaneList_SelectionChanged;
            PaneList.SelectedIndex = 0;
            PaneList.SelectionChanged += PaneList_SelectionChanged;
        }
    }
}
