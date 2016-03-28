using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Com.Aurora.Shared.Helpers;
using Windows.UI;
using Windows.UI.Xaml.Media;
using System;
using Com.Aurora.AuWeather.ViewModels;
using Com.Aurora.Shared.Converters;
using Windows.ApplicationModel.Resources;

namespace Com.Aurora.AuWeather
{
    public sealed partial class MainPage : Page
    {
        private License.License license;

        public MainPage()
        {
            InitializeComponent();
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                TitleBlock.Visibility = Visibility.Collapsed;
            }
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            MainFrame.Navigate(typeof(NowWeatherPage), this);
            license = new License.License();
        }

        private void Hamburger_Click(object sender, RoutedEventArgs e)
        {
            Root.IsPaneOpen = !Root.IsPaneOpen;
        }

        internal void NavigatetoSettings(Type option)
        {
            var loader = new ResourceLoader();
            MainFrame.Navigate(typeof(SettingOptionsPage), option);
            Refresh.Visibility = Visibility.Collapsed;
            Settings.Icon = new SymbolIcon(Symbol.Setting);
            Cities.Icon = new SymbolIcon(Symbol.World);
            Settings.Label = loader.GetString("Settings");
            Cities.Label = loader.GetString("Cities");
            Refresh.Label = loader.GetString("Refresh");
        }

        internal void Navigate(Type page)
        {
            MainFrame.Navigate(page, this);
        }

        internal void ChangeColor(Color back, Color fore, SolidColorBrush foreground)
        {
            UIHelper.ChangeTitlebarButtonColor(back, fore);
            Hamburger.Foreground = foreground;
            TitleBlock.Foreground = foreground;
        }

        internal void ChangeColor(SolidColorBrush foreground)
        {
            Hamburger.Foreground = foreground;
            TitleBlock.Foreground = foreground;
        }

