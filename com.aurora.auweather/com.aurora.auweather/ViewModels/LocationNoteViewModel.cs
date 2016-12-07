using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Com.Aurora.AuWeather.Core.SQL;
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

        private Models.Settings.Cities settings;

        internal ObservableCollection<LocationViewModel> LocationList = new ObservableCollection<LocationViewModel>();

        private Models.Location location;

        public Models.Location Location
        {
            get { return location; }
            set { SetProperty(ref location, value); }
        }


        public event EventHandler<FetchDataCompleteEventArgs> FetchDataComplete;

        public LocationNoteViewModel()
        {
            var task = ThreadPool.RunAsync(async (w) =>
            {
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

        internal void SaveAll()
        {
            settings.Save();
        }

        internal void UpdatePosition(Geoposition pos)
        {
            Location = new Models.Location((float)pos.Coordinate.Point.Position.Latitude, (float)pos.Coordinate.Point.Position.Longitude);
            foreach (var item in LocationList)
            {
                var t = ThreadPool.RunAsync(async (wior) =>
                {

                    await item.GetLocation(pos.Coordinate.Point.Position.Latitude, pos.Coordinate.Point.Position.Longitude);

                });
            }
        }

        internal void ChangeRoute()
        {
            List<LocateRoute> li = new List<LocateRoute>();
            foreach (var item in LocationList)
            {
                li.Add(item.args);
            }
            LocationList[0].City.Longitude = Location.Longitude;
            LocationList[0].City.Latitude = Location.Latitude;
            settings.ChangeRoute(li.ToArray(), LocationList[0].City);
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

        private City city;

        public City City
        {
            get { return city; }
            set { SetProperty(ref city, value); }
        }


        internal LocateRoute args;

        public LocationViewModel(LocateRoute args)
        {
            this.args = args;
            Route = args.GetDisplayName();
        }

        public async Task GetLocation(double lat, double lon)
        {
            City final = null;
            switch (args)
            {
                case LocateRoute.unknown:
                    var near = Models.Location.GetNearsetLocation(new Models.Location((float)lat, (float)lon));
                    final = near;
                    break;
                case LocateRoute.Amap:
                    var acontract = await Models.Location.AmapReGeoAsync(lat, lon);
                    if (acontract != null)
                    {
                        var li = new City
                        {
                            CityEn = acontract.regeocode.addressComponent.district,
                            CityZh = acontract.regeocode.addressComponent.district,
                            LeaderEn = acontract.regeocode.addressComponent.city,
                            LeaderZh = acontract.regeocode.addressComponent.city,
                            CountryCode = "CN",
                            CountryEn = acontract.regeocode.addressComponent.country,
                            //Id = await Models.Location.HeWeatherGeoLookup(acontract.regeocode.addressComponent.district),
                            Latitude = (float)lat,
                            Longitude = (float)lon,
                            ProvinceEn = acontract.regeocode.addressComponent.province,
                            ProvinceZh = acontract.regeocode.addressComponent.province
                        };
                        final = li;
                    }
                    break;
                case LocateRoute.Omap:
                    var ocontract = await Models.Location.OpenMapReGeoAsync(lat, lon);
                    if (ocontract != null && !ocontract.results.IsNullorEmpty())
                    {
                        var li = new City
                        {
                            CityEn = ocontract.results[0].components.county,
                            CityZh = ocontract.results[0].components.county,
                            LeaderEn = ocontract.results[0].components.region,
                            LeaderZh = ocontract.results[0].components.region,
                            CountryCode = ocontract.results[0].components.country_code,
                            CountryEn = ocontract.results[0].components.country,
                            //Id = await Models.Location.HeWeatherGeoLookup(ocontract.results[0].components.county),
                            Latitude = (float)lat,
                            Longitude = (float)lon,
                            ProvinceEn = ocontract.results[0].components.state,
                            ProvinceZh = ocontract.results[0].components.state
                        };
                        final = li;
                    }
                    break;
                case LocateRoute.IP:
                    var id = await Models.Location.ReGeobyIpAsync();
                    if (id != null)
                    {
                        var f = SQLAction.Find(id.CityId);
                        if (f != null)
                            final = f;
                    }
                    break;
                case LocateRoute.Gmap:
                    throw new NotImplementedException("Google map api not implement");
            }

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.High, new DispatchedHandler(() =>
            {
                if (final == null)
                {
                    City = new City
                    {
                        CityEn = "定位失败",
                        Latitude = (float)lat,
                        Longitude = (float)lon,
                        CountryCode = "",
                        Id = "",
                    };
                    return;
                }
                City = final;
                Location = new Models.Location(City.Latitude, City.Longitude);
            }));
        }
    }
}
