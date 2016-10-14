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
using System.Collections.Generic;
using Com.Aurora.AuWeather.CustomControls;
using Com.Aurora.AuWeather.Tile;

namespace Com.Aurora.AuWeather
{
    public sealed partial class MainPage : Page, IThemeble
    {
        private License.License license;
        private ThreadPoolTimer cancelTimer;
        private uint clickCount = 0;
        public static MainPage Current;

        private static PathIcon cityIcon = new PathIcon
        {
            Data = BindingHelper.StringToPath("F1 M 39.7971,9.99986C 39.7971, 13.2018 37.2022, 15.7967 34.0003, 15.7967C 30.7984, 15.7967 28.2035, 13.2018 28.2035, 9.99986C 28.2035, 6.79795 30.7984, 4.20304 34.0003, 4.20304C 37.2022, 4.20304 39.7971,6.79795 39.7971, 9.99986 Z M 34, 2.60895C 33.5201, 2.60895 33.1301, 2.21995 33.1301, 1.73914L 33.1301, 0.870157C 33.1301, 0.390177 33.5201, 0.000204086 34, 0.000204086C 34.48, 0.000204086 34.87,0.390177 34.87, 0.870157L 34.87, 1.73914C 34.87, 2.21995 34.48, 2.60895 34, 2.60895 Z M 26.609, 10C 26.609, 10.48 26.22, 10.87 25.739, 10.87L 24.87, 10.87C 24.3901, 10.87 24.0001, 10.48 24.0001,10C 24.0001, 9.52002 24.3901, 9.13002 24.87, 9.13002L 25.739, 9.13002C 26.22, 9.13002 26.609, 9.52002 26.609, 10 Z M 34.0001, 17.3911C 34.4801, 17.3911 34.8701, 17.78 34.8701, 18.261L 34.8701,19.13C 34.8701, 19.61 34.4801, 20 34.0001, 20C 33.5201, 20 33.1301, 19.61 33.1301, 19.13L 33.1301, 18.261C 33.1301, 17.78 33.5201, 17.3911 34.0001, 17.3911 Z M 41.3912, 9.99996C 41.3912, 9.51998 41.7802,9.12998 42.261, 9.12998L 43.13, 9.12998C 43.61, 9.12998 44, 9.51998 44, 9.99996C 44, 10.4799 43.61, 10.8699 43.13, 10.8699L 42.261, 10.8699C 41.7802, 10.8699 41.3912, 10.4799 41.3912, 9.99996 Z M 28.3481,3.47843C 28.3481, 3.95844 27.9591, 4.34841 27.4781, 4.34841C 26.9982, 4.34841 26.6092, 3.95844 26.6092, 3.47843C 26.6092, 2.99845 26.9982, 2.60848 27.4781, 2.60848C 27.9591, 2.60848 28.3481, 2.99845 28.3481,3.47843 Z M 27.4786, 15.652C 27.9586, 15.652 28.3486, 16.0409 28.3486, 16.5219C 28.3486, 17.0019 27.9586, 17.3909 27.4786, 17.3909C 26.9987, 17.3909 26.6087, 17.0019 26.6087, 16.5219C 26.6087, 16.0409 26.9987,15.652 27.4786, 15.652 Z M 39.6522, 16.5214C 39.6522, 16.0414 40.0412, 15.6514 40.5222, 15.6514C 41.0021, 15.6514 41.3911, 16.0414 41.3911, 16.5214C 41.3911, 17.0014 41.0021, 17.3914 40.5222, 17.3914C 40.0412,17.3914 39.6522, 17.0014 39.6522, 16.5214 Z M 40.5217, 4.34784C 40.0417, 4.34784 39.6517, 3.95886 39.6517, 3.47788C 39.6517, 2.9979 40.0417, 2.60891 40.5217, 2.60891C 41.0017, 2.60891 41.3916, 2.9979 41.3916,3.47788C 41.3916, 3.95886 41.0017, 4.34784 40.5217, 4.34784 Z "),
        };
        private bool nowPanel_Open;

