using System;
using System.Threading.Tasks;
using Com.Aurora.AuWeather.ViewModels.Events;
using Windows.ApplicationModel;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace Com.Aurora.AuWeather.SettingOptions
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class LocationNote : Page
    {
        private Geolocator _geolocator;
        private Geoposition pos;
        private bool updated = false;

        public LocationNote()
        {
            this.InitializeComponent();
            App.Current.Suspending += Current_Suspending;
            Context.FetchDataComplete += Context_FetchDataComplete;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            Context.SaveAll();
        }

        private async void Context_FetchDataComplete(object sender, FetchDataCompleteEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, new Windows.UI.Core.DispatchedHandler(async () =>
            {
                LocateOnSwitch.IsEnabled = true;
                LocateOnSwitch.IsEnabled = true;
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
                    if (Context.EnableLocation)
                    {
                        try
                        {
                            _geolocator = new Geolocator();
                            pos = await _geolocator.GetGeopositionAsync();
                            if ((_geolocator.LocationStatus != PositionStatus.NoData) && (_geolocator.LocationStatus != PositionStatus.NotAvailable) && (_geolocator.LocationStatus != PositionStatus.Disabled))
                                Context.UpdatePosition(pos);
                            else
                            {
                                DeniePos();
                            }
                            //_geolocator.StatusChanged += OnStatusChanged;
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

        private void HidePos()
        {

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
                            Context.UpdatePosition(pos);
                        }
                        break;
                    case PositionStatus.Initializing:
                        if (updated || pos == null)
                        {
                            ShowError();
                        }
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

        private void DeniePos()
        {
            LocateDenied.Begin();
        }

        private void Context_LocateComplete(object sender, FetchDataCompleteEventArgs e)
        {
            updated = true;
        }

        private void Current_Suspending(object sender, SuspendingEventArgs e)
        {
            Context.SaveAll();
        }

        private void ListView_DragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            if (args.DropResult == Windows.ApplicationModel.DataTransfer.DataPackageOperation.Move)
            {
                Context.ChangeRoute();
            }
        }

        private void ListView_Drop(object sender, DragEventArgs e)
        {

        }

        private void ListView_DropCompleted(UIElement sender, DropCompletedEventArgs args)
        {

        }
    }
}
