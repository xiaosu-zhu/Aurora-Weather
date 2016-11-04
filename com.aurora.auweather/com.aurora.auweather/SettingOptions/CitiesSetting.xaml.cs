// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Com.Aurora.AuWeather.ViewModels.Events;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;
using Windows.Devices.Geolocation;
using Windows.System.Threading;
using Com.Aurora.AuWeather.ViewModels;
using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.AuWeather.CustomControls;
using Windows.UI;
using Windows.UI.Xaml.Media;
using System.Threading.Tasks;
using System.Collections.Generic;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather.SettingOptions
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CitiesSetting : Page, IThemeble
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            Context.cities = e.Parameter as List<CityInfo>;
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
                //SearchBox.SuggestionChosen += SearchBox_SuggestionChosen;
                SearchBox.QuerySubmitted += SearchBox_QuerySubmitted;
                SearchBox.IsEnabled = true;
                LocateOnSwitch.IsEnabled = true;
                LocateOnSwitch.IsEnabled = true;
                FetchComplete.Begin();
                CitiesList.ItemsSource = Context.Info;
                await AccesstoLocate();
            }));
        }

        private async Task AccesstoLocate()
        {
            try
            {
                var accessStatus = await Geolocator.RequestAccessAsync();
                if (accessStatus == GeolocationAccessStatus.Allowed)
                {
                    LocateAllowed.Begin();
                    if (Context.EnablePosition)
                    {
                        try
                        {
                            _geolocator = new Geolocator();

                            ShowRefreshing();
                            pos = await _geolocator.GetGeopositionAsync();
                            if ((_geolocator.LocationStatus != PositionStatus.NoData) && (_geolocator.LocationStatus != PositionStatus.NotAvailable) && (_geolocator.LocationStatus != PositionStatus.Disabled))
                                await UpdatePosition(pos);
                            else
                            {
                                DeniePos();
                            }
                            _geolocator.StatusChanged += OnStatusChanged;
                        }
                        catch (Exception)
                        {
                            DeniePos();
                        }

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
            catch (Exception)
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

        private async Task UpdatePosition(Geoposition pos)
        {
            await Context.CalcPosition(pos);
        }

        internal void Complete()
        {
            Context.Complete();
        }

        internal bool CheckCompleted()
        {
            return Context.CheckCompleted();
        }

        private async void OnStatusChanged(Geolocator sender, StatusChangedEventArgs args)
        {
            var e = args;
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(async () =>
             {
                 switch (e.Status)
                 {
                     case PositionStatus.Ready:
                         if (!updated && pos != null)
                         {
                             await UpdatePosition(pos);
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
            try
            {
                MainPage.Current.SetCitiesPanel(Context.GetCities(), Context.GetIndex());
            }
            catch (Exception)
            {

            }

        }

        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            ClearSelect();
            Context.DeleteCity((sender as Button).DataContext as CitySettingsViewModel);
            try
            {
                MainPage.Current.SetCitiesPanel(Context.GetCities(), Context.GetIndex());
            }
            catch (Exception)
            {

            }

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
            try
            {
                MainPage.Current.SetCitiesPanel(Context.GetCities(), Context.GetIndex());
            }
            catch (Exception)
            {

            }
        }

        private void LocatedStar_Click(object sender, RoutedEventArgs e)
        {
            ClearSelect();
            Context.SetCurrent_Locate();
            try
            {
                MainPage.Current.SetCitiesPanel(Context.GetCities(), Context.GetIndex());

            }
            catch (Exception)
            {
            }
        }

        private async void SearchBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if (args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                var text = sender.Text;
                text = text.Replace('\'', ' ');
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
            try
            {
                MainPage.Current.SetCitiesPanel(Context.GetCities(), Context.GetIndex());

            }
            catch (Exception)
            {
            }
        }

        private void SearchBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs args)
        {
            SearchBox.TextChanged -= SearchBox_TextChanged;
            // Set sender.Text. You can use args.SelectedItem to build your text string.
            sender.Text = (args.SelectedItem as CityInfo).City;
            SearchBox.TextChanged += SearchBox_TextChanged;
        }

        private void CitiesList_ItemClick(object sender, ItemClickEventArgs e)
        {
            ClearSelect();
            Context.SetCurrent(e.ClickedItem as CitySettingsViewModel);
            try
            {
                MainPage.Current.SetCitiesPanel(Context.GetCities(), Context.GetIndex());

            }
            catch (Exception)
            {

            }
        }

        private void LocatePanel_PointerPressed(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Press.Begin();
        }

        private void LocatePanel_PointerReleased(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            ClearSelect();
            Context.SetCurrent_Locate();
            try
            {
                MainPage.Current.SetCitiesPanel(Context.GetCities(), Context.GetIndex());
            }
            catch (Exception)
            {

            }

            Release.Begin();
        }

        private void LocatePanel_PointerEntered(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            PointerIn.Begin();
        }

        private void LocatePanel_PointerExited(object sender, Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            PointerOut.Begin();
        }

        private async void CheckLocateAccessButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-settings:privacy-location"));
        }

        private void CitiesList_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            Context.ReArrange();
        }

        public void ChangeThemeColor(Color color)
        {
            var color1 = Color.FromArgb(Convert.ToByte(color.A * 0.9), color.R, color.G, color.B);
            var color2 = Color.FromArgb(Convert.ToByte(color.A * 0.6), color.R, color.G, color.B);
            var color3 = Color.FromArgb(Convert.ToByte(color.A * 0.8), color.R, color.G, color.B);
            (Resources["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            (Resources["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            (Resources["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            (Resources["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            (Resources["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            (Resources["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            (Resources["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            (Resources["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Light"] as ResourceDictionary)["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlBackgroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlDisabledAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlForegroundAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltAccentBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentHighBrush"] as SolidColorBrush).Color = color1;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentLowBrush"] as SolidColorBrush).Color = color2;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightAltListAccentMediumBrush"] as SolidColorBrush).Color = color3;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentHighBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentLowBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHighlightListAccentMediumBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemControlHyperlinkTextBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["ContentDialogBorderThemeBrush"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["JumpListDefaultEnabledBackground"] as SolidColorBrush).Color = color;
            ((Resources.ThemeDictionaries["Dark"] as ResourceDictionary)["SystemThemeMainBrush"] as SolidColorBrush).Color = color;
        }
    }
}