        public MainPage()
        {
            InitializeComponent();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                TitleBlock.Visibility = Visibility.Collapsed;
            }
            Current = this;
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            MainFrame.Navigate(typeof(NowWeatherPage), this);
            license = new License.License();
            var t = ThreadPool.RunAsync(async (w) =>
            {
                var c = Convert.ToUInt64(RoamingSettingsHelper.ReadSettingsValue("MeetDataSourceOnce"));
                if (c < SystemInfoHelper.GetPackageVersionNum())
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
            PaneList.SelectedIndex = 3;
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
                PaneList.SelectedIndex = 2;
            }
            if (page == typeof(SettingsPage))
            {
                PaneList.SelectedIndex = 3;
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
                if (MainFrame.Content is CitiesPage)
                {
                    var loader = new ResourceLoader();
                    Settings.Icon = new SymbolIcon(Symbol.Setting);
                    Cities.Icon = new SymbolIcon(Symbol.World);
                    Settings.Label = loader.GetString("Settings");
                    Cities.Label = loader.GetString("Cities");
                    Refresh.Label = loader.GetString("Refresh");
                    Refresh.IsEnabled = true;
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
                if (PaneList.SelectedIndex == 3)
                {
                    PaneList_SelectionChanged(null, null);
                }
                else
                {
                    PaneList.SelectedIndex = 3;
                }
                Refresh.Visibility = Visibility.Collapsed;
                Settings.Icon = new SymbolIcon(Symbol.Setting);
                Cities.Icon = new SymbolIcon(Symbol.World);
                Settings.Label = loader.GetString("Settings");
                Cities.Label = loader.GetString("Cities");
                Refresh.Label = loader.GetString("Refresh");
                Today.Icon = cityIcon;
                Today.Label = loader.GetString("Now");
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
                PaneList.SelectedIndex = 2;
                Settings.Icon = new SymbolIcon(Symbol.Add);
                Cities.Icon = new SymbolIcon(Symbol.Edit);
                Cities.Label = loader.GetString("Edit");
                Refresh.Label = loader.GetString("Refresh");
                Settings.Label = loader.GetString("Add");
                Refresh.Visibility = Visibility.Visible;
                Today.Icon = cityIcon;
                Today.Label = loader.GetString("Now");
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
            var p = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
             {
                 var i = PaneList.SelectedIndex;
                 PaneList.SelectionChanged -= PaneList_SelectionChanged;
                 DataContext = null;
                 DateNowConverter.Refresh();
                 DataContext = new MainPageViewModel();
                 PaneList.SelectedIndex = i;
                 PaneList.SelectionChanged += PaneList_SelectionChanged;
             }));
        }

        private void Today_Click(object sender, RoutedEventArgs e)
        {
            var loader = new ResourceLoader();
            if (MainFrame.Content is NowWeatherPage)
            {
                PaneList.SelectedIndex = 1;
                Today.Icon = cityIcon;
                Today.Label = loader.GetString("Now");
            }
            else
            {
                PaneList.SelectedIndex = 0;
                Today.Icon = new SymbolIcon(Symbol.View);
                Today.Label = loader.GetString("Details");
            }
            Settings.Icon = new SymbolIcon(Symbol.Setting);
            Cities.Icon = new SymbolIcon(Symbol.World);
            Settings.Label = loader.GetString("Settings");
            Cities.Label = loader.GetString("Cities");
            Refresh.Label = loader.GetString("Refresh");
            Refresh.Visibility = Visibility.Visible;
            Refresh.IsEnabled = true;
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

        private void NowPanel_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            NowPanelPointerIn.Begin();
        }

        private void NowPanel_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (!nowPanel_Open)
                NowPanelPointerOut.Begin();
        }

