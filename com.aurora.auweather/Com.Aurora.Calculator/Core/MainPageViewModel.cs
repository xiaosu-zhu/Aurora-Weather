// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Models;
using Com.Aurora.Shared.Extensions;
using Com.Aurora.Shared.MVVM;
using System;
using System.Collections.ObjectModel;

namespace Com.Aurora.Calculator.Core
{
    internal class MainPageViewModel : ViewModelBase
    {
        public LengthList Length = new LengthList();
        public TempList Temp = new TempList();
        public PressureList Pressure = new PressureList();
        public SpeedList Speed = new SpeedList();
        public AngleList Angle = new AngleList();
        public ProList Pro = new ProList();
    }

    class LengthList : ViewModelBase
    {
        private Length mainValue;

        public LengthList()
        {
            foreach (LengthParameter item in Enum.GetValues(typeof(LengthParameter)))
            {
                Items.Add(new LengthViewModel(item));
            }
            foreach (var item in Items)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
            MainValue = Length.FromKM(0);
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var i = sender as LengthViewModel;
            switch (i.Parameter)
            {
                case LengthParameter.KM:
                    MainValue = Length.FromKM(i.Value);
                    break;
                case LengthParameter.M:
                    MainValue = Length.FromM(i.Value);
                    break;
                case LengthParameter.Mile:
                    MainValue = Length.FromMile(i.Value);
                    break;
                case LengthParameter.NM:
                    MainValue = Length.FromNM(i.Value);
                    break;
                default:
                    break;
            }
            foreach (var item in Items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
                switch (item.Parameter)
                {
                    case LengthParameter.KM:
                        item.Value = MainValue.KM;
                        break;
                    case LengthParameter.M:
                        item.Value = MainValue.M;
                        break;
                    case LengthParameter.Mile:
                        item.Value = MainValue.Mile;
                        break;
                    case LengthParameter.NM:
                        item.Value = MainValue.NM;
                        break;
                    default:
                        break;
                }
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        public ObservableCollection<LengthViewModel> Items = new ObservableCollection<LengthViewModel>();
        public Length MainValue
        {
            get
            {
                return mainValue;
            }
            set
            {
                SetProperty(ref mainValue, value);
            }
        }
    }
    class TempList : ViewModelBase
    {
        private Temperature mainValue;

        public TempList()
        {
            foreach (TemperatureParameter item in Enum.GetValues(typeof(TemperatureParameter)))
            {
                Items.Add(new TempViewModel(item));
            }
            foreach (var item in Items)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
            MainValue = Temperature.FromKelvin(0);
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var i = sender as TempViewModel;
            switch (i.Parameter)
            {
                case TemperatureParameter.Celsius:
                    MainValue = Temperature.FromCelsius(i.Value);
                    break;
                case TemperatureParameter.Fahrenheit:
                    MainValue = Temperature.FromFahrenheit(i.Value);
                    break;
                case TemperatureParameter.Kelvin:
                    MainValue = Temperature.FromKelvin(i.Value);
                    break;
                default:
                    break;
            }
            foreach (var item in Items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
                switch (item.Parameter)
                {
                    case TemperatureParameter.Celsius:
                        item.Value = MainValue.Celsius;
                        break;
                    case TemperatureParameter.Fahrenheit:
                        item.Value = MainValue.Fahrenheit;
                        break;
                    case TemperatureParameter.Kelvin:
                        item.Value = MainValue.Kelvin;
                        break;
                    default:
                        break;
                }
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        public ObservableCollection<TempViewModel> Items = new ObservableCollection<TempViewModel>();
        public Temperature MainValue
        {
            get
            {
                return mainValue;
            }
            set
            {
                SetProperty(ref mainValue, value);
            }
        }
    }
    class PressureList : ViewModelBase
    {
        private Pressure mainValue;

        public PressureList()
        {
            foreach (PressureParameter item in Enum.GetValues(typeof(PressureParameter)))
            {
                Items.Add(new PressureViewModel(item));
            }
            foreach (var item in Items)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
            MainValue = Pressure.FromAtm(1);
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var i = sender as PressureViewModel;
            switch (i.Parameter)
            {
                case PressureParameter.Atm:
                    MainValue = Pressure.FromAtm(i.Value);
                    break;
                case PressureParameter.CmHg:
                    MainValue = Pressure.FromCmHg(i.Value);
                    break;
                case PressureParameter.Hpa:
                    MainValue = Pressure.FromHPa(i.Value);
                    break;
                case PressureParameter.Torr:
                    MainValue = Pressure.FromTorr(i.Value);
                    break;
                default:
                    break;
            }
            foreach (var item in Items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
                switch (item.Parameter)
                {
                    case PressureParameter.Atm:
                        item.Value = MainValue.Atm;
                        break;
                    case PressureParameter.CmHg:
                        item.Value = MainValue.CmHg;
                        break;
                    case PressureParameter.Hpa:
                        item.Value = MainValue.HPa;
                        break;
                    case PressureParameter.Torr:
                        item.Value = MainValue.Torr;
                        break;
                    default:
                        break;
                }
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        public ObservableCollection<PressureViewModel> Items = new ObservableCollection<PressureViewModel>();
        public Pressure MainValue
        {
            get
            {
                return mainValue;
            }
            set
            {
                SetProperty(ref mainValue, value);
            }
        }
    }
    class SpeedList : ViewModelBase
    {
        private Speed mainValue;

        public SpeedList()
        {
            foreach (SpeedParameter item in Enum.GetValues(typeof(SpeedParameter)))
            {
                Items.Add(new SpeedViewModel(item));
            }
            foreach (var item in Items)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
            MainValue = Speed.FromKMPH(0);
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var i = sender as SpeedViewModel;
            switch (i.Parameter)
            {
                case SpeedParameter.KMPH:
                    MainValue = Speed.FromKMPH(i.Value);
                    break;
                case SpeedParameter.MPS:
                    MainValue = Speed.FromMPS(i.Value);
                    break;
                case SpeedParameter.Knot:
                    MainValue = Speed.FromKnot(i.Value);
                    break;
                default:
                    break;
            }
            foreach (var item in Items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
                switch (item.Parameter)
                {
                    case SpeedParameter.KMPH:
                        item.Value = MainValue.KMPH;
                        break;
                    case SpeedParameter.MPS:
                        item.Value = MainValue.MPS;
                        break;
                    case SpeedParameter.Knot:
                        item.Value = MainValue.Knot;
                        break;
                    default:
                        break;
                }
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        public ObservableCollection<SpeedViewModel> Items = new ObservableCollection<SpeedViewModel>();
        public Speed MainValue
        {
            get
            {
                return mainValue;
            }
            set
            {
                SetProperty(ref mainValue, value);
            }
        }
    }
    class ProList : ViewModelBase
    {
        private long mainValue;
        public long MainValue
        {
            get
            {
                return mainValue;
            }
            set
            {
                SetProperty(ref mainValue, value);
            }
        }
        public ProList()
        {
            Items.Add(new ProgramViewModel("Bin", 0));
            Items.Add(new ProgramViewModel("Oct", 1));
            Items.Add(new ProgramViewModel("Dec", 2));
            Items.Add(new ProgramViewModel("Hex", 3));
            foreach (var item in Items)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
            MainValue = 0;
        }

        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var i = sender as ProgramViewModel;
            switch (i.Parameter)
            {
                case 0:
                    MainValue = i.Value.BintoDec();
                    break;
                case 1:
                    MainValue = i.Value.OcttoDec();
                    break;
                case 2:
                    MainValue = long.Parse(i.Value);
                    break;
                case 3:
                    MainValue = i.Value.HextoDec();
                    break;
                default:
                    break;
            }
            foreach (var item in Items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
                switch (item.Parameter)
                {
                    case 0:
                        item.Value = MainValue.DectoBin();
                        break;
                    case 1:
                        item.Value = MainValue.DectoOct();
                        break;
                    case 2:
                        item.Value = MainValue.ToString();
                        break;
                    case 3:
                        item.Value = MainValue.DectoHex();
                        break;
                    default:
                        break;
                }
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        public ObservableCollection<ProgramViewModel> Items = new ObservableCollection<ProgramViewModel>();
    }

    class AngleList : ViewModelBase
    {
        private double mainValue;
        public double MainValue
        {
            get
            {
                return mainValue;
            }
            set
            {
                SetProperty(ref mainValue, value);
            }
        }
        public ObservableCollection<AngleViewModel> Items = new ObservableCollection<AngleViewModel>();
        public AngleList()
        {
            Items.Add(new AngleViewModel("Deg", "Deg"));
            Items.Add(new AngleViewModel("Rad", "Rad"));
            Items.Add(new AngleViewModel("Grad", "Grad"));
            foreach (var item in Items)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
            MainValue = 0;
        }
        private void Item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var i = sender as AngleViewModel;
            switch (i.Parameter)
            {
                case "Deg":
                    MainValue = i.Value;
                    break;
                case "Rad":
                    MainValue = Shared.Helpers.Tools.RadiansToDegrees((float)i.Value);
                    break;
                case "Grad":
                    MainValue = i.Value / 400 * 360;
                    break;
                default:
                    break;
            }
            foreach (var item in Items)
            {
                item.PropertyChanged -= Item_PropertyChanged;
                switch (item.Parameter)
                {
                    case "Deg":
                        item.Value = MainValue;
                        break;
                    case "Rad":
                        item.Value = Shared.Helpers.Tools.DegreesToRadians((float)MainValue);
                        break;
                    case "Grad":
                        item.Value = i.Value / 360 * 400;
                        break;
                    default:
                        break;
                }
                item.PropertyChanged += Item_PropertyChanged;
            }
        }
    }

    internal class LengthViewModel : CalculatorViewModel
    {
        public LengthViewModel(LengthParameter l)
        {
            this.Parameter = l;
            Title = l.GetDisplayName();
        }
        private LengthParameter parameter;
        public LengthParameter Parameter
        {
            get
            {
                return parameter;
            }
            set
            {
                SetProperty(ref parameter, value);
            }
        }
    }

    internal class AngleViewModel : CalculatorViewModel
    {
        public AngleViewModel(string t, string p)
        {
            this.Parameter = p;
            Title = t;
        }
        private string parameter;
        public string Parameter
        {
            get
            {
                return parameter;
            }
            set
            {
                SetProperty(ref parameter, value);
            }
        }
    }

    internal class TempViewModel : CalculatorViewModel
    {
        public TempViewModel(TemperatureParameter l)
        {
            this.Parameter = l;
            Title = l.GetDisplayName();
        }
        private TemperatureParameter parameter;
        public TemperatureParameter Parameter
        {
            get
            {
                return parameter;
            }
            set
            {
                SetProperty(ref parameter, value);
            }
        }
    }
    internal class PressureViewModel : CalculatorViewModel
    {
        public PressureViewModel(PressureParameter l)
        {
            this.Parameter = l;
            Title = l.GetDisplayName();
        }
        private PressureParameter parameter;
        public PressureParameter Parameter
        {
            get
            {
                return parameter;
            }
            set
            {
                SetProperty(ref parameter, value);
            }
        }
    }
    internal class SpeedViewModel : CalculatorViewModel
    {
        public SpeedViewModel(SpeedParameter l)
        {
            this.Parameter = l;
            Title = l.GetDisplayName();
        }
        private SpeedParameter parameter;
        public SpeedParameter Parameter
        {
            get
            {
                return parameter;
            }
            set
            {
                SetProperty(ref parameter, value);
            }
        }
    }
}
