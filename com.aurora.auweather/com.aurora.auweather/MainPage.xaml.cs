using com.aurora.auweather.Models;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Controls;
using System.Windows;
using Windows.UI.Xaml;
using System;
using com.aurora.auweather.ViewModels;

namespace com.aurora.auweather
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            var view = ApplicationView.GetForCurrentView();
            view.TitleBar.ForegroundColor = Colors.White;
            view.TitleBar.InactiveForegroundColor = Colors.White;

            // button
            view.TitleBar.ButtonInactiveForegroundColor = Colors.Gray;
            view.TitleBar.ButtonForegroundColor = Colors.White;
            view.TitleBar.ButtonHoverForegroundColor = Colors.White;
            view.TitleBar.ButtonPressedForegroundColor = Colors.White;
            view.TitleBar.ButtonBackgroundColor = Colors.Transparent;
            Windows.ApplicationModel.Core.CoreApplication.GetCurrentView().TitleBar.ExtendViewIntoTitleBar = true;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            //SettingsModel origin = new SettingsModel();
            //origin.AllowLocation = false;
            //origin.RefreshState = RefreshState.none;
            //origin.SavedCities = new CitySettingsModel[] {new CitySettingsModel(new Models.HeWeather.CityInfo
            //{
            //    City = "北京", Id = "CN101010100"
            //}),new CitySettingsModel(new Models.HeWeather.CityInfo
            //{
            //    City = "郑州", Id = "CN101180101"
            //}) };
            //origin.SaveSettings();
            //var actual = SettingsModel.ReadSettings();
            MainPageViewModel view = new MainPageViewModel();
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
