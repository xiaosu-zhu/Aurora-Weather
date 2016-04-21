// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.AuWeather.Models.HeWeather.JsonContract;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.AuWeather.ViewModels;
using Com.Aurora.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.System.Threading;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CitiesPage : Page
    {
        private MainPage baba;
        private bool isEditMode;
        private License.License license;

        public CitiesPage()
        {
            this.InitializeComponent();
            Context.LocationUpdate += Context_LocationUpdate;
            Context.FetchDataFailed += Context_FetchDataFailed;
            license = new License.License();

        }

        private async void Context_FetchDataFailed(object sender, ViewModels.Events.FetchDataFailedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(async () =>
            {
                var loader = new ResourceLoader();
                var d = new MessageDialog(e.Message);
                d.Title = loader.GetString("Error");
                d.Commands.Add(new UICommand(loader.GetString("Setting"), new UICommandInvokedHandler(NavigateToSettings)));
                d.Commands.Add(new UICommand(loader.GetString("Quit"), new UICommandInvokedHandler(QuitAll)));
                await d.ShowAsync();
            }));
        }

        private void QuitAll(IUICommand command)
        {
            App.Current.Exit();
        }

        private void NavigateToSettings(IUICommand command)
        {
            baba.Navigate(typeof(SettingsPage));
        }

        private async void Context_LocationUpdate(object sender, ViewModels.Events.LocationUpdateEventArgs e)
        {
            var str = await FileIOHelper.ReadStringFromAssetsAsync("cityid.txt");
            var result = JsonHelper.FromJson<CityIdContract>(str);
            var citys = CityInfo.CreateList(result);
            str = null;
            result = null;
            var ui = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(async () =>
             {
                 var accessStatus = await Geolocator.RequestAccessAsync();
                 if (accessStatus == GeolocationAccessStatus.Allowed)
                 {
                     var _geolocator = new Geolocator();
                     var pos = await _geolocator.GetGeopositionAsync();
                     if ((_geolocator.LocationStatus != PositionStatus.NoData) && (_geolocator.LocationStatus != PositionStatus.NotAvailable) && (_geolocator.LocationStatus != PositionStatus.Disabled))
                         CalcPosition(pos, citys);
                     else
                     {
                         FailtoPos();
                     }
                 }
                 else
                 {
                     DeniePos();
                 }
             }));
        }

        private void FailtoPos()
        {

        }

        private void DeniePos()
        {
            Context.DeniePos();
        }

        private void CalcPosition(Geoposition pos, List<CityInfo> citys)
        {
            var t = ThreadPool.RunAsync(async (work) =>
            {
                var final = Models.Location.GetNearsetLocation(citys,
                                new Models.Location((float)pos.Coordinate.Point.Position.Latitude, (float)pos.Coordinate.Point.Position.Longitude));
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
                 {
                     var p = final.ToArray()[0];
                     p.Location = new Models.Location((float)pos.Coordinate.Point.Position.Latitude, (float)pos.Coordinate.Point.Position.Longitude);
                     Context.UpdateLocation(p);
                     citys.Clear();
                     citys = null;
                     final = null;
                 }));
            });
        }

        internal void Add()
        {
            if (isEditMode)
            {
                PinTiles();
            }
            else
            {
                baba.NavigatetoSettings(typeof(Models.Settings.Cities));
            }
        }

        internal void Edit()
        {
            if (isEditMode)
            {
                Delete();
                QuitEditMode();
                baba.CitiesPageQuitEditMode();
            }
            else
            {
                GotoEditMode();
                baba.CitiesPageGotoEditMode();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            baba = e.Parameter as MainPage;
            Color c;
            SolidColorBrush s;
            if (Context.Theme == ElementTheme.Dark)
            {
                var d = this.Resources.ThemeDictionaries["Dark"] as ResourceDictionary;
                c = (Color)d["SystemBaseHighColor"];
                s = (SolidColorBrush)d["SystemControlForegroundBaseHighBrush"];
            }
            else if (Context.Theme == ElementTheme.Light)
            {
                var d = this.Resources.ThemeDictionaries["Light"] as ResourceDictionary;
                c = (Color)d["SystemBaseHighColor"];
                s = (SolidColorBrush)d["SystemControlForegroundBaseHighBrush"];
            }
            else
            {
                c = (Color)Resources["SystemBaseHighColor"];
                s = (SolidColorBrush)Resources["SystemControlForegroundBaseHighBrush"];
            }
            baba.ChangeColor(Colors.Transparent, c, s);
            if (!license.IsPurchased)
            {
                RefreshButton.IsEnabled = false;
            }
        }

        private void RelativePanel_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ((sender as RelativePanel).Children[2] as ScrollViewer).ChangeView(0, 36, 1);
        }

        private void RelativePanel_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ((sender as RelativePanel).Children[2] as ScrollViewer).ChangeView(0, 0, 1);
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditMode)
            {
                PinTiles();
            }
            else
            {
                baba.NavigatetoSettings(typeof(Models.Settings.Cities));
            }
        }


        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditMode)
            {
                Delete();
                QuitEditMode();
                baba.CitiesPageQuitEditMode();
            }
            else
            {
                GotoEditMode();
            }
        }

        private void PinTiles()
        {
            Context.Pin(GridView.SelectedItems);
        }

        private void Delete()
        {
            Context.Delete(GridView.SelectedItems);
        }

        internal void Refresh()
        {
            if (isEditMode)
            {
                QuitEditMode();
                baba.CitiesPageQuitEditMode();
            }
            else
            {
                RefreshAll();
            }
        }

        private void RefreshAll()
        {
            Context.Refresh();
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditMode)
            {
                QuitEditMode();
                baba.CitiesPageQuitEditMode();
            }
            else
            {
                RefreshAll();
            }
        }

        private void RelativePanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            GotoEditMode();
            baba.CitiesPageGotoEditMode();
        }

        private void GotoEditMode()
        {
            var loader = new ResourceLoader();
            isEditMode = true;
            GridView.SelectionMode = ListViewSelectionMode.Multiple;
            GridView.IsItemClickEnabled = false;
            GridView.SelectionChanged += GridView_SelectionChanged;
            GridView.ItemClick -= GridView_ItemClick;
            EditButton.Icon = new SymbolIcon(Symbol.Delete);
            RefreshButton.Icon = new SymbolIcon(Symbol.Cancel);
            AddButton.Icon = new SymbolIcon(Symbol.Pin);
            EditButton.Label = loader.GetString("Delete");
            RefreshButton.Label = loader.GetString("Cacel");
            AddButton.Label = loader.GetString("Pin");
            RefreshButton.IsEnabled = true;
            if (!license.IsPurchased)
            {
                AddButton.IsEnabled = false;
            }
        }

        private void QuitEditMode()
        {
            var loader = new ResourceLoader();
            isEditMode = false;
            GridView.SelectionMode = ListViewSelectionMode.Single;
            GridView.IsItemClickEnabled = true;
            GridView.SelectionChanged -= GridView_SelectionChanged;
            GridView.ItemClick += GridView_ItemClick;
            EditButton.Icon = new SymbolIcon(Symbol.Edit);
            RefreshButton.Icon = new SymbolIcon(Symbol.Refresh);
            AddButton.Icon = new SymbolIcon(Symbol.Add);
            EditButton.Label = loader.GetString("Edit");
            RefreshButton.Label = loader.GetString("Refresh");
            AddButton.Label = loader.GetString("Add");
            AddButton.IsEnabled = true;
            if (!license.IsPurchased)
            {
                RefreshButton.IsEnabled = false;
            }
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            GridView.SelectedIndex = Context.Cities.IndexOf(e.ClickedItem as CityViewModel);
            if (GridView.SelectedIndex == Context.CurrentIndex)
            {
                baba.Navigate(typeof(NowWeatherPage));
                return;
            }
            Context.ChangeCurrent(GridView.SelectedIndex);
            baba.Navigate(typeof(NowWeatherPage));
        }


        private void GridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (GridView.SelectedIndex == Context.CurrentIndex || GridView.SelectionMode == ListViewSelectionMode.Multiple)
            {
                return;
            }
            Context.ChangeCurrent(GridView.SelectedIndex);
        }

    }
}
