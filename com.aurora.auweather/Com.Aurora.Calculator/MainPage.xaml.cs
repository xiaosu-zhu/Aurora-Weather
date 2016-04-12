using Com.Aurora.Calculator.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.StartScreen;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Com.Aurora.Calculator
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var view = ApplicationView.GetForCurrentView();
            view.TryResizeView(new Size(320, 640));
        }

        private void TextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            double dtemp;
            if (!double.TryParse(sender.Text, out dtemp) && sender.Text != "")
            {
                sender.Text = sender.Text.Remove(--sender.SelectionStart, 1);
            }
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            ((sender as TextBox).DataContext as CalculatorViewModel).ChangeValue((sender as TextBox).Text);
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            (sender as TextBox).TextChanged -= TextBox_TextChanged;
            (sender as TextBox).Text = string.Empty;
            (sender as TextBox).TextChanged += TextBox_TextChanged;
        }

        private async void PinButton_Click(object sender, RoutedEventArgs e)
        {
            SecondaryTile s = new SecondaryTile("Com.Aurora.AuWeather.Calculator", "_(:з」∠)_", "Com.Aurora.AuWeather.Calculator", new Uri("ms-appx:///Assets/Square150x150Logo.png"), TileSize.Square150x150);
            await s.RequestCreateAsync();
        }

        private async void AboutButton_Click(object sender, RoutedEventArgs e)
        {
            var d = new MessageDialog(Windows.ApplicationModel.Package.Current.Id.Version.Major.ToString("0") + "." +
                Windows.ApplicationModel.Package.Current.Id.Version.Minor.ToString("0") + "." +
                Windows.ApplicationModel.Package.Current.Id.Version.Build.ToString("0"), "_(:з」∠)_");
            await d.ShowAsync();
        }

        private async void FeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName));
        }

        private void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            AuWeather.LunarCalendar.CalendarInfo calendar = new AuWeather.LunarCalendar.CalendarInfo();
            LunarText.Text = ("农历 " + calendar.LunarYearSexagenary + "年" + calendar.LunarMonthText + "月" + calendar.LunarDayText + "    " + calendar.SolarTermStr).Trim();
            StarText.Text = calendar.SolarConstellation;
            StoneText.Text = calendar.SolarBirthStone;
        }

        private void DatePicker_DateChanged(object sender, DatePickerValueChangedEventArgs e)
        {
            var d = (sender as DatePicker);
            AuWeather.LunarCalendar.CalendarInfo calendar = new AuWeather.LunarCalendar.CalendarInfo(d.Date.Date);
            LunarText.Text = ("农历 " + calendar.LunarYearSexagenary + "年" + calendar.LunarMonthText + "月" + calendar.LunarDayText + "    " + calendar.SolarTermStr).Trim();
            StarText.Text = calendar.SolarConstellation;
            StoneText.Text = calendar.SolarBirthStone;
        }
    }
}