        private void NowPanel_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (nowPanel_Open)
            {
                NowPanelClose.Begin();
            }
            else
            {
                NowPanelOpen.Begin();
            }
            nowPanel_Open = !nowPanel_Open;
        }

        private void NowPanel_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            NowPanelPointerClick.Begin();
        }

        private void NowPanelClose_Completed(object sender, object e)
        {
            CitiesPanel.Visibility = Visibility.Collapsed;
        }

        internal void SetCitiesPanel(ICollection<CitySettingsViewModel> cities, int currentIndex)
        {
            CitiesPanel.SelectionChanged -= CitiesPanel_SelectionChanged;
            CitiesPanel.ItemsSource = cities;
            foreach (var item in cities)
            {
                var b = new AppBarButton
                {
                    Icon = new SymbolIcon(Symbol.Globe),
                    Label = item.City,
                    Name = item.Id
                };
                b.Click += B_Click;
                ActionBar.SecondaryCommands.Add(b);
            }
            CitiesPanel.SelectedIndex = currentIndex;

            CitiesPanel.SelectionChanged += CitiesPanel_SelectionChanged;

        }

        private void B_Click(object sender, RoutedEventArgs e)
        {
            var s = SettingsModel.Get();
            var l = new List<CitySettingsViewModel>();
            l.AddRange(CitiesPanel.ItemsSource as ICollection<CitySettingsViewModel>);
            var index = l.FindIndex(x =>
            {
                return x.Id == (sender as AppBarButton).Name;
            });
            CitiesPanel.SelectionChanged -= CitiesPanel_SelectionChanged;
            CitiesPanel.SelectedIndex = index;
            CitiesPanel.SelectionChanged += CitiesPanel_SelectionChanged;
            s.Cities.CurrentIndex = s.Cities.EnableLocate ? index - 1 : index;
            s.Cities.Save();
            MainFrame.Navigate(typeof(NowWeatherPage));
        }

        private void CitiesPanel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var s = SettingsModel.Get();
            var l = new List<CitySettingsViewModel>();
            l.AddRange(CitiesPanel.ItemsSource as ICollection<CitySettingsViewModel>);
            var index = l.FindIndex(x =>
            {
                return x.Id == (CitiesPanel.SelectedItem as CitySettingsViewModel).Id;
            });
            s.Cities.CurrentIndex = s.Cities.EnableLocate ? index - 1 : index;
            s.Cities.Save();
            if (MainFrame.Content is NowWeatherPage)
            {
                MainFrame.Navigate(typeof(NowWeatherPage));
            }
        }

        internal void SetIndex(int currentIndex)
        {
            if (CitiesPanel.Items.Count > currentIndex)
            {
                CitiesPanel.SelectedIndex = currentIndex;
            }
        }

        private void Root_PaneClosed(SplitView sender, object args)
        {
            NowPanelClose.Begin();
            NowPanelPointerOut.Begin();
            nowPanel_Open = false;
        }

        public void ChangeThemeColor(Color color)
        {
            var color1 = Color.FromArgb(Convert.ToByte(color.A * 0.9), color.R, color.G, color.B);
            var color2 = Color.FromArgb(Convert.ToByte(color.A * 0.6), color.R, color.G, color.B);
            var color3 = Color.FromArgb(Convert.ToByte(color.A * 0.8), color.R, color.G, color.B);
            (Resources["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            (Resources["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            (Resources["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            (Resources["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            (Resources["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            (Resources["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            (Resources["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
            if (MainFrame.Content is IThemeble)
            {
                (MainFrame.Content as IThemeble).ChangeThemeColor(color);
            }
        }

        private void MainFrame_Navigated(object sender, Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            if (App.MainColor.A > 0)
                if (MainFrame.Content is IThemeble)
                {
                    (MainFrame.Content as IThemeble).ChangeThemeColor(App.MainColor);
                }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://publisher/?name=Aurora-Studio"));
        }
    }
}
