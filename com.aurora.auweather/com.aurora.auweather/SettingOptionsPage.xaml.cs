// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.CustomControls;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.AuWeather.SettingOptions;
using Com.Aurora.AuWeather.ViewModels;
using System;
using System.Linq;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI;
using Windows.UI.Xaml.Media;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class SettingOptionsPage : Page, IThemeble
    {
        private Type curretType;
        private bool isWideState;
        public static SettingOptionsPage Current;

        public SettingOptionsPage()
        {
            this.InitializeComponent();
            Current = this;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            curretType = (e.Parameter as Type);
        }

        private void SettingsList_Loaded(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigated += (s, v) =>
            {
                if (App.MainColor.A > 0)
                    if (MainFrame.Content is IThemeble)
                    {
                        (MainFrame.Content as IThemeble).ChangeThemeColor(App.MainColor);
                    }
            };
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
                MainFrame.Navigate((typeof(LocationNote)));
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
            MainPage.Current.ReloadTheme();
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
    }
}
