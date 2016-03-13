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
            baba.ChangeColor(Colors.Transparent, (Color)Resources["SystemBaseHighColor"], (SolidColorBrush)Resources["SystemControlForegroundBaseHighBrush"]);
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
            baba.Navigate(typeof(DonationPage));
        }
    }
}
