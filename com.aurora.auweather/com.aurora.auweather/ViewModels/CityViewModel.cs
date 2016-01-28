using com.aurora.auweather.Models;
using com.aurora.shared.MVVM;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace com.aurora.auweather.ViewModels
{
    public class CityViewModel : ViewModelBase
    {
        private Location location;
        private string city;
        private string province;
        private string country;

        internal Location Location
        {
            get
            {
                return location;
            }

            set
            {
                SetProperty(ref location, value);
            }
        }

        public string City
        {
            get
            {
                return city;
            }

            set
            {
                SetProperty(ref city, value);
            }
        }

        public string Province
        {
            get
            {
                return province;
            }

            set
            {
                SetProperty(ref province, value);
            }
        }

        public string Country
        {
            get
            {
                return country;
            }

            set
            {
                SetProperty(ref country, value);
            }
        }

    }
}
