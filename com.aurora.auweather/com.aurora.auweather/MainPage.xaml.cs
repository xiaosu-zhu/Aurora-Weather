using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Com.Aurora.Shared.Helpers;
using Windows.UI;
using Windows.UI.Xaml.Media;
using System;
using Com.Aurora.AuWeather.ViewModels;
using Com.Aurora.Shared.Converters;

namespace Com.Aurora.AuWeather
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                TitleBlock.Visibility = Visibility.Collapsed;
            }
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            MainFrame.Navigate(typeof(NowWeatherPage));
        }

        private void Hamburger_Click(object sender, RoutedEventArgs e)
        {
            Root.IsPaneOpen = !Root.IsPaneOpen;
        }

        internal void NavigatetoSettings(Type option)
        {
            MainFrame.Navigate(typeof(SettingOptionsPage), option);
        }

        internal void Navigate(Type page)
        {
            MainFrame.Navigate(page);
        }

        internal void ChangeColor(Color transparent, Color black, SolidColorBrush foreground)
        {
            UIHelper.ChangeTitlebarButtonColor(transparent, black);
            Hamburger.Foreground = foreground;
            TitleBlock.Foreground = foreground;
        }

        internal void ChangeColor(Colors colors)
        {
            throw new NotImplementedException();
        }

        private void MainPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            if (MainFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                if (MainFrame.Content is SettingOptionsPage && this.ActualWidth >= 720)
                {
                    MainFrame.Navigate(typeof(NowWeatherPage));
                }
                MainFrame.GoBack();
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {

            MainFrame.Navigate(typeof(SettingsPage), this);
        }

        private void Cities_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(NowWeatherPage));
        }

        private void PaneList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainFrame.Navigate((PaneList.SelectedItem as PaneOption).Page, this);
        }

        internal void ReCalcPaneFormat()
        {
            this.DataContext = null;
            DateNowConverter.Refresh();
            this.DataContext = new MainPageViewModel();
        }

        //private async void Grid_Loaded(object sender, RoutedEventArgs e)
        //{
        //    #region
        //    //string url = "http://apis.baidu.com/heweather/pro/weather";
        //    //string[] param = { "city=zhengzhou" };
        //    //var weatherresult = HeWeatherContract.TrimResult(actual);
        //    //var wa = JsonHelper.FromJson<HeWeatherContract>(weatherresult);
        //    //HeWeatherModel m = new HeWeatherModel(wa);
        //    //string[] param = { "search=allworld" };
        //    //await FileIOHelper.SaveFile(actual, "cityid.txt");
        //    #endregion
        //    //ThreadPool.RunAsync(async work =>
        //    //{
        //    //    var str = await FileIOHelper.ReadStringFromAssets("cityid.txt");
        //    //    var result = JsonHelper.FromJson<CityIdContract>(str);
        //    //    var citys = Models.HeWeather.CityInfo.CreateList(result);
        //    //    allcity = citys;
        //    //    this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
        //    //    {
        //    //        SuggestionBox.TextChanged += SuggestionBox_TextChanged;
        //    //    });
        //    //});
        //}

        //private async void SuggestionBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        //{
        //    // Only get results when it was a user typing, 
        //    // otherwise assume the value got filled in by TextMemberPath 
        //    // or the handler for SuggestionChosen.
        //    if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
        //    {
        //        var text = sender.Text;
        //        await ThreadPool.RunAsync(async work =>
        //         {
        //             var searcharray = text.Split(' ');
        //             StringBuilder searchsb = new StringBuilder(@".*");
        //             foreach (var search in searcharray)
        //             {
        //                 searchsb.Append(search);
        //                 searchsb.Append(@".*");
        //             }
        //             var pattern = searchsb.ToString();
        //             var dataset = allcity.FindAll(x =>
        //              {
        //                  if (x.Country == "中国")
        //                  {
        //                      var pin = PinYinHelper.GetPinyin(x.City);
        //                      return (Regex.IsMatch(pin, pattern, RegexOptions.IgnoreCase) || Regex.IsMatch(x.City, pattern, RegexOptions.IgnoreCase));
        //                  }
        //                  return Regex.IsMatch(x.City, pattern, RegexOptions.IgnoreCase);
        //              });
        //             //Set the ItemsSource to be your filtered dataset
        //             await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
        //               {
        //                   sender.ItemsSource = dataset;
        //               });
        //         });
        //    }
        //}

        //private void SuggestionBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        //{
        //    if (args.ChosenSuggestion != null)
        //    {
        //        // User selected an item from the suggestion list, take an action on it here.
        //    }
        //    else
        //    {
        //        // Use args.QueryText to determine what to do.
        //    }
        //}

        //private void SuggestionBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        //{
        //    // Set sender.Text. You can use args.SelectedItem to build your text string.
        //}
    }
}
