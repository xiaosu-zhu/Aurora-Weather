// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.CustomControls;
using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.AuWeather.ViewModels;
using System;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;
using Windows.Devices.Geolocation;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Threading.Tasks;
using Com.Aurora.AuWeather.SettingOptions;
using Com.Aurora.Shared.Extensions;
using System.Linq;
using Com.Aurora.AuWeather.Models;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上提供

namespace Com.Aurora.AuWeather
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class CitiesPage : Page, IThemeble
    {
        private MainPage baba = MainPage.Current;
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
            if (baba != null)
                baba.Navigate(typeof(SettingsPage));
        }

        private void Context_LocationUpdate(object sender, ViewModels.Events.LocationUpdateEventArgs e)
        {
            var ui = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(async () =>
            {
                if (Context.EnableLocate)
                {
                    var accessStatus = await Geolocator.RequestAccessAsync();
                    if (accessStatus == GeolocationAccessStatus.Allowed)
                    {
                        try
                        {
                            var _geolocator = new Geolocator();
                            var pos = await _geolocator.GetGeopositionAsync();
                            if ((_geolocator.LocationStatus != PositionStatus.NoData) && (_geolocator.LocationStatus != PositionStatus.NotAvailable) && (_geolocator.LocationStatus != PositionStatus.Disabled))
                                await CalcPosition(pos, Context.cities);
                            else
                            {
                                FailtoPos();
                            }
                        }
                        catch (Exception)
                        {
                            FailtoPos();
                        }

                    }
                    else
                    {
                        DeniePos();
                    }
                }
                CityPanel.Navigate(typeof(CitiesSetting), Context.cities);
            }));
        }

        private void FailtoPos()
        {

        }

        private void DeniePos()
        {
            Context.DeniePos();
        }

        private async Task CalcPosition(Geoposition pos, List<CityInfo> citys)
        {
            try
            {
                var final = await Models.Location.ReverseGeoCode((float)pos.Coordinate.Point.Position.Latitude, (float)pos.Coordinate.Point.Position.Longitude, SettingsModel.Current.Cities.Routes, citys);
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(() =>
                {
                    var p = final;
                    p.Location = new Models.Location((float)pos.Coordinate.Point.Position.Latitude, (float)pos.Coordinate.Point.Position.Longitude);
                    Context.UpdateLocation(p);
                }));
            }
            catch (Exception)
            {
                FailtoPos();
            }
        }

        internal async void Add()
        {
            if (isEditMode)
            {
                PinTiles();
            }
            else
            {
                //CityPanel.Width = ActualWidth - 64;
                await CityDailog.ShowAsync();
            }
        }

        internal void Edit()
        {
            if (isEditMode)
            {
                Delete();
                QuitEditMode();
                if (baba != null)
                    baba.CitiesPageQuitEditMode();
            }
            else
            {
                GotoEditMode();
                if (baba != null)
                    baba.CitiesPageGotoEditMode();
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (e.Parameter is string)
            {
                HeaderGrid.Visibility = Visibility.Collapsed;
                var length = new GridLength(0);
                Root.RowDefinitions[0].Height = length;
                Root.RowDefinitions[1].Height = length;
            }
            else
            {
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
                if (baba != null)
                    baba.ChangeColor(Colors.Transparent, c, s);
                if (!license.IsPurchased)
                {
                    RefreshButton.IsEnabled = false;
                }
            }
        }

        private async void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditMode)
            {
                PinTiles();
            }
            else
            {
                //CityPanel.Width = ActualWidth - 64;
                await CityDailog.ShowAsync();
            }
        }


        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (isEditMode)
            {
                Delete();
                QuitEditMode();
                if (baba != null)
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
                if (baba != null)
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
                if (baba != null)
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
            if (baba != null)
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

        private async void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            if ((e.ClickedItem as CityViewModel).Empty)
            {
                //CityPanel.Width = ActualWidth - 64;
                await CityDailog.ShowAsync();
                return;
            }
            GridView.SelectedIndex = Context.Cities.IndexOf(e.ClickedItem as CityViewModel);
            if (GridView.SelectedIndex == Context.CurrentIndex)
            {
                if (baba != null)
                    baba.Navigate(typeof(NowWeatherPage));
                return;
            }
            Context.ChangeCurrent(GridView.SelectedIndex);
            if (baba != null)
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

        internal void Complete()
        {
            (CityPanel.Content as CitiesSetting).Complete();
        }

        internal bool CheckCompleted()
        {
            if (CityPanel.Content != null)
            {
                return (CityPanel.Content as CitiesSetting).CheckCompleted();
            }
            return false;
        }

        private async void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
        {
            if (license.IsPurchased)
            {
                Context.Pin(sender as string);
            }
            else
            {
                MessageDialog p = new MessageDialog("Please donate to unlock this feature.");
                await p.ShowAsync();
            }
        }

        private void GridView_Loaded(object sender, RoutedEventArgs e)
        {
            var p = Math.Round(ActualWidth / 336);
            if (p == 0)
                p = 1;
            Context.SetPanelWidth(ActualWidth / p);
        }

        private void CityDailog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            Context.ReloadCity();
        }
    }
}
