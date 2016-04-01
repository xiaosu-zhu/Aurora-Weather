// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Com.Aurora.AuWeather.Models;
using Com.Aurora.AuWeather.ViewModels;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace Com.Aurora.AuWeather.CustomControls
{
    public sealed partial class ConditionPath : UserControl
    {
        public ConditionPath()
        {
            this.InitializeComponent();
        }

        public void SetCondition(WeatherCondition condition, bool isNight)
        {
            (this.DataContext as ConditionPathViewModel).SetCondition(condition, isNight);
        }
    }
}
