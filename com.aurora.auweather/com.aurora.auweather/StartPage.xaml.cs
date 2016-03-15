using Com.Aurora.AuWeather.SettingOptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System.Threading;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

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

        }
    }
}
