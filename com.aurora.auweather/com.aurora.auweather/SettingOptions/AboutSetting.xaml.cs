using Com.Aurora.AuWeather.Models.Settings;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather.SettingOptions
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class AboutSetting : Page, INotifyPropertyChanged
    {
        public AboutSetting()
        {
            this.InitializeComponent();
            var p = Preferences.Get();
            Theme = p.GetTheme();
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

        private async void FeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName));
        }

        private void Version_Loaded(object sender, RoutedEventArgs e)
        {
            (sender as TextBlock).Text = Windows.ApplicationModel.Package.Current.Id.Version.Major.ToString("0") + "." +
                Windows.ApplicationModel.Package.Current.Id.Version.Minor.ToString("0") + "." +
                Windows.ApplicationModel.Package.Current.Id.Version.Build.ToString("0");
        }
    }
}
