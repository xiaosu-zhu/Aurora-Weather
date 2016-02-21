using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Com.Aurora.AuWeather.CustomControls
{
    public sealed partial class ConditionPath : UserControl
    {
        public ConditionPath()
        {
            this.InitializeComponent();
            this.SetCondition(WeatherCondition.unknown, true);
        }

        public void SetCondition(WeatherCondition condition, bool isNight)
        {
            (this.DataContext as ConditionPathViewModel).SetCondition(condition, isNight);
        }
    }
}
