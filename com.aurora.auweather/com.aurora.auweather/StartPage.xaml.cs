using Com.Aurora.AuWeather.SettingOptions;
using System;
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
    public sealed partial class StartPage : Page
    {
        public StartPage()
        {
            this.InitializeComponent();
        }

        private async void TextBlock_Loaded(object sender, RoutedEventArgs e)
        {
            await Task.Delay(640);
            StartAni.Begin();
            MainFrame.Navigate(typeof(CitiesSetting));
            ThreadPoolTimer.CreatePeriodicTimer(async (x) =>
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
                 {
                     rizhuan.Rotation += 0.75;
                 }));
            }, TimeSpan.FromMilliseconds(16));
        }

        private void CompleteButton_Click(object sender, RoutedEventArgs e)
        {
            if ((MainFrame.Content as CitiesSetting).CheckCompleted())
            {
                (MainFrame.Content as CitiesSetting).Complete();
                (Window.Current.Content as Frame).Navigate(typeof(MainPage));
            }

        }
    }
}
