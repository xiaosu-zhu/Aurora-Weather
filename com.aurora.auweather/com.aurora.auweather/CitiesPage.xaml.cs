using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.AuWeather.Models.HeWeather.JsonContract;
using Com.Aurora.AuWeather.Models.Settings;
using Com.Aurora.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Geolocation;
using Windows.UI;
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

        public CitiesPage()
        {
            this.InitializeComponent();
            Context.LocationUpdate += Context_LocationUpdate;
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
                     CalcPosition(pos, citys);
                 }
                 else
                 {
                     DeniePos();
                 }
             }));
        }

        private void DeniePos()
        {
            Context.DeniePos();
        }

        private void CalcPosition(Geoposition pos, List<CityInfo> citys)
        {

            var final = Models.Location.GetNearsetLocation(citys,
                new Models.Location((float)pos.Coordinate.Point.Position.Latitude, (float)pos.Coordinate.Point.Position.Longitude));
            Context.UpdateLocation(final.ToArray()[0]);
            citys.Clear();
            citys = null;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            baba = e.Parameter as MainPage;
            baba.ChangeColor(Colors.Transparent, (Color)Resources["SystemBaseHighColor"], (SolidColorBrush)Resources["SystemControlForegroundBaseHighBrush"]);
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
                PinTiles(sender);
            }
            else
            {
                baba.NavigatetoSettings(typeof(Cities));
            }
        }


        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditMode)
            {
                Delete();
                QuitEditMode();
            }
            else
            {
                GotoEditMode();
            }
        }

        private void PinTiles(object sender)
        {
            Context.Pin(GridView.SelectedItems);
        }

        private void Delete()
        {
            Context.Delete(GridView.SelectedItems);
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
            }
            else
            {
                RefreshAll();
            }
        }

        private void RelativePanel_RightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            GotoEditMode();
        }

        private void GotoEditMode()
        {
            isEditMode = true;
            GridView.SelectionMode = ListViewSelectionMode.Multiple;
            EditButton.Icon = new SymbolIcon(Symbol.Delete);
            RefreshButton.Icon = new SymbolIcon(Symbol.Cancel);
            AddButton.Icon = new SymbolIcon(Symbol.Pin);
        }

        private void QuitEditMode()
        {
            isEditMode = false;
            GridView.SelectionMode = ListViewSelectionMode.Single;
            EditButton.Icon = new SymbolIcon(Symbol.Edit);
            RefreshButton.Icon = new SymbolIcon(Symbol.Refresh);
            AddButton.Icon = new SymbolIcon(Symbol.Add);
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