        private void MainPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            if (MainFrame.CanGoBack && e.Handled == false)
            {
                e.Handled = true;
                if (MainFrame.Content is SettingOptionsPage && this.ActualWidth >= 720)
                {
                    MainFrame.Navigate(typeof(NowWeatherPage));
                    return;
                }
                if (MainFrame.Content is NowWeatherPage)
                {
                    e.Handled = false;
                    return;
                }
                MainFrame.GoBack();
            }
        }

        internal void ReloadTheme()
        {
            Context.ReloadTheme();
            Color c;
            SolidColorBrush s;
            if (Context.Theme == ElementTheme.Dark)
            {
                var d = this.Resources.ThemeDictionaries["Dark"] as ResourceDictionary;
                c = (Color)d["SystemBaseHighColor"];
                s = (SolidColorBrush)d["SystemControlForegroundBaseHighBrush"];
            }
            else
            {
                c = (Color)Resources["SystemBaseHighColor"];
                s = (SolidColorBrush)Resources["SystemControlForegroundBaseHighBrush"];
            }
            ChangeColor(Colors.Transparent, c, s);
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is CitiesPage)
            {
                (MainFrame.Content as CitiesPage).Add();
            }
            else
            {
                var loader = new ResourceLoader();
                MainFrame.Navigate(typeof(SettingsPage), this);
                Refresh.Visibility = Visibility.Collapsed;
                Settings.Icon = new SymbolIcon(Symbol.Setting);
                Cities.Icon = new SymbolIcon(Symbol.World);
                Settings.Label = loader.GetString("Settings");
                Cities.Label = loader.GetString("Cities");
                Refresh.Label = loader.GetString("Refresh");
            }
        }

        private void Cities_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is CitiesPage)
            {
                (MainFrame.Content as CitiesPage).Edit();
            }
            else
            {
                var loader = new ResourceLoader();
                MainFrame.Navigate(typeof(CitiesPage), this);
                Settings.Icon = new SymbolIcon(Symbol.Add);
                Cities.Icon = new SymbolIcon(Symbol.Edit);
                Cities.Label = loader.GetString("Edit");
                Refresh.Label = loader.GetString("Refresh");
                Settings.Label = loader.GetString("Add");
                Refresh.Visibility = Visibility.Visible;
                if (!license.IsPurchased)
                {
                    Refresh.IsEnabled = false;
                }
            }
        }

        internal void CitiesPageQuitEditMode()
        {
            var loader = new ResourceLoader();
            Cities.Icon = new SymbolIcon(Symbol.Edit);
            Refresh.Icon = new SymbolIcon(Symbol.Refresh);
            Settings.Icon = new SymbolIcon(Symbol.Add);
            Cities.Label = loader.GetString("Edit");
            Refresh.Label = loader.GetString("Refresh");
            Settings.Label = loader.GetString("Add");
            if (!license.IsPurchased)
            {
                Refresh.IsEnabled = false;
            }
            Settings.IsEnabled = true;
        }

        private void PaneList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            MainFrame.Navigate((PaneList.SelectedItem as PaneOption).Page, this);
            if ((PaneList.SelectedItem as PaneOption).Page == typeof(CitiesPage))
            {
                var loader = new ResourceLoader();
                Settings.Icon = new SymbolIcon(Symbol.Add);
                Cities.Icon = new SymbolIcon(Symbol.Edit);
                Cities.Label = loader.GetString("Edit");
                Refresh.Label = loader.GetString("Refresh");
                Settings.Label = loader.GetString("Add");
                if (!license.IsPurchased)
                {
                    Refresh.IsEnabled = false;
                }
            }
            else
            {
                var loader = new ResourceLoader();
                Settings.Icon = new SymbolIcon(Symbol.Setting);
                Cities.Icon = new SymbolIcon(Symbol.World);
                Settings.Label = loader.GetString("Settings");
                Cities.Label = loader.GetString("Cities");
                Refresh.Label = loader.GetString("Refresh");
                Refresh.IsEnabled = true;
            }
            if ((PaneList.SelectedItem as PaneOption).Page == typeof(SettingsPage))
            {
                Refresh.Visibility = Visibility.Collapsed;
            }
            else
            {
                Refresh.Visibility = Visibility.Visible;
            }
        }

        internal void ReCalcPaneFormat()
        {
            this.DataContext = null;
            DateNowConverter.Refresh();
            this.DataContext = new MainPageViewModel();
        }

        private void Today_Click(object sender, RoutedEventArgs e)
        {
            var loader = new ResourceLoader();
            Settings.Icon = new SymbolIcon(Symbol.Setting);
            Cities.Icon = new SymbolIcon(Symbol.World);
            Settings.Label = loader.GetString("Settings");
            Cities.Label = loader.GetString("Cities");
            Refresh.Label = loader.GetString("Refresh");
            Refresh.Visibility = Visibility.Visible;
            Refresh.IsEnabled = true;
            MainFrame.Navigate(typeof(NowWeatherPage), this);
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            if (MainFrame.Content is NowWeatherPage)
            {
                (MainFrame.Content as NowWeatherPage).Refresh();
            }
            else if (MainFrame.Content is CitiesPage)
            {
                (MainFrame.Content as CitiesPage).Refresh();
            }
        }

        internal void CitiesPageGotoEditMode()
        {
            var loader = new ResourceLoader();
            Cities.Icon = new SymbolIcon(Symbol.Delete);
            Refresh.Icon = new SymbolIcon(Symbol.Cancel);
            Settings.Icon = new SymbolIcon(Symbol.Pin);
            Cities.Label = loader.GetString("Delete");
            Refresh.Label = loader.GetString("Cacel");
            Settings.Label = loader.GetString("Pin");
            if (!license.IsPurchased)
            {
                Settings.IsEnabled = false;
            }
        }

        private async void Like_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN=" + Windows.ApplicationModel.Package.Current.Id.FamilyName));
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
