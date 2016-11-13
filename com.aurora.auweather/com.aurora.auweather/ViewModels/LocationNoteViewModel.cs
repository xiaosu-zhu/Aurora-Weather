using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.Models.HeWeather;
using Com.Aurora.AuWeather.Models.HeWeather.JsonContract;
using Com.Aurora.AuWeather.ViewModels.Events;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.Helpers;
using Com.Aurora.Shared.MVVM;
using Windows.ApplicationModel.Core;
using Windows.Devices.Geolocation;
using Windows.System.Threading;
using Windows.UI.Core;

namespace Com.Aurora.AuWeather.ViewModels
{
    class LocationNoteViewModel : ViewModelBase
    {
        private bool enableLocation;

        public bool EnableLocation
        {
            get { return enableLocation; }
            set
            {
                SetProperty(ref enableLocation, value);
                settings.EnableLocate = value;
                settings.Save();
            }
        }

        internal static List<CityInfo> cities;
        private Models.Settings.Cities settings;

        internal ObservableCollection<LocationViewModel> LocationList = new ObservableCollection<LocationViewModel>();

        public event EventHandler<FetchDataCompleteEventArgs> LocateComplete;

        public event EventHandler<FetchDataCompleteEventArgs> FetchDataComplete;

        public LocationNoteViewModel()
        {
            var task = ThreadPool.RunAsync(async (w) =>
            {
                var result = JsonHelper.FromJson<CityIdContract>(Key.cityid);
                cities = CityInfo.CreateList(result);
                result = null;
                settings = SettingsModel.Current.Cities;
                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
                {
                    EnableLocation = settings.EnableLocate;
                    foreach (var item in settings.Routes)
                    {
                        LocationList.Add(new LocationViewModel(item));
                    }
                }));
                this.OnFetchDataComplete();
            });
        }

        private void OnFetchDataComplete()
        {
            FetchDataComplete?.Invoke(this, new FetchDataCompleteEventArgs());
        }

        ~LocationNoteViewModel()
        {
            cities.Clear();
            cities = null;
        }

        internal void SaveAll()
        {
        }

        internal async Task UpdatePosition(Geoposition pos)
        {
            foreach (var item in LocationList)
            {
                await item.GetLocation(pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);
            }
        }

        internal void ChangeRoute()
        {
            List<LocateRoute> li = new List<LocateRoute>();
            foreach (var item in LocationList)
            {
                li.Add(item.args);
            }
            settings.ChangeRoute(li.ToArray());
            settings.Save();
        }
    }

    class LocationViewModel : ViewModelBase
    {
        private string route;

        public string Route
        {
            get { return route; }
            set { route = value; }
        }

        private Models.Location location;

        public Models.Location Location
        {
            get { return location; }
            set { SetProperty(ref location, value); }
        }

        private CityInfo city;

        public CityInfo City
        {
            get { return city; }
            set { SetProperty(ref city, value); }
        }


        internal LocateRoute args;

        public LocationViewModel(LocateRoute args)
        {
            this.args = args;
            Route = LocateRoute.Amap.GetDisplayName();
        }

        public async Task GetLocation(double lat, double lon)
        {
            List<CityInfo> final = null;
            switch (args)
            {
                case LocateRoute.unknown:
                    var near = Models.Location.GetNearsetLocation(LocationNoteViewModel.cities, new Models.Location((float)lat, (float)lon));
                    final = near.ToList();
                    break;
                case LocateRoute.Amap:
                    var acontract = await Models.Location.AmapReGeoAsync(lat, lon);
                    if (acontract != null)
                        final = LocationNoteViewModel.cities.FindAll(x =>
                        {
                            return x.City == acontract.regeocode.addressComponent.district;
                        });
                    break;
                case LocateRoute.Omap:
                    var ocontract = await Models.Location.OpenMapReGeoAsync(lat, lon);
                    if (ocontract != null)
                        final = LocationNoteViewModel.cities.FindAll(x =>
                        {
                            return x.City == ocontract.address.city;
                        });
                    break;
                case LocateRoute.IP:
                    var id = await Models.Location.ReGeobyIpAsync();
                    if (id != null)
                        final = LocationNoteViewModel.cities.FindAll(x =>
                        {
                            return x.Id == id.CityId;
                        });
                    break;
                case LocateRoute.Gmap:
                default:
                    throw new NotImplementedException("Google map api not implement");
            }
            if (final.IsNullorEmpty())
            {
                City = new CityInfo
                {
                    City = "定位失败",
                    Location = new Models.Location((float)lat, (float)lon),
                    Country = "",
                    Id = "",
                    Province = ""
                };
                return;
            }
            City = final[0];
            Location = City.Location;
        }
    }
}
