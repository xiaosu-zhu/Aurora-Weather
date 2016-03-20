using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.AuWeather.SettingOptions;
using Com.Aurora.AuWeather.ViewModels;
using System;
using System.Linq;
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
            SettingsList.SelectionChanged -= SettingsList_SelectionChanged;
            SettingsList.SelectedItem = (SettingsList.ItemsSource as SettingsList).First(x =>
            {
                return x.Option == curretType;
            });
            if (curretType == typeof(Preferences))
            {
                MainFrame.Navigate((typeof(PreferencesSetting)));
            }
            else if (curretType == typeof(Immersive))
            {
                MainFrame.Navigate((typeof(ImmersiveSetting)));
            }
            else if (curretType == typeof(Models.Settings.Cities))
            {
                MainFrame.Navigate((typeof(CitiesSetting)));
            }
            else if (curretType == typeof(About))
            {
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
            MainFrame.Navigate(typeof(DonationPage));
        }

        internal void ReloadTheme()
        {
            Context.ReloadTheme();
            ((Window.Current.Content as Frame).Content as MainPage).ReloadTheme();
        }
    }
}
