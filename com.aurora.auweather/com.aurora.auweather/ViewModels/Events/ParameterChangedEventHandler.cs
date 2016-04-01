// Copyright (c) Aurora Studio. All rights reserved.
//
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace Com.Aurora.AuWeather.ViewModels.Events
{
    public class ParameterChangedEventArgs
    {
        public object Parameter { get; private set; }

        public ParameterChangedEventArgs(object parameter)
        {
            this.Parameter = parameter;
        }
    }
}