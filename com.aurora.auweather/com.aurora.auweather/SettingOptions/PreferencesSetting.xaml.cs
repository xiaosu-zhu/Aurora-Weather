// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.ViewModels.Events;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Com.Aurora.AuWeather.ViewModels;
using System.Threading.Tasks;
using Com.Aurora.AuWeather.Models;
using Windows.Globalization;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Com.Aurora.AuWeather.CustomControls;
using System;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather.SettingOptions
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class PreferencesSetting : Page, IThemeble
    {
        private bool rootIsWideState;
        private License.License license;

        public PreferencesSetting()
        {
            this.InitializeComponent();
            Context.FetchDataComplete += Context_FetchDataComplete;
            App.Current.Suspending += Current_Suspending;
            license = new License.License();
        }

        private void Current_Suspending(object sender, Windows.ApplicationModel.SuspendingEventArgs e)
        {
            Context.SaveAll();
        }

        private void Context_FetchDataComplete(object sender, FetchDataCompleteEventArgs e)
        {
            var task = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
               {
                   Temp.ItemsSource = Context.Temperature;
                   Wind.ItemsSource = Context.Wind;
                   Speed.ItemsSource = Context.Speed;
                   Length.ItemsSource = Context.Length;
                   Pressure.ItemsSource = Context.Pressure;
                   Year.ItemsSource = Context.Year;
                   Month.ItemsSource = Context.Month;
                   Day.ItemsSource = Context.Day;
                   Hour.ItemsSource = Context.Hour;
                   Minute.ItemsSource = Context.Minute;
                   Week.ItemsSource = Context.Week;
                   Theme.ItemsSource = Context.Theme;
                   LanguageBox.ItemsSource = Context.Languages;

                   Temp.SelectedIndex = Context.Temperature.SelectedIndex;
                   Wind.SelectedIndex = Context.Wind.SelectedIndex;
                   Speed.SelectedIndex = Context.Speed.SelectedIndex;
                   Length.SelectedIndex = Context.Length.SelectedIndex;
                   Pressure.SelectedIndex = Context.Pressure.SelectedIndex;
                   Year.SelectedIndex = Context.Year.SelectedIndex;
                   Hour.SelectedIndex = Context.Hour.SelectedIndex;
                   Month.SelectedIndex = Context.Month.SelectedIndex;
                   Day.SelectedIndex = Context.Day.SelectedIndex;
                   Minute.SelectedIndex = Context.Minute.SelectedIndex;
                   Week.SelectedIndex = Context.Week.SelectedIndex;
                   Theme.SelectedIndex = Context.Theme.SelectedIndex;
                   RefreshFreqSlider.Value = Context.RefreshFreq;
                   LanguageBox.SelectedIndex = Context.Languages.SelectedIndex;
                   var span = TimeSpan.FromMinutes(Context.RefreshFreq);
                   RefreshFreq.Text = span.TotalHours + " hours";

                   Temp.SelectionChanged += Enum_SelectionChanged;
                   Wind.SelectionChanged += Enum_SelectionChanged;
                   Speed.SelectionChanged += Enum_SelectionChanged;
                   Length.SelectionChanged += Enum_SelectionChanged;
                   Pressure.SelectionChanged += Enum_SelectionChanged;
                   Theme.SelectionChanged += Theme_SelectionChanged;
                   Year.SelectionChanged += Format_SelectionChanged;
                   Month.SelectionChanged += Format_SelectionChanged;
                   Day.SelectionChanged += Format_SelectionChanged;
                   Hour.SelectionChanged += Format_SelectionChanged;
                   Minute.SelectionChanged += Format_SelectionChanged;
                   Week.SelectionChanged += Format_SelectionChanged;
                   LanguageBox.SelectionChanged += Language_SelectionChanged;

                   if (Context.Theme.SelectedIndex == 1)
                   {
                       AutoThemeSwitch.Visibility = Visibility.Visible;
                   }
                   if (Context.Theme.SelectedIndex == 0)
                   {
                       AccentColorGrid.Visibility = Visibility.Visible;
                   }

                   switch ((DataSource)Context.Data[Context.Data.SelectedIndex].Value)
                   {
                       case DataSource.HeWeather:
                           HeWeatherRadio.IsChecked = true;
                           break;
                       case DataSource.Caiyun:
                           CaiyunRadio.IsChecked = true;
                           break;
                       case DataSource.Wunderground:
                           WundergroundRadio.IsChecked = true;
                           break;
                       default:
                           HeWeatherRadio.IsChecked = true;
                           break;
                   }

                   if (!license.IsPurchased)
                   {
                       EnableEveryDay.IsEnabled = false;
                       EnableAlarm.IsEnabled = false;
                       RefreshFreq.Text = "N/A";
                       RefreshFreqSlider.IsEnabled = false;
                       LockText.Visibility = Visibility.Visible;
                       WundergroundRadio.IsEnabled = false;
                       NeedDonation.Visibility = Visibility.Visible;
                       Context.EnableEveryDay = false;
                       Context.EnableAlarm = false;
                       Context.EnableEvening = false;
                       Context.EnableMorning = false;
                       Context.NowPanelHeight = 320;
                       Context.AQIHide = false;
                       Context.DetailsHide = false;
                       Context.ForecastHide = false;
                       Context.SuggestHide = false;
                       CustomLayout.IsEnabled = false;
                   }
                   CaiyunRadio.Checked += CaiyunRadio_Checked;
                   HeWeatherRadio.Checked += HeWeatherRadio_Checked;
                   WundergroundRadio.Checked += WundergroundRadio_Checked;
               }));
        }

        private void Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ApplicationLanguages.PrimaryLanguageOverride = ((sender as ComboBox).SelectedItem as LanguageViewModel).Key;
            LanguageChangeText.Visibility = Visibility.Visible;
        }

        private void Theme_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var theme = ((sender as ComboBox).SelectedItem as EnumSelector).Value;
            Context.SetEnumValue(theme);
            if ((RequestedTheme)theme == Models.RequestedTheme.Auto)
            {
                AutoThemeSwitch.Visibility = Visibility.Visible;
                if (!Context.ThemeasRiseSet)
                {
                    StartThemeSwitch.Visibility = Visibility.Visible;
                    EndThemeSwitch.Visibility = Visibility.Visible;
                }
                AccentColorGrid.Visibility = Visibility.Collapsed;
            }
            else
            {
                AutoThemeSwitch.Visibility = Visibility.Collapsed;
                StartThemeSwitch.Visibility = Visibility.Collapsed;
                EndThemeSwitch.Visibility = Visibility.Collapsed;
                if ((RequestedTheme)theme == Models.RequestedTheme.Default)
                {
                    AccentColorGrid.Visibility = Visibility.Visible;
                }
                else
                {
                    AccentColorGrid.Visibility = Visibility.Collapsed;
                }
            }
            Context.ReloadTheme();
            (((this.Parent as Frame).Parent as Grid).Parent as SettingOptionsPage).ReloadTheme();
        }

        private async void Format_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Context.SetFormatValue((sender as ComboBox).Name, (sender as ComboBox).SelectedItem as string);
            await Task.Delay(1000);
            ((Window.Current.Content as Frame).Content as MainPage).ReCalcPaneFormat();
        }

        private void Enum_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Context.SetEnumValue(((sender as ComboBox).SelectedItem as EnumSelector).Value);
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Context.SaveAll();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Context.SaveAll();
        }
        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (ActualWidth >= 720)
            {
                RootGotoWideState();
            }
            else
            {
                RootGotoNarrowState();
            }
        }

        private void RootGotoNarrowState()
        {
            if (rootIsWideState)
            {
                RightPanel.Children.Remove(RightPanelChild);
                LeftPanel.Children.Insert(1, RightPanelChild);
                rootIsWideState = false;
            }
        }

        private void RootGotoWideState()
        {
            if (!rootIsWideState)
            {
                LeftPanel.Children.Remove(RightPanelChild);
                RightPanel.Children.Insert(0, RightPanelChild);
                rootIsWideState = true;
            }
        }

        private void CaiyunRadio_Checked(object sender, RoutedEventArgs e)
        {
            Context.SetSource(DataSource.Caiyun);
        }

        private void HeWeatherRadio_Checked(object sender, RoutedEventArgs e)
        {
            Context.SetSource(DataSource.HeWeather);
        }

        private void WundergroundRadio_Checked(object sender, RoutedEventArgs e)
        {
            Context.SetSource(DataSource.Wunderground);
        }

        private void ColorPicker_ColorPicked(object sender, ColorPickedEventArgs e)
        {
            PickedColorGrid.Background = new SolidColorBrush(e.Color);
            if (e.IsSystemAccent)
            {
                Context.SetColor(Colors.Transparent);
                App.ChangeThemeColor((Color)App.Current.Resources["SystemAccentColor"]);
                App.MainColor = (Color)App.Current.Resources["SystemAccentColor"];
            }
            else
            {
                Context.SetColor(e.Color);
                App.ChangeThemeColor(e.Color);
                App.MainColor = e.Color;
            }
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
        }

        private void ToggleSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            if ((sender as ToggleSwitch).IsOn)
            {
                NowPanelSlider.Minimum = 96;
                NowPanelSlider.Maximum = 256;
                NowPanelSlider.Value = 144;
                NowPanelCity.Visibility = Visibility.Collapsed;
                NowPanelHigh.Visibility = Visibility.Collapsed;
                NowPanelLow.Visibility = Visibility.Collapsed;
                NowPanelLowTemp.Visibility = Visibility.Visible;
                NowPanelTemp.Visibility = Visibility.Collapsed;
            }
            else
            {
                NowPanelSlider.Minimum = 144;
                NowPanelSlider.Maximum = 420;
                NowPanelSlider.Value = 256;
                NowPanelCity.Visibility = Visibility.Visible;
                NowPanelHigh.Visibility = Visibility.Visible;
                NowPanelLow.Visibility = Visibility.Visible;
                NowPanelLowTemp.Visibility = Visibility.Collapsed;
                NowPanelTemp.Visibility = Visibility.Visible;
            }
        }
    }
}
