using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Com.Aurora.AuWeather.ViewModels.Events;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Windows.System.Threading;
using Com.Aurora.AuWeather.ViewModels;
using System.Text;
using Com.Aurora.AuWeather.Models.HeWeather;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather.SettingOptions
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CitiesSetting : Page
    {
        private Geolocator _geolocator;
        private Geoposition pos;
        private bool updated = false;

        public CitiesSetting()
        {
            this.InitializeComponent();
            Context.FetchDataComplete += Context_FetchDataComplete;
            Context.LocateComplete += Context_LocateComplete;
            App.Current.Suspending += Current_Suspending;
        }

        private void Context_LocateComplete(object sender, FetchDataCompleteEventArgs e)
        {
            LocatingOut.Begin();
            updated = true;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Context.SaveAll();
        }

        private void Current_Suspending(object sender, SuspendingEventArgs e)
        {
            Context.SaveAll();
        }

        private async void Context_FetchDataComplete(object sender, FetchDataCompleteEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(async () =>
            {
                LocateOnSwitch.Toggled += LocateOnSwitch_Toggled;
                SearchBox.TextChanged += SearchBox_TextChanged;
                SearchBox.SuggestionChosen += SearchBox_SuggestionChosen;
                SearchBox.QuerySubmitted += SearchBox_QuerySubmitted;
                FetchComplete.Begin();
                CitiesList.ItemsSource = Context.Info;
                await AccesstoLocate();
            }));
        }

        private async System.Threading.Tasks.Task AccesstoLocate()
        {
            var accessStatus = await Geolocator.RequestAccessAsync();
            if (accessStatus == GeolocationAccessStatus.Allowed)
            {
                LocateAllowed.Begin();
                if (Context.EnablePosition)
                {
                    _geolocator = new Geolocator();
                    _geolocator.StatusChanged += OnStatusChanged;
                    ShowRefreshing();
                    pos = await _geolocator.GetGeopositionAsync();
                    UpdatePosition(pos);
                }
                else
                {
                    HidePos();
                }

            }
            else
            {
                DeniePos();
            }
        }

        private void DeniePos()
        {
            LocateDenied.Begin();
        }

        private void HidePos()
        {
            LocatingDisable.Begin();
        }

        private void ShowRefreshing()
        {
            LocatingIn.Begin();
        }

        private void UpdatePosition(Geoposition pos)
        {
            var taks = ThreadPool.RunAsync(async (work) =>
             {
                 await Context.CalcPosition(pos);
             });
        }

        private async void OnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            var e = args;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
             {
                 switch (e.Status)
                 {
                     case PositionStatus.Ready:
                         if (!updated && pos != null)
                         {
                             UpdatePosition(pos);
                         }
                         break;
                     case PositionStatus.Initializing:
                         if (!updated && pos != null)
                             ShowRefreshing();
                         break;
                     case PositionStatus.NoData:
                     case PositionStatus.Disabled:
                     case PositionStatus.NotInitialized:
                     case PositionStatus.NotAvailable:
                         ShowError();
                         break;
                     default:
                         break;
                 }
             }));
        }

        private void ShowError()
        {
            LocateDenied.Begin();
        }

        private void Page_Unloaded(object sender, RoutedEventArgs e)
        {
            Context.SaveAll();
        }

        private async void LocateDenied_Click(object sender, RoutedEventArgs e)
        {
            await AccesstoLocate();
        }

        private async void LocateOnSwitch_Toggled(object sender, RoutedEventArgs e)
        {
            Context.ChangePosition(LocateOnSwitch.IsOn);
            await AccesstoLocate();
        }

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            SearchBoxIn.Begin();
            SearchBox.Text = "";
        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            ClearSelect();
            Context.DeleteCity((sender as Button).DataContext as CitySettingsViewModel);
        }

        private void ClearSelect()
        {
            foreach (var item in CitiesList.Items)
            {
                (item as CitySettingsViewModel).IsCurrent = false;
            }
            Context.Is_Located_Current = false;
        }

        private void ListStarButton_Click(object sender, RoutedEventArgs e)
        {
            ClearSelect();
            Context.SetCurrent((sender as ToggleButton).DataContext as CitySettingsViewModel);
        }

        private void LocatedStar_Click(object sender, RoutedEventArgs e)
        {
            ClearSelect();
            Context.SetCurrent_Locate();
        }

        private async void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var text = sender.Text;
                await ThreadPool.RunAsync(async (work) =>
                 {
                     var dataset = Context.Search_TextChanged(text);
                     //Set the ItemsSource to be your filtered dataset
                     await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () =>
                       {
                           sender.ItemsSource = dataset;
                       });
                 });
            }
        }

        private void SearchBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null)
            {
                // User selected an item from the suggestion list, take an action on it here.
                Context.AddCity((args.ChosenSuggestion as CityInfo));
            }
            else
            {
                // Use args.QueryText to determine what to do.
                Context.AddCity(args.QueryText);
            }
            SearchBoxOut.Begin();
        }

        private void SearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            // Set sender.Text. You can use args.SelectedItem to build your text string.
            sender.Text = (args.SelectedItem as CityInfo).City;
        }
    }
}
