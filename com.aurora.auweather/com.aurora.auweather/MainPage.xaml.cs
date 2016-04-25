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
using Windows.ApplicationModel.Core;
using Windows.UI.ViewManagement;
using System.Threading.Tasks;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.Shared.Extensions;

namespace Com.Aurora.AuWeather
{
    public sealed partial class MainPage : Page
    {
        private License.License license;
        private ThreadPoolTimer cancelTimer;
        private uint clickCount = 0;

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
                var c = Convert.ToUInt64(RoamingSettingsHelper.ReadSettingsValue("MeetDataSourceOnce"));
#if DEBUG
                if (true)
#else
                if (c < SystemInfoHelper.GetPackageVersionNum())
#endif
                {
                    RoamingSettingsHelper.WriteSettingsValue("MeetDataSourceOnce", SystemInfoHelper.GetPackageVersionNum());
                    await Task.Delay(1000);
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
                    {
                        VersionText.Text = SystemInfoHelper.GetPackageVer();
                        ShowUpdateDetail();
                    }));
                }
                else
                {
                    HideUpdateButton_Click(null, null);
                }
            });
        }

        private void ShowUpdateDetail()
        {
            ShowUpdateDetailAni.Begin();
        }

        private void Hamburger_Click(object sender, RoutedEventArgs e)
        {
            Root.IsPaneOpen = !Root.IsPaneOpen;
        }

        private async void HideUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                HideUpdateDetailAni.Begin();
                UpdatePanel.ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.None;
                UpdatePanel.ManipulationDelta -= UpdatePanel_ManipulationDelta;
                UpdatePanel.ManipulationCompleted -= UpdatePanel_ManipulationCompleted;
                await Task.Delay(1000);

                MainPanel.Children.Remove(UpdatePanel);
                UpdatePanel = null;
            }
            catch (Exception)
            {

            }
        }

        internal void NavigatetoSettings(Type option)
        {
            ApplicationView.GetForCurrentView().ExitFullScreenMode();
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
            else if (Context.Theme == ElementTheme.Light)
            {
                var d = this.Resources.ThemeDictionaries["Light"] as ResourceDictionary;
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
                if (PaneList.SelectedIndex == 2)
                {
                    PaneList_SelectionChanged(null, null);
                }
                else
                {
                    PaneList.SelectedIndex = 2;
                }
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
            if (UpdatePanel != null)
            {
                HideUpdateButton_Click(null, null);
            }
            ApplicationView.GetForCurrentView().ExitFullScreenMode();
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

        internal void ChangeCondition(WeatherCondition condition, bool isNight, string city, Temperature nowL, Temperature nowH)
        {
            NowCondition.SetCondition(condition, isNight);
            NowCity.Text = city;
            var p = TempratureandDegreeConverter.Parameter;
            NowText.Text = condition.GetDisplayName() + " | " + nowL.Actual(p) + '~' + nowH.Actual(p);
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

        private async void TextBlock_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            CreateCancel();
            clickCount++;
#if DEBUG
            if (clickCount == 1)
#else
            if (clickCount == 5)
#endif
            {
                var loader = new ResourceLoader();
                var d = new MessageDialog(loader.GetString("YouKnow"), "_(:з」∠)_");
                d.Commands.Add(new UICommand(loader.GetString("Calculator"), new UICommandInvokedHandler(OpenCalculator)));
                d.Commands.Add(new UICommand("Cancel"));
                d.CancelCommandIndex = 1;
                await d.ShowAsync();
            }
        }

        private async void OpenCalculator(IUICommand command)
        {
            CoreApplicationView newView = CoreApplication.CreateNewView();
            int newViewId = 0;
            await newView.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Frame frame = new Frame();
                frame.Navigate(typeof(Calculator.MainPage), null);
                Window.Current.Content = frame;
                Window.Current.Activate();
                var view = ApplicationView.GetForCurrentView();
                newViewId = view.Id;
            });
            bool viewShown = await ApplicationViewSwitcher.TryShowAsStandaloneAsync(newViewId);
        }

        private void CreateCancel()
        {
            if (cancelTimer != null)
            {
                cancelTimer.Cancel();
            }
            cancelTimer = ThreadPoolTimer.CreateTimer((x) =>
            {
                clickCount = 0;
            }, TimeSpan.FromSeconds(1));
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            if (cancelTimer != null)
            {
                cancelTimer.Cancel();
                cancelTimer = null;
            }
        }

        private void UpdatePanel_ManipulationDelta(object sender, Windows.UI.Xaml.Input.ManipulationDeltaRoutedEventArgs e)
        {
            e.Handled = true;
            UpdatePanelTrans.X += e.Delta.Translation.X;
        }

        private void UpdatePanel_ManipulationCompleted(object sender, Windows.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            e.Handled = true;
            if (UpdatePanelTrans.X >= UpdatePanel.ActualWidth / 5)
            {
                HideUpdateButton_Click(null, null);
            }
            else
            {
                ShowUpdateDetailAni.Begin();
            }
        }
    }
}
